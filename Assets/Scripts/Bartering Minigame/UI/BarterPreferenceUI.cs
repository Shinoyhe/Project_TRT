using UnityEngine;

public class BarterPreferenceUI : MonoBehaviour 
{
    [SerializeField, Tooltip("The NPC data that we're a")]
    private NPCData npcData;
    private JournalPreferenceEntry[] _entries;

    private void Awake()
    {
        _entries = GetComponentsInChildren<JournalPreferenceEntry>(true);

        // If npcData was preset in the inspector, display immediately.
        // Otherwise, wait for the explicit initialization.
        if (npcData != null) {
            Initialize(npcData);
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

        if (_entries.Length < data.Cards.Length) {
            Debug.LogError("BarterPreferenceUI Error: Awake failed. We needed to display matches "
                        + $"for {data.Cards.Length} cards, but only have enough "
                        + $"children with JournalPreferenceEntry to show {_entries.Length}");
        }

        for (int i = 0; i < data.Cards.Length; i++) {
            _entries[i].Load(data, data.Cards[i]);
        }

        npcData = data;
    }
}