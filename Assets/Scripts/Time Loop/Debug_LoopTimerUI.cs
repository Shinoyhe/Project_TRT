using UnityEngine;
using TMPro;

public class Debug_LoopTimerUI : MonoBehaviour
{
    [Header("Percentage Label")]
    [SerializeField, Tooltip("A prefix added to the start of our printed string.")]
    private string prefix;
    [SerializeField, Tooltip("A suffix added to the end of our printed string.")]
    private string suffix;

    private TMP_Text _textObject;

    private void Start()
    {
        _textObject = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        float s = Mathf.Max(0, GameManager.TimeLoopManager.SecondsLeft);
        string timeString = $"{Mathf.Floor(s/60f):00}:{Mathf.Floor(s%60):00}";

        // Percentage label
        _textObject.text = prefix + timeString + suffix;
    }
}

    