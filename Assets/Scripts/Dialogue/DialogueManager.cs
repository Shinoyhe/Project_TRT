using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Processes Ink file and controls conversation flow.
/// </summary>
public class DialogueManager : Singleton<DialogueManager> {

    // Parameters =================================================================================

    [Header("Dependencies")]
    public GameObject DialogueUiPrefab;

    public struct ProcessedTags {

        public bool isNpcTalking;
        public bool isBarterTrigger;

        public ProcessedTags(bool isBarterTrigger = false, bool isNpcTalking = false) {
            this.isNpcTalking = isNpcTalking;
            this.isBarterTrigger = isBarterTrigger;
        }
    }

    // Misc Internal Variables ====================================================================

    private bool _inConversation;
    private bool _conversationPaused;
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
        _dialogueUiManager = SetupUi(npcBubblePos, Player.Transform.position);

        // Parse Ink File
        _currentStory = new Story(inkJson.text);

        // Show First Line
        ShowNextLine();
    }

    /// <summary>
    /// Callback to show choices when text finishes displaying.
    /// </summary>
    public void ShowChoicesCallBack() {

        if (_dialogueUiManager == null) {
            ThrowNullError("ShowChoicesCallBack()", "DialogueUiManager");
        }

        _dialogueUiManager.SetupChoices(_currentStory.currentChoices);
    }

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Instantiate dialogue UI in scene.
    /// </summary>
    /// <param name="npcBubblePos"> World pos of NPC Speech bubble. </param>
    /// <param name="playerBubblePos"> World pos of Player Speech bubble. </param>
    /// <returns> The Dialogue UI's manager script. </returns>
    DialogueUiManager SetupUi(Vector3 npcBubblePos, Vector3 playerBubblePos) {

        if (DialogueUiPrefab == null) {
            ThrowNullError("SetupUi()", "DialogueUiPrefab");
        }

        _dialogueUiInstance = Instantiate(DialogueUiPrefab, Vector3.zero, Quaternion.identity);

        if(_dialogueUiInstance == null) {
            ThrowNullError("SetupUi()", "DialogueUiInstance");
        }

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

        if (_dialogueUiManager == null) {
            ThrowNullError("ProcessDialogueChoice()", "dialougeUiManager");
        }
        if (_currentStory == null) {
            ThrowNullError("ProcessDialogueChoice()", "story instance");
        }

        _currentStory.ChooseChoiceIndex(choiceIndex);
        _dialogueUiManager.HideChoices();
        ShowNextLine();
    }

    /// <summary>
    /// Check if _currentStory is over.
    /// </summary>
    /// <returns> True if story is over. False otherwise. </returns>
    bool AtEndOfStory() {
        if (_currentStory == null) {
            Debug.LogError("Called AtEndOfStory() with no story initialized.");
        }

        bool canContinue = _currentStory.canContinue;
        bool hasChoices = _currentStory.currentChoices != null && _currentStory.currentChoices.Count != 0;

        return canContinue == false && hasChoices == false;
    }

    /// <summary>
    /// Show next line of current conversation.
    /// </summary>
    /// <returns> True if a line was available, false otherwise.</returns>
    void ShowNextLine() {

        // Precondition: Must have a story set
        if (_currentStory == null) {
            Debug.LogWarning("Called ShowNextLine() on empty story.");
            return;
        }

        // Precondition: Has not reached end of story
        if (AtEndOfStory()) {
            EndStory();
            return;
        }

        // Precondition: Must be able to contine
        if (_currentStory.canContinue == false) {
            return;
        }

        // Get next line properties
        string nextLine = _currentStory.Continue();
        ProcessedTags foundTags = ProcessTags(_currentStory.currentTags);

        // If choice was Action, skip the line.
        if (foundTags.isBarterTrigger) {
            StartBarter();
            EndStory();
            return;
        }

        // Queue next line
        bool lineHasChoices = _currentStory.currentChoices.Count > 0;
        if (lineHasChoices) {
            _dialogueUiManager.DisplayLineOfText(nextLine, foundTags, ShowChoicesCallBack);
        } else {
            _dialogueUiManager.DisplayLineOfText(nextLine, foundTags);
        }
    }

    /// <summary>
    /// Convert ink tags to a ProcessedTag struct.
    /// </summary>
    /// <param name="lineTags">Ink tags</param>
    /// <returns>Our new ProcessedTag struct.</returns>
    ProcessedTags ProcessTags(List<string> lineTags) {

        if(lineTags == null) {
            ThrowNullError("ProcessTags()", "tag array");
        }

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
                case "barter":
                    foundTags.isBarterTrigger = true;
                    break;
            }
        }

        return foundTags;
    }

    void StartBarter() {
        Debug.Log("Barter Starting!");
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

    void ThrowNullError(string functionOrigin, string whatWasNull) {
        Debug.LogError("Called " + functionOrigin + " with a null " + whatWasNull + ".");
    }
}
