using System.Collections.Generic;
using UnityEngine;
using Cards.ScriptableObjects;
using System.Linq;
using TMPro;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

namespace Cards
{
    public class DeckCreator : MonoBehaviour
    {
        public static DeckCreator DeckCreatorSingleton;
        [SerializeField]
        private CardPackConfiguration[] _commonPacks;
        [SerializeField]
        private GameObject _cardPrefab;
        private Vector2 _defaultCommonPosition;
        [SerializeField]
        private float _scale = 0.8f;
        [SerializeField]
        private GameObject _commonStartPosition;
        [SerializeField]
        private GameObject _classStartPosition;
        [SerializeField]
        private CardPackConfiguration[] _classPacks;
        private List<HeroPortrait> _heroPortraits = new();
        [SerializeField]
        private Camera _camera;
        private Vector3 _cameraPositionOnChoosingCards = new Vector3(0, 270, 0);
        [SerializeField]
        private PlayerDeck _playerDeck;
        [SerializeField]
        private TextMeshPro _deckSizeText;
        [SerializeField]
        private TextMeshPro _chooseHeroForPlayerText;
        private string _pathToPacks = "Cards";
        private static GameObject _draggingCard;
        private Vector3 _cardPositionBeforeDrag;
        private List<Card> _chosenCards = new();
        [SerializeField]
        private int _maxDeckSize = 10;
        private PlayerSide _currentPlayer;
        private void Awake()
        {
            if (DeckCreatorSingleton != null) Destroy(this);
            else DeckCreatorSingleton = this;
            _heroPortraits.AddRange(FindObjectsOfType<HeroPortrait>());
        }
        private void OnEnable()
        {
            foreach (var hero in _heroPortraits)
            {
                hero.OnHeroChosen += ChoseHero;
            }
        }
        private void OnDisable()
        {
            foreach (var hero in _heroPortraits)
            {
                hero.OnHeroChosen -= ChoseHero;
            }
        }
        private void ChoseHero(SideType hero)
        {
            string key = _currentPlayer == PlayerSide.One ? "PlayerOneHero" : "PlayerTwoHero";
            PlayerPrefs.SetString(key, hero.ToString());
            // Перемещение камеры на составление колоды
            _camera.transform.position = _cameraPositionOnChoosingCards;
            CommonCardsConfiguration();
            ClassCardsConfiguration(hero);
        }
        private void Start()
        {
            // Чистит префы если уже деки поставлены но попал сюда
            if (PlayerPrefs.GetString("PlayerTwoDeck").Length > 0) PlayerPrefs.DeleteAll();
            SettingPlayer();
            _defaultCommonPosition.x = _commonStartPosition.transform.position.x;
            _defaultCommonPosition.y = _commonStartPosition.transform.position.y;
            _commonPacks = CreatingCommonCardsList(_pathToPacks).ToArray();
            _classPacks = CreatingClassCardsList(_pathToPacks).ToArray();
        }
        private void SettingPlayer()
        {
            if (PlayerPrefs.GetString("PlayerOneDeck").Length <= 0)
            {
                _chooseHeroForPlayerText.text = "Choose Hero for Player One";
                _currentPlayer = PlayerSide.One;
            }
            else if (PlayerPrefs.GetString("PlayerTwoDeck").Length <= 0)
            {
                _chooseHeroForPlayerText.text = "Choose Hero for Player Two";
                _currentPlayer = PlayerSide.Two;
            }
        }
        private IEnumerable<CardPackConfiguration> CreatingCommonCardsList(string path)
        {
            CardPackConfiguration[] commonPacks = Resources.LoadAll(path).Cast<CardPackConfiguration>().Where(pack => pack._sideType == SideType.Common).ToArray();
            return commonPacks;
        }
        private IEnumerable<CardPackConfiguration> CreatingClassCardsList(string path)
        {
            CardPackConfiguration[] packs = Resources.LoadAll(path).Cast<CardPackConfiguration>().Where(pack => pack._sideType != SideType.Common).ToArray();
            return packs;
        }
        private void CommonCardsConfiguration()
        {
            foreach (var pack in _commonPacks)
            {
                foreach (var cardProps in pack._cards)
                {
                    if (CardUtility.CheckUncollectible(cardProps.Id)) continue;
                    var cardGO = Instantiate(_cardPrefab, _commonStartPosition.transform.position, Quaternion.identity);
                    cardGO.transform.localScale *= _scale;
                    cardGO.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 180));
                    cardGO.name = cardProps.Name;
                    Card cardComponent = cardGO.GetComponent<Card>();
                    cardComponent.SetCardDataAndVisuals(Converting.ConvertToProperty(cardProps));
                    cardComponent.OnDragBegin += BeginDrag;
                    cardComponent.OnDragging += Dragging;
                    cardComponent.OnDragEnd += EndDrag;
                    _commonStartPosition.transform.position += _scale * 110f * Vector3.right;
                }
                _commonStartPosition.transform.position = new Vector3(_defaultCommonPosition.x, _defaultCommonPosition.y, _commonStartPosition.transform.position.z - 180f * _scale);
            }
        }
        private void ClassCardsConfiguration(SideType hero)
        {
            CardPackConfiguration heroPack = _classPacks.FirstOrDefault((pack) => pack._sideType == hero);
            if (heroPack == null) return;
            foreach (var cardProps in heroPack._cards)
            {
                CardPropertyData card = Converting.ConvertToProperty(cardProps);
                if (ClassCardUtility.CheckUncollectible(cardProps.Id)) continue;
                var cardGO = Instantiate(_cardPrefab, _classStartPosition.transform.position, Quaternion.identity);
                cardGO.transform.localScale *= _scale;
                cardGO.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 180));
                cardGO.name = card._name;
                Card cardComponent = cardGO.GetComponent<Card>();
                cardComponent.SetCardDataAndVisuals(card);
                cardComponent.OnDragBegin += BeginDrag;
                cardComponent.OnDragging += Dragging;
                cardComponent.OnDragEnd += EndDrag;
                _classStartPosition.transform.position += _scale * 110f * Vector3.right;
            }
        }        
        private void BeginDrag(Card card)
        {
            _draggingCard = card.gameObject;
            _cardPositionBeforeDrag = card.transform.position;
        }
        private void Dragging(Card card)
        {
            if (_draggingCard == null) return;
            Vector3 pos = new(_camera.ScreenToWorldPoint(Input.mousePosition).x, _draggingCard.transform.position.y, _camera.ScreenToWorldPoint(Input.mousePosition).z);
            _draggingCard.transform.position = pos;
        }
        private void EndDrag(Card card)
        {
            LayerMask.GetMask("Deck");
            if (Physics.Raycast(card.transform.position, Vector3.down, out RaycastHit hitBoard, 20, 1 << 8) && hitBoard.collider.TryGetComponent(out PlayerDeck playerDeck))
            {
                _chosenCards.Add(card);
                Debug.Log("Got it");
                // todo запись в текст файл
                card.OnDragBegin -= BeginDrag;
                card.OnDragging -= Dragging;
                card.OnDragEnd -= EndDrag;
                Destroy(card.gameObject);
                _deckSizeText.text = _chosenCards.Count + "/" + _maxDeckSize;
                if (_chosenCards.Count >= _maxDeckSize)
                {
                    //todo выход или переключение на некст хуйню
                    WritingInPref(ChosenToString());
                }
            }
            else
            {
                _draggingCard.transform.position = _cardPositionBeforeDrag;
            }
        }
        private string ChosenToString()
        {
            StringBuilder sb = new();
            foreach (var card in _chosenCards)
            {
                sb.Append(card.GetCardPropertyData()._name + ",");
            }
            return sb.ToString().Trim(',');
        }
        private void WritingInPref(string deckCards)
        {
            if (_currentPlayer == PlayerSide.One)
            {
                PlayerPrefs.SetString("PlayerOneDeck", deckCards);
                SceneManager.LoadScene(0);
            }
            else if (_currentPlayer == PlayerSide.Two)
            {
                PlayerPrefs.SetString("PlayerTwoDeck", deckCards);
                SceneManager.LoadScene(1);
            }
        }
    }
}