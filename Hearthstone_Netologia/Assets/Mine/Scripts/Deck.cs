using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Cards.CardPropertyData;
namespace Cards
{
    public class Deck : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerSide Player { get; private set; }
        // public List<GameObject> _deckCards { get; private set; }
        [field: SerializeField]
        public List<CardPropertyData> DeckCards { get; private set; }
        public void SetDeck(IEnumerable<CardPropertyData> deck)
        {
            DeckCards = deck.ToList();
        }
        public CardPropertyData GetRandomCard(bool isMulliganing = false)
        {
            CardPropertyData data = DeckCards[Random.Range(0, DeckCards.Count)];
            // if (!isMulliganing)
            DeckCards.Remove(data);
            return data;
        }
        public void AddCard(Card card)
        {
            DeckCards.Add(card.GetCardPropertyData());
        }
        public void AddCard(CardPropertyData data)
        {
            DeckCards.Add(data);
        }
        public void AddCard(CardPropertiesData card)
        {
            DeckCards.Add(Converting.ConvertToProperty(card));
        }
        public void AddCard(GameObject card)
        {
            if(card.TryGetComponent(out Card cardComp))
            AddCard(cardComp);
        }
    }
}