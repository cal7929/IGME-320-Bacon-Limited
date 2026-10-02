using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectClicker : MonoBehaviour
{
    [SerializeField] LayerMask interactLayers;

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(
            new Vector2(Screen.width / 2f, Screen.height / 2f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactLayers))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.OnInteract();
                Debug.Log("clicked on interactable object: " + hit.collider.name);
            }
        }
    }
}