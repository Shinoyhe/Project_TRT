using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleCore : MonoBehaviour
{
    [Header("Dependencies")]
    public TMP_Text Text;
    public Image Image;

    public void Hide() {
        this.gameObject.SetActive(false);
    }
    public void Show() {
        this.gameObject.SetActive(true);
    }
}
