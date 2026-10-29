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

    [Header("Jumping")]
    [SerializeField]
    bool canJump = true;
    [SerializeField]
    float jumpSpeed = 5f;

    Rigidbody body;
    Collider col;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    /// <summary>
    /// Moves the creature relative to the camera. Call this from FixedUpdate.
    /// </summary>
    /// <param name="input">Move input</param>
    /// <param name="cameraYaw">The camera's horizontal angle</param>
    public void Move(Vector2 input, float cameraYaw)
    {
        //Rotates the input by the camera's horizontal angle to get a world-space direction
        Vector3 dir = Quaternion.Euler(0f, cameraYaw, 0f) * new Vector3(input.x, 0f, input.y);

        //Sets sideways speed from input but keep the current vertical speed, so gravity and jumping still work
        Vector3 v = body.linearVelocity;
        body.linearVelocity = new Vector3(dir.x * moveSpeed, v.y, dir.z * moveSpeed);

        //Only turn when there is input, otherwise the creature would snap to face forward,
        //this lets the player keep the creature facing one direction while the camera faces another
        if (dir.sqrMagnitude > 0.01f)
        {
            //Smoothly rotate from the current facing toward the movement direction
            Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
            body.MoveRotation(Quaternion.Slerp(body.rotation, target, turnSpeed * Time.fixedDeltaTime));
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
    }

    public bool IsGrounded()
    {
        Bounds b = col.bounds;

        //Cast a ray from the center straight down, this allows for small gaps to still be considered grounded
        return Physics.Raycast(b.center, Vector3.down, b.extents.y + 0.1f, ~0, QueryTriggerInteraction.Ignore);
    }
}
