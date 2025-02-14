using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
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
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }
    else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
    {
        Debug.Log("Joining Photon Lobby...");
        PhotonNetwork.JoinLobby(); // ✅ Ensures Photon starts fetching rooms
    }
    else
    {
        Debug.LogError("Photon is not ready. Waiting...");
    }
}


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Clear previous list
        foreach (Transform child in roomListPanel)
        {
            Destroy(child.gameObject);
        }

        cachedRoomList.Clear();

        // Display each room in the list
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
    }

    public void JoinByCode()
    {
        string roomCode = roomCodeInput.text.Trim();
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room code cannot be empty!");
            return;
        }

        Debug.Log("Attempting to join room: " + roomCode);
        PhotonNetwork.JoinRoom(roomCode);
    }

    public void JoinRoom(string roomName)
    {
        Debug.Log("Joining room: " + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("LobbyScene"); // Move to lobby after joining
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }
}
