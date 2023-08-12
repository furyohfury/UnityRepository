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
        public List<CardPropertiesData> DeckCards { get; private set; }
        public void SetDeck(IEnumerable<CardPropertiesData> deck)
        {
            DeckCards = deck.ToList();
        }
        public CardPropertiesData GetRandomCard(bool isMulliganing = false)
        {
            CardPropertiesData data = DeckCards[Random.Range(0, DeckCards.Count)];
            // if (!isMulliganing) DeckCards.Remove(data);
            return data;
        }
        public void AddCard(Card card)
        {
            CardPropertiesData newCard = Convert(card.GetCardPropertyData());
            DeckCards.Add(newCard);
            //todo че с id и описанием
        }
    }
}