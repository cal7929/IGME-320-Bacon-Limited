using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
