using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// YOUTUBE VIDEO REFERENCED: https://www.youtube.com/watch?v=1mf730eb5Wo&ab_channel=SasquatchBStudios

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();
    private static InGameUi _gameUi;

    [System.Serializable]
    public struct SaveData
    {
        public InventorySaveData inventoryData;
        public NPCSaveData npcSaveData;
    }

    #region ========== [ PUBLIC METHODS ] ===========

    // SAVE

    /// <summary>
    /// Saves the game. Specifically, the Journal data and Inventory Data
    /// </summary>
    /// <param name="clearInventory">whether or not to clear everything except INFO in the Inventory</param>
    public static void Save(bool clearInventory)
    {
        HandleSaveData(clearInventory);

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }


    /// <summary>
    /// Loads the game. Specifically, the Journal data and Inventory Data
    /// </summary>
    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());

        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }


    /// <summary>
    /// Checks if there is save data available.
    /// </summary>
    /// <returns>True if save data exists, otherwise false.</returns>
    public static bool HasSaveData()
    {
        string saveFile = SaveFileName();

        if (!File.Exists(saveFile))
        {
            return false;
        }

        try
        {
            string saveContent = File.ReadAllText(saveFile);
            JsonUtility.FromJson<SaveData>(saveContent);
            return true;
        }
        catch
        {
            // If the file exists but contains invalid data, return false
            return false;
        }
    }

    #endregion

    #region ========== [ PRIVATE METHODS ] ==========

    private static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    // SAVE
    private static void HandleSaveData(bool clearInventory)
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
            GameManager.Inventory.Save(ref _saveData.inventoryData, clearInventory);
        }

        if (GameManager.NPCGlobalList == null)
        {
            Debug.LogError("Cannot save Journal. Gamemanager.NPCGlobalList is null");
            return;
        }
        else
        {
            GameManager.NPCGlobalList.Save(ref _saveData.npcSaveData);
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

        if (GameManager.NPCGlobalList == null)
        {
            Debug.LogError("Cannot load Journal. Gamemanager.NPCGlobalList is null");
            return;
        } else
        {
            GameManager.NPCGlobalList.Load(_saveData.npcSaveData);
        }
    }

    #endregion
}
