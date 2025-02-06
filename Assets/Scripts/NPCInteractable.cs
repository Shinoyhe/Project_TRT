using UnityEngine;

public class NpcCore : Interactable
{
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
        bool convoStarted = DialogueManager.Instance.StartConversation(NpcConversation,this.transform.position);

        if (convoStarted) {
            DialogueManager.Instance.SetPrizeCard(CardToGiveOnWin);
            PlayerInputHandler.Instance.SetActive(false);
        }
    }
}
