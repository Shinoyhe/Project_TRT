using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalNPC : InventoryAction
{
    #region ======== [ VARIABLES ] ========

    [Header("NPC Info")]
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private Image iconDisplay;
    [SerializeField] private TextMeshProUGUI bioDisplay;

    [Header("Preference Table")]
    [SerializeField] private Transform preferenceTable;
    [SerializeField] private GameObject journalEntryPrefab;

    [Header("Known Trades")]
    [SerializeField] private Image playerItem;
    [SerializeField] private Image oppItem;

    private List<NPCData> _knownNPCs = new List<NPCData>();
    private NPCData _npcData;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Adds this NPCData to the known NPCs for the NPC
    /// </summary>
    /// <param name="npcData"></param>
    public void AddNPC(NPCData npcData)
    {
        _knownNPCs.Add(npcData);
    }


    /// <summary>
    /// Loads this NPC to be displayed in the journal.
    /// </summary>
    /// <param name="npc"></param>
    public void LoadNPC(NPCData npc)
    {
        _npcData = npc;

        nameDisplay.text = npc.Name;
        iconDisplay.sprite = npc.Icon;
        bioDisplay.text = npc.Bio;

        LoadPreferenceTable();
        
    }


    public override void ActionOnClick(ActionContext context)
    {
        InventoryCardData cardData = context.cardData;

        playerItem.sprite = cardData ? cardData.Sprite : null;
        oppItem.sprite = _npcData.GetKnownTrade(cardData) ? 
            _npcData.GetKnownTrade(cardData).Sprite : null;
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========
    
    private void LoadPreferenceTable()
    {
        // Create the Preference
        foreach (Transform row in preferenceTable)
        {
            if (row.TryGetComponent<JournalPreferenceEntry>(out var _))
            {
                Destroy(row.gameObject);
            }
        }

        foreach (var card in _npcData.Cards)
        {
            var journalEntry = Instantiate(journalEntryPrefab, preferenceTable);
            journalEntry.GetComponent<JournalPreferenceEntry>().Load(_npcData, card);
        }
    }

    #endregion
}
