using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Processes Ink file and controls conversation flow.
/// </summary>
public class DialogueManager : Singleton<DialogueManager> {

    public GameObject DialogueUiPrefab;
    public TextAsset TestInkFile;

    private bool _inConversation;
    private Story _currentStory;
    private DialogueUiManager _dialogueUiManager;
    private GameObject _dialogueUiInstance;

    private struct ProcessedTags {

        public string speakerName;
        public bool isAction;

        public ProcessedTags(string speakerName = "", bool isAction = false) {
            this.speakerName = speakerName;
            this.isAction = isAction;
        }
    }

    protected override void Awake() {
        base.Awake();
        if (Instance != this) return;

        _inConversation = false;
    }

    /// <summary>
    /// Start a conversation using an Ink JSON file. 
    /// </summary>
    /// <param name="inkJson"> Ink file conversation will use. </param>
    public void StartConversation(TextAsset inkJson) {
        if (_inConversation) return;
        _inConversation = true;

        // Create UI instance
        _dialogueUiManager = SetupUi();

        // Parse Ink File
        _currentStory = new Story(inkJson.text);

        // Show First Line
        ShowNextLine();
    }

    /// <summary>
    /// Callback to show choices when text finishes displaying.
    /// </summary>
    public void ShowChoicesCallBack() {
        _dialogueUiManager.SetupChoices(_currentStory.currentChoices);
    }

    /// <summary>
    /// Instantiate dialogue UI in scene.
    /// </summary>
    /// <returns> The Dialogue UI's manager script. </returns>
    DialogueUiManager SetupUi() {
        _dialogueUiInstance = Instantiate(DialogueUiPrefab, Vector3.zero, Quaternion.identity);

        DialogueUiManager dialogueUiManager = _dialogueUiInstance.GetComponent<DialogueUiManager>();

        LinkUiButtons(ref dialogueUiManager);

        dialogueUiManager.SetupUi("A","B"); // Need to get these somehow

        return dialogueUiManager;
    }

    /// <summary>
    /// Connect UI dialogue buttons to this manager.
    /// </summary>
    /// <param name="dialogueUiManager"> Holds the dialoge buttons. </param>
    void LinkUiButtons(ref DialogueUiManager dialogueUiManager) {
        for (int i = 0; i < dialogueUiManager.UiButtons.Count; i++) {
            Button currentButton = dialogueUiManager.UiButtons[i];
            int choiceIndex = i;
            currentButton.onClick.AddListener(delegate { ProcessDialogueChoice(choiceIndex); });
        }
    }

    /// <summary>
    /// Processes player input and displays the next line.
    /// </summary>
    /// <param name="choiceIndex"></param>
    void ProcessDialogueChoice(int choiceIndex) {
        _currentStory.ChooseChoiceIndex(choiceIndex);
        _dialogueUiManager.HideChoices();
        ShowNextLine();
    }

    /// <summary>
    /// Show next line of current conversation.
    /// </summary>
    /// <returns> True if a line was available, false otherwise.</returns>
    bool ShowNextLine() {

        if(_currentStory == null) {
            return false;
        }

        bool canContinue = _currentStory.canContinue;
        bool hasChoices = _currentStory.currentChoices != null && _currentStory.currentChoices.Count != 0;

        if (canContinue == false && hasChoices == false) {
            EndStory();
            return false;
        }

        if (canContinue == false) {
            return false;
        }

        // Get next line properties
        string nextLine = _currentStory.Continue();
        ProcessedTags foundTags = ProcessTags(_currentStory.currentTags);

        // Queue next line
        if (_currentStory.currentChoices.Count > 0) {
            _dialogueUiManager.DisplayLineOfText(nextLine, foundTags.speakerName, ShowChoicesCallBack);
        } else {
            _dialogueUiManager.DisplayLineOfText(nextLine, foundTags.speakerName);
        }

        ApplyTags(foundTags);

        return true;
    }

    /// <summary>
    /// Apply current dialogue tags to UI.
    /// </summary>
    /// <param name="tagsToApply"> The tags to apply to our UI.</param>
    void ApplyTags(ProcessedTags tagsToApply) {

        // Apply Choice Type
        if (tagsToApply.isAction) {
            Debug.Log("Action Chosen, time to barter!");
        }
    }

    /// <summary>
    /// Convert ink tags to a ProcessedTag struct.
    /// </summary>
    /// <param name="lineTags">Ink tags</param>
    /// <returns>Our new ProcessedTag struct.</returns>
    ProcessedTags ProcessTags(List<string> lineTags) {
        ProcessedTags foundTags = new ProcessedTags();

        foreach (string tag in lineTags) {
            // Get current tag key and value
            string[] tagSplit = tag.Split(":");
            string key = tagSplit[0];
            string value = tagSplit.Length == 2 ? tagSplit[1] : "";
            key = key.ToLower();

            // Process Tag
            switch (key) {
                case "speaker":
                    foundTags.speakerName = value;
                    break;
                case "action":
                    foundTags.isAction = true;
                    break;
            }
        }

        return foundTags;
    }

    /// <summary>
    /// Called to kill UI and prep for next dialogue.
    /// </summary>
    void EndStory() {
        _inConversation = false;
        _currentStory = null;
        _dialogueUiManager = null;

        Destroy(_dialogueUiInstance);
    }   

    private void Update() {
        // Check for Player Input
        if (Input.GetKeyDown(KeyCode.Space)) {

            if (_dialogueUiManager.IsLineFinished()) {
                ShowNextLine();
            } else {
                _dialogueUiManager.SkipLineAnimation();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            StartConversation(TestInkFile);
        }
    }
}
