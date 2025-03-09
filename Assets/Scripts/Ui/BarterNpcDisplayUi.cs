using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarterNpcDisplayUi : MonoBehaviour
{
    public Image npcPicture;
    public TMPro.TMP_Text npcName;

    public void UpdateData(NPCData data) {
        npcPicture.sprite = data.Icon;
        npcName.text = data.Name;
    }
}
