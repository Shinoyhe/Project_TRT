using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultSelection : MonoBehaviour
{
    public Button DefaultButton;
    private void OnEnable() {

        if (DefaultButton != null) {
            DefaultButton.Select();
        }
    }
}
