using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Ink.Runtime;
using System.Collections;
using static DialogueUiManager;

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

    [Header("Speaking Settings")]
    public float TextSpeed = 0.05f;

    [HideInInspector]
    public delegate void CallAfterLineFinished();

    private string _speakerA;
    private string _speakerB;
    private TMP_Text _currentText;
    private int _charactersForThisLine;
    private bool _finishedTypingText;

    private CallAfterLineFinished currentCallBack;

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

        ClearButtons();
    }

    /// <summary>
    /// Type out a line of text.
    /// </summary>
    /// <param name="text"> Text to display. </param>
    /// <param name="callback"> Callback after line of text is fully displayed.</param>
    public void DisplayLineOfText(String text, string speakerName, CallAfterLineFinished callAfterLineFinished = null) {

        if (speakerName == null) {
            speakerName = _speakerA;
        }

        string formattedName = speakerName.ToLower().Trim();

        SpeachBubbleA.SetActive(false);
        SpeachBubbleB.SetActive(false);

        if (_speakerA == formattedName) {
            _currentText = SpeechTextA;
            SpeachBubbleA.SetActive(true);
        }

        if (_speakerB == formattedName) {
            _currentText = SpeechTextB;
            SpeachBubbleB.SetActive(true);
        }

        _currentText.maxVisibleCharacters = 0;
        _currentText.text = text;
        _charactersForThisLine = text.Length;

        currentCallBack = callAfterLineFinished;

        // Start Printing
        StartCoroutine("NextCharacter");
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
        ClearButtons();

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
            UiButtons[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-200, 0, 0);
            UiButtons[1].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(200, 0, 0);
        }

        if (numberOfOptions == 3) {
            UiButtons[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-400, 0, 0);
            UiButtons[1].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            UiButtons[2].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(400, 0, 0);
        }
    }

    public void ClearButtons() {
        // Reset buttons
        foreach (Button button in UiButtons) {
            button.gameObject.SetActive(false);
        }
    }

    IEnumerator NextCharacter() {

        int index = Mathf.Clamp(_currentText.maxVisibleCharacters, 0, _currentText.text.Length - 1);

        char currentCharacter = _currentText.text[index];

        float actualTextSpeed = TextSpeed;

        if (currentCharacter == '.') {
            actualTextSpeed *= 5;
        }

        yield return new WaitForSeconds(actualTextSpeed);

        // Play sound every other character or if a punctuation
        if (currentCharacter == '.' || _currentText.maxVisibleCharacters % 2 == 0) {
            //playTalkSound(currentCharacter);
        }

        // Show next character
        _currentText.maxVisibleCharacters += 1;

        if (_currentText.maxVisibleCharacters >= _charactersForThisLine) {
            EndLineOfText();
        } else {
            StartCoroutine("NextCharacter");
        }
    }

    void EndLineOfText() {
        _finishedTypingText = true;

        if (currentCallBack != null) {
            currentCallBack();
        }
    }
}
