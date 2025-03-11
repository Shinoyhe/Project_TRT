using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class BoldTextOnHover : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TMP_Text TextToBold;

    private void OnDisable() {
        TextToBold.fontStyle = FontStyles.Normal;
    }

    public void OnDeselect(BaseEventData eventData) {
        TextToBold.fontStyle = FontStyles.Normal;
    }

    public void OnSelect(BaseEventData eventData) {
        TextToBold.fontStyle = FontStyles.UpperCase;
    }

    public void TriggerBold() {
        TextToBold.fontStyle = FontStyles.UpperCase;
    }
}
