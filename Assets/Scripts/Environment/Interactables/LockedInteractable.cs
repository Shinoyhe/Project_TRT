using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LockedInteractable : Interactable
{
    public InventoryCardData RequiredCard;
    [SerializeField] private UnityEvent callbackFunction;

    public override void Interaction()
    {
        throw new System.NotImplementedException();
    }

    public override void Highlight()
    {

    }

    public override void UnHighlight()
    {
        
    }
}
