using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomCodeText;
    public Transform playerListPanel;
    public GameObject playerListItemPrefab;
    public Button startGameButton;
    public Button closeLobbyButton;
    public Button backButton;

    private void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("❌ Not in a Photon Room! Returning to Main Menu...");
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("roomCode", out object roomCode))
        {
            roomCodeText.text = "Room Code:\n" + roomCode.ToString();
        }
        else
        {
            roomCodeText.text = "Room Code: ERROR";
        }

        startGameButton.interactable = PhotonNetwork.IsMasterClient;
        closeLobbyButton.interactable = PhotonNetwork.IsMasterClient;

        UpdatePlayerList();

        // Listen for custom property updates
        PhotonNetwork.NetworkingClient.EventReceived += OnPhotonEvent;
    }

    public void UpdatePlayerList()
    {
        Debug.Log("🔄 Updating player list... Total Players: " + PhotonNetwork.PlayerList.Length);

        foreach (Transform child in playerListPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (string.IsNullOrEmpty(player.NickName))
            {
                player.NickName = "Player_" + player.ActorNumber;
                Debug.Log("⚡ Assigned default name: " + player.NickName);
            }

            GameObject playerItem = Instantiate(playerListItemPrefab, playerListPanel);
            TMP_Text nameText = playerItem.transform.Find("PlayerNameText")?.GetComponent<TMP_Text>();
            Button kickButton = playerItem.transform.Find("KickButton")?.GetComponent<Button>();

            if (nameText != null)
            {
                nameText.text = player.NickName;
                if (player.IsMasterClient)
                {
                    nameText.text += " (Host)";
                }
            }

            if (kickButton != null)
            {
                bool isHost = PhotonNetwork.IsMasterClient;
                bool isNotSelf = !player.IsLocal;
                kickButton.gameObject.SetActive(isHost && isNotSelf);
                kickButton.onClick.AddListener(() => KickPlayer(player));
            }

            Debug.Log($"👤 Player Found: {player.NickName} | ID: {player.ActorNumber}");
        }
    }

    public void KickPlayer(Player player)
    {
        if (PhotonNetwork.IsMasterClient && player != null)
        {
            Debug.Log($"🦶 Marking Player as Kicked: {player.NickName}");

            Hashtable props = new Hashtable
            {
                { "IsKicked", true }
            };

            player.SetCustomProperties(props);
        }
    }

    private void OnPhotonEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCode.PropertiesChanged)
        {
            if (photonEvent.Parameters.TryGetValue(ParameterCode.TargetActorNr, out object actorNumberObj))
            {
                int actorNumber = (int)actorNumberObj;
                Player affectedPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

                if (affectedPlayer != null && affectedPlayer.IsLocal && affectedPlayer.CustomProperties.ContainsKey("IsKicked"))
                {
                    bool isKicked = (bool)affectedPlayer.CustomProperties["IsKicked"];
                    if (isKicked)
                    {
                        Debug.Log("🚫 You were kicked. Leaving room...");
                        PhotonNetwork.LeaveRoom();
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnPhotonEvent;
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
            Debug.Log("🎮 Starting game...");
            PhotonNetwork.LoadLevel("MainBoard");
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
        Debug.Log("✅ Player Joined: " + newPlayer.NickName);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("🚪 Player Left: " + otherPlayer.NickName);
        UpdatePlayerList();
    }
}
