using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class Commoner : MonoBehaviourPunCallbacks
{
    public GameObject iceBlockGO;

    public bool isFrozen;
    public bool isAbleToUnfreeze = true;

    [PunRPC]
    public void FreezeCommoner()
    {
        NormalModeGameManager.instance.IncreaseFrozenPlayerCount();
        iceBlockGO.SetActive(true);
        isFrozen = true;
        GetComponent<PlayerMovement>().enabled = false;
    }

    public void FreezeCommonerRPC()
    {
        photonView.RPC(nameof(FreezeCommoner), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void UnfreezeCommoner()
    {
        iceBlockGO.SetActive(false);
        isFrozen = false;
        GetComponent<PlayerMovement>().enabled = true;
        NormalModeGameManager.instance.DecreaseFrozenPlayerCount();
    }

    private  void OnCollisionEnter(Collision collision)
    {
        if(isAbleToUnfreeze == false)
        {
            return;
        } 

        Commoner touchedCommoner = collision.gameObject.GetComponent<Commoner>();

        if(touchedCommoner != null)
        {
            if(this.photonView.Owner.ActorNumber != touchedCommoner.photonView.Owner.ActorNumber && touchedCommoner.isFrozen == true)
            {
                touchedCommoner.photonView.RPC(nameof(UnfreezeCommoner), RpcTarget.AllBuffered);
                touchedCommoner.photonView.RPC(nameof(SpawnKillFeed), RpcTarget.AllBuffered, touchedCommoner.photonView.Owner.NickName);
            }   
        }
    }

    [PunRPC]
    public void SpawnKillFeed(string touchedObject)
    {
        GameObject killFeed = Instantiate(NormalModeGameManager.instance.killFeedItemPrefab);
        killFeed.GetComponent<KillFeedItem>().ChangeKillFeedName(photonView.Owner.NickName, touchedObject, "Unfreeze");
        killFeed.transform.SetParent(NormalModeGameManager.instance.killFeedParent.transform);
        killFeed.transform.localScale = Vector3.one;
        Destroy(killFeed, 5);
    }
}
