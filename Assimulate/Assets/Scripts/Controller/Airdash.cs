using UnityEngine;
using UnityEngine.InputSystem;

public class Airdash : MonoBehaviour
{
    [SerializeField] float dashSpeed;
    InputAction dash;
    Rigidbody body;    

    void Awake()
    {
        dash = InputSystem.actions.FindAction("Player/Dash", true);

        if(dashSpeed <= 0)
        {
            dashSpeed = 5; // Default dash speed
        }
        body = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (dash.WasPressedThisFrame())
        {
            PerformDash();
        }
    }

    void PerformDash()
    {
        Vector3 v = body.linearVelocity;
        body.linearVelocity = new Vector3(dashSpeed * v.x, 0, dashSpeed * v.z);
        //body.linearVelocity = new Vector3(v.x, dashSpeed, v.z);
        Debug.Log("Dash Performed");
    }
}
