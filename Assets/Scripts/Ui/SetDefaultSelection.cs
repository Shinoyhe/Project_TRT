using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultSelection : MonoBehaviour
{
    public Button DefaultButton;
    public bool TriggerOnClick = false;
    public bool OnlyFirstBoot = false;

    private bool firstBootTracker = false;

    private void OnEnable() {

        if (DefaultButton != null) {

            if (OnlyFirstBoot == true && firstBootTracker == true) return;

            DefaultButton.Select();
            if (TriggerOnClick) {
                DefaultButton.onClick.Invoke();
            }
            var buttonHandler = DefaultButton.gameObject.GetComponent<BoldTextOnHover>();
            if (buttonHandler) {
                buttonHandler.TriggerBold();
            }

            firstBootTracker = true;
        }
    }
}
