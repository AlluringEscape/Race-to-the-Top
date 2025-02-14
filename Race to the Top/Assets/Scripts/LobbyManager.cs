using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_Text playersListText;
    public Button StartGameButton;
    private List<Player> players = new List<Player>();

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            CreateOrJoinRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        CreateOrJoinRoom();
    }

    void CreateOrJoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 10 });
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true; // Only host can start game
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
        }
    }

    void UpdatePlayerList()
    {
        playersListText.text = "Players in Lobby:\n";
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            playersListText.text += p.NickName + "\n";
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game...");
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel("MainBoard"); // ✅ Correct scene name
        }
    }

    public void LeaveLobby()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("MainMenu"); // ✅ Fix: SceneManager now works
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu"); // ✅ Fix: SceneManager now works
    }
}
