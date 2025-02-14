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
            Debug.Log("🔌 Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private IEnumerator WaitForPhotonAndHostGame()
    {
        // Wait for Photon to be fully connected
        Debug.Log("⏳ Waiting for Photon to be ready...");
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);

        Debug.Log("✅ Photon is ready. Creating Room...");
        CreateRoom();
    }

    public void CreateRoom()
    {
        string roomCode = GenerateRoomCode(); // Generate a random 6-character code

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsVisible = true;  // ✅ Ensure the room appears in the public list
        roomOptions.IsOpen = true;

        // Store room code as a custom property
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties["roomCode"] = roomCode;
        roomOptions.CustomRoomProperties = roomProperties;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "roomCode" };

        Debug.Log("🟢 Creating Room with Code: " + roomCode);
        PhotonNetwork.CreateRoom(roomCode, roomOptions);
    }

    // ✅ Function to generate a random **unique** 6-character code
    private string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string newCode;
        
        do
        {
            char[] codeArray = new char[6];
            for (int i = 0; i < codeArray.Length; i++)
            {
                codeArray[i] = chars[Random.Range(0, chars.Length)];
            }
            newCode = new string(codeArray);
        }
        while (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.Name == newCode); // Prevent duplicate codes

        return newCode;
    }

    // ✅ Called when the room is successfully created
    public override void OnCreatedRoom()
{
    Debug.Log("✅ Room Created: " + PhotonNetwork.CurrentRoom.Name);
    Debug.Log("🔍 Room Details - Open: " + PhotonNetwork.CurrentRoom.IsOpen + ", Visible: " + PhotonNetwork.CurrentRoom.IsVisible);
    
    ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
    if (properties.ContainsKey("roomCode"))
    {
        Debug.Log("🆔 Room Code (Custom Property): " + properties["roomCode"]);
    }
    else
    {
        Debug.LogError("❌ Room Code NOT found in Custom Properties!");
    }

    SceneManager.LoadScene("LobbyScene"); // ✅ Move to the Lobby Scene after creating room
}


    // ❌ Called if room creation fails
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("❌ Room creation failed: " + message);
    }

    public void GoBack()
    {
        Debug.Log("🔙 Returning to Multiplayer Menu...");
        SceneManager.LoadScene("MultiplayerMenu"); // ✅ Return to Multiplayer Menu
    }
}
