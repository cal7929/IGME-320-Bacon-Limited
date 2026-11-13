using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void StartSandbox()
    {
        SceneManager.LoadScene(1);
    }

    public void StartLevel1()
    {
        SceneManager.LoadScene(2);
    }
    public void StartLevel2()
    {
        SceneManager.LoadScene(3);
    }
    public void StartLevel3()
    {
        SceneManager.LoadScene(4);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
