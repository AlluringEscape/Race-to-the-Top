// ----------------------------
// 📄 TitleScreenManager.cs
// ----------------------------

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
