using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cards.ScriptableObjects;
using System.Linq;

namespace Cards
{
    public class DecksManager : MonoBehaviour
    {
        public static DecksManager DeckManagerSingleton;
        [SerializeField]
        public string[] _playerOneCardNames;
        [SerializeField]
        public string[] _playerTwoCardNames;
        [SerializeField]
        private Deck _playerOneDeck;
        [SerializeField]
        private Deck _playerTwoDeck;
        private string _pathToPacks = "Cards";
        private CardPackConfiguration[] _packs;
        private List<CardPropertiesData> AllCards = new();
        // private List<CardPropertiesData> _commonCards = new();
        // private List<CardPropertiesData> _classCards = new();
        [SerializeField]
        private GameObject _cardPrefab;

        private void Awake()
        {
            if (DeckManagerSingleton != null) Destroy(this);
            else DeckManagerSingleton = this;
            SetAllDecks();
        }
        private IEnumerable<CardPropertiesData> CreatingAllCardsList(string path)
        {
            List<CardPropertiesData> all = new();
            CardPackConfiguration[] packs = Resources.LoadAll(path).Cast<CardPackConfiguration>().ToArray();
            foreach (CardPackConfiguration pack in packs)
            {
                foreach (CardPropertiesData card in pack._cards)
                {
                    all.Add(card);
                }
            }
            return all;
        }
        private IEnumerable<CardPropertiesData> AddCardsToDeck(string[] CardNames)
        {
            List<CardPropertiesData> cards = new();
            foreach (string cardName in CardNames)
            {
                // Ќаходим данные дл€ карты с нужным именем
                CardPropertiesData cardData = AllCards.SingleOrDefault((card) => card.Name == cardName);
                cards.Add(cardData);
            }
            return cards;
        }
        private void SetAllDecks()
        {
            // —оставление списка всех карт
            AllCards.AddRange(CreatingAllCardsList(_pathToPacks));
            //ƒобавление кард в колоды из имен в CardPropertiesData
            _playerOneDeck.SetDeck(AddCardsToDeck(_playerOneCardNames));
            _playerTwoDeck.SetDeck(AddCardsToDeck(_playerTwoCardNames));
        }
        public GameObject GetRandomCardFromDeck(PlayerSide player, bool isMulliganing = false)
        {
            Deck deck = (player == PlayerSide.One) ? _playerOneDeck : _playerTwoDeck;
            // —оздание нового GO карты и присваивание ей данных рандомной карты из колоды
            GameObject cardGO = Instantiate(_cardPrefab, deck.transform.position, Quaternion.identity);
            Card cardComp = cardGO.GetComponent<Card>();
            cardComp.SetCardDataAndVisuals(deck.GetRandomCard(isMulliganing));
            return cardGO;
        }
    }
}