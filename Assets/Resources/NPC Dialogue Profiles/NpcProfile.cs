using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NpcProfile", menuName = "ScriptableObjects/NpcProfile", order = 1)]
public class NpcProfile : ScriptableObject
{
    public string Name;
    public Sprite SpeachBubbleBackground;
    public Sprite NameTagBackground;
    public Color NameTextColor;
    public Color SpeachTextColor;
}
