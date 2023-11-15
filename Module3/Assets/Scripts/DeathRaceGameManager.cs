using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathRaceGameManager : RacingGameManager
{
    public GameObject killFeedItemPrefab;
    public GameObject killFeedParent;

    public GameObject eliminatedText;

    public GameObject lastPlayerGO;
    public TMP_Text lastPlayerText;

    public int finishOrder;

    public static DeathRaceGameManager instance = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
