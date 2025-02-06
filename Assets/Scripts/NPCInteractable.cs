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
        DialogueManager.Instance.StartConversation(NpcConversation,this.transform.position);
        DialogueManager.Instance.SetPrizeCard(CardToGiveOnWin);
    }
}
