using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject specificPlayer;
    [SerializeField] private float pushableMass = 1f;
    [SerializeField] private float nonPushableMass = 1f;

    private Collider triggerCollider;
    private Rigidbody rb;

    private void Awake()
    {
        triggerCollider = GetComponents<Collider>()[1];
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != specificPlayer) return;
        rb.mass = pushableMass;
        Debug.Log("Player entered the trigger zone.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != specificPlayer) return;
        rb.mass = nonPushableMass;
        Debug.Log("Player exited the trigger zone.");
    }


    public void OnInteract()
    {
        //implement later
    }

}