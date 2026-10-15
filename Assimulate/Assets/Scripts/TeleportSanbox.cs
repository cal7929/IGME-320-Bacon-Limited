using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportSanbox : MonoBehaviour
{
   private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with TeleportSandbox");
            SceneManager.LoadScene("Test Scene");
        }
    }
}
