using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadlyThing : MonoBehaviour
{
    [SerializeField] private GameObject specificPlayer;
    bool isdeadly = true;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if blocker is what entered
        if (other.gameObject == specificPlayer)
        {
            Debug.Log("Blocker is blocking");
            isdeadly = false;
            meshRenderer.enabled = false;
            return;
        }

        //if its a player and blocker isnt blocking, kill
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered the deadly trigger zone.");

            if (isdeadly)
            {
                Debug.Log("Player is killed");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.Log("Blocker is blocking, player is safe.");
            }
        }
        else if(other.gameObject.CompareTag("InteractableObj") && isdeadly)
        {
            Debug.Log("Destroy Interactable");
            other.gameObject.SetActive(false);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        //check if blocker is what entered
        if (other.gameObject == specificPlayer)
        {
            Debug.Log("Blocker is no longer blocking");
            meshRenderer.enabled = true;
            isdeadly = true;

            DestroyInteractablesInBounds();
        }
    }

    //This is AI assisted, but helps catch some edge cases I missed in initial implementation
    private void DestroyInteractablesInBounds()
    {
        Collider triggerCollider = GetComponent<Collider>();

        // Get all colliders currently inside the trigger's bounds
        Collider[] objectsInside = Physics.OverlapBox(
            triggerCollider.bounds.center,
            triggerCollider.bounds.extents,
            triggerCollider.transform.rotation
        );

        foreach (Collider col in objectsInside)
        {
            if (col.gameObject.CompareTag("InteractableObj"))
            {
                Debug.Log("Destroy Interactable already inside");
                col.gameObject.SetActive(false);
            }
        }
    }

}
