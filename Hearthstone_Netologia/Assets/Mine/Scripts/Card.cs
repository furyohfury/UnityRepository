using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Cards
{
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [field: SerializeField]
        public PlayerSide Player { get; set; }
        [SerializeField]
        private MeshRenderer _image;
        [SerializeField]
        private TextMeshPro _cost;
        [SerializeField]
        private TextMeshPro _name;
        [SerializeField]
        private TextMeshPro _attack;
        [SerializeField]
        private TextMeshPro _health;
        [SerializeField]
        private TextMeshPro _type;
        [SerializeField]
        private TextMeshPro _description;
        protected static GameObject _draggingCard;
        [field: SerializeField]
        public BoardPosition LinkedBoardPosition { get; private set; }
        [field: SerializeField]
        public HandPosition LinkedHandPosition { get; private set; }
        private CardPropertyData _cardData;
        private int _maxHP;
        public bool Charge { get; set; } = false;
        public bool Taunt { get; set; } = false;
        public bool CanAttack { get; set; } = false;
        public List<Effect> Effects { get; private set; } = new();

        public bool IsBeingMulliganned { get; private set; } = false;
        private void OnDestroy()
        {
            if (LinkedBoardPosition != null) LinkedBoardPosition.SetLinkedCard(null);
            if (LinkedHandPosition != null) LinkedHandPosition.SetLinkedCard(null);
        }
        #region DragiItd
        public void OnBeginDrag(PointerEventData eventData)
        {
            OnDragBegin?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragging?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnd?.Invoke(this);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Скалирование при наведении
            if (IsBeingMulliganned) return;
            gameObject.transform.localScale *= 1.2f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Скалирование при выходе из наведения
            if (IsBeingMulliganned) return;
            gameObject.transform.localScale /= 1.2f;
        }

        #endregion
        public CardPropertyData GetCardPropertyData() => _cardData;
        #region DataChange
        public void SetCardDataAndVisuals(Texture texture, int cost, string name, int attack, int health, CardUnitType type, string description)
        {
            _image.material.mainTexture = texture;
            _image.material.mainTextureScale = new Vector2(1, 1);
            _cardData._image = texture;
            _cost.text = "" + cost;
            _cardData._cost = cost;
            _name.text = name;
            _cardData._name = name;
            _attack.text = "" + attack;
            _cardData._attack = attack;
            _health.text = "" + health;
            _cardData._health = health;
            _maxHP = health;
            _type.text = "" + type;
            _cardData._type = type;
            _description.text = description;
            _cardData._description = description;
        }
        public void SetCardDataAndVisuals(CardPropertyData data)
        {
            _cardData = data;
            _maxHP = data._health;
            _image.material.mainTexture = data._image;
            _image.material.mainTextureScale = new Vector2(1, 1);
            _cost.text = "" + data._cost;
            _name.text = data._name;
            _attack.text = "" + data._attack;
            _health.text = "" + data._health;
            _type.text = "" + data._type;
            _description.text = data._description;
        }
        public void ChangeAttack(int delta)
        {
            _cardData._attack += delta;
            _attack.text = _cardData._attack + "";
        }
        public void ChangeHP(int delta, bool isHealing = false)
        {
            if (isHealing)
            {
                if (_cardData._health + delta <= _maxHP) _cardData._health += delta;
                else _cardData._health = _maxHP;
            }
            else
            {
                _cardData._health += delta;
                _maxHP += delta;
            }
            _health.text = _cardData._health + "";

        }
        #endregion
        public void SetBeingMulliganed(bool mulliganed) => IsBeingMulliganned = mulliganed;
        public void SetLinkedBoardPosition(BoardPosition boardPosition) => LinkedBoardPosition = boardPosition;
        public void SetLinkedHandPosition(HandPosition handPosition) => LinkedHandPosition = handPosition;
        public void AddEffect(Effect effect) => Effects.Add(effect);
        public void RemoveEffect(Effect effect) => Effects.Remove(effect);

        public delegate void Drags(Card card);
        public event Drags OnDragBegin;
        public event Drags OnDragging;
        public event Drags OnDragEnd;
    }
}