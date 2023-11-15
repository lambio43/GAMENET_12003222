using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TMP_Text timerText;
    public GameObject timerBackground;

    public float timeToStartRace = 5f;


    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            timerText = RacingGameManager.instance.timeText;
            timerBackground = RacingGameManager.instance.timerBackground;
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            timerText = DeathRaceGameManager.instance.timeText;
            timerBackground = DeathRaceGameManager.instance.timerBackground;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.All, timeToStartRace);
            }
            else if(timeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if(time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
            timerBackground.SetActive(false);
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<VehicleMovement>().isControlEnabled = true;
        this.enabled = false;
    }
}
