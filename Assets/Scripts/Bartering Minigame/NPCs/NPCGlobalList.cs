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

    public void Save(ref NPCSaveData data)
    {
        data.NPCs = AllNPCs;
        data.KnownNPCs = KnownNPCs;
    }

    public void Load(NPCSaveData data)
    {
        AllNPCs = data.NPCs;

        foreach (NPC npc in AllNPCs)
        {
            npc.LoadFromSerialized();
        }

        foreach (NPC npc in data.KnownNPCs)
        {
            InGameUi inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();

            if (inGameUi != null)
            {
                inGameUi.Journal.AddNPC(npc.Data);
            }
        }
    }

    #endregion
}

[System.Serializable]
public struct NPCSaveData
{
    public List<NPC> NPCs;
    public List<NPC> KnownNPCs;
}