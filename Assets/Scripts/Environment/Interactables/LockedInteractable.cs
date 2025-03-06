using UnityEngine;
using UnityEngine.Events;

public class LockedInteractable : Interactable
{
    public InventoryCardData RequiredCard;
    [SerializeField] private bool removeIfItem = false;
    [SerializeField] private bool locked = true;
    [SerializeField] private UnityEvent callbackFunction;

    private Vector3 _iconPositionStorage;
    private bool _useTransformStorage;

    private void Start()
    {
        _iconPositionStorage = IconLocalPosition;
        _useTransformStorage = UseTransform;
    }

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
        if (!GameManager.Instance) { return; }

        if (!locked || GameManager.Inventory.HasCard(RequiredCard))
        {
            UseTransform = _useTransformStorage;
            IconLocalPosition = _iconPositionStorage;
        } else if (locked || !GameManager.Inventory.HasCard(RequiredCard))
        {
            UseTransform = false;
            IconLocalPosition = Vector3.down * 100;
        }
    }

    public override void UnHighlight()
    {
        if (!GameManager.Instance) { return; }

        if (!locked || GameManager.Inventory.HasCard(RequiredCard))
        {
            UseTransform = _useTransformStorage;
            IconLocalPosition = _iconPositionStorage;
        }
        else if (locked || !GameManager.Inventory.HasCard(RequiredCard))
        {
            UseTransform = false;
            IconLocalPosition = Vector3.down * 100;
        }
    }
}
