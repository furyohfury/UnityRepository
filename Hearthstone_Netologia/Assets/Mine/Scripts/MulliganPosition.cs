using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    public class MulliganPosition : MonoBehaviour, IPointerClickHandler
    {
        public Card Card { get; private set; }
        public bool Changed { get; private set; } = false;
        private PlayerSide _currentPlayer;

        public void OnPointerClick(PointerEventData eventData)
        {
            Changed = true;
            if (!Changed) OnMulliganClick?.Invoke(_currentPlayer, this);
        }

        void Start()
        {

        }
        void Update()
        {

        }
        public delegate void MulliganClick(PlayerSide player, MulliganPosition mulpos);
        public event MulliganClick OnMulliganClick;
    }
}