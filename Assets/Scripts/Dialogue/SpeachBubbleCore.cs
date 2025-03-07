using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleCore : MonoBehaviour
{
    [Header("Possible Assets")]
    public Sprite PlayerTalking;
    public Sprite NPCTalking;

    [Header("Dependencies")]
    public TMP_Text Text;
    public Image Image;

    public void Init(bool IsNPCTalking) {
        if (IsNPCTalking) {
            Image.sprite = NPCTalking;
        } else {
            Image.sprite = PlayerTalking;
        }
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }
    public void Show() {
        this.gameObject.SetActive(true);
    }
}
