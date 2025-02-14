using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
}

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit(); // Closes the game
    }
}
