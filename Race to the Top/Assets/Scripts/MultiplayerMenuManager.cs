using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class MultiplayerMenuManager : MonoBehaviourPunCallbacks
{
    public GameObject joinByCodePanel;
    public TMP_InputField roomCodeInput;

    public void HostGame()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        StartCoroutine(WaitForMasterServerAndCreateRoom());
    }

    private IEnumerator WaitForMasterServerAndCreateRoom()
    {
        yield return new WaitUntil(() =>
            PhotonNetwork.IsConnectedAndReady &&
            PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer
        );

        CreateRoom();
    }

    private void CreateRoom()
    {
        string roomCode = GenerateRoomCode();

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 10,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "roomCode", roomCode } },
            CustomRoomPropertiesForLobby = new string[] { "roomCode" }
        };

        PhotonNetwork.CreateRoom(roomCode, roomOptions);
    }

    private string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] codeArray = new char[6];
        for (int i = 0; i < codeArray.Length; i++)
            codeArray[i] = chars[Random.Range(0, chars.Length)];

        return new string(codeArray);
    }

    // ✅ JOIN BUTTON
    public void ShowJoinByCodePanel()
    {
        joinByCodePanel.SetActive(true);
    }

    public void CancelJoin()
    {
        joinByCodePanel.SetActive(false);
    }

    public void JoinRoomByCode()
    {
        string code = roomCodeInput.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogError("Room code is empty.");
            return;
        }

        Debug.Log("Attempting to join room: " + code);
        StartCoroutine(WaitForLobbyThenJoin(code));
    }

    private IEnumerator WaitForLobbyThenJoin(string code)
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        yield return new WaitUntil(() =>
            PhotonNetwork.IsConnectedAndReady &&
            PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer
        );

        PhotonNetwork.JoinRoom(code);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
        SceneManager.LoadScene("LobbyScene");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void OfflineMode()
    {
        Debug.Log("Offline Mode (Not Implemented)");
    }
}
