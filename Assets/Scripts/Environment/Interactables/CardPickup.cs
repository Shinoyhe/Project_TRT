using UnityEngine;
using NaughtyAttributes;

public class CardPickup : Interactable 
{
    [SerializeField] [Required] private InventoryCardData CardToGive;

    public override void Highlight() {
        // ADD SHADER FOR HIGHLIGHT?
    }

    public override void Interaction() {
        GameManager.Inventory.AddCard(CardToGive);
        GameManager.MasterCanvas.GetComponent<InGameUi>().Notification
            .Notify($"Picked up {CardToGive.CardName}");
        Destroy(this.gameObject);
    }

    public override void UnHighlight() {
        // ADD SHADER FOR HIGHLIGHT?
    }
}
