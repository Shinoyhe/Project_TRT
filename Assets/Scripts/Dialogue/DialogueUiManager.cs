using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine.InputSystem;

/// <summary>
/// Manager to display text on screen.
/// No Access to Ink files.
/// </summary>
public class DialogueUiManager : MonoBehaviour {
    [Header("Dependencies")]
    public TMP_Text SpeechText;

    public List<Button> UiButtons;
    public List<TMP_Text> UiButtonsText;
    public TMP_Text SpeakerNameText;

    public Image SpeachBubbleSprite;
    public Image NameTagSprite;

    /// <summary>
    /// Type out a line of text.
    /// </summary>
    /// <param name="text"> Text to display. </param>
    /// <param name="callback"> Callback after line of text is fully displayed.</param>
    public void DisplayLine(String text) {
        SpeechText.text = text;
    }

    /// <summary>
    /// Skip the typing animation of the current line of text.
    /// </summary>
    public void SkipLineAnimation() {

    }

    /// <summary>
    /// Enable Option Buttons based on numberOfOptions.
    /// </summary>
    /// <param name="numberOfOptions"> Number of options to toggle on. </param>
    public void SetupOptions(List<Choice> choices) {
        int numberOfOptions = choices.Count;

        if (numberOfOptions > UiButtons.Count) return;

        // Reset buttons
        foreach (Button button in UiButtons) {
            button.gameObject.SetActive(false);
        }

        // Enable all needed buttons
        for (int i = 0; i < numberOfOptions; i++) {
            Button currentButton = UiButtons[i];
            currentButton.gameObject.SetActive(true);
            UiButtonsText[i].text = choices[i].text;
        }
    }

    /// <summary>
    /// Add speaker name to UI.
    /// </summary>
    public void AddSpeakerName(string speakerName) {
        SpeakerNameText.text = speakerName;
    }

    /// <summary>
    /// Update UI to match a NpcProfile struct.
    /// </summary>
    /// <param name="profile"> Struct to load into UI.</param>
    public void LoadNpcProfile(NpcProfile profile) {
        if (profile == null) return;

        // Load name tag properties
        NameTagSprite.sprite = profile.NameTagBackground;
        SpeakerNameText.color = profile.NameTextColor;

        // Load speach bubble properties
        SpeachBubbleSprite.sprite = profile.SpeachBubbleBackground;
        SpeechText.color = profile.SpeachTextColor;
    }
}
