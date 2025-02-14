using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class HostGameSetupManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput; // Room name input field
    public TMP_Dropdown privacyDropdown; // Public or Friends-Only dropdown
    public TMP_InputField passwordInput; // Password field (Optional)
    public Button hostButton; // Host button
    public Button backButton; // Back button

    private void Start()
    {
        // Ensure buttons are assigned
        if (hostButton != null) hostButton.onClick.AddListener(() => StartCoroutine(WaitForPhotonAndHostGame()));
        if (backButton != null) backButton.onClick.AddListener(GoBack);

        // Ensure we are connected to Photon
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private IEnumerator WaitForPhotonAndHostGame()
    {
        // Wait for Photon to be fully connected
        Debug.Log("Waiting for Photon to be ready...");
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);

        Debug.Log("Photon is ready. Creating Room...");
        CreateRoom();
    }

    public void CreateRoom()
    {
        string roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Room name cannot be empty!");
            return;
        }

        bool isPrivate = privacyDropdown.value == 1; // 0 = Public, 1 = Friends-Only
        string password = passwordInput.text.Trim();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsVisible = !isPrivate; // If private, room is not listed
        roomOptions.IsOpen = true;

        // Add password as a custom property if entered
        if (!string.IsNullOrEmpty(password))
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties["password"] = password;
            roomOptions.CustomRoomProperties = roomProperties;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };
        }

        Debug.Log($"Creating Room: {roomName} | Private: {isPrivate} | Password: {(string.IsNullOrEmpty(password) ? "None" : password)}");
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // Called when the room is successfully created
    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("LobbyScene"); // ✅ Move to the Lobby Scene after creating room
    }

    // Called if room creation fails
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
    }

    public void GoBack()
    {
        Debug.Log("Returning to Multiplayer Menu...");
        SceneManager.LoadScene("MultiplayerMenu"); // ✅ Return to Multiplayer Menu
    }
}
