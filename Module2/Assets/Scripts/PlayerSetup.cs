using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject nonFpsModel;

    public GameObject playerUIPrefab;

    public PlayerMovement playerMovement;
    public Camera fpsCamera;

    private Animator animator;

    public Avatar fpsAvatar;
    public Avatar nonFpsAvatar;

    private Shooting shooting;

    void Start()
    {
        playerMovement = this.GetComponent<PlayerMovement>();
        fpsModel.SetActive(photonView.IsMine);
        nonFpsModel.SetActive(!photonView.IsMine);
        animator = this.GetComponent<Animator>();
        animator.SetBool("isLocalPlayer", photonView.IsMine);
        animator.avatar = photonView.IsMine ? fpsAvatar : nonFpsAvatar;

        shooting = this.GetComponent<Shooting>();

        if(photonView.IsMine)
        {
            GameObject playerUI = Instantiate(playerUIPrefab);
            playerMovement.fixedTouchField = playerUI.transform.Find("RotationTouchFieldPanel").GetComponent<FixedTouchField>();
            playerMovement.joystick = playerUI.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();
            fpsCamera.enabled = true;

            playerUI.transform.Find("Fire Button").GetComponent<Button>().onClick.AddListener(()=> shooting.Fire());
            playerUI.transform.SetParent(shooting.gameObject.transform);
        }
        else
        {
            playerMovement.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
        }
    }
}
