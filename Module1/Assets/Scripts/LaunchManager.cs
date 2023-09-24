using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject EnterGamePanel;
    public GameObject ConncectionStatusPanel;
    public GameObject LobbyPanel;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        EnterGamePanel.SetActive(true);
        ConncectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + "Connected to Photon Servers");
        Debug.Log(PhotonNetwork.CloudRegion);
        ConncectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            EnterGamePanel.SetActive(false);
            ConncectionStatusPanel.SetActive(true);
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        CreateAndJoinRoom();
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + Random.Range(0, 10000);
        
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name + 
        ". Room has now " + PhotonNetwork.CurrentRoom.PlayerCount + " players");
    }
}
