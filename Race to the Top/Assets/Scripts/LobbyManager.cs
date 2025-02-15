using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TMP_Text roomCodeText; // Displays the room code
    public Transform playerListPanel; // Parent panel for player list items
    public GameObject playerListItemPrefab; // Prefab for player name display
    public Button startGameButton; // Start Game (Host Only)
    public Button closeLobbyButton; // Close Lobby (Host Only)
    public Button backButton; // Leave lobby button

    private void Start()
    {
        Debug.Log("✅ Lobby Manager Loaded.");

        // **🔴 NULL CHECKS** - Prevents common mistakes
        if (roomCodeText == null) Debug.LogError("❌ roomCodeText is NULL! Assign it in the Inspector.");
        if (playerListPanel == null) Debug.LogError("❌ playerListPanel is NULL! Assign it in the Inspector.");
        if (playerListItemPrefab == null) Debug.LogError("❌ playerListItemPrefab is NULL! Assign it in the Inspector.");
        if (startGameButton == null) Debug.LogError("❌ startGameButton is NULL! Assign it in the Inspector.");
        if (closeLobbyButton == null) Debug.LogError("❌ closeLobbyButton is NULL! Assign it in the Inspector.");
        if (backButton == null) Debug.LogError("❌ backButton is NULL! Assign it in the Inspector.");

        // **Ensure we're in a room**
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("❌ Not in a Photon Room! Returning to Main Menu...");
            SceneManager.LoadScene("MainMenu");
            return;
        }

        // **Retrieve & Display Room Code**
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("roomCode", out object roomCode))
        {
            roomCodeText.text = "Room Code: " + roomCode.ToString();
        }
        else
        {
            roomCodeText.text = "Room Code: ERROR";
        }

        // **Update Player List**
        UpdatePlayerList();

        // **Enable/Disable host controls**
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

        // **Back Button Listener**
        backButton.onClick.AddListener(LeaveLobby);
    }

    public void UpdatePlayerList()
    {
        Debug.Log("🔄 Updating player list... Total Players: " + PhotonNetwork.PlayerList.Length);

        // **Clear previous UI**
        foreach (Transform child in playerListPanel)
        {
            Destroy(child.gameObject);
        }

        if (PhotonNetwork.PlayerList.Length == 1)
        {
            Debug.Log("👤 Only one player in the room. Showing Waiting Message...");
            GameObject waitingItem = Instantiate(playerListItemPrefab, playerListPanel);
            TMP_Text waitingText = waitingItem.GetComponentInChildren<TMP_Text>();
            waitingText.text = "Waiting for more players...";
            return;
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // **Force Assign a Username if Missing**
            if (string.IsNullOrEmpty(player.NickName))
            {
                player.NickName = "Player_" + player.ActorNumber;
                Debug.Log("⚡ Assigned Default Name: " + player.NickName);
            }

            Debug.Log("👤 Player Found: " + player.NickName + " | ID: " + player.ActorNumber);

            // **Create Player UI Element**
            GameObject playerItem = Instantiate(playerListItemPrefab, playerListPanel);
            TMP_Text playerText = playerItem.GetComponentInChildren<TMP_Text>();
            playerText.text = player.NickName;

            // **Mark Host**
            if (player.IsMasterClient)
            {
                playerText.text += " (Host)";
            }

            // **Kick Button Handling**
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
            Debug.Log("🔴 Kicked Player: " + player.NickName);
        }
    }

    public void CloseLobby()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            Debug.Log("🔒 Lobby is now closed.");
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("🚀 Starting game...");
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
        Debug.Log("👥 Player Joined: " + newPlayer.NickName + " | ID: " + newPlayer.ActorNumber);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("👥 Player Left: " + otherPlayer.NickName);
        UpdatePlayerList();
    }
}
