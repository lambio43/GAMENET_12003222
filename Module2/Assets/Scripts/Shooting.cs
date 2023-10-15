using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.VisualScripting;
using TMPro;
using Photon.Pun.UtilityScripts;
using System;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public GameObject hitEffectPrefab;

    [Header ("Hp related stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    public TMP_Text nameText;

    private Animator animator;

    public int killCount;
    public bool isAlive = true;
    public bool isSpawning = false;

    public GameObject killFeedItemPrefab;
    public GameObject killFeedParent;

    void Start()
    {
        health = startHealth;
        healthBar .fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();
        nameText.text = photonView.Owner.NickName;
        killFeedParent = GameManager.instance.killFeed;
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);
            
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if(hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);

                if(hit.collider.gameObject.GetComponent<Shooting>().isAlive == false &&
                hit.collider.gameObject.GetComponent<Shooting>().isSpawning == false)
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("ChangeIsSpawningValue", RpcTarget.AllBuffered, true);
                    photonView.RPC("IncreaseKillCount", RpcTarget.AllBuffered, this.photonView.Owner.NickName);
                    photonView.RPC("AddKillFeed", RpcTarget.All, this.photonView.Owner.NickName, 
                    hit.collider.gameObject.GetComponent<Shooting>().photonView.Owner.NickName);
                }
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {   
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if(health <= 0) //Die
        {
            photonView.RPC("Die", RpcTarget.All);
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
            GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(hitEffectGameObject, 0.2f);
    }

    [PunRPC]
    public void Die()
    {
        isAlive = false;
        if(photonView.IsMine && isSpawning == false)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("Respawn Text");
        float respawnTime = 5.0f;
        while(respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovement>().enabled = false;
            respawnText.GetComponent<TMP_Text>().text = "You are Killed. Respawning in: " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<TMP_Text>().text = "";

        this.transform.position = SpawnManager.instance.RandomSpawnPoint();
        transform.GetComponent<PlayerMovement>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
        isAlive = true;
        photonView.RPC("ChangeIsSpawningValue", RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    public void IncreaseKillCount(string killerName)
    {
        killCount++;

        if(killCount == 10) //Change to 10
        {
          GameManager.instance.WinGame();
          GameManager.instance.DisplayWinUI(killerName);
        }
    }

    [PunRPC]
    public void AddKillFeed(string killerName, string killedName)
    {
        GameObject killFeed = Instantiate(killFeedItemPrefab);
        killFeed.GetComponent<KillFeedItem>().ChangeKillFeedName(killerName, killedName);
        killFeed.transform.SetParent(killFeedParent.transform);
        killFeed.transform.localScale = Vector3.one;
        Destroy(killFeed, 5);
    }

    [PunRPC]
    public void ChangeIsSpawningValue(bool val)
    {
        isSpawning = val;
    }
}
