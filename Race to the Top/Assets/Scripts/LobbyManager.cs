using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomCodeText; // Displays the room code
    public Transform playerListPanel; // Parent panel for player list items
    public GameObject playerListItemPrefab; // Prefab for player name display
    public Button startGameButton; // Start Game (Host Only)
    public Button closeLobbyButton; // Close Lobby (Host Only)
    public Button backButton; // Leave lobby button

    private void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("Not in a Photon Room! Returning to Main Menu...");
            SceneManager.LoadScene("MainMenu");
            return;
        }

        // Retrieve and display Room Code
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("roomCode", out object roomCode))
        {
            roomCodeText.text = "Room Code: " + roomCode.ToString();
        }
        else
        {
            roomCodeText.text = "Room Code: ERROR";
        }

        // Show Player List
        UpdatePlayerList();

        // Enable/Disable buttons based on host status
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
            closeLobbyButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
            closeLobbyButton.interactable = false;
        }
    }

   public void UpdatePlayerList()
{
    // Clear existing player list UI
    foreach (Transform child in playerListPanel)
    {
        Destroy(child.gameObject);
    }

    foreach (Player player in PhotonNetwork.PlayerList)
    {
        GameObject playerItem = Instantiate(playerListItemPrefab, playerListPanel);
        TMP_Text playerText = playerItem.GetComponentInChildren<TMP_Text>();

        // Check if this player is the host
        if (player.IsMasterClient)
        {
            playerText.text = player.NickName + " (Host)";
        }
        else
        {
            playerText.text = player.NickName;
        }

        // Show Kick Button only for host
        Button kickButton = playerItem.GetComponentInChildren<Button>();
        if (PhotonNetwork.IsMasterClient && player != PhotonNetwork.LocalPlayer)
        {
            kickButton.onClick.AddListener(() => KickPlayer(player));
            kickButton.gameObject.SetActive(true);
        }
        else
        {
            kickButton.gameObject.SetActive(false);
        }
    }
}

    public void KickPlayer(Player player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CloseConnection(player);
            Debug.Log("Kicked Player: " + player.NickName);
        }
    }

    public void CloseLobby()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            Debug.Log("Lobby is now closed.");
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting game...");
            PhotonNetwork.LoadLevel("MainBoard"); // ✅ Change "MainBoard" to your actual game scene name
        }
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
{
    Debug.Log("Player Joined: " + newPlayer.NickName);
    UpdatePlayerList();
}


    public override void OnPlayerLeftRoom(Player otherPlayer)
{
    Debug.Log("Player Left: " + otherPlayer.NickName);
    UpdatePlayerList();
}

}
