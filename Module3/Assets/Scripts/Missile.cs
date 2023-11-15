using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviourPunCallbacks
{
    public string owner;
    public float dmg;
    public float moveSpeed;
    public GameObject hitEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("DestroySelfStart", RpcTarget.All, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); 
    }

    private void OnTriggerEnter(Collider collider)
    {
        DeathRacePlayer enemyPlayer = collider.GetComponent<DeathRacePlayer>();

        if(enemyPlayer != null && enemyPlayer.photonView.Owner.NickName != owner)
        {
            enemyPlayer.TakeDamageRPC(dmg, owner);
            photonView.RPC("DestroySelf", RpcTarget.All);
            photonView.RPC("CreateHitEffects", RpcTarget.All, transform.position);
        }
    }

    public void InitializeValue(string nickname, float dmgValue)
    {
        owner = nickname;
        dmg = dmgValue;
    }

    [PunRPC]
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    [PunRPC]
    public void DestroySelfStart(float delay)
    {
        Destroy(this.gameObject, delay);
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
            GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(hitEffectGameObject, 0.5f);
    }
}
