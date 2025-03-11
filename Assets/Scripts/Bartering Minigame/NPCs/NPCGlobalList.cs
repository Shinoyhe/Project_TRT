using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class NPCGlobalList : MonoBehaviour
{
    [SerializeField] List<NPCData> AllNPCDatas;
    [SerializeField, NaughtyAttributes.ReadOnly] List<NPC> AllNPCs;
    [SerializeField, NaughtyAttributes.ReadOnly] List<NPC> KnownNPCs;


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

    public void Save(bool clearKnownNPCs)
    {
        if (clearKnownNPCs) { KnownNPCs.Clear(); }


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
        Dictionary<NPC, NPCSaveData> tempPerNPCData = new Dictionary<NPC, NPCSaveData>();

        foreach (NPC npcData in KnownNPCs.ToList())
        {
            NPCSaveData thisNPCsData;
            thisNPCsData.journalKnownTrades = Serialize.FromDict(npcData.journalKnownTrades);
            thisNPCsData.journalTonePrefs = Serialize.FromDict(npcData.journalTonePreferences);

            tempPerNPCData.Add(npcData, thisNPCsData);
        }

        data.PerNPCData = Serialize.FromDict(tempPerNPCData);
    }

    public void Load(KnownNPCsSaveData data)
    {
        KnownNPCs = data.KnownNPCs;

        foreach (NPC npcData in KnownNPCs)
        {
            Dictionary<NPC, NPCSaveData> perNPCDataDict = Serialize.ToDict(data.PerNPCData);

            npcData.LoadKnownTrades(Serialize.ToDict(perNPCDataDict[npcData].journalKnownTrades));
            npcData.LoadTonePrefs(Serialize.ToDict(perNPCDataDict[npcData].journalTonePrefs));
        }
    }

    #endregion
}

[System.Serializable]
public struct KnownNPCsSaveData
{
    public List<NPC> KnownNPCs;
    public List<Pair<NPC, NPCSaveData>> PerNPCData;
}

[System.Serializable]
public struct NPCSaveData
{
    public List<Pair<PlayingCard, NPC.CardPreference>> journalTonePrefs;
    public List<Pair<InventoryCardData, InventoryCardData>> journalKnownTrades;
}