using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class JoinGameManager : MonoBehaviourPunCallbacks
{
    public Transform roomListPanel; // Parent panel for room list
    public GameObject roomListItemPrefab; // Prefab for each room entry
    public TMP_InputField roomCodeInput; // Input field for joining by code
    public Button joinByCodeButton; // Join by Room Code button
    public Button backButton; // Back button

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private void Start()
{
    if (!PhotonNetwork.IsConnected)
    {
        Debug.Log("🔌 Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }
    else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
    {
        Debug.Log("🔄 Joining Photon Lobby...");
        PhotonNetwork.JoinLobby(); // ✅ Ensures the client joins the lobby
    }
    else
    {
        Debug.LogError("❌ Photon is not ready. Waiting...");
        StartCoroutine(WaitForPhotonReady());
    }
}


private IEnumerator WaitForPhotonReady()
{
    yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
    Debug.Log(" Connected to Master Server. Joining lobby...");
    PhotonNetwork.JoinLobby();
}


    public override void OnJoinedLobby()
{
    Debug.Log("✅ Joined Photon Lobby! Waiting for room list update...");
    // Room list will automatically update via OnRoomListUpdate()
}

   public override void OnRoomListUpdate(List<RoomInfo> roomList)
{
    Debug.Log("🔄 Room list updated! Found " + roomList.Count + " rooms.");
    
    if (roomList.Count == 0)
    {
        Debug.LogWarning("⚠️ No rooms are visible! Either no rooms exist or they are set to private.");
    }

    foreach (RoomInfo room in roomList)
    {
        Debug.Log("📌 Room Found: " + room.Name + " | Players: " + room.PlayerCount + "/" + room.MaxPlayers + " | Open: " + room.IsOpen + " | Visible: " + room.IsVisible);
    }
}


    private void RefreshRoomList(List<RoomInfo> roomList = null)
    {
        // Clear previous list
        foreach (Transform child in roomListPanel)
        {
            Destroy(child.gameObject);
        }

        cachedRoomList.Clear();

        if (roomList == null)
        {
            Debug.Log("🔄 Requesting room list...");
            PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");
            return;
        }

        foreach (RoomInfo room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible) continue; // Skip private/closed rooms

            GameObject roomItem = Instantiate(roomListItemPrefab, roomListPanel);
            TMP_Text roomText = roomItem.GetComponentInChildren<TMP_Text>();

            roomText.text = room.Name + " (" + room.PlayerCount + "/" + room.MaxPlayers + ")";
            cachedRoomList[room.Name] = room;

            // Add button to join the room
            Button joinButton = roomItem.GetComponentInChildren<Button>();
            joinButton.onClick.AddListener(() => JoinRoom(room.Name));
        }

        Debug.Log("✅ Room list updated. Found " + roomList.Count + " rooms.");
    }

    public void JoinByCode()
{
    string roomCode = roomCodeInput.text.Trim();
    if (string.IsNullOrEmpty(roomCode))
    {
        Debug.LogError("❌ Room code cannot be empty!");
        return;
    }

    Debug.Log("🔄 Attempting to join room by code: " + roomCode);
    StartCoroutine(WaitForPhotonReadyThenJoin(roomCode));
}

private IEnumerator WaitForPhotonReadyThenJoin(string roomCode)
{
    Debug.Log("⏳ Waiting for Photon to be ready before joining...");

    // Wait until Photon is fully connected
    yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

    // Wait until the client has joined the lobby
    yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.JoinedLobby);

    Debug.Log("✅ Connected & in Lobby. Attempting to join room: " + roomCode);
    PhotonNetwork.JoinRoom(roomCode);
}


    public void JoinRoom(string roomName)
    {
        Debug.Log("🔄 Joining room: " + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("✅ Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("LobbyScene"); // ✅ Move to lobby after joining
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("❌ Failed to join room: " + message);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }
}
