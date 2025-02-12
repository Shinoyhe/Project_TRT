using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LockedInteractable : Interactable
{
    public InventoryCardData RequiredCard;
    public bool Locked = true;
    [SerializeField] private UnityEvent callbackFunction;

    public override void Interaction()
    {
        if (GameManager.Inventory.HasCard(RequiredCard))
        {
            Locked = false;
            
            if (RequiredCard.Type == GameEnums.CardTypes.ITEM)
            {
                GameManager.Inventory.RemoveCard(RequiredCard);
            }
        }

        if (!Locked)
        {
            callbackFunction.Invoke();
        }
    }

    public override void Highlight()
    {

    }

    public override void UnHighlight()
    {
        
    }
}
