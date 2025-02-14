using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainBoard"); // Loads the multiplayer game scene
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit(); // Closes the game
    }
}
