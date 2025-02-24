using UnityEngine;

public abstract class NeutralBehavior_Random : BarterNeutralBehavior
{
    public override PlayingCard GetCard(CardUser cardUser)
    {
        int handIndex = Random.Range(0, cardUser.HandList.Count);
        return cardUser.HandList[handIndex];
    }
}