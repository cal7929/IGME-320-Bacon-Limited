using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    [SerializeField] 
    Vector2 sensitivity = new Vector2(0.15f, 0.15f);

    [SerializeField]
    Vector2 pitchLimits = new Vector2(-20f, 70f);

    [SerializeField]
    float smoothTime = 0.15f;

    [SerializeField]
    float radius = 0.25f;

    [SerializeField]
    LayerMask obstacles;

    /// <summary>
    /// Yaw means horizontal, player controlled creatures move based on this angle
    /// </summary>
    public float Yaw { get; private set; }

    Transform target;
    float targetHeight, targetDistance; 

    float pitch = 20f;
    Vector3 pivot, pivotVelocity;
    float currentDistance, currentDistVelocity;

    void Awake()
    {
        Yaw = transform.eulerAngles.y;
    }

    /// <summary>
    /// Run during CharacterController's Update() to let the player look around freely
    /// </summary>
    /// <param name="delta">vector of camera movement based on player input</param>
    public void Look(Vector2 delta)
    {
        Yaw += delta.x * sensitivity.x;
        pitch = Mathf.Clamp(pitch - delta.y * sensitivity.y, pitchLimits.x, pitchLimits.y);
    }

    /// <summary>
    /// Makes sure the camera follows the player's position when they possess a new host or leave a possessed host.
    /// </summary>
    /// <param name="newTarget">The new host/parasite</param>
    /// <param name="newHeight">Height of new host/parasite</param>
    /// <param name="newDistance">Distance between player and new host/parasite</param>
    public void Follow(Transform newTarget, float newHeight, float newDistance)
    {
        if (target == null) 
        {
            pivot = newTarget.position + Vector3.up * newHeight;
            currentDistance = newDistance;
        }
        target = newTarget;
        targetHeight = newHeight;
        targetDistance = newDistance;
    }

    /// <summary>
    /// Late update is used so that the camera always moves AFTER the player moves or swaps creatures
    /// </summary>
    void LateUpdate()
    {
        if (target == null) return;

        //Smoothly slide the pivot toward the target's position, raised by the target's camera height.
        //This is for a smooth transition between possession, so it doesn't just jump.
        pivot = Vector3.SmoothDamp(pivot, target.position + Vector3.up * targetHeight, ref pivotVelocity, smoothTime);

        //Smoothly adjust the cameras current distance too, since hosts can use different distances based on their size
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref currentDistVelocity, smoothTime);

        //Build the camera's rotation from the mouse-controlled angles
        Quaternion rotation = Quaternion.Euler(pitch, Yaw, 0f);

        //Determine where the camera should sit in the orbit
        Vector3 back = rotation * Vector3.back;

        //Start with the full distance, then pull the camera closer if something is in the way
        float d = currentDistance;

        //Sweep a small sphere from the pivot toward the camera position. If it hits a wall or obstacle,
        //move the camera to the hit point instead so it never ends up inside map geometry.
        //The minimum of 0.3 stops it from squashing into the pivot.
        if (Physics.SphereCast(pivot, radius, back, out RaycastHit hit, currentDistance, obstacles, QueryTriggerInteraction.Ignore))
        {
            d = Mathf.Max(hit.distance, 0.3f);
        }

        //Finally, place the camera in the right position based on previous checks and calculations
        transform.SetPositionAndRotation(pivot + back * d, rotation);
    }
}