using System.Collections.Generic;
using UnityEngine;
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
        [SerializeField]
        private GameObject _cardPrefab;
        private readonly string _pathToPacks = "Cards";
        public List<CardPropertiesData> AllCards { get; private set; } = new();

        private void Awake()
        {
            if (DeckManagerSingleton != null) Destroy(this);
            else DeckManagerSingleton = this;
            _playerOneCardNames = PlayerPrefs.GetString("PlayerOneDeck").Split(',');
            _playerTwoCardNames = PlayerPrefs.GetString("PlayerTwoDeck").Split(',');
            SetAllDecks();
        }
        private IEnumerable<CardPropertiesData> CreatingAllCardsList(string path)
        {
            // �������� ������ ���� ����
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
        private IEnumerable<CardPropertyData> AddCardsToDeck(string[] CardNames)
        {
            // ���������� � ������ ���� �� ������ ���� ����
            List<CardPropertyData> cards = new();
            foreach (string cardName in CardNames)
            {
                // ������� ������ ��� ����� � ������ ������
                CardPropertiesData cardData = AllCards.SingleOrDefault((card) => card.Name == cardName);
                cards.Add(Converting.ConvertToProperty(cardData));
            }
            return cards;
        }
        private void SetAllDecks()
        {
            // ����������� ������ ���� ����
            AllCards.AddRange(CreatingAllCardsList(_pathToPacks));
            //���������� ���� � ������ �� ���� � CardPropertiesData
            _playerOneDeck.SetDeck(AddCardsToDeck(_playerOneCardNames));
            _playerTwoDeck.SetDeck(AddCardsToDeck(_playerTwoCardNames));
        }
        public GameObject GetRandomCardFromDeck(PlayerSide player)
        {
            // ��������� ��������� ����� �� ������
            Deck deck = (player == PlayerSide.One) ? _playerOneDeck : _playerTwoDeck;
            // �������� ������ GO ����� � ������������ �� ������ ��������� ����� �� ������
            GameObject cardGO = Instantiate(_cardPrefab, deck.transform.position, Quaternion.identity);
            cardGO.transform.position = deck.transform.position;
            Card cardComp = cardGO.GetComponent<Card>();
            cardComp.SetCardDataAndVisuals(deck.GetRandomCard());
            // ���������� ������� �����
            cardComp.Player = player;
            if (Charge.ChargeCards.Contains(cardComp.GetCardPropertyData()._name))
            {
                cardComp.Charge = true;
                cardComp.CanAttack = true;
            }
            if (Taunt.TauntCards.Contains(cardComp.GetCardPropertyData()._name)) cardComp.Taunt = true;
            cardGO.name = cardComp.GetCardPropertyData()._name;

            return cardGO;
        }
        public void AddCardToDeck(PlayerSide player, Card card)
        {
            Deck deck = (player == PlayerSide.One) ? _playerOneDeck : _playerTwoDeck;
            deck.AddCard(card);
        }
        public Vector3 GetDeckPosition(PlayerSide player)
        {
            // ������� ������� ������ ����������
            Vector3 pos = (player == PlayerSide.One) ? _playerOneDeck.transform.position : _playerTwoDeck.transform.position;
            return pos;
        }
    }
}