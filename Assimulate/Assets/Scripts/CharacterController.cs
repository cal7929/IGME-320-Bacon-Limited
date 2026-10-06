using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [SerializeField] 
    OrbitalCamera orbitalCamera;

    [SerializeField] 
    Rigidbody parasite;

    [Header("Movement")]
    [SerializeField] 
    float moveSpeed = 4f;

    [SerializeField] 
    float turnSpeed = 12f;

    [SerializeField] 
    float jumpSpeed = 5f;

    [Header("Possession Range")]
    [SerializeField] 
    float interactRange = 1.5f;     

    [Header("Camera")]
    [SerializeField] 
    float parasiteCameraHeight = 0.2f;

    [SerializeField] 
    float parasiteCameraDistance = 2f;

    Rigidbody body;       
    Host host;            
    Host nearbyHost;

    //Connects to the input action system
    InputAction move, look, jump, interact;

    void Awake()
    {
        //Uses all of the unity input action systems
        move = InputSystem.actions.FindAction("Player/Move", true);
        look = InputSystem.actions.FindAction("Player/Look", true);
        jump = InputSystem.actions.FindAction("Player/Jump", true);
        interact = InputSystem.actions.FindAction("Player/Interact", true);

        //Sets the first player controlled create to be the parasite
        body = parasite;
    }

    void Start()
    {
        //Hide and lock the mouse cursor so mouse movement only rotates the camera
        Cursor.lockState = CursorLockMode.Locked;

        //The camera always attaches to the parsite first, so always start the camera on the parasite too
        FollowParasite();
    }

    void Update()
    {
        //Read mouse input every frame so the orbital camera can function
        orbitalCamera.Look(look.ReadValue<Vector2>());

        //Don't look for hosts while inside a host, (for game performance)
        if (host == null)
        {
            UpdateNearbyHost();
        }

        //Interect possesses and un-posseses a host
        if (interact.WasPerformedThisFrame())
        {
            if (host != null)
            {
                Release();
            }
            else if (nearbyHost != null)
            {
                Possess(nearbyHost);
            }
        }

        //Jump only when the button was pressed this frame and we are on the ground (no air jumping)
        if (jump.WasPressedThisFrame() && IsGrounded())
        {
            Vector3 v = body.linearVelocity;
            body.linearVelocity = new Vector3(v.x, jumpSpeed, v.z);
        }
    }

    void FixedUpdate()
    {
        //Movement is relative to the camera, the creature always turns to where the camera is facing
        Vector2 input = move.ReadValue<Vector2>();

        //Rotates the input by the camera's horizontal angle to get a world-space direction
        Vector3 dir = Quaternion.Euler(0f, orbitalCamera.Yaw, 0f) * new Vector3(input.x, 0f, input.y);

        //Sets sideways speed from input but keep the current vertical speed, so gravity and jumping still work
        Vector3 v = body.linearVelocity;
        body.linearVelocity = new Vector3(dir.x * moveSpeed, v.y, dir.z * moveSpeed);

        //Only turn when there is input, otherwise the creature would snap to face forward, 
        //lets player keep creatures faced in one direction while camera faces another
        if (dir.sqrMagnitude > 0.01f)
        {
            //Smoothly rotate from the current facing toward the movement direction
            Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
            body.MoveRotation(Quaternion.Slerp(body.rotation, target, turnSpeed * Time.fixedDeltaTime));
        }
    }

    /// <summary>
    /// Finds the closest host within range of the parasite and stores it in nearbyHost.
    /// Also eventually will show the interact prompt on that host and hides it on the previous one.
    /// </summary>
    void UpdateNearbyHost()
    {
        //Track the closest host found, starting with the maximum distance
        Host best = null;
        float bestDist = interactRange;

        //Check every active host and keep the closest one that is in range
        foreach (Host h in Host.All)
        {
            float d = h.DistanceTo(parasite.position);
            if (d < bestDist)
            {
                bestDist = d; best = h;
            }
        }

        //If nothing changed since last frame, so there is no prompt to update
        if (best == nearbyHost) return;

        //When we add an interact prompt on screen, this will hide the old hosts
        //prompt so multiple aren't on screen at the same time
        if (nearbyHost != null)
        {
            nearbyHost.ShowPrompt(false);
        }

        nearbyHost = best;

        //Show the prompt on the new closest host
        if (nearbyHost != null)
        {
            nearbyHost.ShowPrompt(true);
        }
    }

    /// <summary>
    /// Moves the player's control from the parasite into the host.
    /// The parasite is then hidden, and the camera switches to follow the host.
    /// </summary>
    /// <param name="target">The host being possessed</param>
    void Possess(Host target)
    {
        //Remove the prompt after possessing the host
        target.ShowPrompt(false);
        nearbyHost = null;

        //Make the host the main body
        host = target;
        body = host.Body;

        //Hide the parasite while it is inside the host,
        //(may be removed later depending on how rex wants to do the possession animations)
        parasite.gameObject.SetActive(false);
        orbitalCamera.Follow(host.transform, host.cameraHeight, host.cameraDistance);
    }

    /// <summary>
    /// Moves the player's control from the current host back to the parasite.
    /// </summary>
    void Release()
    {
        //Stops sideways velocity from the host so it doesn't keep moving after you leave
        //Verticle is kept so it doesn't stick in the air
        Vector3 v = body.linearVelocity;
        body.linearVelocity = new Vector3(0f, v.y, 0f);

        //Reactivate the parsite at the eject point
        parasite.transform.position = host.transform.TransformPoint(host.ejectOffset);
        parasite.gameObject.SetActive(true);
        parasite.linearVelocity = Vector3.zero;

        //Make the parasite the main body again
        host = null;
        body = parasite;
        FollowParasite();
    }

    //helper methods

    bool IsGrounded()
    {
        //get whatever collider the player is currently in
        Bounds b = body.GetComponent<Collider>().bounds;

        //Cast a ray from the center straight down, this allows for small gaps to still be considered grounded
        return Physics.Raycast(b.center, Vector3.down, b.extents.y + 0.1f, ~0, QueryTriggerInteraction.Ignore);
    }

    void FollowParasite()
    {
        //Points the camera at the parasite
        orbitalCamera.Follow(parasite.transform, parasiteCameraHeight, parasiteCameraDistance);
    }
}