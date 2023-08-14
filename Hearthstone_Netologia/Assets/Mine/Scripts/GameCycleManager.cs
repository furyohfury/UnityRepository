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
        private Dictionary<PlayerSide, (uint currentMana, uint maxMana)> _manaDict = new();
        [SerializeField]
        private TextMeshProUGUI _manaText;
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
            // ������ ����
            _manaDict.Add(PlayerSide.One, (1, 1));
            _manaDict.Add(PlayerSide.Two, (0, 0));
            SetupHandsBoardDecksDictionaries();
        }
        /* private void GiveTurnToNextPlayer()
        {
            if ((int)CurrentPlayer + 1 < Enum.GetNames(typeof(PlayerSide)).Length)
            {
                ++CurrentPlayer;
            }
            else CurrentPlayer = 0;
        } */
        
        private void EndTurn()
        {
            ChangeCurrentPlayerAndCardsVisibility();
            StartCoroutine(CameraMovementAndGivingCardStart());
            ChangeMana(CurrentPlayer, 1, 1);
        }
        private void ChangeCurrentPlayerAndCardsVisibility()
        {
            foreach (var handPosition in _handsDict[CurrentPlayer])
            {
                if (handPosition.LinkedCard != null) handPosition.LinkedCard.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            if ((int)CurrentPlayer + 1 < Enum.GetNames(typeof(PlayerSide)).Length)
            {
                ++CurrentPlayer;
            }
            else CurrentPlayer = 0;
            foreach (var handPosition in _handsDict[CurrentPlayer])
            {
                if (handPosition.LinkedCard != null) handPosition.LinkedCard.transform.eulerAngles = new Vector3(0, 0, 180);
            }
        }
        private IEnumerator CameraMovementAndGivingCardStart() //todo ����� ����� �� ���)))))))))))
        {
            /* float time = 0;
            if (_camera.transform.eulerAngles.y == 0 )
            {
                while (time < _timeForCameraMove)
                {
                    _camera.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (180 * Time.deltaTime) / _timeForCameraMove));
                    // _camera.transform.eulerAngles += new Vector3(0, (180 * Time.deltaTime) / _timeForCameraMove, 0);
                    time += Time.deltaTime;
                    yield return null;
                }
                _camera.transform.eulerAngles = new Vector3(_camera.transform.eulerAngles.x, _camera.transform.eulerAngles.y, 180);
            }
            else
            {
                while (time < _timeForCameraMove)
                {
                    _camera.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -(180 * Time.deltaTime) / _timeForCameraMove));
                    // _camera.transform.eulerAngles -= new Vector3(0, (180 * Time.deltaTime) / _timeForCameraMove, 0);
                    time += Time.deltaTime;
                    yield return null;
                }
                 _camera.transform.eulerAngles = new Vector3(_camera.transform.eulerAngles.x, _camera.transform.eulerAngles.y, 0);
            } */
            yield return null;
            // ������ ����� ������ ������
            GiveNewPlayerACard();
        }
        #region GiveACard
        private void GiveNewPlayerACard()
        {
            var newCard = DeckManagerSingleton.GetRandomCardFromDeck(CurrentPlayer);
            Card newCardComp = newCard.GetComponent<Card>();
            //���������� ������� �����
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
            emptyHandPosition.SetLinkedCard(newCard.GetComponent<Card>());
        }
        #endregion
        private void ChangeMana(PlayerSide player, uint deltaCurrent, uint deltaMax = 0)
        {
            _manaDict[player] = (_manaDict[player].currentMana + deltaCurrent, _manaDict[player].maxMana + deltaMax);
            _manaText.text = _manaDict[player].currentMana + "/" + _manaDict[player].maxMana;
        }
        private void SetupHandsBoardDecksDictionaries()
        {
            // ���������� � ������� HandPositions (����� ��� ������ ��������� ����� �� ��� ���� �� �������)
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
            // ���������� � ������� BoardPositions)))))))))))))
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