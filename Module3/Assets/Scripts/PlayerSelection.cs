using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{
    public GameObject[] SelectablePlayers;

    public int PlayerSelectionNumber;

    // Start is called before the first frame update
    void Start()
    {
        PlayerSelectionNumber = 0;

        ActivatePlayer(PlayerSelectionNumber);

        ExitGames.Client.Photon.Hashtable PlayerSelectionProperties = new ExitGames.Client.Photon.Hashtable() 
        { {Constants.PLAYER_SELECTION_NUMBER, PlayerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerSelectionProperties);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActivatePlayer(int x)
    {
        foreach(GameObject go in SelectablePlayers)
        {
            go.SetActive(false);
        }

        SelectablePlayers[x].SetActive(true);
    }

    public void GoToNextPlayer()
    {
        PlayerSelectionNumber++;

        if(PlayerSelectionNumber >= SelectablePlayers.Length)
        {
            PlayerSelectionNumber = 0;
        }

        ActivatePlayer(PlayerSelectionNumber);

        //Setting the player selection for vehicle
        ExitGames.Client.Photon.Hashtable PlayerSelectionProperties = new ExitGames.Client.Photon.Hashtable() 
        { {Constants.PLAYER_SELECTION_NUMBER, PlayerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerSelectionProperties);
    }

    public void GoToPrevPlayer()
    {
        PlayerSelectionNumber--;

        if(PlayerSelectionNumber < 0)
        {
            PlayerSelectionNumber = SelectablePlayers.Length - 1;
        }

        ActivatePlayer(PlayerSelectionNumber);

        //Setting the player selection for vehicle
        ExitGames.Client.Photon.Hashtable PlayerSelectionProperties = new ExitGames.Client.Photon.Hashtable() 
        { {Constants.PLAYER_SELECTION_NUMBER, PlayerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerSelectionProperties);
    }
}
