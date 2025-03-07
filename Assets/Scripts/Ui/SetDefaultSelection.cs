using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultSelection : MonoBehaviour
{
    public Button DefaultButton;
    public bool TriggerOnClick = false;

    private void OnEnable() {

        if (DefaultButton != null) {
            DefaultButton.Select();
            if (TriggerOnClick) {
                DefaultButton.onClick.Invoke();
            }
            var buttonHandler = DefaultButton.gameObject.GetComponent<BoldTextOnHover>();
            if (buttonHandler) {
                buttonHandler.TriggerBold();
            }
        }
    }
}
