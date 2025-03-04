using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalNPC : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private Image iconDisplay;
    [SerializeField] private TextMeshProUGUI bioDisplay;
    [SerializeField] private Transform preferenceTable;
    [SerializeField] private GameObject journalEntryPrefab;

    private List<NPCData> knownNPCs = new List<NPCData>();

    /// <summary>
    /// Adds this NPCData to the known NPCs for the NPC
    /// </summary>
    /// <param name="npcData"></param>
    public void AddNPC(NPCData npcData)
    {
        knownNPCs.Add(npcData);
    }


    /// <summary>
    /// Loads this NPC to be displayed in the journal.
    /// </summary>
    /// <param name="npc"></param>
    public void LoadNPC(NPCData npc)
    {
        nameDisplay.text = npc.Name;
        iconDisplay.sprite = npc.Icon;
        bioDisplay.text = npc.Bio;

        foreach (Transform row in preferenceTable)
        {
            if (row.TryGetComponent<JournalPreferenceEntry>(out var _))
            {
                Destroy(row.gameObject);
            }
        }

        foreach (var card in npc.Cards)
        {
            var journalEntry = Instantiate(journalEntryPrefab, preferenceTable);
            journalEntry.GetComponent<JournalPreferenceEntry>().Load(npc, card);
        }
    }
}
