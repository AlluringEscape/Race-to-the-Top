using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MultiplayerMenu"); // ✅ Loads Multiplayer Menu
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit(); // ✅ Exits the game
    }
}
