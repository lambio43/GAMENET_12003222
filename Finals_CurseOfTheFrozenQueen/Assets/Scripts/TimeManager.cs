using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public float totalSeconds;

    [Range(0, 5)]
    public int minuteTime;
    public int currentMinuteTime;

    [Range(0f, 60f)]
    public float secondsTime;
    public float currentSecondsTime;
     
    public TMP_Text minuteText;
    public TMP_Text secondsText;

    public GameObject escapeTextGO;

    public static TimeManager instance = null;

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
        if(photonEvent.Code == (byte)RaiseEventCodes.ReduceTime) 
        {
            //object[] data = (object[]) photonEvent.CustomData;
            totalSeconds -= Time.deltaTime;
            currentSecondsTime -= Time.deltaTime;

            if(currentSecondsTime <= 0)
            {
                currentMinuteTime--;
                currentSecondsTime = 60f;
                if(currentMinuteTime < 0)
                {
                    currentMinuteTime = 0;
                    currentSecondsTime = 0;
                }
            }
            SetTimeText();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        totalSeconds = (minuteTime * 60f) + secondsTime;
        currentMinuteTime = minuteTime;
        currentSecondsTime = secondsTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(totalSeconds > 0)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                object[] data = new object[] {};

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = false
                };

                PhotonNetwork.RaiseEvent((byte) RaiseEventCodes.ReduceTime, data, raiseEventOptions, sendOptions);
            }
        }
        else
        {
            StopTimer();
        }
    }

    public void SetTimeText()
    {
        minuteText.text = ((int)currentMinuteTime).ToString();
        secondsText.text = ((int)currentSecondsTime).ToString();
    }

    public void StopTimer()
    {   
        escapeTextGO.SetActive(true);
        minuteText.gameObject.SetActive(false);
        secondsText.gameObject.SetActive(false);

        object[] data = new object[] {};

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventCodes.PlayersCanEscape, data, raiseEventOptions, sendOptions);

        this.enabled = false;
    }
}
