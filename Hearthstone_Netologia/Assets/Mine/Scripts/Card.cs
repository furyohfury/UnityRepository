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
        public CardPropertyData CardData { get; private set; }
        public bool CanAttack { get; set; } = false;
        public bool IsBeingMulliganned { get; private set; } = false;
        private Vector3 _positionBeforeDrag;
        [field: SerializeField]
        public BoardPosition LinkedBoardPosition { get; private set; }
        [field: SerializeField]
        public HandPosition LinkedHandPosition { get; private set; }
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
            // �������� ������������ ����� �� ����
            LayerMask boardPositionMask = LayerMask.GetMask("Board");
            LayerMask playerPortraitMask = LayerMask.GetMask("Player");
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitBoard, 20, boardPositionMask) && hitBoard.collider.TryGetComponent(out BoardPosition boardPosition) && Player == boardPosition.Player && boardPosition.LinkedCard == null && LinkedBoardPosition == null)
            {
                // ����������� ����� �� BoardPosition
                _draggingCard.transform.position = new Vector3(hitBoard.collider.transform.position.x, _draggingCard.transform.position.y, hitBoard.collider.transform.position.z);
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
                GameCycleSingleton.ChangeCurrentMana(Player, -CardData._cost);
            }
            // ���� �� � ����
            else if (CanAttack && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitPlayer, 20, playerPortraitMask) && hitPlayer.collider.TryGetComponent(out PlayerPortrait playerPortrait) && Player != playerPortrait.Player)
            {
                //todo �������� �� �����
                StartCoroutine(HitFaceAnimation(playerPortrait));
            }
            else
            {
                // �� �������� �� ���� � �� �������
                Debug.Log("returning");
                transform.position = _positionBeforeDrag;
            }
            _draggingCard = null; */
            OnDragEnd?.Invoke(this);
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
            return CardData;
        }
        public void SetCardDataAndVisuals(Texture texture, int cost, string name, int attack, int health, CardUnitType type, string description)
        {
            CardPropertyData local = new();
            _image.material.mainTexture = texture;
            _image.material.mainTextureScale = new Vector2(1, 1);
            local._image = texture;
            _cost.text = "" + cost;
            local._cost = cost;
            _name.text = name;
            local._name = name;
            _attack.text = "" + attack;
            local._attack = attack;
            _health.text = "" + health;
            local._health = health;
            _type.text = "" + type;
            local._type = type;
            _description.text = description;
            local._description = description;
            CardData = local;
        }
        public void SetCardDataAndVisuals(CardPropertyData data)
        {
            CardData = data;
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
            CardPropertyData local = CardData;
            local._attack += delta;
            _attack.text = local._attack + "";
            CardData = local;
        }
        public void ChangeHP(int delta)
        {
            CardPropertyData local = CardData;
            local._health += delta;
            _health.text = local._health + "";
            CardData = local;
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
                //todo ����� �������� �����
            }
        } */
        public delegate void Drags(Card card);
        public event Drags OnDragBegin;
        public event Drags OnDragging;
        public event Drags OnDragEnd;
    }    
}