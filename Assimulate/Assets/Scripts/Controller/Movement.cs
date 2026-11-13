using UnityEngine;

/// <summary>
/// handles movement for the parasite and creatures.
/// Each creature gets its own stats, which can be changed in the inspector.
/// PlayerController tells whichever creature is currently possessed to move or jump.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float moveSpeed = 4f;
    [SerializeField]
    float turnSpeed = 12f;

    /// <summary>
    /// Allows other creature specific cripts to seamlessly change the speed and turn speed 
    /// </summary>
    public float SpeedMultiplier { get; set; } = 1f;
    public float TurnMultiplier { get; set; } = 1f;


    [Header("Jumping")]
    [SerializeField]
    bool canJump = true;
    [SerializeField]
    float jumpSpeed = 5f;
    [SerializeField]
    float gravityMultiplier = 1.0f;

    [Header("Friction")]
    [SerializeField] PhysicsMaterial controlledMaterial;
    [SerializeField] PhysicsMaterial idleMaterial;

    Rigidbody body;
    Collider col;

    //Tracks whether SetFriction has already been called, so Start doesn't overwrite it
    bool frictionSet;

    //Event for primarily sound effect purposes,
    //these can be used by other scripts to trigger stuff when the specific event happens.
    public event System.Action OnJump;

    void Awake()
    {
        CacheComponents();
    }

    void Start()
    {
        //Creatures that nobody has set friction on (hosts that are never possessed) start with idle friction.
        //Skipped if SetFriction already ran, which stops the parasite losing its no-friction material.
        if (!frictionSet && idleMaterial != null)
        {
            col.sharedMaterial = idleMaterial;
        }
    }

    //Gets the components if they haven't been fetched yet, so this script
    //works no matter which Awake runs first (this caused the build-only null reference)
    void CacheComponents()
    {
        if (body == null) body = GetComponent<Rigidbody>();
        if (col == null) col = GetComponent<Collider>();
    }

    /// <summary>
    /// Moves the creature relative to the camera. Call this from PlayerController's FixedUpdate.
    /// </summary>
    /// <param name="input">Move input</param>
    /// <param name="cameraYaw">The camera's horizontal angle</param>
    public void Move(Vector2 input, float cameraYaw)
    {
        //Rotates the input by the camera's horizontal angle to get a world-space direction
        Vector3 dir = Quaternion.Euler(0f, cameraYaw, 0f) * new Vector3(input.x, 0f, input.y);

        //Sets sideways speed from input but keep the current vertical speed, so gravity and jumping still work
        Vector3 v = body.linearVelocity;
        float speed = moveSpeed * SpeedMultiplier;
        body.linearVelocity = new Vector3(dir.x * speed, v.y, dir.z * speed);

        //Only turn when there is input, otherwise the creature would snap to face forward,
        //this lets the player keep the creature facing one direction while the camera faces another
        if (dir.sqrMagnitude > 0.01f)
        {
            //Smoothly rotate from the current facing toward the movement direction
            Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
            float turn = turnSpeed * TurnMultiplier;
            body.MoveRotation(Quaternion.Slerp(body.rotation, target, turn * Time.fixedDeltaTime));
        }
    }

    /// <summary>
    /// Makes the creature jump, but only if it is allowed to and is standing on the ground (no air jumping).
    /// </summary>
    public void Jump()
    {
        if (!canJump || !IsGrounded()) return;

        Vector3 v = body.linearVelocity;
        body.linearVelocity = new Vector3(v.x, jumpSpeed, v.z);

        OnJump?.Invoke();
    }

    public bool IsGrounded()
    {
        Bounds b = col.bounds;

        //Cast a ray from the center straight down, this allows for small gaps to still be considered grounded
        return Physics.Raycast(b.center, Vector3.down, b.extents.y + 0.1f, ~0, QueryTriggerInteraction.Ignore);
    }

    public void SetFriction(bool controlled)
    {
        //Make sure col and body exist even if this is called before our own Awake
        CacheComponents();
        frictionSet = true;

        col.sharedMaterial = controlled ? controlledMaterial : idleMaterial;

        if (!controlled)
        {
            Vector3 v = body.linearVelocity;
            body.linearVelocity = new Vector3(0f, v.y, 0f);
        }
    }

    void FixedUpdate()
    {
        //Calculate the extra gravity force
        //gravmult - 1 accounts for the default gravity already being applied
        Vector3 extraGravityForce = Physics.gravity * (gravityMultiplier - 1);

        //ForceMode.Acceleration makes fall speed unaffected by mass
        body.AddForce(extraGravityForce, ForceMode.Acceleration);
    }
}