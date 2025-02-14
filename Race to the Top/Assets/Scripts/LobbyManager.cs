using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Required for TextMeshPro UI

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_Text playersListText; // Displays list of players in the room
    public TMP_InputField roomNameInput; // Input field for room name
    public Button StartGameButton; // Button for the host to start the game

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Already connected to Photon. Joining Lobby...");
            StartCoroutine(WaitForLobbyReady());
        }
        else
        {
            Debug.Log("Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Called when Photon successfully connects to the Master Server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");

        // Ensure we do NOT call JoinLobby() twice
        if (PhotonNetwork.InLobby)
        {
            Debug.LogWarning("Already in a lobby. No need to join again.");
        }
        else if (PhotonNetwork.NetworkClientState == ClientState.JoiningLobby)
        {
            Debug.LogWarning("Already trying to join a lobby. Waiting...");
        }
        else
        {
            StartCoroutine(WaitForLobbyReady());
        }
    }

    // Coroutine to wait until Photon is fully ready before joining the lobby
    private IEnumerator WaitForLobbyReady()
    {
        Debug.Log("Waiting for Photon to be fully ready...");

        // Wait until Photon is fully connected to the Master Server
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);

        // Check if already in a lobby or still attempting
        if (PhotonNetwork.InLobby)
        {
            Debug.LogWarning("Already in a lobby. Skipping JoinLobby()...");
        }
        else if (PhotonNetwork.NetworkClientState == ClientState.JoiningLobby)
        {
            Debug.LogWarning("Already attempting to join a lobby. Waiting...");
            yield return new WaitUntil(() => PhotonNetwork.InLobby); // Wait until joined
            Debug.Log("Successfully joined lobby.");
        }
        else
        {
            Debug.Log("Now joining the lobby...");
            PhotonNetwork.JoinLobby();
            yield return new WaitUntil(() => PhotonNetwork.InLobby);
            Debug.Log("Successfully joined the lobby!");
        }
    }

    // Called when the player successfully joins the lobby
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Photon Lobby.");
    }

    // ✅ **Create Room Function (Manual Room Creation)**
    public void CreateRoom()
    {
        if (roomNameInput == null)
        {
            Debug.LogError("roomNameInput is not assigned in the Inspector!");
            return;
        }

        string roomName = roomNameInput.text.Trim(); // ✅ Trim to avoid accidental spaces
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Room name cannot be empty!");
            return;
        }

        RoomOptions options = new RoomOptions { MaxPlayers = 10 };
        PhotonNetwork.CreateRoom(roomName, options);

        Debug.Log("Creating Room: " + roomName);
    }

    // ✅ **Join Room Function (Manual Join)**
    public void JoinRoom()
    {
        if (roomNameInput == null)
        {
            Debug.LogError("roomNameInput is not assigned in the Inspector!");
            return;
        }

        string roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Room name cannot be empty!");
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
        Debug.Log("Joining Room: " + roomName);
    }

    // Called when the player successfully joins a room
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        UpdatePlayerList();

        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.interactable = true;
        }
    }

    // Called when another player joins the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined the room.");
        UpdatePlayerList();
    }

    // Called when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " left the room.");
        UpdatePlayerList();

        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.interactable = true;
        }
    }

    // Updates the UI with the list of players in the room
    void UpdatePlayerList()
    {
        if (playersListText == null)
        {
            Debug.LogError("playersListText is not assigned in the Inspector!");
            return;
        }

        playersListText.text = "Players in Room:\n";
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            playersListText.text += p.NickName + "\n";
        }
    }

    // The host starts the game
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game...");
            PhotonNetwork.LoadLevel("MainBoard");
        }
    }

    // Leaves the current lobby and returns to Main Menu
    public void LeaveLobby()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // When the player leaves a room, return to Main Menu
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
