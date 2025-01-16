using Ink.Parsed;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    public GameObject DialogueUiPrefab;

    private Story _currentStory;
    private DialogueUiManager _dialogueUiInstance;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
    }

    /// <summary>
    /// Start a conversation using an Ink JSON file. 
    /// </summary>
    /// <param name="inkJson"> Ink file conversation will use. </param>
    public void StartConversation(TextAsset inkJson)
    {
        // Create UI and Set Instance

        // Stop Player

        // Parse Ink File

        // Show First Line
    }

    /// <summary>
    /// Instantiate Dialogue UI in scene.
    /// </summary>
    /// <returns> The Dialogue UI's manager script. </returns>
    DialogueUiManager SetupUi()
    {
        return null;
    }

    /// <summary>
    /// Show next line of current conversation.
    /// </summary>
    void ShowNextLine()
    {

    }

    private void Update()
    {
        // Check for Player Input

        // Skip Text
    }
}
