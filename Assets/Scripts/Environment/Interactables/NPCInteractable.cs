using UnityEngine;

public class NpcInteractable : Interactable
{
    [SerializeField] private Vector3 dialogueBubbleOffset;
    [SerializeField] private InventoryCardData cardToGiveOnWin;
    [SerializeField] private TextAsset npcConversation;

    public override void Highlight()
    {
        // TODO: Add Highlight Shader
    }

    public override void UnHighlight()
    {
        // TODO: Remove Highlight Shader
    }

    public override void Interaction()
    {
        bool convoStarted = GameManager.DialogueManager.StartConversation(npcConversation, 
                                                                          transform.position+dialogueBubbleOffset);

        if (convoStarted) {
            GameManager.DialogueManager.SetPrizeCard(cardToGiveOnWin);
            GameManager.PlayerInput.IsActive = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + dialogueBubbleOffset, 0.25f);
    }
}
