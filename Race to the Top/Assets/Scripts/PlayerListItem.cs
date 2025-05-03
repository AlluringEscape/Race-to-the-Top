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

        // Show Kick button only for host and not for self
        if (PhotonNetwork.IsMasterClient && !player.IsLocal)
        {
            kickButton.gameObject.SetActive(true);
        }
        else
        {
            kickButton.gameObject.SetActive(false);
        }

        // Attach button logic
        kickButton.onClick.AddListener(KickPlayer);
    }

    public void KickPlayer()
    {
        if (PhotonNetwork.IsMasterClient && player != null)
        {
            PhotonNetwork.CloseConnection(player);
        }
    }
}
