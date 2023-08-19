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
        [SerializeField]
        private MulliganEnd _mulliganEnd;
        [SerializeField]
        private int _mulliganSize = 3;
        [SerializeField]
        private GameObject _mulliganPositionPrefab;
        [SerializeField]
        private float _timeToMove = 1f;
        public bool InputMull { get; private set; } = true;
        private PlayerSide _currentPlayer = PlayerSide.One;
        private List<MulliganPosition> _mulliganPositions = new();
        private Dictionary<PlayerSide, PlayerPortrait> _playerPortraitsDict = new();
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
            SettingHeroes();
            CreateMulliganPositions();
            MulliganStart(_currentPlayer);
        }
        private void SettingHeroes()
        {
            // Портреты героев из префов
            for (int i = 0; i < 2; i++)
            {
                _playerPortraitsDict.Add((PlayerSide)i, FindObjectsOfType<PlayerPortrait>().Single(portrait => portrait.Player == (PlayerSide)i));
            }
            for (int i = 0; i < 2; i++)
            {
                string heroName = (PlayerSide)i == PlayerSide.One ? PlayerPrefs.GetString("PlayerOneHero") : PlayerPrefs.GetString("PlayerTwoHero");
                Texture heroTexture = (Texture)Resources.Load("Heroes/" + heroName);
                _playerPortraitsDict[(PlayerSide)i].Hero = Enum.Parse<SideType>(heroName);
                MeshRenderer mesh = _playerPortraitsDict[(PlayerSide)i].gameObject.GetComponent<MeshRenderer>();
                mesh.material.mainTexture = heroTexture;
                mesh.material.mainTextureScale = new Vector2(-1, -1);
            }
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
            InputMull = false;
            // Взятие карт из колоды на муллиган
            ResetMulliganPositions();
            List<GameObject> _mulliganCards = new();
            do
            {
                GameObject card = DeckManagerSingleton.GetRandomCardFromDeck(player);
                if (!_mulliganCards.Contains(card))
                {
                    _mulliganCards.Add(card);
                    card.transform.position = DeckManagerSingleton.GetDeckPosition(player);
                    card.GetComponent<Card>().SetBeingMulliganed(true);
                }
                else Destroy(card.gameObject);
            } while (_mulliganCards.Count < _mulliganSize);
            InputMull = true;
            // Анимация выдачи и присваивание позициям как LinkedCard
            StartCoroutine(MulliganStartAnimation(player, _mulliganCards));
        }
        private IEnumerator MulliganStartAnimation(PlayerSide player, IEnumerable<GameObject> mulliganCards)
        {
            InputMull = false;
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
                    mulCards[i].transform.rotation *= Quaternion.Euler(new Vector3(0, 0, (180 * Time.deltaTime) / _timeToMove));
                    time += Time.deltaTime;
                    yield return null;
                }
                mulCards[i].transform.position = endPos;
                mulCards[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                _mulliganPositions[i].SetLinkedCard(mulCards[i].GetComponent<Card>());
            }
            InputMull = true;
        }
        private void MulliganChange(PlayerSide player, IEnumerable<MulliganPosition> mulliganPositions)
        {
            StartCoroutine(MulliganChangeCardsAnimation(player, mulliganPositions));
        }
        private IEnumerator MulliganChangeCardsAnimation(PlayerSide player, IEnumerable<MulliganPosition> mulliganPositions)
        {
            InputMull = false;
            //вернуть одну, дестрой её, отдать новую
            foreach (MulliganPosition mullPos in mulliganPositions)
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
                Destroy(mullPos.LinkedCard.gameObject);
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
        }

        private void MulliganFinish(PlayerSide player)
        {
            // Проверка всех нажатых и смена у них карт
            List<MulliganPosition> mulliganToChange = _mulliganPositions.Where((position) => position.Change).ToList();
            MulliganChange(player, mulliganToChange);
        }
        private IEnumerator MulliganFinishAnimation(PlayerSide player, IEnumerable<HandPosition> hand)
        {
            InputMull = false;
            List<HandPosition> handPositions = hand.ToList();
            int i = 0;
            // Перенос кард в руку
            foreach (var mullPos in _mulliganPositions)
            {
                // HandPosition и Card присваиваем инфу друг о друге
                handPositions[i].SetLinkedCard(mullPos.LinkedCard);
                mullPos.LinkedCard.SetLinkedHandPosition(handPositions[i]);
                mullPos.LinkedCard.Player = player;
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
                mullPos.LinkedCard.GetComponent<Card>().SetBeingMulliganed(false);
            }
            InputMull = true;
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
            foreach (var mullPos in _mulliganPositions)
            {
                Destroy(mullPos.gameObject);
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