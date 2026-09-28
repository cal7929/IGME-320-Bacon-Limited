using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MyCharacterController : MonoBehaviour
{
    [SerializeField] float lookSpeed = 100f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Vector2 lookSensitivity = new Vector2(1, 1);
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundingOffset = 1.0f;
    [SerializeField] float jumpForce = 100f;

    Vector2 moveInput;

    Camera myCamera;
    Rigidbody rb;
    bool grounded;

    void Start()
    {
        grounded = true;
        rb = GetComponent<Rigidbody>();
        myCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;

        Vector3 pos = transform.position;

        pos += transform.forward * moveInput.y * moveSpeed * Time.fixedDeltaTime;
        pos += transform.right * moveInput.x * moveSpeed * Time.fixedDeltaTime;

        if (grounded)
        {
            rb.linearVelocity = Vector3.zero;

            RaycastHit hit;
            if (Physics.Raycast(pos, -transform.up, out hit, Mathf.Infinity, groundMask))
            {
                pos = hit.point;
                pos.y += groundingOffset;
                pos.y = hit.point.y + groundingOffset;
            }
        }

        grounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundMask);

        rb.MovePosition(pos);

    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && grounded)
        {
            grounded = false;
            rb.AddForce(transform.up * jumpForce);
        }
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        Vector2 lookInput = ctx.ReadValue<Vector2>();

        if (lookInput.x != 0f)
        {
            transform.Rotate(0,
                lookSpeed * Time.deltaTime * lookInput.x * lookSensitivity.x,
                0);
        }

        if (lookInput.y != 0f)
        {
            float xAngle = myCamera.transform.rotation.eulerAngles.x;
            if (xAngle > 180)
            {
                xAngle -= 360;
            }

            xAngle -= lookSpeed * Time.deltaTime * lookInput.y * lookSensitivity.y;
            xAngle = Mathf.Clamp(xAngle, -89, 89);

            myCamera.transform.rotation = Quaternion.Euler(
                xAngle,
                myCamera.transform.rotation.eulerAngles.y,
                myCamera.transform.rotation.eulerAngles.z
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((groundMask & (1 << collision.gameObject.layer)) != 0)
        {
            grounded = true;
        }
    }
}