using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cards.DecksManager;

namespace Cards
{
    public class MulliganManager : MonoBehaviour
    {
        public static MulliganManager MulliganSingleton;
        private List<GameObject> _mulliganPositions = new();
        [SerializeField]
        private MulliganEnd _mulliganEnd;
        [SerializeField]
        private int _mulliganSize = 3;
        [SerializeField]
        private GameObject _mulliganPositionPrefab;
        private bool _input = true;
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
            foreach (GameObject mulliganPos in _mulliganPositions)
            {
                MulliganPosition mulliganPosComp = mulliganPos.GetComponent<MulliganPosition>();
                mulliganPosComp.OnMulliganClick -= MulliganChange;
            }
        }       

        private void Start()
        {
            CreateMulliganPositions();
            foreach (GameObject mulliganPos in _mulliganPositions)
            {
                MulliganPosition mulliganPosComp = mulliganPos.GetComponent<MulliganPosition>();
                mulliganPosComp.OnMulliganClick += MulliganChange;
            }
        }
        public void MulliganStart(PlayerSide player)
        {
            // Выдача карт на муллиган
            List<GameObject> _mulliganCards = new();
            do
            {
                var card = DeckManagerSingleton.GetRandomCardFromDeck(player, true);
                if (!_mulliganCards.Contains(card)) _mulliganCards.Add(card);
                else Destroy(card);
            } while (_mulliganCards.Count < _mulliganSize);
            // Анимация выдачи
            StartCoroutine(MulliganStartAnimation(player, _mulliganCards));
        }
        private void MulliganChange(PlayerSide player, MulliganPosition mulliganPosition)
        {
            StartCoroutine(MulliganChangeCardsAnimation(player, mulliganPosition));
        }
        private IEnumerator MulliganStartAnimation(PlayerSide player, IEnumerable<GameObject> mulliganCards)
        {
            _input = false;
            // выдать карты на позиции
            yield return null;
            _input = true;
        }
        private IEnumerator MulliganChangeCardsAnimation(PlayerSide player, MulliganPosition mulliganPosition)
        {
            _input = false;
            //вернуть одну, дестрой её, отдать новую
            //анимация вернуть
            mulliganPosition.Card
            yield return null;
            _input = true;
        }
        private IEnumerator MulliganEndAnimation(PlayerSide player)
        {
            yield return null;
        }
        private void CreateMulliganPositions()
        {
            Vector3 position = _mulliganPositionPrefab.transform.position;
            for (int i = 0; i < _mulliganSize; i++)
            {
                GameObject mulliganPosition = Instantiate(_mulliganPositionPrefab, position, Quaternion.identity);
                _mulliganPositions.Add(mulliganPosition);
                position += (3f / _mulliganSize) * 200 * Vector3.right;
            }
        }
        private void MulliganFinish(PlayerSide player)
        {
            StartCoroutine(MulliganEndAnimation(player));
            //todo добавление выбранных карт в руку
        }
    }
}