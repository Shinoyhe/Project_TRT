using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using static UnityEditorInternal.ReorderableList;

public class JournalNavCore : MonoBehaviour 
{
    #region ======== Variables ========

    [Header("Dependencies")]
    public JournalNPC NPC;
    public JournalInfo InfoCards;
    public JournalItem Items;
    [Label("NPC Title")] [SerializeField] private Transform npcTitle;
    [SerializeField] private Transform miscTitle;
    [SerializeField][Label("NPC Button Prefab")] private GameObject npcButtonPrefab;


    public enum UiStates 
    {
        NPC,
        InfoCards,
        Items
    }

    private UiStates _currentCanvasState;

    [Header("DEBUG")]
    [SerializeField] private List<NPCData> npcDataQueue = new List<NPCData>();
    [SerializeField] private List<InventoryCardData> tradeQueue = new List<InventoryCardData>();
    private NPCData _lastDebugAddedNPC;

    #endregion

    #region ======== Public Methods ========

    /// <summary>
    /// Transition Start Ui to a new state.
    /// </summary>
    /// <param name="newState"> State to move to. </param>
    /// <param name="npcIndex"> If newState is UiStates.NPC, then the npcIndex would specify which NPC to use </param>
    public void MoveTo(UiStates newState) 
    {
        StopState(_currentCanvasState);
        StartState(newState);
    }

    public void AddNPC(NPCData newNPCData)
    {
        // Error Catching
        if (npcButtonPrefab == null)
        {
            Debug.LogError("Make sure the NPC Button Prefab is not null!");
            return;
        }
        if (!npcButtonPrefab.TryGetComponent(out Button _))
        {
            Debug.LogError("Make sure the NPC Button Prefab has a button component on its object!");
            return;
        }
        if (npcButtonPrefab.GetComponentInChildren<TextMeshProUGUI>() == null)
        {
            Debug.LogError("Makes the Prefab has a child that has a TextMeshProUGUI");
            return;
        }

        npcTitle.gameObject.SetActive(true);

        // Create Button and Move it Above the Misc Title
        GameObject npcButtonObject = Instantiate(npcButtonPrefab, miscTitle.parent);
        npcButtonObject.GetComponentInChildren<TextMeshProUGUI>().text = newNPCData.Name;
        npcButtonObject.transform.SetSiblingIndex(miscTitle.GetSiblingIndex());

        // Connect the button to MoveToNPC and JournalNPC.LoadNPC
        Button npcButton = npcButtonObject.GetComponent<Button>();
        npcButton.onClick.AddListener(() => this.MoveToNPC());
        NPC.AddNPC(newNPCData);
        npcButton.onClick.AddListener(() => NPC.LoadNPC(newNPCData));
    }

    // Used for button OnClick calls as they don't let enums to be passed through :|
    public void MoveToNPC() => MoveTo(UiStates.NPC);
    public void MoveToInfoCards() => MoveTo(UiStates.InfoCards);
    public void MoveToItems() => MoveTo(UiStates.Items);

    #endregion

    #region ======== Built-in Unity Methods ========
    void Start()
    {
        if (NPC == null)
        {
            Debug.LogError("NPC Canvas dependency not set.");
        }

        // Swap with Accessibility Check
        MoveToInfoCards();
        npcTitle.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // MoveToInfoCards();
    }

    #endregion

    #region ======== Private Methods ========

    /// <summary>
    /// Stop a currently running Ui state.
    /// </summary>
    /// <param name="stateToStop"> State that will stop. </param>
    private void StopState(UiStates stateToStop) {

        // Can't stop transition states
        // (MoveToTitle)

        GameObject canvas = stateToStop switch
        {
            UiStates.NPC => NPC.gameObject,
            UiStates.InfoCards => InfoCards.gameObject,
            UiStates.Items => Items.gameObject,
            _ => null
        };

        canvas?.SetActive(false);
    }

    /// <summary>
    /// Start a new state.
    /// </summary>
    /// <param name="stateToStart">State that will start.</param>
    private void StartState(UiStates stateToStart) {

        // Set our new state
        _currentCanvasState = stateToStart;

        GameObject canvas = stateToStart switch
        {
            UiStates.NPC => NPC.gameObject,
            UiStates.InfoCards => InfoCards.gameObject,
            UiStates.Items => Items.gameObject,
            _ => null
        };

        canvas?.SetActive(true);
    }

    #endregion

    #region ======== Debug Methods ========

    [Button("Add NPCData Button")]
    private void DebugAddNPCData()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("\"Add NPC Button\" is only meant to run during Play Mode.");
            return;
        }

        if (npcDataQueue.Count == 0)
        {
            Debug.LogWarning("Out of NPCDatas in the Queue");
            return;
        }
        if (EditorSceneManager.IsPreviewScene(gameObject.scene))
        {
            Debug.LogWarning("\"Add NPC Button\" is not meant to run in the prefab preview.");
            return;
        }

        AddNPC(npcDataQueue[0]);
        _lastDebugAddedNPC = npcDataQueue[0];
        npcDataQueue.RemoveAt(0);
    }

    [Button("Add Known Trade")]
    private void DebugAddNPCTrade()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("\"Add NPC Button\" is only meant to run during Play Mode.");
            return;
        }

        if (npcDataQueue.Count == 0)
        {
            Debug.LogWarning("Out of NPCDatas in the Queue");
            return;
        }
        if (EditorSceneManager.IsPreviewScene(gameObject.scene))
        {
            Debug.LogWarning("\"Add NPC Button\" is not meant to run in the prefab preview.");
            return;
        }

        _lastDebugAddedNPC.AddJournalKnownTrade(tradeQueue[0], tradeQueue[0]);
        tradeQueue.RemoveAt(0);
    }

    #endregion
}
