using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using static Cards.GameCycleManager;

namespace Cards
{
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public PlayerSide Player { get; set; }
        protected static GameObject _draggingCard;
        private Camera _camera;
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
        private CardPropertyData _cardData;
        public bool CanAttack { get; set; }  = false;
        public bool IsBeingMulliganned { get; private set; } = false;
        private Vector3 _positionBeforeDrag;
        [field: SerializeField]
        public BoardPosition LinkedBoardPosition { get; private set; }
        [field: SerializeField]
        public HandPosition LinkedHandPosition { get; private set; }
        private void Awake()
        {
            _camera = Camera.main;
        }
        #region DragiItd
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsBeingMulliganned || !GameCycleSingleton.Input || Player != GameCycleSingleton.CurrentPlayer || GameCycleSingleton.ManaDict[Player].currentMana < _cardData._cost) return;
            _draggingCard = gameObject;
            _positionBeforeDrag = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_draggingCard == null || IsBeingMulliganned || !GameCycleSingleton.Input || Player != GameCycleSingleton.CurrentPlayer) return;
            Vector3 pos = new(_camera.ScreenToWorldPoint(Input.mousePosition).x, _draggingCard.transform.position.y, _camera.ScreenToWorldPoint(Input.mousePosition).z);
            _draggingCard.transform.position = pos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_draggingCard == null ||  IsBeingMulliganned || !GameCycleSingleton.Input || Player != GameCycleSingleton.CurrentPlayer) return;
            LayerMask boardPositionMask = LayerMask.GetMask("Board");
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 20, boardPositionMask) && hit.collider.TryGetComponent(out BoardPosition boardPosition) && Player == boardPosition.Player && boardPosition.LinkedCard == null && LinkedBoardPosition == null)
            {
                // ����������� ����� �� BoardPosition
                _draggingCard.transform.position = new Vector3(hit.collider.transform.position.x, _draggingCard.transform.position.y, hit.collider.transform.position.z);
                //������� ����� �� HandPosition
                if (LinkedHandPosition != null)
                {
                    LinkedHandPosition.SetLinkedCard(null);
                    LinkedHandPosition = null;
                }
                // �������� ����� � BoardPosition
                boardPosition.SetLinkedCard(this);
                if (LinkedBoardPosition != null) LinkedBoardPosition.SetLinkedCard(null);
                LinkedBoardPosition = boardPosition;
                // ����� ����
                GameCycleSingleton.ChangeCurrentMana(Player, -_cardData._cost);
            }
            else
            {
                Debug.Log("returning");
                transform.position = _positionBeforeDrag;
            }
            _draggingCard = null;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            //todo ����� �������� ����������
            if (IsBeingMulliganned) return;
            gameObject.transform.localScale *= 1.2f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsBeingMulliganned) return;
            gameObject.transform.localScale /= 1.2f;
        }
        #endregion
        public CardPropertyData GetCardPropertyData()
        {
            return _cardData;
        }
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
            _type.text = "" + type;
            _cardData._type = type;
            _description.text = description;
            _cardData._description = description;
        }
        /* public void SetCardDataAndVisuals(CardPropertiesData data)
        {
            _image.material.mainTexture = data.Texture;
            _image.material.mainTextureScale = new Vector2(1, 1);
            _cardData._image = data.Texture;
            _cost.text = "" + data.Cost;
            _cardData._cost = data.Cost;
            _name.text = name;
            _cardData._name = name;
            _attack.text = "" + data.Attack;
            _cardData._attack = data.Attack;
            _health.text = "" + data.Health;
            _cardData._health = data.Health;
            _type.text = "" + data.Type;
            _cardData._type = data.Type;
            _description.text = CardUtility.GetDescriptionById(data.Id);
            _cardData._description = CardUtility.GetDescriptionById(data.Id);
        } */
        public void SetCardDataAndVisuals(CardPropertyData data)
        {
            _image.material.mainTexture = data._image;
            _image.material.mainTextureScale = new Vector2(1, 1);
            _cardData._image = data._image;
            _cost.text = "" + data._cost;
            _cardData._cost = data._cost;
            _name.text = data._name;
            _cardData._name = data._name;
            _attack.text = "" + data._attack;
            _cardData._attack = data._attack;
            _health.text = "" + data._health;
            _cardData._health = data._health;
            _type.text = "" + data._type;
            _cardData._type = data._type;
            _description.text = data._description;
            _cardData._description = data._description;
        }
        public void ChangeAttack(int delta)
        {
            _cardData._attack += delta;
        }
        public void ChangeHP(int delta)
        {
            _cardData._health += delta;
        }
        public void BeingMulliganed(bool mulliganed)
        {
            IsBeingMulliganned = mulliganed;
        }
        public void SetLinkedBoardPosition (BoardPosition boardPosition)
        {
            LinkedBoardPosition = boardPosition;
        }
        public void SetLinkedHandPosition(HandPosition handPosition)
        {
            LinkedHandPosition = handPosition;
        }
    }
}