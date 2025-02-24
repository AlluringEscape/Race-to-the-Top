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
    public TMP_Text roomCodeText;
    public Transform playerListPanel;
    public GameObject playerListItemPrefab;
    public Button startGameButton;
    public Button closeLobbyButton;
    public Button backButton;

    private void Start()
    {
        Debug.Log("✅ LobbyManager Started!");

        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("❌ Not in a Photon Room! Returning to Main Menu...");
            SceneManager.LoadScene("MainMenu");
            return;
        }

        // Check for missing references
        if (roomCodeText == null || playerListPanel == null || playerListItemPrefab == null)
        {
            Debug.LogError("❌ UI Elements Not Assigned! Check Inspector.");
            return;
        }

        // Retrieve and display Room Code
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("roomCode", out object roomCode))
        {
            roomCodeText.text = "📌 Room Code: " + roomCode.ToString();
        }
        else
        {
            roomCodeText.text = "❌ Room Code: ERROR";
        }

        UpdatePlayerList();

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
        Debug.Log("🔄 Updating player list... Total Players: " + PhotonNetwork.PlayerList.Length);

        // Null checks
        if (playerListPanel == null)
        {
            Debug.LogError("❌ playerListPanel is NULL! Assign it in the Inspector.");
            return;
        }

        if (playerListItemPrefab == null)
        {
            Debug.LogError("❌ playerListItemPrefab is NULL! Assign it in the Inspector.");
            return;
        }

        // Clear previous player list
        foreach (Transform child in playerListPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab, playerListPanel);
            TMP_Text playerText = playerItem.GetComponentInChildren<TMP_Text>();

            if (playerText != null)
            {
                playerText.text = player.NickName + (player.IsMasterClient ? " (Host)" : "");
                Debug.Log("👤 Player Found: " + player.NickName + " | ID: " + player.ActorNumber);
            }
            else
            {
                Debug.LogError("❌ TMP_Text not found in playerListItemPrefab! Make sure it has a TextMeshPro component.");
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
            Debug.Log("🚪 Lobby is now closed.");
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("🎮 Starting game...");
            PhotonNetwork.LoadLevel("MainBoard"); // Change "MainBoard" to your actual game scene name
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
        Debug.Log("👥 Player Joined: " + newPlayer.NickName);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("🚪 Player Left: " + otherPlayer.NickName);
        UpdatePlayerList();
    }
}
