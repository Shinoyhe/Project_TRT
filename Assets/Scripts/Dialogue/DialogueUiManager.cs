using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Ink.Runtime;
using System.Collections;
using static DialogueManager;

/// <summary>
/// Manager to display text on screen.
/// No Access to Ink files.
/// </summary>
public class DialogueUiManager : MonoBehaviour {

    // Parameters =================================================================================

    [Header("Dependencies")]
    public List<Button> UiButtons;
    public List<TMP_Text> UiButtonsText;
    public Canvas RenderCanvas;
    public GameObject SpeechBubblePrefab;

    [Header("Speaking Settings")]
    public float TextSpeed = 0.05f;

    [HideInInspector]
    public delegate void CallAfterLineFinished();

    // Misc Internal Variables ====================================================================
    private TMP_Text _currentTextBox;
    private GameObject _npcSpeechBubble;
    private GameObject _playerSpeechBubble;

    private class LineInformation {
        public int TotalCharacters = 0;
        public bool FinishedTyping = false;
        public CallAfterLineFinished Callback = null;
    }
    private LineInformation _currentLineData;

    // Public Utility Methods ====================================================================

    /// <summary>
    /// Setup UI with two speakers.
    /// </summary>
    /// <param name="speakerA"></param>
    /// <param name="speakerB"></param>
    public void SetupUi(Vector3 npcBubblePos, Vector3 playerBubblePos) {

        _npcSpeechBubble = CreateSpeechBubble(npcBubblePos);
        _playerSpeechBubble = CreateSpeechBubble(playerBubblePos);

        HideChoices();
    }

    /// <summary>
    /// Type out a line of text.
    /// </summary>
    /// <param name="text"> Text to display. </param>
    /// <param name="speakerName"> Name of speaker saying line. </param>
    /// <param name="callAfterLineFinished"> Callback after line of text is fully displayed.</param>
    public void DisplayLineOfText(String text, ProcessedTags foundTags, CallAfterLineFinished callAfterLineFinished = null) {

        _npcSpeechBubble.SetActive(false);
        _playerSpeechBubble.SetActive(false);

        if (foundTags.isNpcTalking) {
            _npcSpeechBubble.SetActive(true);
            _currentTextBox = _npcSpeechBubble.GetComponentInChildren<TMP_Text>();
        } else {
            _playerSpeechBubble.SetActive(true);
            _currentTextBox = _playerSpeechBubble.GetComponentInChildren<TMP_Text>();
        }

        _currentTextBox.maxVisibleCharacters = 0;
        _currentTextBox.text = text;

        _currentLineData = new LineInformation();
        _currentLineData.TotalCharacters = text.Length;
        _currentLineData.Callback = callAfterLineFinished;

        // Start Printing
        StartCoroutine("NextCharacter");
    }

    /// <summary>
    /// Skip the typing animation of the current line of text.
    /// </summary>
    public void SkipLineAnimation() {
        StopCoroutine("NextCharacter");
        _currentTextBox.maxVisibleCharacters = _currentLineData.TotalCharacters;
        EndLineOfText();
    }

    /// <summary>
    /// Create Dialogue Choices from array
    /// </summary>
    /// <param name="choices"> The choices to create buttons for. </param>
    public void SetupChoices(List<Choice> choices) {

        int numberOfOptions = choices.Count;
        if (numberOfOptions > UiButtons.Count) return;

        // Reset buttons
        HideChoices();

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

    /// <summary>
    /// Hide all UI buttons in scene.
    /// </summary>
    public void HideChoices() {
        // Reset buttons
        foreach (Button button in UiButtons) {
            button.gameObject.SetActive(false);
        }
    }

    public bool IsLineFinished() {
        return _currentLineData.FinishedTyping;
    }

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Display next character in dialogue string.
    /// </summary>
    IEnumerator NextCharacter() {

        int index = Mathf.Clamp(_currentTextBox.maxVisibleCharacters, 0, _currentTextBox.text.Length - 1);

        char currentCharacter = _currentTextBox.text[index];

        float actualTextSpeed = TextSpeed;

        if (currentCharacter == '.') {
            actualTextSpeed *= 5;
        }

        yield return new WaitForSeconds(actualTextSpeed);

        // Play sound every other character or if a punctuation
        if (currentCharacter == '.' || _currentTextBox.maxVisibleCharacters % 2 == 0) {
            //playTalkSound(currentCharacter);
        }

        // Show next character
        _currentTextBox.maxVisibleCharacters += 1;

        if (_currentTextBox.maxVisibleCharacters >= _currentLineData.TotalCharacters) {
            EndLineOfText();
        } else {
            StartCoroutine("NextCharacter");
        }
    }

    /// <summary>
    /// Calls once line of text finishes displaying.
    /// </summary>
    void EndLineOfText() {
        if (_currentLineData.FinishedTyping) return; // Don't double end

        _currentLineData.FinishedTyping = true;

        if (_currentLineData.Callback != null) {
            _currentLineData.Callback();
        }
    }

    GameObject CreateSpeechBubble(Vector3 worldPos) {

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);

        Vector2 canvasResolution = RenderCanvas.GetComponent<CanvasScaler>().referenceResolution;
        Vector2 canvasPos = new Vector2(viewportPos.x * canvasResolution.x, viewportPos.y * canvasResolution.y);

        return Instantiate(SpeechBubblePrefab, canvasPos, Quaternion.identity, RenderCanvas.transform);
    }
}
