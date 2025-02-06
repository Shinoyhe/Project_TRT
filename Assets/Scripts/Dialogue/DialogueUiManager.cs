using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Ink.Runtime;
using System.Collections;

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
    public delegate void CallAfterButtonPress(int X);

    // Misc Internal Variables ====================================================================
    private TMP_Text _currentTextBox;

    private SpeechBubble _npcBubble;
    private SpeechBubble _playerBubble;

    private class SpeechBubble {
        public GameObject GameObject;
        public TMP_Text Text;

        public void Hide() {
            GameObject.SetActive(false);
        }
        public void Show() {
            GameObject.SetActive(true);
        }
    }

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

        _npcBubble = new SpeechBubble();
        _playerBubble = new SpeechBubble();

        // Setup NPC bubble
        _npcBubble.GameObject = CreateSpeechBubble(npcBubblePos);
        _npcBubble.Text = _npcBubble.GameObject.GetComponentInChildren<TMP_Text>();

        // Setup Player bubble
        _playerBubble.GameObject = CreateSpeechBubble(playerBubblePos);
        _playerBubble.Text = _playerBubble.GameObject.GetComponentInChildren<TMP_Text>();

        HideChoices();
    }

    /// <summary>
    /// Type out a line of text.
    /// </summary>
    /// <param name="text"> Text to display. </param>
    /// <param name="speakerName"> Name of speaker saying line. </param>
    /// <param name="callAfterLineFinished"> Callback after line of text is fully displayed.</param>
    public void DisplayLineOfText(String text, DialogueManager.ProcessedTags foundTags, CallAfterLineFinished callAfterLineFinished = null) {

        _npcBubble.Hide();
        _playerBubble.Hide();

        if (foundTags.isNpcTalking) {
            _npcBubble.Show();
            _currentTextBox = _npcBubble.Text;
        } else {
            _playerBubble.Show();
            _currentTextBox = _playerBubble.Text;
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

        if (choices == null) {
            Debug.LogError("Tried to setup choices with null List reference.");
        }

        if (choices.Count > UiButtons.Count) {
            Debug.LogError("Can't have more than 3 choices displayed.");
            return;
        }

        if (UiButtons == null || UiButtons.Count == 0) {
            Debug.LogError("Called SetupChoices() with no Ui Buttons added to DialogueUiManager!");
        }

        if (UiButtonsText == null || UiButtons.Count != UiButtonsText.Count) {
            Debug.LogError("Ui Button Texts added to DialogueUiManager != to Ui Buttons.");
        }

        // Reset buttons
        HideChoices();

        // Get Choices
        int numberOfOptions = choices.Count;

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

    /// <summary>
    /// Connect UI dialogue buttons to this manager.
    /// </summary>
    /// <param name="call"> Callback for Button Clicks. </param>
    public void PairChoices(CallAfterButtonPress call) {

        if (UiButtons == null || UiButtons.Count == 0) {
            Debug.LogError("Called PairChoices() with no Ui Buttons added to DialogueUiManager!");
        }

        for (int i = 0; i < UiButtons.Count; i++) {
            Button currentButton = UiButtons[i];
            int currentIndex = i;
            currentButton.onClick.AddListener(delegate { call(currentIndex); });
        }
    }

    public bool IsLineFinished() {

        if(_currentLineData == null) {
            Debug.LogError("Called IsLineFinished() with no line set.");
        }

        return _currentLineData.FinishedTyping;
    }

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Display next character in dialogue string.
    /// </summary>
    IEnumerator NextCharacter() {

        if(_currentTextBox == null) {
            Debug.LogError("Called next character when no textBox is set.");
        }

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

        bool textFinished = _currentTextBox.maxVisibleCharacters >= _currentLineData.TotalCharacters;
        if (textFinished) {
            EndLineOfText();
        } else {
            StartCoroutine("NextCharacter");
        }
    }

    /// <summary>
    /// Calls once line of text finishes displaying.
    /// </summary>
    void EndLineOfText() {

        if(_currentLineData == null) {
            Debug.LogError("Called EndLineOfText() when no line is set.");
        }

        if (_currentLineData.FinishedTyping) return;

        _currentLineData.FinishedTyping = true;

        if (_currentLineData.Callback != null) {
            _currentLineData.Callback();
        }
    }

    /// <summary>
    /// Create a Speech Bubble at set position.
    /// </summary>
    /// <param name="worldPos"> Position to create bubble. </param>
    /// <param name="verticalOffset"> How far above worldPos to create bubble. </param>
    /// <returns></returns>
    GameObject CreateSpeechBubble(Vector3 worldPos) {

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);

        if (RenderCanvas == null) {
            Debug.LogError("Render Canvas not set in DialogueUiManager.");
        }

        Vector2 canvasResolution = RenderCanvas.GetComponent<CanvasScaler>().referenceResolution;
        Vector2 canvasPos = new Vector2(viewportPos.x * canvasResolution.x, viewportPos.y * canvasResolution.y);

        if (SpeechBubblePrefab == null) {
            Debug.LogError("No Speech Bubble Prefab in DialogueUiManager.");
        }

        return Instantiate(SpeechBubblePrefab, canvasPos, Quaternion.identity, RenderCanvas.transform);
    }
}
