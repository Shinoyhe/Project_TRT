using UnityEngine;
using TMPro;

public class Debug_LoopTimerUI : MonoBehaviour
{
    [Header("Percentage Label")]
    [SerializeField, Tooltip("A prefix added before our printed string, when the loop is unpaused.")]
    private string unpausedPrefix;
    [SerializeField, Tooltip("A prefix added before our printed string, when the loop is unpaused.")]
    private string pausedPrefix;
    [SerializeField, Tooltip("A suffix added to the end of our printed string.")]
    private string suffix;

    private TMP_Text _textObject;

    private void Start()
    {
        _textObject = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (TimeLoopManager.Instance != null) {
            float s = Mathf.Max(0, TimeLoopManager.SecondsLeft);
            string prefix = TimeLoopManager.LoopPaused ? pausedPrefix : unpausedPrefix;

            string timeString = $"{Mathf.Floor(s/60f):00}:{Mathf.Floor(s%60):00}";
            _textObject.text = prefix + timeString + suffix;
        } else {
            _textObject.text = "<size=110%>TimeLoopManager.Instance is null</size>";
        }
    }
}

    