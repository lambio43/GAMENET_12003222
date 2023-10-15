using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints;

    public static SpawnManager instance;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public Vector3 RandomSpawnPoint()
    {
        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);

        return spawnPoints[randomSpawnPointIndex].transform.position;
    }
}
