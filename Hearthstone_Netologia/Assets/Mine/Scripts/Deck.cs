using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;
using static Cards.CardPropertyData;
namespace Cards
{
    public class Deck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [field: SerializeField]
        public PlayerSide Player { get; private set; }
        // public List<GameObject> _deckCards { get; private set; }
        [field: SerializeField]
        public List<CardPropertyData> DeckCards { get; private set; }
        [SerializeField]
        private TextMeshPro _deckSizeText; //todo отображение размера колоды
        public void SetDeck(IEnumerable<CardPropertyData> deck)
        {
            DeckCards = deck.ToList();
            _deckSizeText.text = DeckCards.Count + "";
        }
        public CardPropertyData GetRandomCard()
        {
            CardPropertyData data = DeckCards[Random.Range(0, DeckCards.Count)];
            DeckCards.Remove(data); //todo нормально не работает
            _deckSizeText.text = DeckCards.Count + "";
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
            if (card.TryGetComponent(out Card cardComp))
                AddCard(cardComp);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _deckSizeText.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _deckSizeText.gameObject.SetActive(false);
        }
    }
}