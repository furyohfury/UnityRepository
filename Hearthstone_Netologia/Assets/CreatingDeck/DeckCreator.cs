using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System;
using UnityEditor;
using Cards.ScriptableObjects;

namespace Cards
{
    public class DeckCreator : MonoBehaviour
    {
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
        private Dictionary<SideType, CardPackConfiguration> _classPacks = new();
        private void Awake()
        {
            _defaultCommonPosition.x = _commonStartPosition.transform.position.x;
            _defaultCommonPosition.y = _commonStartPosition.transform.position.y;

        }

        private void Start()
        {
            CommonCardsConfiguration();
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

    }
}