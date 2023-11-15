using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VehicleMovement : MonoBehaviourPunCallbacks
{
    public float speed = 20;
    public float rotationSpeed = 200;
    public float currentSpeed = 0;

    public bool isControlEnabled;

    public bool isPlayerFinish = false;

    public Shooting shooting;

    void Start()
    {
        isControlEnabled = false;
        shooting = this.GetComponent<Shooting>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(isControlEnabled)
        {
            float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

            transform.Translate(0, 0, translation);
            currentSpeed = translation;

            transform.Rotate(0, rotation, 0);
        }
    }

    void Update()
    {
        if(shooting != null)
        {
                if(Input.GetKey(KeyCode.J) && shooting.isAbleToFire == true)
            {
                shooting.Shoot();
            } 
        
            if(shooting.isAbleToFire == false)
            {
                shooting.currentTimeBeforeShoot -= Time.deltaTime;

                if(shooting.currentTimeBeforeShoot <= 0)
                {
                    shooting.currentTimeBeforeShoot = shooting.timeBeforeShoot;
                    shooting.isAbleToFire = true;
                }
            }
        }
    }

    public void ChangeIsPlayerFinish(bool isFinish)
    {
        photonView.RPC("ChangeIsPlayerFinishRPC", RpcTarget.AllBuffered, isFinish);
    }

    [PunRPC]
    public void ChangeIsPlayerFinishRPC(bool isFinish)
    {
        isPlayerFinish = isFinish;
    }
}
