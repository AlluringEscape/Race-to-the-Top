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

        // Add each player to the UI
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab, playerListPanel);
            TMP_Text playerText = playerItem.GetComponentInChildren<TMP_Text>();

            // If player is the host, add "(Host)" next to their name
            playerText.text = player.NickName + (player.IsMasterClient ? " (Host)" : "");

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
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
}
