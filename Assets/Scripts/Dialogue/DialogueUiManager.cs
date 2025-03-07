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
    public Vector2 CreateNewBubblePoint;

    [Header("Speaking Settings")]
    public float TextSpeed = 0.05f;

    [HideInInspector]
    public delegate void CallAfterLineFinished();
    public delegate void CallAfterButtonPress(int X);

    // Misc Internal Variables ====================================================================
    private TMP_Text _currentTextBox;

    private List<SpeechBubbleCore> _bubbles = new List<SpeechBubbleCore>();

    private class LineInformation {
        public int TotalCharacters = 0;
        public bool FinishedTyping = false;
        public CallAfterLineFinished Callback = null;
    }
    private LineInformation _currentLineData;

    // Public Utility Methods ====================================================================

    public void Reset() {

        foreach (SpeechBubbleCore x in _bubbles) {
            Destroy(x.gameObject);
        }
        for (int i = 0; i < UiButtons.Count; i++) {
            UiButtons[i].onClick.RemoveAllListeners();
        }


        _bubbles.Clear();
    }

    public void SetupUi() {
        HideChoices();
    }

    /// <summary>
    /// Type out a line of text.
    /// </summary>
    /// <param name="text"> Text to display. </param>
    /// <param name="speakerName"> Name of speaker saying line. </param>
    /// <param name="callAfterLineFinished"> Callback after line of text is fully displayed.</param>
    public void DisplayLineOfText(string text, DialogueManager.ProcessedTags foundTags, CallAfterLineFinished callAfterLineFinished = null) {

        foreach(SpeechBubbleCore x in _bubbles) {
            float lastHeight = _bubbles[_bubbles.Count - 1].gameObject.GetComponent<RectTransform>().sizeDelta.y;
            x.gameObject.transform.position += new Vector3(0, lastHeight,0);
        }

        // Create new speech bubble
        GameObject bubble = Instantiate(SpeechBubblePrefab, new Vector2(1920/2,1080/2), Quaternion.identity, RenderCanvas.gameObject.transform);
        SpeechBubbleCore currentBubble = bubble.GetComponent<SpeechBubbleCore>();
        _bubbles.Add(currentBubble);

        if (foundTags.IsNpcTalking) {
            currentBubble.Init(true);
            bubble.transform.position += new Vector3(100, 0, 0);
        } else {
            currentBubble.Init(false);
            bubble.transform.position -= new Vector3(100, 0, 0);
        }

        _currentTextBox = currentBubble.Text;

        _currentTextBox.maxVisibleCharacters = 0;
        _currentTextBox.text = text;

        _currentLineData = new LineInformation();
        _currentLineData.TotalCharacters = text.Length;
        _currentLineData.Callback = callAfterLineFinished;

        // Start Printing
        StartCoroutine(NextCharacter());
    }

    /// <summary>
    /// Skip the typing animation of the current line of text.
    /// </summary>
    public void SkipLineAnimation() {
        StopCoroutine(NextCharacter());
        _currentTextBox.maxVisibleCharacters = _currentLineData.TotalCharacters;
        EndLineOfText();
    }

    /// <summary>
    /// Show Ink Dialogue Choices from array
    /// </summary>
    /// <param name="choices"> Choices in Ink format to display. </param>
    public void ShowChoices(List<Choice> choices) {

        if (choices == null) {
            Debug.LogError("Tried to setup choices with null List reference.");
        }

        if (choices.Count > UiButtons.Count) {
            Debug.LogError("Can't have more than 3 choices displayed.");
            return;
        }

        if (UiButtons == null || UiButtons.Count == 0) {
            Debug.LogError("Called SetupChoices() with no UI Buttons added to DialogueUiManager!");
        }

        if (UiButtonsText == null || UiButtons.Count != UiButtonsText.Count) {
            Debug.LogError("Ui Button Texts added to DialogueUiManager != to UI Buttons.");
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

    public void ChooseChoiceOne() => GameManager.DialogueManager.ProcessDialogueChoice(0);
    public void ChooseChoiceTwo() => GameManager.DialogueManager.ProcessDialogueChoice(1);
    public void ChooseChoiceThree() => GameManager.DialogueManager.ProcessDialogueChoice(2);

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
            StartCoroutine(NextCharacter());
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

        _currentLineData.Callback?.Invoke();
    }
}
