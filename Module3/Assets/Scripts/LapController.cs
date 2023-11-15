using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> lapTriggers = new List<GameObject>();
    private int lapTriggerCount;

    [SerializeField] private int finishOrder = 0;

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
        if(photonEvent.Code == (byte)RaiseEventCodes.WhoFinishedEventCode)
        {
            object[] data = (object[]) photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);
            GameObject orderUiText = null;

            if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                orderUiText = RacingGameManager.instance.finisherTextUi[finishOrder - 1];
                orderUiText.SetActive(true);
            }
            else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                orderUiText = DeathRaceGameManager.instance.finisherTextUi[finishOrder - 1];
                orderUiText.SetActive(true);
            }
        
            if(viewId == photonView.ViewID) //this is u
            {
                orderUiText.transform.Find("FinishedPlaceNameText").GetComponent<TMP_Text>().text = nickNameOfFinishedPlayer + "(YOU)";
                orderUiText.transform.Find("FinishedPlaceNameText").GetComponent<TMP_Text>().color = Color.red;
            }
            else
            {
                orderUiText.transform.Find("FinishedPlaceNameText").GetComponent<TMP_Text>().text = nickNameOfFinishedPlayer;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                foreach(GameObject go in RacingGameManager.instance.lapTriggers)
                {
                    lapTriggers.Add(go);
                }
            }
            else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                foreach(GameObject go in DeathRaceGameManager.instance.lapTriggers)
                {
                    lapTriggers.Add(go);
                }
            }
        

        lapTriggerCount = lapTriggers.Count;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(lapTriggers.Contains(collider.gameObject) && collider.gameObject.tag != "FinishTrigger")
        {
            int indexOfTrigger = lapTriggers.IndexOf(collider.gameObject);

            lapTriggers[indexOfTrigger].SetActive(false);
            lapTriggerCount--;
        }

        if(collider.gameObject.tag == "FinishTrigger" && lapTriggerCount == 1)
        {
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        StartCoroutine(StopControlDelay());

        finishOrder++;

        GetComponent<VehicleMovement>().ChangeIsPlayerFinishRPC(true);

        string nickname = PhotonNetwork.LocalPlayer.NickName;
        int viewId = photonView.ViewID;

        //event Data
        object[] data = new object[] { nickname, finishOrder, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventCodes.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
    }

    IEnumerator StopControlDelay()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<VehicleMovement>().enabled = false;
    }
}
