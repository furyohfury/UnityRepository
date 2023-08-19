using System.Collections.Generic;
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
                        foreach (var boardPos in Targets)
                        {
                            if (boardPos.LinkedCard == null || boardPos.LinkedCard == SourceCard || boardPos.LinkedCard.Effects.Contains(this)) continue;
                            boardPos.LinkedCard.ChangeAttack(1);
                            AddEffectToCardComp(boardPos.LinkedCard);
                        }
                    }
                    break;
                case "Stormwind Champion":
                    {
                        foreach (var boardPos in Targets)
                        {
                            if (boardPos.LinkedCard == null || boardPos.LinkedCard == SourceCard || boardPos.LinkedCard.Effects.Contains(this)) continue;
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
                        foreach (var boardPos in Targets)
                        {
                            if (boardPos.LinkedCard == null || boardPos.LinkedCard == SourceCard) continue;
                            boardPos.LinkedCard.ChangeAttack(-1);
                            RemoveEffectsFromCardComp(boardPos.LinkedCard);
                        }
                    }
                    break;
                case "Stormwind Champion":
                    {
                        foreach (var boardPos in Targets)
                        {
                            if (boardPos.LinkedCard == null || boardPos.LinkedCard == SourceCard) continue;
                            boardPos.LinkedCard.ChangeAttack(-1);
                            if (boardPos.LinkedCard.GetCardPropertyData()._health - 1 > 0) boardPos.LinkedCard.ChangeHP(-1);
                            RemoveEffectsFromCardComp(boardPos.LinkedCard);
                        }
                    }
                    break;
            }
        }
        private void AddEffectToCardComp(Card card) => card.AddEffect(this);
        private void RemoveEffectsFromCardComp(Card card) => card.RemoveEffect(this);
    }
}