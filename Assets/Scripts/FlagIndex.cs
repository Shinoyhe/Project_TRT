using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FlagIndex", menuName = "ScriptableObjects/FlagIndex")]
public class FlagIndex : ScriptableObject
{
    [SerializeField] List<Flag> flags = new List<Flag>();
    [SerializeField] List<TextAsset> inkFiles = new List<TextAsset>();
    [SerializeField] List<InventoryCardData> cardIndex = new List<InventoryCardData>();

    public Flag this[string id] 
    {
        get => flags.SingleOrDefault(f => f.ID == id); 
        set => flags.Add(value);
    }
    
    #region Public Methods
    /// <summary>
    /// Creates flags from the variables in the ink files
    /// </summary>
    public void CreateFlags()
    {
        foreach (TextAsset inkJson in inkFiles)
        {
            Story temp = new Story(inkJson.text);
            
            foreach (string id in temp.variablesState)
            {
                CreateFlag(id);
            }
        }
    }

    /// <summary>
    /// Resets all flags to their default value.
    /// </summary>
    public void ResetFlags()
    {
        foreach (Flag flag in flags){
            flag.Value = flag.DefaultValue;
        }
    }
    
    /// <summary>
    /// Clears the list of flags.
    /// </summary>
    public void ClearFlags()
    {
        flags.Clear();
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Creates a flag with id if necessary
    /// </summary>
    /// <param name="id"></param>
    void CreateFlag(string id)
    {
        Flag flag = this[id];
        
        if (flag == null)
        {
            flag = new Flag(id);
            flags.Add(flag);
        }
        flag.Value = flag.DefaultValue;
        
        if (flag.Type == Flag.FlagType.InventoryCard)
        {
            if (!flag.Card) flag.Card = cardIndex.FirstOrDefault(x => x.ID == id[3..]);
        }
    }
    #endregion
    
    #region Unity Methods
    private void OnDisable() {
        ResetFlags();
    }
    #endregion
}

#if UNITY_EDITOR 
[CustomEditor(typeof(FlagIndex))]
public class FlagIndexEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();

        // Add a custom button in the Inspector
        if (GUILayout.Button("Create Flags")) 
        {
            ((FlagIndex)target).CreateFlags();
        }
        
        if (GUILayout.Button("Reset Flags")) 
        {
            ((FlagIndex)target).ResetFlags();
        }
        
        if (GUILayout.Button("Clear Flags")) 
        {
            ((FlagIndex)target).ClearFlags();
        }
    }
}
#endif