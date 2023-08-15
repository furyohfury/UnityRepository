using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Cards.DecksManager;
using TMPro;
namespace Cards
{
    public class GameCycleManager : MonoBehaviour
    {
        public static GameCycleManager GameCycleSingleton;
        public PlayerSide CurrentPlayer { get; private set; } = PlayerSide.One;
        [SerializeField]
        private EndTurnButton _endTurnButton;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private float _timeForCameraMove = 2;
        private float _timeToMove = 1;
        private Dictionary<PlayerSide, List<HandPosition>> _handsDict = new();
        private Dictionary<PlayerSide, List<BoardPosition>> _boardDict = new();
        private Dictionary<PlayerSide, Deck> _decksDict = new();
        private int _numberOfPlayers;
        // private (PlayerSide player, uint currentMana, uint maxMana) _manaData = (PlayerSide.One, 1, 1);
        public Dictionary<PlayerSide, (int currentMana, int maxMana)> ManaDict { get; private set; }  = new();
        private int _maxPossibleMana = 10;
        [SerializeField]
        private TextMeshProUGUI _manaText;
        [SerializeField]
        private GameObject _board;
        public bool Input { get; private set; } = true;
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
        }
        #region EndTurnPressed
        private void EndTurn()
        {
            ChangeCurrentPlayerAndCardsVisibility();
            StartCoroutine(CameraMovementAndGivingCardStart());
            ChangeStartTurnMana(CurrentPlayer);
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
            Input = false;
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
            Input = true;
            // Выдача карты новому игроку
            GiveNewPlayerACard();
        }
        
        private void GiveNewPlayerACard()
        {
            var newCard = DeckManagerSingleton.GetRandomCardFromDeck(CurrentPlayer);
            Card newCardComp = newCard.GetComponent<Card>();
            //Нахождение пустого места
            HandPosition emptyHandPosition = _handsDict[CurrentPlayer].FirstOrDefault(pos => pos.LinkedCard == null);
            if (emptyHandPosition == default) return;
            StartCoroutine(GiveCard(CurrentPlayer, newCardComp, emptyHandPosition));
        }
        private IEnumerator GiveCard(PlayerSide player, Card newCard, HandPosition emptyHandPosition)
        {
            float time = 0;
            Vector3 startPos = newCard.transform.position;
            Vector3 endPos = emptyHandPosition.transform.position;
            while (time < _timeToMove)
            {
                newCard.transform.position = Vector3.Lerp(startPos, endPos, time / _timeToMove);
                newCard.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (180 * Time.deltaTime) / _timeToMove));
                time += Time.deltaTime;
                yield return null;
            }
            newCard.transform.position = endPos;
            newCard.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            newCard.Player = player;
            newCard.SetLinkedHandPosition(emptyHandPosition);
            emptyHandPosition.SetLinkedCard(newCard.GetComponent<Card>());
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