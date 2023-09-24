using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TakingDamage : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image hpBarImg;

    private float startHp = 100;
    public float health;

    void Start()
    {
        health = startHp;
        UpdateHpBar();
    }

    [PunRPC]
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        Debug.Log(health);
        UpdateHpBar();
        if(health <= 0)
        {
            Die();
        }
    }

    [PunRPC]
    public void UpdateHpBar()
    {
        hpBarImg.fillAmount = health / startHp;
    }

    private void Die()
    {
        if(photonView.IsMine)
        {
            GameManager.instance.LeaveRoom();
        }
    }
}
