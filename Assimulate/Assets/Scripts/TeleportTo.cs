using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportTo : MonoBehaviour
{
   private void OnCollisionEnter(Collision collision)
    {

        int sceneNumber = SceneManager.GetActiveScene().buildIndex;
        if (collision.gameObject.CompareTag("Player"))
        {
            switch(sceneNumber)
            {
                case 2:
                    SceneManager.LoadScene(3);
                    break;

                case 3:
                    SceneManager.LoadScene(1);
                    break;
            }
        }
    }
}
