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

    public List<NpcProfile> NpcProfiles;

    private bool _inConversation;
    private Story _currentStory;
    private DialogueUiManager _dialogueUiManager;
    private GameObject _dialogueUiInstance;
    private Dictionary<string, NpcProfile> _npcProfiles;

    private struct ProcessedTags {
        public string speakerName;

        public ProcessedTags(string speakerName = "") {
            this.speakerName = speakerName;
        }
    }

    protected override void Awake() {
        base.Awake();
        if (Instance != this) return;

        _inConversation = false;
        _npcProfiles = new Dictionary<string, NpcProfile>();

        foreach (NpcProfile profile in NpcProfiles) {
            _npcProfiles.Add(profile.Name.Trim().ToLower(), profile);
        }
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
    /// Instantiate dialogue UI in scene.
    /// </summary>
    /// <returns> The Dialogue UI's manager script. </returns>
    DialogueUiManager SetupUi() {
        _dialogueUiInstance = Instantiate(DialogueUiPrefab, Vector3.zero, Quaternion.identity);

        DialogueUiManager dialogueUiManager = _dialogueUiInstance.GetComponent<DialogueUiManager>();

        LinkUiButtons(ref dialogueUiManager);

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
        ShowNextLine();
    }

    /// <summary>
    /// Show next line of current conversation.
    /// </summary>
    /// <returns> True if a line was available, false otherwise.</returns>
    bool ShowNextLine() {
        bool canContinue = _currentStory.canContinue;

        if (!canContinue) return false;

        string nextLine = _currentStory.Continue();

        _dialogueUiManager.DisplayLine(nextLine);
        _dialogueUiManager.SetupOptions(_currentStory.currentChoices);

        ProcessedTags foundTags = ProcessTags(_currentStory.currentTags);

        if (foundTags.speakerName == null) return canContinue;

        _dialogueUiManager.AddSpeakerName(foundTags.speakerName);

        NpcProfile profileToLoad = _npcProfiles[foundTags.speakerName.Trim().ToLower()];
        if (profileToLoad) {
            _dialogueUiManager.LoadNpcProfile(profileToLoad);
        }

        return true;
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
            }
        }

        return foundTags;
    }

    /// <summary>
    /// Skip a line of dialogue if available.
    /// </summary>
    void SkipLine() {
        bool canContinue = _currentStory.canContinue;

        if (canContinue) {
            _currentStory.Continue();
        }
    }

    private void Update() {
        // Check for Player Input
        if (Input.GetKey(KeyCode.Space)) {
            ShowNextLine();
        }

        if (Input.GetKey(KeyCode.LeftShift)) {
            StartConversation(TestInkFile);
        }

        // Skip Text
    }
}
