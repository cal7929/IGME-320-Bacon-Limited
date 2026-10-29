using UnityEngine;

/// <summary>
/// Handles possessing and un-possessing hosts, and keeps track of which creature the player currently controls.
/// </summary>
public class PossessionHandler : MonoBehaviour
{
    [SerializeField]
    OrbitalCamera orbitalCamera;

    [SerializeField]
    Rigidbody parasite;

    [SerializeField]
    float possessionRange = 1.5f;

    [SerializeField]
    float parasiteCameraHeight = 0.2f;

    [SerializeField]
    float parasiteCameraDistance = 2f;

    /// <summary>
    /// The host currently being possessed. Null while the player is controlling the parasite.
    /// </summary>
    public Host CurrentHost { get; private set; }

    /// <summary>
    /// The Movement of whatever creature the player is controlling right now 
    /// </summary>
    public Movement CurrentMovement { get; private set; }

    Movement parasiteMovement;
    Host nearbyHost;

    void Awake()
    {
        //The player always starts as the parasite
        parasiteMovement = parasite.GetComponent<Movement>();
        CurrentMovement = parasiteMovement;
    }

    void Start()
    {
        //The camera always starts on the parasite
        FollowParasite();
    }

    void Update()
    {
        //Only look for hosts when you are the parasite
        if (CurrentHost == null)
        {
            UpdateNearbyHost();
        }
    }

    /// <summary>
    /// Called when the interact button is pressed. Possesses, or un-ppossesses a host
    /// </summary>
    public void Interact()
    {
        if (CurrentHost != null)
        {
            Release();
        }
        else if (nearbyHost != null)
        {
            Possess(nearbyHost);
        }
    }

    /// <summary>
    /// Finds the closest host within range of the parasite and stores it in nearbyHost.
    /// Also shows the interact prompt on that host and hides it on the previous one.
    /// </summary>
    void UpdateNearbyHost()
    {
        //Track the closest host found, starting with the maximum distance
        Host best = null;
        float bestDist = possessionRange;

        //Check every active host and keep the closest one that is in range
        foreach (Host h in Host.All)
        {
            float d = h.DistanceTo(parasite.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = h;
            }
        }

        //If the closest host is the current one, change nothing
        if (best == nearbyHost) return;

        //Hide the old host's prompt so multiple aren't on screen at the same time
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
    /// The parasite is hidden, and the camera switches to follow the host.
    /// </summary>
    void Possess(Host target)
    {
        //Every host needs a Movement script so the player can control it
        Movement targetMovement = target.GetComponent<Movement>();
        if (targetMovement == null)
        {
            Debug.LogWarning(target.name + " has no Movement script, so it can't be possessed.");
            return;
        }

        //Remove the prompt after possessing the host
        target.ShowPrompt(false);
        nearbyHost = null;

        //Make the target host the current host
        CurrentHost = target;
        CurrentMovement = targetMovement;

        //Hide the parasite while it is inside the host, and then set camera to follow the host
        parasite.gameObject.SetActive(false);
        orbitalCamera.Follow(target.transform, target.cameraHeight, target.cameraDistance);
    }

    /// <summary>
    /// Moves the player's control from the current host back to the parasite.
    /// </summary>
    void Release()
    {
        //Stops sideways velocity from the host so it doesn't keep moving after you leave, vertical is kept so it doesn't stick in the air
        Rigidbody hostBody = CurrentHost.Body;
        Vector3 v = hostBody.linearVelocity;
        hostBody.linearVelocity = new Vector3(0f, v.y, 0f);

        //Reactivate the parasite at the eject point
        parasite.transform.position = CurrentHost.transform.TransformPoint(CurrentHost.ejectOffset);
        parasite.gameObject.SetActive(true);
        parasite.linearVelocity = Vector3.zero;

        //Make the parasite the controlled creature again
        CurrentHost = null;
        CurrentMovement = parasiteMovement;
        FollowParasite();
    }

    void FollowParasite()
    {
        orbitalCamera.Follow(parasite.transform, parasiteCameraHeight, parasiteCameraDistance);
    }
}