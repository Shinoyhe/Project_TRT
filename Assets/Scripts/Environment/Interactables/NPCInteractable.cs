using NaughtyAttributes;
using UnityEngine;

public class NpcInteractable : Interactable
{
    [SerializeField] private Vector3 dialogueBubbleOffset;
    [SerializeField] private TextAsset npcConversation;

    [BoxGroup("Trade Settings")] public InventoryCardData AcceptedCard;
    [BoxGroup("Trade Settings")] public InventoryCardData PrizeCard;

    [BoxGroup("Barter Settings")] public BarterResponseMatrix BarterResponseMatrix;
    [BoxGroup("Barter Settings")] public BarterNeutralBehavior BarterNeutralBehaviour;
    [BoxGroup("Barter Settings"), Range(0, 25)] public float DecayPerSecond = 5;
    [BoxGroup("Barter Settings"), Range(0, 50)] public float WillingnessPerMatch = 5;
    [BoxGroup("Barter Settings"), Range(0, -50)] public float WillingnessPerFail = -5;
    [BoxGroup("Barter Settings"), Range(0, 100)] public float StartingWillingness = 50;

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

        if (!convoStarted) {
            return;
        }

        GameManager.PlayerInput.IsActive = false;

        // Set up the settings for the BarterStarter
        BarterStarter barterStarter = GameManager.BarterStarter;

        if (BarterResponseMatrix != null)
        {
            barterStarter.BarterResponseMatrix = BarterResponseMatrix;
        }
        else
        {
            Debug.LogError("NPCInteractable: Could not find BarterResponseMatrix");
        }

        if (BarterNeutralBehaviour != null)
        {
            barterStarter.BarterNeutralBehaviour = BarterNeutralBehaviour;
        }
        else
        {
            Debug.LogError("NPCInteractable: Could not find BarterNeutralBehaviour");
        }

        barterStarter.PrizeCard = PrizeCard;
        barterStarter.DecayPerSecond = DecayPerSecond;
        barterStarter.WillingnessPerMatch = WillingnessPerMatch;
        barterStarter.WillingnessPerFail = WillingnessPerFail;
        barterStarter.StartingWillingness = StartingWillingness;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + dialogueBubbleOffset, 0.25f);
    }
}
