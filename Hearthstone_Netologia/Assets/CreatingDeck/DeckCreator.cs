using System.Collections.Generic;
using UnityEngine;
using Cards.ScriptableObjects;
using System.Linq;
using TMPro;

namespace Cards
{
    public class DeckCreator : MonoBehaviour
    {
        /* public static DeckCreator DeckCreatorSingleton;
        [SerializeField]
        private CardPackConfiguration[] _commonPacks;
        [SerializeField]
        private GameObject _cardPrefab;
        private Vector2 _defaultCommonPosition;
        [SerializeField]
        private float _scale = 1;
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

        private void Start()
        {
            _defaultCommonPosition.x = _commonStartPosition.transform.position.x;
            _defaultCommonPosition.y = _commonStartPosition.transform.position.y;
            // CommonCardsConfiguration();
        }
        private void CommonCardsConfiguration()
        {
            foreach (var pack in _commonPacks)
            {
                foreach (var card in pack._cards)
                {
                    if (CardUtility.CheckUncollectible(card.Id)) continue;
                    var cardGO = Instantiate(_cardPrefab, _commonStartPosition.transform.position, Quaternion.identity);
                    cardGO.transform.localScale *= _scale;
                    cardGO.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 180));
                    cardGO.name = card.Name;
                    Card cardComponent = cardGO.GetComponent<Card>();
                    cardComponent.SetCardDataAndVisuals(card.Texture, pack.GetPackCost(), card.Name, card.Attack, card.Health, card.Type, CardUtility.GetDescriptionById(card.Id));
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
                CardPropertyData card = CardUtility.ConvertToProperty(cardProps);
                if (ClassCardUtility.CheckUncollectible(card.Id)) continue;
                var cardGO = Instantiate(_cardPrefab, _classStartPosition.transform.position, Quaternion.identity);
                cardGO.transform.localScale *= _scale;
                cardGO.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 180));
                cardGO.name = card.Name;
                Card cardComponent = cardGO.GetComponent<Card>();
                cardComponent.SetCardDataAndVisuals(card.Texture, card.Cost, card.Name, card.Attack, card.Health, card.Type, ClassCardUtility.GetDescriptionById(card.Id));
                _classStartPosition.transform.position += _scale * 110f * Vector3.right;
            }
        }
        private void ChoseHero(SideType hero)
        {
            // Перемещение камеры на составление колоды
            _camera.transform.position = _cameraPositionOnChoosingCards;
            CommonCardsConfiguration();
            ClassCardsConfiguration(hero);
        } */
    }
}