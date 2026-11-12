using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Laser : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float laserDistance = 8f;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private GameObject ImmunePlayer;
    [SerializeField] private UnityEvent OnHitTarget;

    private RaycastHit rayHit;
    private Ray ray;

    private void Awake()
    {
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        ray = new Ray(transform.position, transform.forward);

        // Shoot raycast with specified distance and layer mask
        if (Physics.Raycast(ray, out rayHit, laserDistance, ~ignoreMask))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, rayHit.point);

            // Check if the laser hit the immune player
            if (rayHit.collider.gameObject == ImmunePlayer)
            {
                //Play the laser block sound from the block guy
                ImmunePlayer.GetComponent<SoundBody>().IsBlockingLaser = true;
                return;
            }

            //if is nothing is blocking laser, set to blocking to false for sounds
            ImmunePlayer.GetComponent<SoundBody>().IsBlockingLaser = false;

            // Check if the laser hit a Player or box
            if (rayHit.collider.CompareTag("Player") || rayHit.collider.CompareTag("InteractableObj"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }

        }
        else
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);

            //if is nothing is blocking laser, set to blocking to false for sounds
            ImmunePlayer.GetComponent<SoundBody>().IsBlockingLaser = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * laserDistance);

        if (rayHit.collider != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rayHit.point, 0.5f);
        }
    }
}

