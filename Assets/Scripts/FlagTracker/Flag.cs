using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class Flag
{
    public enum FlagType {InventoryCard, Relay};
    
    [ReadOnly] public string ID;
    public FlagType Type;
    public InventoryCardData Card;
    public bool DefaultValue = false;
    public bool Value = false;
    
    /// <summary>
    /// Checks whether this flag is a default value
    /// </summary>
    /// <returns>If this flag is the default</returns>
    public bool IsDefault(){
        return ID == "default";
    }
    
    // === Constructors ===
    public Flag()
    {
        ID = "default";
        Type = FlagType.Relay;
        DefaultValue = false;
        Value = false;
    }
    
    public Flag(string id) : this()
    {
        ID = id;
        Type = (id.Length > 3 && id[..3] == "IC_") ? FlagType.InventoryCard : FlagType.Relay;
    }
    
    public Flag(string id, bool dv, bool v) : this(id)
    {
        DefaultValue = dv;
        Value = v;
    }
    
    // === System Modifiers ===
    public static implicit operator bool(Flag f){ return f.Value; }
}

[Serializable]
public class FlagReference
{
    [SerializeField] string id;
    Flag flag;
    
    // === System Modifiers ===
    public static implicit operator bool(FlagReference f)
    { 
        if (f.flag == null || f.flag.IsDefault() ) {
            f.flag = GameManager.FlagTracker.GetFlag(f.id);
        }
        return f.flag;
    }
}