using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class FrozenQueen : MonoBehaviourPunCallbacks
{
    private  void OnCollisionEnter(Collision collision)
    {
        Commoner touchedCommoner = collision.gameObject.GetComponent<Commoner>();

        if(touchedCommoner != null)
        {
            if(this.photonView.Owner.ActorNumber != touchedCommoner.photonView.Owner.ActorNumber && touchedCommoner.isFrozen == false)
            {
                touchedCommoner.FreezeCommonerRPC();
                photonView.RPC(nameof(SpawnKillFeed), RpcTarget.All, touchedCommoner.photonView.Owner.NickName);
            } 
        }
    }

    [PunRPC]
    public void SpawnKillFeed(string touchedObject)
    {
        GameObject killFeed = Instantiate(NormalModeGameManager.instance.killFeedItemPrefab);
        killFeed.GetComponent<KillFeedItem>().ChangeKillFeedName(photonView.Owner.NickName, touchedObject, "Freeze");
        killFeed.transform.SetParent(NormalModeGameManager.instance.killFeedParent.transform);
        killFeed.transform.localScale = Vector3.one;
        Destroy(killFeed, 5);
    }
}
