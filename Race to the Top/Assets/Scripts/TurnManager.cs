using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public Text turnText; // UI Element to display turn info
    private int currentTurn = 0; // Tracks whose turn it is
    private int totalPlayers;
    private Player currentPlayer; // Tracks the current player

    void Start()
    {
        totalPlayers = PhotonNetwork.PlayerList.Length;

        if (PhotonNetwork.PlayerList.Length > 0)
        {
            currentPlayer = PhotonNetwork.PlayerList[currentTurn]; // Set first player as starter
        }
        else
        {
            Debug.LogError("No players in room!");
        }

        UpdateTurnUI();
    }

    public void RollDice()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == currentTurn)
        {
            int roll = Random.Range(1, 7);
            Debug.Log("Player " + PhotonNetwork.LocalPlayer.NickName + " rolled: " + roll);
            photonView.RPC("MovePlayer", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, roll);
        }
        else
        {
            Debug.Log("Not your turn!");
        }
    }

    [PunRPC]
    void MovePlayer(int playerID, int steps)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
        {
            BoardManager.Instance.MovePlayer(steps);
        }

        NextTurn();
    }

    void NextTurn()
    {
        currentTurn = (currentTurn + 1) % totalPlayers;
        currentPlayer = PhotonNetwork.PlayerList[currentTurn]; // Update to next player
        UpdateTurnUI();
    }

    public void UpdateTurnUI()
    {
        if (turnText != null && currentPlayer != null)
        {
            turnText.text = "Current Turn: " + currentPlayer.NickName;
        }
        else
        {
            Debug.LogError("Turn UI or Current Player is null!");
        }
    }
}
