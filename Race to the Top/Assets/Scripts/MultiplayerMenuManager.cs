using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class MultiplayerMenuManager : MonoBehaviourPunCallbacks
{
    public void HostGameDirectly()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("🔌 Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(WaitForConnectionThenCreateRoom());
        }
        else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            Debug.Log("🟢 Already connected. Creating Room...");
            CreateRoom();
        }
        else
        {
            Debug.LogError("⚠️ Cannot create room yet. Waiting for Master Server connection...");
            StartCoroutine(WaitForConnectionThenCreateRoom());
        }
    }

    private IEnumerator WaitForConnectionThenCreateRoom()
    {
        Debug.Log("⏳ Waiting for Photon to connect...");

        // Wait until Photon is fully connected to the Master Server
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady 
                                        && PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);

        Debug.Log("🟢 Connected to Master Server! Creating Room...");
        CreateRoom();
    }

    private void CreateRoom()
    {
        string roomCode = GenerateRoomCode();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        // Store room code as a custom property
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties["roomCode"] = roomCode;
        roomOptions.CustomRoomProperties = roomProperties;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "roomCode" };

        Debug.Log("🟢 Creating Room with Code: " + roomCode);
        PhotonNetwork.CreateRoom(roomCode, roomOptions);
    }

    private string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] codeArray = new char[6];
        for (int i = 0; i < codeArray.Length; i++)
        {
            codeArray[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(codeArray);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("✅ Room Created: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("LobbyScene"); // ✅ Go directly to the lobby
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("❌ Room creation failed: " + message);
    }

    public void GoToJoinGameMenu()
    {
        Debug.Log("➡️ Loading Join Game Menu...");
        SceneManager.LoadScene("JoinGameMenu");
    }

    public void GoToOfflineMode()
    {
        Debug.Log("🚀 Offline Mode (Not implemented yet).");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu"); // Back to Main Menu
    }
}
