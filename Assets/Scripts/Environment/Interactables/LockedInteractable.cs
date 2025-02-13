using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class LockedInteractable : Interactable
{
    public InventoryCardData RequiredCard;
    [SerializeField] private bool removeIfItem = false;
    [SerializeField] private bool locked = true;
    [SerializeField] private UnityEvent callbackFunction;

    public override void Interaction()
    {
        if (GameManager.Inventory.HasCard(RequiredCard))
        {
            locked = false;
            
            if (RequiredCard.Type == GameEnums.CardTypes.ITEM && removeIfItem)
            {
                GameManager.Inventory.RemoveCard(RequiredCard);
            }
        }

        if (!locked)
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
