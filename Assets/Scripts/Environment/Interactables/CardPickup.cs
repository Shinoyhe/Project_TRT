using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPickup : Interactable {

    [SerializeField] private InventoryCardData CardToGive;

    public override void Highlight() {
        // ADD SHADER FOR HIGHLIGHT
    }

    public override void Interaction() {
        GameManager.Inventory.AddCard(CardToGive);
        Destroy(this.gameObject);
    }

    public override void UnHighlight() {
        // ADD SHADER FOR HIGHLIGHT
    }
}
