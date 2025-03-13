using UnityEngine;

public class BarterPreferenceUI : MonoBehaviour 
{
    // Parameters and Publics =====================================================================

    [Tooltip("Whether or not our UI is open.")]
    public bool Open = false;
    [SerializeField, Tooltip("The BarterDirector we read NPCData from.")]
    private BarterDirector director;
    [SerializeField, Tooltip("The GameObject rooting our UI for the unfolded, visible menu.")]
    private GameObject openParent;
    [SerializeField, Tooltip("The GameObject rooting our UI for the folded, hidden menu.")]
    private GameObject closedParent;

    // Misc Internal Variables ====================================================================

    private JournalPreferenceEntry[] _entries;

    // Initializers ===============================================================================

    private void Awake()
    {
        _entries = GetComponentsInChildren<JournalPreferenceEntry>(true);
    }

    private void Start()
    {
        if (director.NpcData != null) {
            Initialize(director.NpcData);
        }
    }

    /// <summary>
    /// Initializes our sprites from an NPCData. Does nothing if either the data
    /// or our array of JournalPreferenceEntries is null.
    /// </summary>
    /// <param name="data">NPCData - the data to load into our JournalPreferenceEntries.</param>
    public void Initialize(NPCData data)
    {
        if (data == null || _entries == null) {
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("Cannot Initialize BarterPreferenceUI without GameManager");
        }

        NPC npcWrapper = GameManager.NPCGlobalList.GetNPCFromData(data);


        if (_entries.Length < npcWrapper.Cards.Length) {
            Debug.LogError("BarterPreferenceUI Error: Awake failed. We needed to display matches "
                        + $"for {npcWrapper.Cards.Length} cards, but only have enough "
                        + $"children with JournalPreferenceEntry to show {_entries.Length}");
        }

        for (int i = 0; i < npcWrapper.Cards.Length; i++) {
            _entries[i].Load(npcWrapper, npcWrapper.Cards[i]);
        }
    }

    // Update methods =============================================================================

    private void Update()
    {
        bool menu1 = GameManager.PlayerInput.GetMenu1();

        if (menu1 != Open) {
            Open = menu1;
            openParent.SetActive(Open);
            closedParent.SetActive(!Open);
        }
    }
}