using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerMenuManager : MonoBehaviour
{
    public void GoToHostGame()
    {
        SceneManager.LoadScene("HostGameSetup"); //  Loads Host Game Setup
    }

    public void GoToJoinGame()
    {
        Debug.Log("Joining Public Lobby List (Not implemented yet).");
    }

    public void GoToOfflineMode()
    {
        Debug.Log("Offline Mode (Not implemented yet).");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu"); // Back to Main Menu
    }
}
