using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class Effect
    {
        public Card SourceCard { get; private set; }
        public Effect(Card sourceCard)
        {
            SourceCard = sourceCard;
        }
        public void SetEffect(Card sourceCard, Card targetCard)
        {
            switch (sourceCard.GetCardPropertyData()._name)
            {
                case "Raid Leader":
                    {
                        if (targetCard.Player == sourceCard.Player)
                        {
                            targetCard.ChangeAttack(1);
                        }
                    }
                    break;
                case "Stormwind Champion":
                    {
                        if (targetCard.Player == sourceCard.Player)
                        {
                            targetCard.ChangeAttack(1);
                            targetCard.ChangeHP(1);
                        }
                    }
                    break;
            }
        }

        public void RemoveEffect(Card targetCard, Card sourceCard)
        {
            switch (sourceCard.GetCardPropertyData()._name)
            {
                case "Raid Leader":
                    {
                        if (targetCard.Player == sourceCard.Player)
                        {
                            targetCard.ChangeAttack(-1);
                        }
                    }
                    break;
                case "Stormwind Champion":
                    {
                        if (targetCard.Player == sourceCard.Player)
                        {
                            targetCard.ChangeAttack(-1);
                            if (targetCard.GetCardPropertyData()._health - 1 > 0) targetCard.ChangeHP(-1);
                        }
                    }
                    break;
            }
        }       
    /* private void PlayPassiveEffects(Card card)
    {
        switch (card.GetCardPropertyData()._name)
        {
            case "Raid Leader":
                {
                    ChangeAllMinionsStats(card, Targets.Friendly, 1, 0);
                }
                break;
            case "Stormwind Champion":
                {
                    ChangeAllMinionsStats(card, Targets.Friendly, 1, 1);
                }
                break;
        }
    }
    private void ReversePassiveEffects(Card card)
    {
        switch (card.GetCardPropertyData()._name)
        {
            case "Raid Leader":
                {
                    ChangeAllMinionsStats(card, Targets.Friendly, -1, 0);
                }
                break;
            case "Stormwind Champion":
                {
                    ChangeAllMinionsStats(card, Targets.Friendly, -1, -1);
                }
                break;
        }
    }
    private void ChangeAllMinionsStats(Card card, Targets targets, int deltaAttack, int deltaHealth)
    {
        switch (targets)
        {
            case Targets.Friendly:
                {
                    foreach(var boardPosition in _boardDict[card.Player])
                    {
                        if (boardPosition.LinkedCard != null && boardPosition.LinkedCard != card)
                        {
                            boardPosition.LinkedCard.ChangeAttack(deltaAttack);
                            if (deltaHealth < 0 && boardPosition.LinkedCard.GetCardPropertyData()._health - deltaHealth > 0) boardPosition.LinkedCard.ChangeHP(deltaHealth);
                        }
                    }
                }
                break;
            case Targets.Enemies:
                {
                    foreach (var boardPositions in _boardDict.Values)
                    {
                        foreach (var boardPos in boardPositions)
                        {
                            if (boardPos.LinkedCard != null && boardPos.LinkedCard != card && boardPos.Player != card.Player)
                            {
                                boardPos.LinkedCard.ChangeAttack(deltaAttack);
                                if (deltaHealth < 0 && boardPos.LinkedCard.GetCardPropertyData()._health - deltaHealth > 0) boardPos.LinkedCard.ChangeHP(deltaHealth);
                            }
                        }
                    }
                }
                break;
            case Targets.All:
                {
                    foreach (var boardPositions in _boardDict.Values)
                    {
                        foreach (var boardPos in boardPositions)
                        {
                            if (boardPos.LinkedCard != null && boardPos.LinkedCard != card)
                            {
                                boardPos.LinkedCard.ChangeAttack(deltaAttack);
                                if (deltaHealth < 0 && boardPos.LinkedCard.GetCardPropertyData()._health - deltaHealth > 0) boardPos.LinkedCard.ChangeHP(deltaHealth);
                            }
                        }
                    }
                }
                break;
        }
    } */
}
}