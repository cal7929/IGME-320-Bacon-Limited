using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Airdash : MonoBehaviour
{
    [SerializeField] float dashForce = 15f;
    [SerializeField] PlayerController playerController;

    InputAction dash;
    Rigidbody body;
    Movement movement;
    bool isDashing;

    void Awake()
    {
        dash = InputSystem.actions.FindAction("Player/Dash", true);
        body = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();

        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }
    }

    void Update()
    {
        if (dash.WasPressedThisFrame() && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        //remove existing horizontal movement before applying the dash.
        Vector3 velocity = body.linearVelocity;
        body.linearVelocity = new Vector3(0f, velocity.y, 0f);

        //apply impulse.
        body.AddForce(transform.forward * dashForce, ForceMode.Impulse);

        //hold the dash for a short duration before re-enabling player control via the coroutine.
        yield return new WaitUntil(() => movement.IsGrounded());

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isDashing = false;
    }
}
