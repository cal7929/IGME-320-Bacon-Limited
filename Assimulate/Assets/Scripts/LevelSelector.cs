using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void StartSandbox()
    {
        SceneManager.LoadScene("Test Scene");
    }

    public void StartPlaytest()
    {
        Debug.Log("Starting Playtest");
        SceneManager.LoadScene("Playtest Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
