using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject freeLookCamera;
    public GameObject thirdPersonControllerCamera;

    void Start()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("NM"))
        {
            GetComponent<PlayerMovement>().enabled = photonView.IsMine;
            thirdPersonControllerCamera.SetActive(photonView.IsMine);
            camera.enabled = photonView.IsMine;
            freeLookCamera.SetActive(photonView.IsMine);
            if(GetComponent<Commoner>() != null)
            {
                GetComponent<Commoner>().enabled = photonView.IsMine;
                GetComponent<Commoner>().isAbleToUnfreeze = true;
            }
            if(GetComponent<FrozenQueen>() != null)
            {
                GetComponent<FrozenQueen>().enabled = photonView.IsMine;
            }
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("ZD"))
        {
            GetComponent<PlayerMovement>().enabled = photonView.IsMine;
            thirdPersonControllerCamera.gameObject.SetActive(photonView.IsMine);
            camera.enabled = photonView.IsMine;
            freeLookCamera.SetActive(photonView.IsMine);
            if(GetComponent<Commoner>() != null)
            {
                GetComponent<Commoner>().enabled = photonView.IsMine;
                GetComponent<Commoner>().isAbleToUnfreeze = false;
            }
            if(GetComponent<FrozenQueen>() != null)
            {
                GetComponent<FrozenQueen>().enabled = photonView.IsMine;
            }
        }
    }

    
}
