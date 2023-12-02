using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using Photon.Pun.Demo.PunBasics;
using Unity.VisualScripting;

public class EscapePath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Commoner>() != null)
        {
            bool isFrozenQueenWin = false;

            object[] data = new object[] {isFrozenQueenWin};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = false
            };

            PhotonNetwork.RaiseEvent((byte) RaiseEventCodes.FinishGame, data, raiseEventOptions, sendOptions);
        }
    }
}
