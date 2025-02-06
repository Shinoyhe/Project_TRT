using UnityEngine;

public class NpcCore : Interactable
{
    public Vector3 dialogueBubbleOffset;

    [SerializeField] private InventoryCard CardToGiveOnWin;
    [SerializeField] private TextAsset NpcConversation;


    void Start()
    {

    }

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
        bool convoStarted = GameManager.DialogueManager.StartConversation(NpcConversation,this.transform.position + dialogueBubbleOffset);

        if (convoStarted) {
            GameManager.DialogueManager.SetPrizeCard(CardToGiveOnWin);
            GameManager.PlayerInput.SetActive(false);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + dialogueBubbleOffset, 0.25f);
    }
}
