using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRaycaster : MonoBehaviour
{
    [SerializeField] LayerMask interactLayers;

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Ray myRay = Camera.main.ScreenPointToRay(Mouse.current.position.value);

        RaycastHit hit;
        if (Physics.Raycast(myRay, out hit, Mathf.Infinity, interactLayers))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
    }
}

