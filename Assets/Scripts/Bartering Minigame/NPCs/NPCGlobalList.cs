using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class NPCGlobalList : MonoBehaviour
{
    [SerializeField] List<NPCData> AllNPCDatas;
    [SerializeField, NaughtyAttributes.ReadOnly] public List<NPC> AllNPCs;
    [SerializeField, NaughtyAttributes.ReadOnly] public List<NPC> KnownNPCs;


    private void Awake()
    {
        if (AllNPCDatas == null)
        {
            AllNPCDatas = new List<NPCData>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupAllNPCs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region ======== [ PUBLIC METHODS ] ========

    public NPC GetNPCFromData(NPCData data)
    {
        foreach (NPC npc in AllNPCs)
        {
            if (npc.Data == data) return npc;
        }

        return null;
    }

    public void AddKnownNPC(NPC npc)
    {
        KnownNPCs.Add(npc);
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    private void SetupAllNPCs()
    {
        AllNPCs = new List<NPC>();

        foreach (NPCData data in AllNPCDatas)
        {
            AllNPCs.Add(new NPC(data));
        }
    }

    #endregion

    #region ======== [ SAVE AND LOAD ] ========

    public void Save(ref KnownNPCsSaveData data)
    {
        data.KnownNPCs = KnownNPCs;
        Dictionary<string, NPCSaveData> tempPerNPCData = new Dictionary<string, NPCSaveData>();

        foreach (NPC npcData in KnownNPCs.ToList())
        {
            NPCSaveData thisNPCsData;
            thisNPCsData.journalKnownTrades = Serialize.FromDict(npcData.journalKnownTrades);
            thisNPCsData.journalTonePrefs = Serialize.FromDict(npcData.journalTonePreferences);

            tempPerNPCData.Add(npcData.Name, thisNPCsData);
        }

        data.PerNPCData = Serialize.FromDict(tempPerNPCData);
    }

    public void Load(KnownNPCsSaveData data)
    {
        foreach (NPC npcData in data.KnownNPCs)
        {
            Dictionary<string, NPCSaveData> perNPCDataDict = Serialize.ToDict(data.PerNPCData);

            InGameUi inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
            if (inGameUi != null)
            {
                inGameUi.Journal.AddNPC(npcData.Data);
            }
        }
    }

    #endregion
}

[System.Serializable]
public struct KnownNPCsSaveData
{
    public List<NPC> KnownNPCs;
    public List<Pair<string, NPCSaveData>> PerNPCData;
}

[System.Serializable]
public struct NPCSaveData
{
    public List<Pair<PlayingCard, CardPreference>> journalTonePrefs;
    public List<Pair<InventoryCardData, InventoryCardData>> journalKnownTrades;
}