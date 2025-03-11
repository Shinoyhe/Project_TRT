using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameEnums;
using System.Linq;

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
    [SerializeField] private Sprite oppItemNoTradeKnown;

    public HashSet<NPCData> KnownNPCs = new HashSet<NPCData>();
    private NPCData _npcData;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Adds this NPCData to the known NPCs for the NPC. Will do nothing if the NPC is already known.
    /// </summary>
    /// <param name="npcData"></param>
    public void AddNPC(NPCData npcData)
    {
        KnownNPCs.Add(npcData);
    }


    public bool IsKnown(NPCData npcData)
    {
        return KnownNPCs.Contains(npcData);
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


    /// <summary>
    /// Displays the trade for the selected item
    /// </summary>
    /// <param name="context"></param>
    public override void ActionOnClick(ActionContext context)
    {
        InventoryCardData cardData = context.cardData;

        playerItem.sprite = cardData ? cardData.Sprite : null;
        oppItem.sprite = _npcData.GetKnownTrade(cardData) ? 
            _npcData.GetKnownTrade(cardData).Sprite : oppItemNoTradeKnown;

        if(playerItem.sprite == null) {
            playerItem.color = Color.clear;
            oppItem.color = Color.clear;
        } else {
            playerItem.color = Color.white;
            oppItem.color = Color.white;
        }
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

    #region ======== [ SAVE AND LOAD ] ========

    public void Save(ref KnownNPCsSaveData data)
    {
        data.KnownNPCs = KnownNPCs.ToList();
        Dictionary<NPCData, NPCSaveData> tempPerNPCData = new Dictionary<NPCData, NPCSaveData>();

        foreach (NPCData npcData in KnownNPCs.ToList())
        {
            NPCSaveData thisNPCsData;
            thisNPCsData.journalKnownTrades = Serialize.FromDict(npcData.journalKnownTrades);
            thisNPCsData.journalTonePrefs = Serialize.FromDict(npcData.journalTonePreferences);


        }

        data.PerNPCData = Serialize.FromDict(tempPerNPCData);
    }

    public void Load(KnownNPCsSaveData data)
    {
        KnownNPCs = data.KnownNPCs.ToHashSet();

        foreach (NPCData npcData in KnownNPCs)
        {
            Dictionary<NPCData, NPCSaveData> perNPCDataDict = Serialize.ToDict(data.PerNPCData);

            npcData.LoadKnownTrades(Serialize.ToDict(perNPCDataDict[npcData].journalKnownTrades));
            npcData.LoadTonePrefs(Serialize.ToDict(perNPCDataDict[npcData].journalTonePrefs));
        }
    }

    #endregion
}

[System.Serializable]
public struct KnownNPCsSaveData
{
    public List<NPCData> KnownNPCs;
    public List<Pair<NPCData, NPCSaveData>> PerNPCData;
}

[System.Serializable]
public struct NPCSaveData
{
    public List<Pair<PlayingCard, NPC.CardPreference>> journalTonePrefs;
    public List<Pair<InventoryCardData, InventoryCardData>> journalKnownTrades;
}