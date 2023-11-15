using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public TMP_Text playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        playerNameText.text = photonView.Owner.NickName;

        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;
            playerNameText.enabled = !photonView.IsMine;
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            GetComponent<Shooting>().enabled = photonView.IsMine;
            GetComponent<DeathRacePlayer>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;
            playerNameText.enabled = !photonView.IsMine;
        }
    }

    
}
