using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class RacingGameManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;

    public static RacingGameManager instance = null;

    public TMP_Text timeText;
    public GameObject timerBackground;

    public List<GameObject> lapTriggers = new List<GameObject>();

    public GameObject[] finisherTextUi;

    public List<GameObject> playerRacers = new List<GameObject>();

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

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            StartCoroutine(DelayedPlayerSpawn());
        }

        foreach(GameObject go in finisherTextUi)
        {
            go.SetActive(false);
        }
    }

    IEnumerator DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(0.001f);
        object playerSelectionNumber;

            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 intantiatePosition = startingPositions[actorNumber - 1].position;
                playerRacers.Add(PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, 
                intantiatePosition, Quaternion.identity));
            }
    }
}
