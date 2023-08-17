using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using static Cards.GameCycleManager;
using UnityEditor;

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
        private int _maxHP;
        public bool CanAttack { get; set; } = false;
        public bool IsBeingMulliganned { get; private set; } = false;
        private Vector3 _positionBeforeDrag;
        [field: SerializeField]
        public BoardPosition LinkedBoardPosition { get; private set; }
        [field: SerializeField]
        public HandPosition LinkedHandPosition { get; private set; }
        public bool Charge { get; set; } = false;
        public bool Taunt { get; set; } = false;
        private void Awake()
        {
            // _camera = Camera.main;
        }
        private void OnDestroy()
        {
            if (LinkedBoardPosition != null) LinkedBoardPosition.SetLinkedCard(null);
            if (LinkedHandPosition != null) LinkedHandPosition.SetLinkedCard(null);
        }
        #region DragiItd
        public void OnBeginDrag(PointerEventData eventData)
        {
            /* if (IsBeingMulliganned || !GameCycleSingleton.Input || Player != GameCycleSingleton.CurrentPlayer || GameCycleSingleton.ManaDict[Player].currentMana < CardData._cost) return;
            _draggingCard = gameObject;
            _positionBeforeDrag = transform.position; */
            OnDragBegin?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            /* if (_draggingCard == null || IsBeingMulliganned || !GameCycleSingleton.InputOn || Player != GameCycleSingleton.CurrentPlayer) return;
            Vector3 pos = new(_camera.ScreenToWorldPoint(Input.mousePosition).x, _draggingCard.transform.position.y, _camera.ScreenToWorldPoint(Input.mousePosition).z);
            _draggingCard.transform.position = pos; */
            OnDragging?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            /* if (_draggingCard == null || IsBeingMulliganned || !GameCycleSingleton.InputOn || Player != GameCycleSingleton.CurrentPlayer) return;
            // Проверка выкладывания карты на поле
            LayerMask boardPositionMask = LayerMask.GetMask("Board");
            LayerMask playerPortraitMask = LayerMask.GetMask("Player");
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitBoard, 20, boardPositionMask) && hitBoard.collider.TryGetComponent(out BoardPosition boardPosition) && Player == boardPosition.Player && boardPosition.LinkedCard == null && LinkedBoardPosition == null)
            {
                // Перемещение карты на BoardPosition
                _draggingCard.transform.position = new Vector3(hitBoard.collider.transform.position.x, _draggingCard.transform.position.y, hitBoard.collider.transform.position.z);
                //Отвязка карты от HandPosition
                if (LinkedHandPosition != null)
                {
                    LinkedHandPosition.SetLinkedCard(null);
                    LinkedHandPosition = null;
                }
                // Привязка карты к BoardPosition
                boardPosition.SetLinkedCard(this);
                if (LinkedBoardPosition != null) LinkedBoardPosition.SetLinkedCard(null);
                LinkedBoardPosition = boardPosition;
                // Минус мана
                GameCycleSingleton.ChangeCurrentMana(Player, -CardData._cost);
            }
            // Бьет ли в лицо
            else if (CanAttack && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitPlayer, 20, playerPortraitMask) && hitPlayer.collider.TryGetComponent(out PlayerPortrait playerPortrait) && Player != playerPortrait.Player)
            {
                //todo проверка на таунт
                StartCoroutine(HitFaceAnimation(playerPortrait));
            }
            else
            {
                // Не выложена на поле и не атакует
                Debug.Log("returning");
                transform.position = _positionBeforeDrag;
            }
            _draggingCard = null; */
            OnDragEnd?.Invoke(this);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            //todo через анимацию требование
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
            
        }
        public void BeingMulliganed(bool mulliganed)
        {
            IsBeingMulliganned = mulliganed;
        }
        public void SetLinkedBoardPosition(BoardPosition boardPosition)
        {
            LinkedBoardPosition = boardPosition;
        }
        public void SetLinkedHandPosition(HandPosition handPosition)
        {
            LinkedHandPosition = handPosition;
        }
        /* private IEnumerator HitFaceAnimation(PlayerPortrait playerPortrait)
        {
            float time = 0;
            float timeToMove = GameCycleSingleton.TimeToMove / 2;
            Vector3 startPos = _positionBeforeDrag;
            Vector3 endPos = playerPortrait.transform.position;
            while (time < timeToMove)
            {
                transform.position = Vector3.Lerp(startPos, endPos, time / timeToMove);
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
            startPos = transform.position;
            endPos = _positionBeforeDrag;
            while (time < GameCycleSingleton.TimeToMove)
            {
                transform.position = Vector3.Lerp(startPos, endPos, time / timeToMove);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = _positionBeforeDrag;
            playerPortrait.ChangePlayerHealth(-CardData._attack);
            CanAttack = false;
            if (playerPortrait.Health <= 0)
            {
#if UNITY_EDITOR
                EditorApplication.isPaused = true;
#endif
                //todo взрыв портрета героя
            }
        } */
        public void PlayedEffect()
        {
            // Если чардж то сразу может бить
            if (Charge) CanAttack = true;

        }
        public delegate void Drags(Card card);
        public event Drags OnDragBegin;
        public event Drags OnDragging;
        public event Drags OnDragEnd;
    }    
}