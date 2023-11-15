using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    private enum ShootingStyle
    {
        RAYCAST,
        PROJECTILE
    }

    public float gunDmg;

    public GameObject missilePrefab;

    [SerializeField] private ShootingStyle shootStyle;

    public GameObject fireStartingPoint;

    public GameObject hitEffectPrefab;

    public float timeBeforeShoot;
    public float currentTimeBeforeShoot;
    public bool isAbleToFire = true;

    public LayerMask layerMaskToHit;

    void Start()
    {
        currentTimeBeforeShoot = timeBeforeShoot;
    }

    public void Shoot()
    {
        if(isAbleToFire)
        {
            if(shootStyle == ShootingStyle.RAYCAST)
            {
                RayCastFire();
            }
            else if(shootStyle == ShootingStyle.PROJECTILE)
            {
                ProjectileFire();
            }

            isAbleToFire = false;
        }
    }

    public void ProjectileFire()
    {
        PhotonNetwork.Instantiate(missilePrefab.name, fireStartingPoint.transform.position, fireStartingPoint.transform.rotation)
        .GetComponent<Missile>().InitializeValue(this.photonView.Owner.NickName, gunDmg);
    }

    public void RayCastFire()
    {
        
        RaycastHit hit;
        Ray ray = new Ray(fireStartingPoint.transform.position, fireStartingPoint.transform.forward);

        if(Physics.Raycast(ray, out hit, layerMaskToHit))
        {
            Debug.Log(hit.collider.gameObject.name);

            if(hit.collider.gameObject.GetComponent<DeathRacePlayer>() != null 
            && !hit.collider.gameObject.GetComponent<DeathRacePlayer>().photonView.IsMine)
            {
                hit.collider.gameObject.GetComponent<DeathRacePlayer>().TakeDamageRPC(gunDmg, this.photonView.Owner.NickName);
                photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);
                Debug.Log("hit success");
            }
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
            GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(hitEffectGameObject, 0.5f);
    }
}
