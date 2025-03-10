using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// YOUTUBE VIDEO REFERENCED: https://www.youtube.com/watch?v=1mf730eb5Wo&ab_channel=SasquatchBStudios

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public InventorySaveData inventoryData;
    }

    #region ========== [ PUBLIC METHODS ] ===========
    
    // SAVE
    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }


    // LOAD
    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());

        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    #endregion

    #region ========== [ PRIVATE METHODS ] ==========

    private static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    // SAVE
    private static void HandleSaveData()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Cannot save data, GameManager is null");
            return;
        }

        if (GameManager.Inventory == null)
        {
            Debug.LogError("Cannot save Inventory, GameManager.Inventory is null");
            return;
        } else
        {
            GameManager.Inventory.Save(ref _saveData.inventoryData);
        }

    }

    private static void HandleLoadData()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Cannot load data, GameManager is null");
            return;
        }

        if (GameManager.Inventory == null)
        {
            Debug.LogError("Cannot load Inventory, GameManager.Inventory is null");
            return;
        } else
        {
            GameManager.Inventory.Load(_saveData.inventoryData);
        }

        // REWORK ClearExceptType in inventory. Cannot delete items and loop

        // Journal Access: Gamemanager -> MasterCanvas.getcomponent -> InGameUI -> JournalNavCore -> NPC
        // KnownNPCs is in JournalNPC
        // Tone Card Preferences is in NPCData
        // Known Trades is in NPCData
        // not saving data for NPC's you don't know, because its in KnownNPCs
    }

    #endregion
}
