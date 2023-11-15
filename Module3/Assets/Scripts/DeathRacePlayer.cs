using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class DeathRacePlayer : MonoBehaviourPunCallbacks
{
    public float maxHp;
    public float currentHp;
    public Shooting shooting;

    public string lastDmgDealer;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

/*
PUNRPC
hp - 1

RAISEEVENT

hp - 1

3 players

hp - 3

*/

    void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventCodes.WhoDieFinishEventCode) //&& GetComponent<VehicleMovement>().isPlayerFinish == true)
        {
                object[] data = (object[]) photonEvent.CustomData;

                string nickNameOfFinishedPlayer = (string)data[0];
                DeathRaceGameManager.instance.finishOrder = (int)data[1];
                int viewId = (int)data[2];

                Debug.Log(nickNameOfFinishedPlayer + " " + DeathRaceGameManager.instance.finishOrder);

                GameObject orderUiText = DeathRaceGameManager.instance.finisherTextUi[DeathRaceGameManager.instance.finishOrder - 1];
                orderUiText.SetActive(true);

                if(viewId == photonView.ViewID) //this is u
                {
                    orderUiText.transform.Find("FinishedPlaceNameText").GetComponent<TMP_Text>().text = nickNameOfFinishedPlayer + "(YOU)";
                    orderUiText.transform.Find("FinishedPlaceNameText").GetComponent<TMP_Text>().color = Color.red;
                    DeathRaceGameManager.instance.eliminatedText.SetActive(true);
                }
                else
                {
                    orderUiText.transform.Find("FinishedPlaceNameText").GetComponent<TMP_Text>().text = nickNameOfFinishedPlayer;
                }
        }
        else if (photonEvent.Code == (byte)RaiseEventCodes.FinishGameEventCode)
        {
            object[] data = (object[]) photonEvent.CustomData;

            DeathRaceGameManager.instance.lastPlayerText.text = (string)data[0];
            DeathRaceGameManager.instance.lastPlayerGO.SetActive(true);
        }
    }

    void Start()
    {
        DeathRaceGameManager.instance.finishOrder = PhotonNetwork.CurrentRoom.PlayerCount + 1;
        shooting = this.GetComponent<Shooting>();
    }

    public void TakeDamageRPC(float dmg, string dmgDealer)
    {
        photonView.RPC("TakeDamage", RpcTarget.AllBuffered, dmg, dmgDealer);
    }

    [PunRPC]
    public void TakeDamage(float dmg, string dmgDealer)
    {
        currentHp -= dmg;
        lastDmgDealer = dmgDealer;

        if(currentHp <= 0 && GetComponent<VehicleMovement>().isPlayerFinish == false)
        {
            GetComponent<VehicleMovement>().ChangeIsPlayerFinishRPC(true);
            DeathRaceGameManager.instance.finishOrder--;
            DeathRaceGameFinish();
        }
    }

    public void PlayerDeath()
    {
        GameObject killFeed = Instantiate(DeathRaceGameManager.instance.killFeedItemPrefab);
        killFeed.GetComponent<KillFeedItem>().ChangeKillFeedName(lastDmgDealer, photonView.Owner.NickName);
        killFeed.transform.SetParent(DeathRaceGameManager.instance.killFeedParent.transform);
        killFeed.transform.localScale = Vector3.one;
        Destroy(killFeed, 5);
    }

    public void DeathRaceGameFinish()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        StartCoroutine(StopControlDelay());
    
        string nickname = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        //event Data
        object[] data = new object[] { nickname, DeathRaceGameManager.instance.finishOrder, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventCodes.WhoDieFinishEventCode, data, raiseEventOptions, sendOptions);

        PlayerDeath();

        if(DeathRaceGameManager.instance.finishOrder == 2)
        {
            FinishGame();
        }
    }

    public void FinishGame()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        StartCoroutine(StopControlDelay());

        object[] data = new object[] {lastDmgDealer};

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventCodes.FinishGameEventCode, data, raiseEventOptions, sendOptions);
    }

    IEnumerator StopControlDelay()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<VehicleMovement>().enabled = false;
        GetComponent<Shooting>().enabled = false;
    }
}
