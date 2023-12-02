using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using TMPro;

public class NormalModeGameManager : MonoBehaviourPunCallbacks
{
    public GameObject commonerPrefab;
    public GameObject frozenQueenPrefab;
    public Transform[] playerStartingPositions;
    public Transform queenStartingPositions;

    public static NormalModeGameManager instance = null;

    public int frozenPlayers;

    public int gateNumberToOpen;
    public GameObject[] escapeGates;

    public GameObject escapsedMessage;
    public GameObject caughtMessage;

    public GameObject killFeedItemPrefab;
    public GameObject killFeedParent;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventCodes.PlayersCanEscape) 
        {
            OpenGate();
        }
        if(photonEvent.Code == (byte)RaiseEventCodes.FinishGame)
        {
            object[] data = (object[]) photonEvent.CustomData;

            bool isFrozenQueenWin = (bool)data[0];

            if(isFrozenQueenWin == true)
            {
                EndGameCaught();
            }
            else if(isFrozenQueenWin == false)
            {
                EndGameEscaped();
            }
        }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            StartCoroutine(DelayedPlayerSpawn());
        }

        gateNumberToOpen = Random.Range(0, escapeGates.Length);
    }

    IEnumerator DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(0.001f);
        object playerRoleNumber;

            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_ROLE, out playerRoleNumber))
            {
                Debug.Log((int)playerRoleNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                Vector3 instantiatePosition;

                if((int)playerRoleNumber == 0)
                {
                    instantiatePosition = queenStartingPositions.position;
                    PhotonNetwork.Instantiate(frozenQueenPrefab.name, 
                        instantiatePosition, Quaternion.identity);
                }
                else
                {
                    if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("ZD"))
                    {
                        instantiatePosition = playerStartingPositions[Random.Range(0, playerStartingPositions.Length)].position;
                        PhotonNetwork.Instantiate(commonerPrefab.name, 
                        instantiatePosition, Quaternion.identity).GetComponent<Commoner>().isAbleToUnfreeze = false;
                    }
                    else
                    {
                        instantiatePosition = playerStartingPositions[Random.Range(0, playerStartingPositions.Length)].position;
                        PhotonNetwork.Instantiate(commonerPrefab.name, 
                        instantiatePosition, Quaternion.identity);
                    }
                }
            }
    }

    public void IncreaseFrozenPlayerCount()
    {
        frozenPlayers++;

        if(frozenPlayers >= PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            bool isFrozenQueenWin = true;

            object[] data = new object[] {isFrozenQueenWin};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = false
            };

            PhotonNetwork.RaiseEvent((byte) RaiseEventCodes.FinishGame, data, raiseEventOptions, sendOptions);
        }
    }

    public void DecreaseFrozenPlayerCount()
    {
        frozenPlayers--;
    }

    public void OpenGate()
    {
        escapeGates[gateNumberToOpen].SetActive(false);
    }

    public void EndGameCaught()
    {
        caughtMessage.SetActive(true);
    }

    public void EndGameEscaped()
    {
        escapsedMessage.SetActive(true);
    }
}
