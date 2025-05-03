using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviour
{
    public TMP_Text playerNameText;
    public Button kickButton;

    private Player player;

    public void Setup(Player newPlayer)
    {
        player = newPlayer;
        playerNameText.text = player.NickName;

        if (PhotonNetwork.IsMasterClient && !player.IsLocal)
        {
            kickButton.gameObject.SetActive(true);
            kickButton.onClick.AddListener(KickPlayer);
        }
        else
        {
            kickButton.gameObject.SetActive(false);
        }
    }

    public void KickPlayer()
    {
        if (PhotonNetwork.IsMasterClient && player != null)
        {
            Debug.Log($"🦶 Marking Player as Kicked: {player.NickName}");

            // Mark player as kicked using custom property
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "IsKicked", true }
            };

            player.SetCustomProperties(props);
        }
    }
}
