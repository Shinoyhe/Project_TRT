using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Processes Ink file and controls conversation flow.
/// </summary>
public class DialogueManager : Singleton<DialogueManager> {

    // Parameters =================================================================================

    [Header("Dependencies")]
    public GameObject Player;
    public GameObject DialogueUiPrefab;

    public struct ProcessedTags {

        public bool isNpcTalking;
        public bool isAction;

        public ProcessedTags(bool isAction = false, bool isNpcTalking = false) {
            this.isNpcTalking = isNpcTalking;
            this.isAction = isAction;
        }
    }

    // Misc Internal Variables ====================================================================

    private bool _inConversation;
    private Story _currentStory;
    private DialogueUiManager _dialogueUiManager;
    private GameObject _dialogueUiInstance;

    // Initializers and Update ================================================================

    protected override void Awake() {
        base.Awake();
        if (Instance != this) return;

        _inConversation = false;
    }

    private void Update() {

        if (_inConversation == false) return;

        // Check for Player Input
        if (PlayerInputHandler.Instance.GetInteractDown()) {

            if (_dialogueUiManager.IsLineFinished()) {
                ShowNextLine();
            } else {
                _dialogueUiManager.SkipLineAnimation();
            }

        }
    }

    // Public Utility Methods ====================================================================

    /// <summary>
    /// Start a conversation using an Ink JSON file. 
    /// </summary>
    /// <param name="inkJson"> Ink file conversation will use. </param>
    /// <param name="npcBubblePos"> Where we want a NPC speech bubble.</param>
    public void StartConversation(TextAsset inkJson, Vector3 npcBubblePos) {
        if (_inConversation) return;
        _inConversation = true;

        // Create UI instance
        _dialogueUiManager = SetupUi(npcBubblePos, Player.transform.position);

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

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Instantiate dialogue UI in scene.
    /// </summary>
    /// <returns> The Dialogue UI's manager script. </returns>
    DialogueUiManager SetupUi(Vector3 npcBubblePos, Vector3 playerBubblePos) {

        _dialogueUiInstance = Instantiate(DialogueUiPrefab, Vector3.zero, Quaternion.identity);

        DialogueUiManager dialogueUiManager = _dialogueUiInstance.GetComponent<DialogueUiManager>();

        dialogueUiManager.PairChoices(ProcessDialogueChoice);
        dialogueUiManager.SetupUi(npcBubblePos,playerBubblePos);

        return dialogueUiManager;
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

        if(ApplyTags(foundTags) == true) {
            return true;
        }

        // Queue next line
        if (_currentStory.currentChoices.Count > 0) {
            _dialogueUiManager.DisplayLineOfText(nextLine, foundTags, ShowChoicesCallBack);
        } else {
            _dialogueUiManager.DisplayLineOfText(nextLine, foundTags);
        }

        return true;
    }

    /// <summary>
    /// Apply current dialogue tags to UI.
    /// </summary>
    /// <param name="tagsToApply"> The tags to apply to our UI.</param>
    /// <returns> True if tag causes line to end. </returns>
    bool ApplyTags(ProcessedTags tagsToApply) {

        bool shouldSkipLine = false;

        // Apply Choice Type
        if (tagsToApply.isAction) {
            Debug.Log("Action Chosen!");
            ShowNextLine(); // Skip saying action in dialogue
            shouldSkipLine = true;
        }

        return shouldSkipLine;
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
                case "npc":
                    foundTags.isNpcTalking = true;
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
}
