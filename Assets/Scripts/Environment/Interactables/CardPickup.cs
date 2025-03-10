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
        Destroy(gameObject);
    }

    public override void UnHighlight() {
        // ADD SHADER FOR HIGHLIGHT?
    }
}
