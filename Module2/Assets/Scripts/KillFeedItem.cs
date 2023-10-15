using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour
{
    public TMP_Text killerName;
    public TMP_Text killedName;

    public void ChangeKillFeedName(string killer, string killed)
    {
        killerName.text = killer;
        killedName.text = killed;
    }
}
