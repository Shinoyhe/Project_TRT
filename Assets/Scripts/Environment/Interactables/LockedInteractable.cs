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

    // Set by other scripts, a force switch to hide the icon
    [HideInInspector] public bool HideIcon = false;

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

        UpdateInteractIconLocation();
    }

    public override void Highlight()
    {
        UpdateInteractIconLocation();
    }

    public override void UnHighlight()
    {
        UpdateInteractIconLocation();
    }

    private void UpdateInteractIconLocation()
    {
        if (!GameManager.Instance) { return; }

        // Show using variables set in inspector, downside is it can't update in runtime
        if ((!locked || GameManager.Inventory.HasCard(RequiredCard)) && !HideIcon)
        {
            UseTransform = _useTransformStorage;
            IconLocalPosition = _iconPositionStorage;
        }
        // Hide the Icon
        else if (locked || !GameManager.Inventory.HasCard(RequiredCard) || HideIcon)
        {
            // Manually hiding it out of bounds
            UseTransform = false;
            IconLocalPosition = Vector3.down * 100;

            // Turning off the icon if it exists in scene (attached to player)
            InteractionIcon interactionIcon = FindFirstObjectByType<InteractionIcon>();
            if (interactionIcon != null)
            {
                interactionIcon.Hide();
            }
        }
    }
}
