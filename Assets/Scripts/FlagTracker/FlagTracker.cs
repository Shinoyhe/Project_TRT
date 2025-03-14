using UnityEngine;

public class FlagTracker : MonoBehaviour
{
    [SerializeField] FlagIndex flagIndex;
    
    Inventory _inventory => GameManager.Inventory;

    #region Unity Methods
    void Awake()
    {
        if (!flagIndex) flagIndex = (FlagIndex) Resources.Load("Assets/Resources/FlagIndex.asset");
        flagIndex.CreateFlags();
    }

    private void OnDisable() 
    {
        ResetFlags();
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Check the value of flag with id
    /// </summary>
    /// <returns>The value of the flag</returns>
    public bool CheckFlag(string id) => flagIndex[id];
    
    /// <summary>
    /// Returns flag with id
    /// </summary>
    /// <returns>The flag with id. Null if it doesn't exist.</returns>
    public Flag GetFlag(string id) => flagIndex[id];
    
    /// <summary>
    /// Sets the flag with id 
    /// </summary>
    /// <param name="id">The id</param>
    /// <param name="value">Value to set it to</param>
    /// <param name="editInventory">Whether or not to edit the inventory</param>
    /// <returns>Whether the flag existed or not</returns>
    public bool SetFlag(string id, bool value = true, bool editInventory = true)
    {
        bool code = true;
        Flag flag = flagIndex[id];
        
        if (flag == null)
        {
            code = false;
            flag = new Flag(id);
            flagIndex[id] = flag;
        }
        flag.Value = value;
        
        if (flag.Type == Flag.FlagType.InventoryCard)
        {
            if (!flag.Card) flag.Card = _inventory.GetCardByID(flag.ID[3..]).Data;
            if (editInventory)
            {
                if (value) _inventory.AddCard(flag.Card);
                else _inventory.RemoveCard(flag.Card);
            }
        }
        
        return code;
    }

    /// <summary>
    /// Given an inventory card, sets the corresponding flag to value
    /// </summary>
    /// <returns>Whether the flag existed or not</returns>
    public bool SetFlag(InventoryCardData card, bool value) => SetFlag("IC_"+card.ID, value, false);

    /// <summary>
    /// Update all Inventory Card flags to match the inventory
    /// </summary>
    public void UpdateICFlags()
    {
        foreach (InventoryCard card in _inventory.Get())
        {
            SetFlag(card.Data, true);
        }
    }

    /// <summary>
    /// Resets all flags to their default value.
    /// </summary>
    public void ResetFlags()
    {
        flagIndex.ResetFlags();
        UpdateICFlags();
    }
    #endregion
}