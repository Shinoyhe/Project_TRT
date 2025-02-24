using UnityEngine;

public abstract class BarterNeutralBehavior : ScriptableObject
{
    public abstract PlayingCard GetCard(CardUser cardUser);
}