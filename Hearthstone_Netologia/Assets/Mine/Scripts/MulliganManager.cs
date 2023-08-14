using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cards.DecksManager;
using static Cards.GameCycleManager;
using System.Linq;

namespace Cards
{
    public class MulliganManager : MonoBehaviour
    {
        public static MulliganManager MulliganSingleton;
        private List<MulliganPosition> _mulliganPositions = new();
        [SerializeField]
        private MulliganEnd _mulliganEnd;
        [SerializeField]
        private int _mulliganSize = 3;
        [SerializeField]
        private GameObject _mulliganPositionPrefab;
        public bool Input { get; private set; } = true;
        [SerializeField]
        private float _timeToMove = 1f;
        private PlayerSide _currentPlayer = PlayerSide.One;
        private void Awake()
        {
            if (MulliganSingleton != null) Destroy(this);
            else MulliganSingleton = this;
        }
        private void OnEnable()
        {
            _mulliganEnd.OnMulliganEndClick += MulliganFinish;
        }
        private void OnDisable()
        {
            _mulliganEnd.OnMulliganEndClick -= MulliganFinish;
        }       

        private void Start()
        {
            CreateMulliganPositions();
            MulliganStart(_currentPlayer);
        }
        private void CreateMulliganPositions()
        {
            Vector3 position = _mulliganPositionPrefab.transform.position;
            for (int i = 0; i < _mulliganSize; i++)
            {
                GameObject mulliganPosition = Instantiate(_mulliganPositionPrefab, position, Quaternion.identity);
                _mulliganPositions.Add(mulliganPosition.GetComponent<MulliganPosition>());
                position += (3f / _mulliganSize) * 200 * Vector3.right;
                mulliganPosition.name = "MulliganPosition" + i;
            }
        }
        private void MulliganStart(PlayerSide player)
        {
            // Взятие карт из колоды на муллиган
            ResetMulliganPositions();
            List<GameObject> _mulliganCards = new();
            do
            {
                GameObject card = DeckManagerSingleton.GetRandomCardFromDeck(player, true);
                if (!_mulliganCards.Contains(card))
                {
                    _mulliganCards.Add(card);
                    card.transform.position = DeckManagerSingleton.GetDeckPosition(player);
                    card.GetComponent<Card>().BeingMulliganed(true);
                }
                else Destroy(card);
            } while (_mulliganCards.Count < _mulliganSize);
            // Анимация выдачи и присваивание позициям как LinkedCard
            StartCoroutine(MulliganStartAnimation(player, _mulliganCards));
        }
        private IEnumerator MulliganStartAnimation(PlayerSide player, IEnumerable<GameObject> mulliganCards)
        {
            Input = false;
            List<GameObject> mulCards = mulliganCards.ToList();            
            // Выдать карты на позиции
            for (int i = 0; i < _mulliganSize; i++)
            {
                float time = 0;
                Vector3 startPos = mulCards[i].transform.position;
                Vector3 endPos = new Vector3(_mulliganPositions[i].transform.position.x, _mulliganPositions[i].transform.position.y - 1, _mulliganPositions[i].transform.position.z);
                while (time < _timeToMove)
                {                    
                    mulCards[i].transform.position = Vector3.Lerp(startPos, endPos, time / _timeToMove);
                    mulCards[i].transform.rotation *= Quaternion.Euler(new Vector3(0, 0,  (180 * Time.deltaTime) / _timeToMove));
                    time += Time.deltaTime;
                    yield return null;
                }
                mulCards[i].transform.position = endPos;
                mulCards[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                _mulliganPositions[i].SetLinkedCard(mulCards[i].GetComponent<Card>());
            }
            Input = true;
        }
        private void MulliganChange(PlayerSide player, IEnumerable<MulliganPosition> mulliganPositions)
        {
            StartCoroutine(MulliganChangeCardsAnimation(player, mulliganPositions));
        }        
        private IEnumerator MulliganChangeCardsAnimation(PlayerSide player, IEnumerable<MulliganPosition> mulliganPositions)
        {
            Input = false;
            //вернуть одну, дестрой её, отдать новую
            foreach(MulliganPosition mullPos in mulliganPositions)
            {
                float time = 0;
                Vector3 startPos = mullPos.LinkedCard.transform.position;
                Vector3 endPos = DeckManagerSingleton.GetDeckPosition(player);
                //лерп старой карты к колоде
                while (time < _timeToMove)
                {
                    mullPos.LinkedCard.transform.position = Vector3.Lerp(startPos, endPos, time / _timeToMove);
                    time += Time.deltaTime;
                    yield return null;
                }
                mullPos.LinkedCard.transform.position = endPos;
                DeckManagerSingleton.AddCardToDeck(player, mullPos.LinkedCard);
                // Лерп карты newCard к mullPos
                GameObject newCard = DeckManagerSingleton.GetRandomCardFromDeck(player);
                time = 0;
                startPos = newCard.transform.position;
                endPos = mullPos.transform.position;
                while (time < _timeToMove)
                {
                    newCard.transform.position = Vector3.Lerp(startPos, endPos, time / _timeToMove);
                    newCard.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (180 * Time.deltaTime) / _timeToMove));
                    time += Time.deltaTime;
                    yield return null;
                }
                newCard.transform.position = endPos;
                newCard.transform.eulerAngles = new Vector3(0, 0, 180);
                mullPos.SetLinkedCard(newCard.GetComponent<Card>());                
            }
            // Добавление выбранных карт в руку
            List<HandPosition> handOfPlayer = FindObjectsOfType<HandPosition>().Where((handPos) => handPos.Player == player).OrderBy((pos) => pos.gameObject.name).ToList();
            StartCoroutine(MulliganFinishAnimation(player, handOfPlayer));
            Input = true;            
        }

        private void MulliganFinish(PlayerSide player)
        {
            // Проверка всех нажатых и смена у них карт
            List<MulliganPosition> mulliganToChange = _mulliganPositions.Where((position) => position.Change).ToList();
            MulliganChange(player, mulliganToChange);                     
        }
        private IEnumerator MulliganFinishAnimation(PlayerSide player, IEnumerable<HandPosition> hand)
        {
            Input = false;
            List<HandPosition> handPositions = hand.ToList();
            int i = 0;
            // Перенос кард в руку
            foreach (var mullPos in _mulliganPositions)
            {
                handPositions[i].SetLinkedCard(mullPos.LinkedCard);
                float time = 0;
                Vector3 startPos = mullPos.LinkedCard.transform.position;
                Vector3 endPos = handPositions[i++].transform.position;
                // Лерп карты к позиции в руке
                while (time < _timeToMove)
                {
                    mullPos.LinkedCard.transform.position = Vector3.Lerp(startPos, endPos, time / _timeToMove);
                    time += Time.deltaTime;
                    yield return null;
                }
                mullPos.LinkedCard.transform.position = endPos;
                mullPos.LinkedCard.GetComponent<Card>().BeingMulliganed(false);
            }            
            Input = true;
            if ((int)_currentPlayer + 1 < Enum.GetNames(typeof(PlayerSide)).Length)
            {
                MulliganStart(++_currentPlayer);
            }
            else MulliganOver();
        }
        private void MulliganOver()
        {
            // Уничтожение всех элементов для муллигана
            Destroy(_mulliganEnd.gameObject);
            foreach(var mullPos in _mulliganPositions)
            {
                Destroy(mullPos);
            }
            Debug.Log("Mulligan is over");
            // Включение менеджера цикла игры
            GameCycleSingleton.enabled = true;
        }
        private void ResetMulliganPositions()
        {
            foreach (var mullPos in _mulliganPositions)
            {
                mullPos.ResetMullliganPosition();
            }
        }
    }
}