using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void HostGame()
    {
        Debug.Log("Hosting Game...");
        SceneManager.LoadScene("LobbyScene");
    }

    public void JoinGame()
    {
        Debug.Log("Joining Game...");
        // You said you want Join to go straight to room code entry — we’ll implement that as a popup, not a new scene
    }

    public void ExitToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}

