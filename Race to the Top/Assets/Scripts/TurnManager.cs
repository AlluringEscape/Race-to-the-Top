using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public Text turnText;
    private int currentTurn = 0;
    private int totalPlayers;

    void Start()
    {
        totalPlayers = PhotonNetwork.PlayerList.Length;
        UpdateTurnUI();
    }

    public void RollDice()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == currentTurn)
        {
            int roll = Random.Range(1, 7);
            Debug.Log("Player " + PhotonNetwork.LocalPlayer.ActorNumber + " rolled: " + roll);
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
        BoardManager.Instance.MovePlayer(steps);
        NextTurn();
    }

    void NextTurn()
    {
        currentTurn = (currentTurn + 1) % totalPlayers;
        UpdateTurnUI();
    }

    void UpdateTurnUI()
    {
        turnText.text = "Player " + (currentTurn + 1) + "'s Turn";
    }
}
