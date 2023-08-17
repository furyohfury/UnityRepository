using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Cards.DecksManager;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor;
namespace Cards
{
    public partial class GameCycleManager : MonoBehaviour
    {
        public static GameCycleManager GameCycleSingleton;
        public PlayerSide CurrentPlayer { get; private set; } = PlayerSide.One;
        [SerializeField]
        private EndTurnButton _endTurnButton;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private float _timeForCameraMove = 2;
        public float TimeToMove { get; private set; }  = 1;
        private Dictionary<PlayerSide, List<HandPosition>> _handsDict = new();
        private Dictionary<PlayerSide, List<BoardPosition>> _boardDict = new();
        private Dictionary<PlayerSide, Deck> _decksDict = new();
        private int _numberOfPlayers;
        // private (PlayerSide player, uint currentMana, uint maxMana) _manaData = (PlayerSide.One, 1, 1);
        public Dictionary<PlayerSide, (int currentMana, int maxMana)> ManaDict { get; private set; } = new();
        private int _maxPossibleMana = 10;
        [SerializeField]
        private TextMeshProUGUI _manaText;
        [SerializeField]
        private GameObject _board;
        public bool InputOn { get; private set; } = true;
        private GameObject _draggingCard;
        private Vector3 _cardPositionBeforeDrag;
        [SerializeField]
        private GameObject _cardPrefab;
        private void Awake()
        {
            if (GameCycleSingleton != null) Destroy(this);
            else GameCycleSingleton = this;
        }
        private void OnEnable()
        {
            _endTurnButton.OnEndTurn += EndTurn;
        }
        private void OnDisable()
        {
            _endTurnButton.OnEndTurn -= EndTurn;
        }
        private void Start()
        {
            _numberOfPlayers = Enum.GetNames(typeof(PlayerSide)).Length;
            // Дефолт мана
            ManaDict.Add(PlayerSide.One, (1, 1));
            ManaDict.Add(PlayerSide.Two, (0, 0));
            SetupHandsBoardDecksDictionaries();
            // Подписка на перемещение карт
            foreach(var hands in _handsDict.Values)
            {
                foreach(var hand in hands)
                {
                    if (hand.LinkedCard != null)
                    {
                        hand.LinkedCard.OnDragBegin += DragBegin;
                        hand.LinkedCard.OnDragging += DraggingCard;
                        hand.LinkedCard.OnDragEnd += DragEnd;
                    }
                }
            }
        }
        #region EndTurnPressed
        private void EndTurn()
        {
            ChangeCurrentPlayerAndCardsVisibility();
            StartCoroutine(CameraMovementAndGivingCardStart());
            ChangeStartTurnMana(CurrentPlayer);
            EnableCardsOnBoardToAttack();
        }
        private void ChangeCurrentPlayerAndCardsVisibility()
        {
            // Скрытие карт текущего игрока
            foreach (var handPosition in _handsDict[CurrentPlayer])
            {
                if (handPosition.LinkedCard != null) handPosition.LinkedCard.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            // Передача хода следующему игроку
            if ((int)CurrentPlayer + 1 < Enum.GetNames(typeof(PlayerSide)).Length) ++CurrentPlayer;
            else CurrentPlayer = 0;
            // Открытие карт нового игрока
            foreach (var handPosition in _handsDict[CurrentPlayer])
            {
                if (handPosition.LinkedCard != null) handPosition.LinkedCard.transform.eulerAngles = new Vector3(0, 0, 180);
            }
            // Отзеркаливание стола
            _board.transform.eulerAngles = (CurrentPlayer == PlayerSide.One) ? new Vector3(0, 180, 0) : Vector3.zero;
            Vector3 firstDeckPos = _decksDict[PlayerSide.One].transform.position;
            _decksDict[PlayerSide.One].transform.position = _decksDict[PlayerSide.Two].transform.position;
            _decksDict[PlayerSide.Two].transform.position = firstDeckPos;
        }
        private IEnumerator CameraMovementAndGivingCardStart()
        {
            InputOn = false;
            float time = 0;
            if (_camera.transform.eulerAngles.y == 0) //todo хз почему вообще работает, если не лень будет, разобраться
            {
                while (time < _timeForCameraMove)
                {
                    _camera.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (180 * Time.deltaTime) / _timeForCameraMove));
                    time += Time.deltaTime;
                    yield return null;
                }
                _camera.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 180));
            }
            else
            {
                while (time < _timeForCameraMove)
                {
                    _camera.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -(180 * Time.deltaTime) / _timeForCameraMove));
                    time += Time.deltaTime;
                    yield return null;
                }
                _camera.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            }
            InputOn = true;
            // Выдача карты новому игроку
            GivePlayerACard(CurrentPlayer);
        }

        public void GivePlayerACard(PlayerSide player)
        {
            var newCard = DeckManagerSingleton.GetRandomCardFromDeck(player);
            Card newCardComp = newCard.GetComponent<Card>();
            //Нахождение пустого места
            HandPosition emptyHandPosition = _handsDict[player].FirstOrDefault(pos => pos.LinkedCard == null);
            if (emptyHandPosition == default) return;
            StartCoroutine(GiveCard(player, newCardComp, emptyHandPosition));
        }
        private IEnumerator GiveCard(PlayerSide player, Card newCard, HandPosition emptyHandPosition)
        {
            InputOn = false;
            float time = 0;
            Vector3 startPos = newCard.transform.position;
            Vector3 endPos = emptyHandPosition.transform.position;
            while (time < TimeToMove)
            {
                newCard.transform.position = Vector3.Lerp(startPos, endPos, time / TimeToMove);
                newCard.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (180 * Time.deltaTime) / TimeToMove));
                time += Time.deltaTime;
                yield return null;
            }
            newCard.transform.position = endPos;
            newCard.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            newCard.Player = player;
            newCard.SetLinkedHandPosition(emptyHandPosition);
            newCard.OnDragBegin += DragBegin;
            newCard.OnDragging += DraggingCard;
            newCard.OnDragEnd += DragEnd;
            emptyHandPosition.SetLinkedCard(newCard.GetComponent<Card>());
            InputOn = true;
        }
        #endregion
        private void ChangeStartTurnMana(PlayerSide player)
        {
            if (ManaDict[player].maxMana < _maxPossibleMana)
            {
                ManaDict[player] = (ManaDict[player].maxMana + 1, ManaDict[player].maxMana + 1);
            }
            else ManaDict[player] = (_maxPossibleMana, _maxPossibleMana);
            _manaText.text = ManaDict[player].currentMana + "/" + ManaDict[player].maxMana;
        }
        public void ChangeCurrentMana(PlayerSide player, int deltaCurrent)
        {
            ManaDict[player] = (ManaDict[player].currentMana + deltaCurrent, ManaDict[player].maxMana);
            _manaText.text = ManaDict[player].currentMana + "/" + ManaDict[player].maxMana;
        }
        private void EnableCardsOnBoardToAttack()
        {
            foreach (var boardPositions in _boardDict.Values)
            {
                foreach (var boardPosition in boardPositions)
                {
                    if (boardPosition.LinkedCard != null) boardPosition.LinkedCard.CanAttack = true;
                }
            }
        }
        private void DragBegin(Card card)
        {
            if (card.IsBeingMulliganned || !InputOn || card.Player != CurrentPlayer || (card.LinkedHandPosition != null && ManaDict[card.Player].currentMana < card.GetCardPropertyData()._cost) || (card.LinkedBoardPosition != null && !card.CanAttack)) return;
            _draggingCard = card.gameObject;
            _cardPositionBeforeDrag = card.transform.position;
        }
        private void DraggingCard(Card card)
        {
            if (_draggingCard == null || card.IsBeingMulliganned || !InputOn || card.Player != CurrentPlayer) return;
            Vector3 pos = new(_camera.ScreenToWorldPoint(Input.mousePosition).x, _draggingCard.transform.position.y, _camera.ScreenToWorldPoint(Input.mousePosition).z);
            _draggingCard.transform.position = pos;
        }
        private void DragEnd(Card card)
        {
            if (_draggingCard == null || card.IsBeingMulliganned || !InputOn || card.Player != CurrentPlayer) return;
            // Проверка выкладывания карты на поле
            LayerMask boardPositionMask = LayerMask.GetMask("Board");
            LayerMask playerPortraitMask = LayerMask.GetMask("Player");
            // Разыгрывается на стол
            if (Physics.Raycast(card.transform.position, Vector3.down, out RaycastHit hitBoard, 20, boardPositionMask) && hitBoard.collider.TryGetComponent(out BoardPosition boardPosition) && card.Player == boardPosition.Player && boardPosition.LinkedCard == null && card.LinkedBoardPosition == null)
            {
                // Перемещение карты на BoardPosition
                _draggingCard.transform.position = new Vector3(hitBoard.collider.transform.position.x, _draggingCard.transform.position.y, hitBoard.collider.transform.position.z);
                //Отвязка карты от HandPosition
                if (card.LinkedHandPosition != null)
                {
                    card.LinkedHandPosition.SetLinkedCard(null);
                    card.SetLinkedHandPosition(null);
                }
                // Привязка карты к BoardPosition
                boardPosition.SetLinkedCard(card);
                if (card.LinkedBoardPosition != null) card.LinkedBoardPosition.SetLinkedCard(null);
                card.SetLinkedBoardPosition(boardPosition);
                // Минус мана
                ChangeCurrentMana(card.Player, -card.GetCardPropertyData()._cost);
                // Есть ли батлкрай
                if (BattlecryList.BattlecryCards.Contains(card.GetCardPropertyData()._name))
                {
                    Battlecry(card);
                }
            }
            // Бьет ли в лицо
            else if (card.CanAttack && Physics.Raycast(card.transform.position, Vector3.down, out RaycastHit hitPlayer, 20, playerPortraitMask) && hitPlayer.collider.TryGetComponent(out PlayerPortrait playerPortrait) && card.Player != playerPortrait.Player)
            {
                // Если есть таунт
                if (_boardDict[playerPortrait.Player].Where(boardPos=> boardPos.LinkedCard != null && boardPos.LinkedCard.Taunt).ToArray().Length > 0)
                {
                    ReturnCard(card);
                    Debug.Log("there's taunt");
                    //todo текст (анимация) что есть таунт
                }
                else StartCoroutine(HitFaceAnimation(card, playerPortrait));
            }
            // Бьет ли вражескую карту
            else if (Physics.Raycast(card.transform.position, Vector3.down, out RaycastHit hitBoardwEnemy, 20, boardPositionMask) && hitBoardwEnemy.collider.TryGetComponent(out BoardPosition boardPositionwEnemy) && card.Player != boardPositionwEnemy.Player && boardPositionwEnemy.LinkedCard != null && card.LinkedBoardPosition != null)
            {
                // Если эта карта не таунт и есть другие таунты
                if (!boardPositionwEnemy.LinkedCard.Taunt && _boardDict[boardPositionwEnemy.Player].Where(boardPos => boardPos.LinkedCard != null && boardPos.LinkedCard.Taunt).ToArray().Length > 0)
                {
                    ReturnCard(card);
                    Debug.Log("there's taunt");
                    //todo текст (анимация) что есть таунт
                }
                else  StartCoroutine(HitEnemyAnimation(card, boardPositionwEnemy));
            }
            else
            {
                // Не выложена на поле. А если выложена - не атакует
                Debug.Log("returning");
                ReturnCard(card);
            }
            _draggingCard = null;
        }       
        private void ReturnCard(Card card)
        {
            card.transform.position = _cardPositionBeforeDrag;
        }

        private IEnumerator HitFaceAnimation(Card card, PlayerPortrait playerPortrait)
        {
            float time = 0;
            float halfTimeToMove = TimeToMove / 2;
            Vector3 startPos = _cardPositionBeforeDrag;
            Vector3 endPos = playerPortrait.transform.position;
            while (time < halfTimeToMove)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time / halfTimeToMove);
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
            startPos = card.transform.position;
            endPos = _cardPositionBeforeDrag;
            while (time < halfTimeToMove)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time / halfTimeToMove);
                time += Time.deltaTime;
                yield return null;
            }
            card.transform.position = _cardPositionBeforeDrag;
            card.CanAttack = false;
            playerPortrait.ChangePlayerHealth(-card.GetCardPropertyData()._attack);          
            
        }
        private IEnumerator HitEnemyAnimation(Card card, BoardPosition boardPositionwEnemy)
        {
            float time = 0;
            float halfTimeToMove = TimeToMove / 2;
            Vector3 startPos = _cardPositionBeforeDrag;
            Vector3 endPos = boardPositionwEnemy.transform.position;
            while (time < TimeToMove)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time / halfTimeToMove);
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
            startPos = card.transform.position;
            endPos = _cardPositionBeforeDrag;
            while (time < halfTimeToMove)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time / halfTimeToMove);
                time += Time.deltaTime;
                yield return null;
            }
            card.transform.position = _cardPositionBeforeDrag;
            card.CanAttack = false;
            // Обе получают пизды
            card.ChangeHP(-boardPositionwEnemy.LinkedCard.GetCardPropertyData()._attack);
            boardPositionwEnemy.LinkedCard.ChangeHP(-card.GetCardPropertyData()._attack);
            // Кто-то сдох?
            CheckAliveAndDestroyCard(card);
            CheckAliveAndDestroyCard(boardPositionwEnemy.LinkedCard);
        }
        private void CheckAliveAndDestroyCard(Card card)
        {
            if (card.GetCardPropertyData()._health <= 0)
            {
                card.OnDragBegin -= DragBegin;
                card.OnDragging -= DraggingCard;
                card.OnDragEnd -= DragEnd;
                Destroy(card.gameObject);
            }
        }
        private void SetupHandsBoardDecksDictionaries()
        {
            // Добавление в словарь HandPositions (зачем так сложно непонятно делал бы для двух не парился)
            var allHandPositions = FindObjectsOfType<HandPosition>();
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                List<HandPosition> handPositions = new();
                foreach (var handPos in allHandPositions.Where(handP => (int)handP.Player == i).OrderBy(handP => handP.gameObject.name))
                {
                    handPositions.Add(handPos);
                }
                _handsDict.Add((PlayerSide)i, handPositions);
            }
            // Добавление в словарь BoardPositions)))))))))))))
            var allBoardPositions = FindObjectsOfType<BoardPosition>();
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                List<BoardPosition> boardPositions = new();
                foreach (var boardPos in allBoardPositions.Where((boardP) => (int)boardP.Player == i).OrderBy(handP => handP.gameObject.name))
                {
                    boardPositions.Add(boardPos);
                }
                _boardDict.Add((PlayerSide)i, boardPositions);
            }
            var allDecks = FindObjectsOfType<Deck>();
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                _decksDict.Add((PlayerSide)i, allDecks.Single((deck) => (int)deck.Player == i));
            }
        }
    }
}