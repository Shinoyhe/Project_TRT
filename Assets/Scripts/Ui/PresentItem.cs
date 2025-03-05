using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PresentItem : MonoBehaviour
{
    // Parameters =================================================================================
    [ReadOnly, Tooltip("The card that the NPC will accept for a barter, set by BarterStarter")]
    public Trades PossibleTrades;

    [SerializeField] private TMP_Text submissionText;
    [SerializeField] private InventoryCardObject submissionCard;

    public System.Action OnAccepted;
    public System.Action OnClosed;

    // Private Methods ============================================================================

    void Start()
    {

    }

    void Update()
    {
        
    }

    // Public Functions ===========================================================================

    public void Submit()
    {
        if (PossibleTrades.TradeIsPossible(submissionCard.Card))
        {
            if (GameManager.BarterStarter)
            {
                GameManager.BarterStarter.CurrentTrade = PossibleTrades.GetTradeFromCard(submissionCard.Card);
            }

            OnAccepted.Invoke();
            return;
        }

        submissionText.text = "That's not what I need.";
    }

    public void Back()
    {
        OnClosed.Invoke();
    }
}
