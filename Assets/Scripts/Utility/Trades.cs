using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Trades
{
    [SerializeField] public List<Trade> trades = new List<Trade>();

    public Trade GetTradeFromCard(InventoryCardData card)
    {
        if (card == null)
        {
            return null;        
        }

        foreach (var trade in trades)
        {
            if (trade.AcceptedCard == card)
            {
                return trade;
            }
        }

        return null;
    }

    public bool TradeIsPossible(Trade trade)
    {
        if (trade == null)
        {
            return false;
        }

        return trade.TradeIsPossible;
    }

    public bool TradeIsPossible(InventoryCardData card)
    {
        Trade trade = GetTradeFromCard(card);

        if (trade == null)
        {
            return false;
        }

        return trade.TradeIsPossible;
    }
}

[Serializable]
public class Trade
{
    [SerializeField] public InventoryCardData AcceptedCard;
    [SerializeField] public InventoryCardData RewardCard;
    [SerializeField] public bool TradeIsPossible = true;
}
