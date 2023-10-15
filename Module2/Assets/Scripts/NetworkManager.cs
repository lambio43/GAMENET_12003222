using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using WebSocketSharp;
using Photon.Realtime;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status Text")]
    public Text connectionStatusText;

    [Header("UI Login Panel")]
    public InputField playerNameInput;
    public GameObject loginUiPanel;

    [Header("Game Options Panel")]
    public GameObject gameOptionsPanel;

    [Header("Create Room Panel")]
    public GameObject createRoomPanel;
    public InputField roomNameInputField;
    public InputField playerCountInputField;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomPanel;

    [Header("Show Room List Panel")]
    public GameObject showRoomListPanel;

    [Header("Inside Room Panel")]
    public GameObject insideRoomPanel;
    public Text roomInfoText;
    public GameObject playerListItemPrefab;
    public GameObject playerListViewParent;
    public GameObject startGameButton;

    [Header("Room List Panel")]
    public GameObject roomListPanel;
    public GameObject roomItemPrefab;
    public GameObject roomListParent;


    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Functions

    void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
        ActivatePanel(loginUiPanel);

        PhotonNetwork.AutomaticallySyncScene = true; 
    }

    void Update()
    {
        connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region UI Callbacks
    public void OnLoginButtonClick()
    {
        string playerName = playerNameInput.text;

        if(playerName.IsNullOrEmpty())
        {
            Debug.LogWarning("Player name is invalid");
            return;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnCreateRoomButtonClick()
    {
        string roomName = roomNameInputField.text;

        if(string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(playerCountInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }   

    public void OnCancelButtonClick()
    {
        ActivatePanel(gameOptionsPanel);
    }

    public void OnShowRoomListButtonClick()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(showRoomListPanel);
    }

    public void OnBackButtonClick()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(gameOptionsPanel);
    }

    public void OnLeaverGameButtonClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinRandomRoomClick()
    {
        ActivatePanel(joinRandomRoomPanel);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartGameButtonClick()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    #endregion

    #region Pun Callback

    public override void OnConnected()
    {  
       Debug.Log("Connected to internet"); 
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has Connected to Photon Network");
        ActivatePanel(gameOptionsPanel);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insideRoomPanel);

        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + "Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/"
            + PhotonNetwork.CurrentRoom.MaxPlayers;

        if(playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab);
            playerItem.transform.SetParent(playerListViewParent.transform);
            playerItem.transform.localScale = Vector3.one;

            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

            playerListGameObjects.Add(player.ActorNumber, playerItem);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListGameObjects();

        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        foreach (RoomInfo info in roomList)
        {
            Debug.Log(info.Name);

            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if(cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                //update existing room info
                if(cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        foreach(RoomInfo info in cachedRoomList.Values)
        {
            GameObject listItem = Instantiate(roomItemPrefab);
            listItem.transform.SetParent(roomListParent.transform);
            listItem.transform.localScale = Vector3.one;

            listItem.transform.Find("RoomNameText").GetComponent<Text>().text = info.Name;
            listItem.transform.Find("RoomPlayersText").GetComponent<Text>().text = "Player count: " + info.PlayerCount
                + "/" +info.MaxPlayers;

            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnRoomJoinedClicked(info.Name));

            roomListGameObjects.Add(info.Name, listItem);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListGameObjects();
        cachedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
            roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + "Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/"
            + PhotonNetwork.CurrentRoom.MaxPlayers;

            GameObject playerItem = Instantiate(playerListItemPrefab);
            playerItem.transform.SetParent(playerListViewParent.transform);
            playerItem.transform.localScale = Vector3.one;

            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

            playerListGameObjects.Add(newPlayer.ActorNumber, playerItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + "Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/"
            + PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObjects[otherPlayer.ActorNumber]);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        foreach(var gameObject in playerListGameObjects.Values)
        {
            Destroy(gameObject);
        }

        playerListGameObjects.Clear();
        playerListGameObjects = null;

        ActivatePanel(gameOptionsPanel);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion

    #region Private Methods

    private void OnRoomJoinedClicked(string roomName)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListGameObjects()
    {
        foreach(var item in roomListGameObjects.Values)
        {
            Destroy(item);
        }

        roomListGameObjects.Clear();
    }

    #endregion

    #region Public Methods

    public void ActivatePanel(GameObject panelToActivate)
    {
        loginUiPanel.SetActive(panelToActivate.Equals(loginUiPanel));
        gameOptionsPanel.SetActive(panelToActivate.Equals(gameOptionsPanel));
        createRoomPanel.SetActive(panelToActivate.Equals(createRoomPanel));
        joinRandomRoomPanel.SetActive(panelToActivate.Equals(joinRandomRoomPanel));
        showRoomListPanel.SetActive(panelToActivate.Equals(showRoomListPanel));
        insideRoomPanel.SetActive(panelToActivate.Equals(insideRoomPanel));
        roomListPanel.SetActive(panelToActivate.Equals(roomListPanel));
    }

    #endregion

}
