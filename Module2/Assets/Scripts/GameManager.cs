using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public GameObject killFeed;

    public Camera worldCamera;
    public GameObject winUi;
    public TMP_Text playerWinNameText;

    public static GameManager instance;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            StartCoroutine(DelayedPlayerSpawn());
        }
    }

    IEnumerator DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(3);
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, SpawnManager.instance.RandomSpawnPoint(), Quaternion.identity);
        killFeed.SetActive(true);
        worldCamera.gameObject.SetActive(false);
    }

    public void WinGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        worldCamera.gameObject.SetActive(true);

        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(6f);

        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        Debug.Log("End Game");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("On Left Room");
        SceneManager.LoadScene("LobbyScene");
    }

    public void DisplayWinUI(string playerWinnerName)
    {
        winUi.SetActive(true);
        playerWinNameText.text = playerWinnerName;
    }
}
