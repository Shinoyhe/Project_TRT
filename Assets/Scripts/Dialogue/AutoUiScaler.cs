using TMPro;
using UnityEngine;

public class AutoUiScaler : MonoBehaviour {
    public TMP_Text TextForScale;
    public RectTransform RectTransform;
    public float Padding = 50;
    public float LineLength = 700;

    private bool _readyToRescale = false;
    private TMP_TextInfo _info;

    // Start is called before the first frame update
    void Start() {
        _info = TextForScale.textInfo;
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

        if (_info.lineCount == 0) return;
        float textWidth = _info.lineInfo[_info.lineCount - 1].width;
        float textActualWidth = TextForScale.renderedWidth;
        float textHeight = TextForScale.renderedHeight;

        if (textWidth >= 1) {
            // Debug.Log(textWidth);
            if (textWidth > LineLength) {
                RectTransform.sizeDelta = new Vector2(textActualWidth + Padding, textHeight + Padding);
            } else {
                RectTransform.sizeDelta = new Vector2(textActualWidth + Padding, RectTransform.sizeDelta.y);
            }
        }
       /* if (textHeight >= 1) {
            // RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, textHeight + Padding);
        }*/
    }
}
