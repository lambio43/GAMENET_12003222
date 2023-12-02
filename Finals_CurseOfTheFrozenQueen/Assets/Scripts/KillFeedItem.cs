using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour
{
    public TMP_Text killerName;
    public TMP_Text killedName;
    public TMP_Text whatHappenedText;

    public void ChangeKillFeedName(string killer, string killed, string action)
    {
        killerName.text = killer;
        killedName.text = killed;
        whatHappenedText.text = action;
    }
}