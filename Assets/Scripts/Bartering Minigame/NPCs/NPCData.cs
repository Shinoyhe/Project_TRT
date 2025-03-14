using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

[CreateAssetMenu(fileName = "New NPCData", menuName = "ScriptableObjects/NPCData"), System.Serializable]
public class NPCData : ScriptableObject
{
    #region ======== [ VARIABLES ] ========

    public string Name;
    public Sprite Icon;
    [TextArea] public string Bio;

    // I'm thinking that the BarterResponseMatrix could be referenced and called from here instead. Maybe this script could combine with it?
    public BarterResponseMatrix Matrix;

    #endregion
}
