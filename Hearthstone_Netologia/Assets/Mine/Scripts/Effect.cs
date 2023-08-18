using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cards.GameCycleManager;

namespace Cards
{
    public class Effect
    {
        public Card SourceCard { get; set; }
        public List<BoardPosition> Targets { get; private set; }
        public Effect(Card sourceCard)
        {
            SourceCard = sourceCard;
            // Поиск целей
            switch (SourceCard.GetCardPropertyData()._name)
            {
                case "Raid Leader":
                    {
                        Targets = GameCycleSingleton.BoardDict[sourceCard.Player];
                    }
                    break;
                case "Stormwind Champion":
                    {
                        Targets = GameCycleSingleton.BoardDict[sourceCard.Player];
                    }
                    break;
            }
        }
        
        public void ApplyEffect()
        {
            switch (SourceCard.GetCardPropertyData()._name)
            {
                case "Raid Leader":
                    {
                        foreach(var boardPos in Targets)
                        {
                            if (boardPos.LinkedCard == null || boardPos.LinkedCard == SourceCard || boardPos.LinkedCard.Effects.Contains(this)) return;
                            boardPos.LinkedCard.ChangeAttack(1);
                            AddEffectToCardComp(boardPos.LinkedCard);
                        }
                    }
                    break;
                case "Stormwind Champion":
                    {
                        foreach (var boardPos in Targets)
                        {
                            if (boardPos.LinkedCard == null || boardPos.LinkedCard == SourceCard || boardPos.LinkedCard.Effects.Contains(this)) return;
                            boardPos.LinkedCard.ChangeAttack(1);
                            boardPos.LinkedCard.ChangeHP(1);
                            AddEffectToCardComp(boardPos.LinkedCard);
                        }
                    }
                    break;
            }
        }

        public void RemoveEffect()
        {
            switch (SourceCard.GetCardPropertyData()._name)
            {
                case "Raid Leader":
                    {
                        foreach(var boardPos in Targets)
                        {
                            if (boardPos.LinkedCard == null || boardPos.LinkedCard == SourceCard) return;
                            boardPos.LinkedCard.ChangeAttack(-1);
                            AddEffectToCardComp(boardPos.LinkedCard);
                        }
                    }
                    break;
                case "Stormwind Champion":
                    {
                        foreach (var boardPos in Targets)
                        {
                            if (boardPos.LinkedCard == null || boardPos.LinkedCard == SourceCard) return;
                            boardPos.LinkedCard.ChangeAttack(-1);
                            if (boardPos.LinkedCard.GetCardPropertyData()._health - 1 > 0) boardPos.LinkedCard.ChangeHP(-1);
                            RemoveEffectsFromCardComp(boardPos.LinkedCard);
                        }
                    }
                    break;
            }
        }       
        private void AddEffectToCardComp(Card card)
        {
            card.AddEffect(this);
        }
        private void RemoveEffectsFromCardComp(Card card)
        {
            card.RemoveEffect(this);
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