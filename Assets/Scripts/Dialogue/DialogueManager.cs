using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Processes Ink file and controls conversation flow.
/// </summary>
public class DialogueManager : MonoBehaviour 
{
    // Parameters =================================================================================

    [Header("Dependencies")]
    [SerializeField, Tooltip("The prefab for dialogue UI.")]
    private DialogueUiManager DialogueUiManager;

    public struct ProcessedTags {

        public bool IsNpcTalking;
        public bool IsBarterTrigger;

        public ProcessedTags(bool isBarterTrigger = false, bool isNpcTalking = false) {
            IsNpcTalking = isNpcTalking;
            IsBarterTrigger = isBarterTrigger;
        }
    }

    // Misc Internal Variables ====================================================================

    private bool _inConversation;
    private bool _onDelay;
    private Story _currentStory;

    // Initializers and Update ================================================================

    protected void Awake()
    {
        _inConversation = false;
        _onDelay = false;
    }

    private void Update()
    {

        if (_inConversation == false) return;

        // Check for Player Input
        if (GameManager.UiInput.GetProgressDialogueDown()) {

            if (DialogueUiManager.IsLineFinished()) {
                ShowNextLine();
            } else {
                DialogueUiManager.SkipLineAnimation();
            }

        }
    }

    // Public Utility Methods ====================================================================

    /// <summary>
    /// Start a conversation using an Ink JSON file. 
    /// </summary>
    /// <returns> True if conversation started successfully. </returns>
    /// <param name="inkJson"> Ink file conversation will use. </param>
    /// <param name="npcBubblePos"> Where we want a NPC speech bubble.</param>
    public bool StartConversation(TextAsset inkJson, string NPCName, Sprite NPCProfilePic)
    {
        if (_inConversation) return false;
        if (_onDelay) return false;

        InGameUi UiController = GameManager.MasterCanvas.gameObject.GetComponent<InGameUi>();
        if (UiController == null) return false;

        UiController.MoveToDialogue();
        DialogueUiManager = GameManager.MasterCanvas.gameObject.GetComponentInChildren<DialogueUiManager>();

        _inConversation = true;
        TimeLoopManager.SetLoopPaused(true);

        // Create UI instance
        SetupUi(NPCName,NPCProfilePic);

        // Parse Ink File
        _currentStory = new Story(inkJson.text);

        // Show First Line
        ShowNextLine();
        return true;
    }

    /// <summary>
    /// Callback to show choices when text finishes displaying.
    /// </summary>
    public void ShowChoicesCallBack()
    {

        if (DialogueUiManager == null) {
            ThrowNullError("ShowChoicesCallBack()", "DialogueUiManager");
        }

        DialogueUiManager.ShowChoices(_currentStory.currentChoices);
    }

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Set up the dialogue UI in scene.
    /// </summary>
    void SetupUi(string npcName, Sprite image)
    {

        if (DialogueUiManager == null) {
            ThrowNullError("SetupUi()", "instancedDialogueUiCanvas");
        }

        DialogueUiManager.gameObject.SetActive(true);
        DialogueUiManager.SetupUi(npcName,image);
    }

    /// <summary>
    /// Processes player input and displays the next line.
    /// </summary>
    /// <param name="choiceIndex"></param>
    public void ProcessDialogueChoice(int choiceIndex)
    {

        if (DialogueUiManager == null) {
            ThrowNullError("ProcessDialogueChoice()", "dialougeUiManager");
        }
        if (_currentStory == null) {
            ThrowNullError("ProcessDialogueChoice()", "story instance");
        }
       
        _currentStory.ChooseChoiceIndex(choiceIndex);
        DialogueUiManager.HideChoices();
        ShowNextLine();
    }

    /// <summary>
    /// Check if _currentStory is over.
    /// </summary>
    /// <returns> True if story is over. False otherwise. </returns>
    bool AtEndOfStory()
    {
        if (_currentStory == null) {
            Debug.LogError("Called AtEndOfStory() with no story initialized.");
        }

        bool canContinue = _currentStory.canContinue;
        bool hasChoices = (_currentStory.currentChoices != null) && (_currentStory.currentChoices.Count != 0);

        return (canContinue == false) && (hasChoices == false);
    }

    /// <summary>
    /// Show next line of current conversation.
    /// </summary>
    /// <returns> True if a line was available, false otherwise.</returns>
    void ShowNextLine()
    {

        // Precondition: Must have a story set
        if (_currentStory == null) {
            Debug.LogWarning("Called ShowNextLine() on empty story.");
            return;
        }

        // Precondition: Has not reached end of story
        if (AtEndOfStory()) {
            EndStory(true);
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
        if (foundTags.IsBarterTrigger) {
            EndStory(false);
            GameManager.BarterStarter.PresentItem();
            return;
        }

        // Queue next line
        bool lineHasChoices = _currentStory.currentChoices.Count > 0;
        if (lineHasChoices) {
            DialogueUiManager.DisplayLineOfText(nextLine, foundTags, ShowChoicesCallBack);
        } else {
            DialogueUiManager.DisplayLineOfText(nextLine, foundTags);
        }
    }

    /// <summary>
    /// Convert ink tags to a ProcessedTag struct.
    /// </summary>
    /// <param name="lineTags">Ink tags</param>
    /// <returns>Our new ProcessedTag struct.</returns>
    ProcessedTags ProcessTags(List<string> lineTags)
    {

        if (lineTags == null) {
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
                    foundTags.IsNpcTalking = true;
                    break;
                case "barter":
                    foundTags.IsBarterTrigger = true;
                    break;
            }
        }

        return foundTags;
    }

    /// <summary>
    /// Called to kill UI and prep for next dialogue.
    /// </summary>
    /// <param name="enablePlayerInput"> True if we want to enable player input after ending story. </param>
    void EndStory(bool enablePlayerInput)
    {
        _inConversation = false;
        _currentStory = null;

        TimeLoopManager.SetLoopPaused(false);

        DialogueUiManager.Reset();
        DialogueUiManager.gameObject.SetActive(false);
        GameManager.PlayerInput.IsActive = enablePlayerInput;
        _onDelay = true;
        StartCoroutine(ConversationDelay());
    }

    void ThrowNullError(string functionOrigin, string whatWasNull)
    {
        Debug.LogError("Called " + functionOrigin + " with a null " + whatWasNull + ".");
    }

    // Delay to prevent ghost inputs, convo ends and starts right again.
    IEnumerator ConversationDelay()
    {
        yield return new WaitForSeconds(0.25f);
        _onDelay = false;
    }
}
