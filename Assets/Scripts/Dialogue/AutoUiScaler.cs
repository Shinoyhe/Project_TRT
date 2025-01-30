using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoUiScaler : MonoBehaviour {
    public TMP_Text TextForScale;
    public RectTransform RectTransform;
    public float Padding = 100;

    private bool _readyToRescale = false;

    // Start is called before the first frame update
    void Start() {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ScaleToText);
    }

    /// <summary>
    /// Scale a Rect Transform to a text size.
    /// </summary>
    /// <param name="obj"> The TMP_Text that has updated. </param>
    void ScaleToText(Object obj) {
        if (obj != TextForScale) return;
        if (_readyToRescale) return;

        _readyToRescale = true;
    }
    private void Update() {

        if (_readyToRescale == false) return;

        float textHeight = TextForScale.renderedHeight;

        if (textHeight < 1) return;

        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, textHeight + Padding); 
    }
}
