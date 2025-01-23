using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Ink.Runtime;

/// <summary>
/// Manager to display text on screen.
/// No Access to Ink files.
/// </summary>
public class DialogueUiManager : MonoBehaviour {
    [Header("Dependencies")]
    public TMP_Text SpeechTextA;
    public TMP_Text SpeechTextB;

    public List<Button> UiButtons;
    public List<TMP_Text> UiButtonsText;

    public GameObject SpeachBubbleA;
    public GameObject SpeachBubbleB;

    private string _speakerA;
    private string _speakerB;

    /// <summary>
    /// Setup UI with two speakers.
    /// </summary>
    /// <param name="speakerA"></param>
    /// <param name="speakerB"></param>
    public void SetupUi(string speakerA, string speakerB) {
        _speakerA = speakerA.ToLower().Trim();
        _speakerB = speakerB.ToLower().Trim();

        SpeachBubbleA.SetActive(false);
        SpeachBubbleB.SetActive(false);
    }

    /// <summary>
    /// Type out a line of text.
    /// </summary>
    /// <param name="text"> Text to display. </param>
    /// <param name="callback"> Callback after line of text is fully displayed.</param>
    public void DisplayLine(String text, string speakerName) {

        if(speakerName == null) {
            speakerName = _speakerA;
        }

        string formattedName = speakerName.ToLower().Trim();

        SpeachBubbleA.SetActive(_speakerA == formattedName);
        SpeachBubbleB.SetActive(_speakerB == formattedName);

        if (_speakerA == formattedName) {
            SpeechTextA.text = text;
        }

        if (_speakerB == formattedName) {
            SpeechTextB.text = text;
        }
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

        // Format positions
        if (numberOfOptions == 1) {
            UiButtons[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        }

        if (numberOfOptions == 2) {
            UiButtons[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-200,0,0);
            UiButtons[1].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3( 200,0,0);
        }

        if (numberOfOptions == 3) {
            UiButtons[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-400, 0, 0);
            UiButtons[1].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(   0, 0, 0);
            UiButtons[2].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3( 400, 0, 0);
        }
    }
}
