using NaughtyAttributes;
using UnityEngine;

public class NpcInteractable : Interactable
{
    [SerializeField] private Vector3 dialogueBubbleOffset;
    [SerializeField] private TextAsset npcConversation;

    [BoxGroup("Trade Settings")] public Trades PossibleTrades;

    [BoxGroup("Barter Settings")] public NPCData NpcData;
    [BoxGroup("Barter Settings")] public BarterResponseMatrix BarterResponseMatrix;
    [BoxGroup("Barter Settings")] public BarterNeutralBehavior BarterNeutralBehaviour;
    [BoxGroup("Barter Settings"), Range(0, 25)] public float BaseDecay = 1;
    [BoxGroup("Barter Settings"), Range(0, 1)] public float DecayAcceleration = 0.025f;
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

        InitBarterStarter();
    }

    private void InitBarterStarter()
    {
        // Set up the settings for the BarterStarter
        BarterStarter barterStarter = GameManager.BarterStarter;

        if (NpcData != null) { 
            barterStarter.NpcData = NpcData;
        } else {
            Debug.LogError("BarterStarter: Could not find NpcData");
        }

        if (BarterResponseMatrix != null) {
            barterStarter.BarterResponseMatrix = BarterResponseMatrix;
        } else {
            Debug.LogError("NPCInteractable: BarterResponseMatrix is not set");
        }

        if (BarterNeutralBehaviour != null) {
            barterStarter.BarterNeutralBehaviour = BarterNeutralBehaviour;
        } else {
            Debug.LogError("NPCInteractable: BarterNeutralBehaviour is not set");
        }

        if (PossibleTrades != null) {
            barterStarter.PossibleTrades = PossibleTrades;
        } else {
            Debug.LogError("NPCInteractable: PossibleTrades are not set");
        }

        barterStarter.BaseDecay = BaseDecay;
        barterStarter.DecayAcceleration = DecayAcceleration;
        barterStarter.WillingnessPerMatch = WillingnessPerMatch;
        barterStarter.WillingnessPerFail = WillingnessPerFail;
        barterStarter.StartingWillingness = StartingWillingness;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(transform.position + dialogueBubbleOffset, 0.25f);
    }
}
