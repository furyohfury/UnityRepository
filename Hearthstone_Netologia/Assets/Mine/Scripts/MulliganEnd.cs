using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Cards
{
    public class MulliganEnd : MonoBehaviour, IPointerClickHandler
    {
        private PlayerSide _currentPlayer;
        public void OnPointerClick(PointerEventData eventData)
        {
            OnMulliganEndClick?.Invoke(_currentPlayer);
        }

        void Start()
        {

        }

        void Update()
        {

        }
        public delegate void MulliganEndClick(PlayerSide player);
        public event MulliganEndClick OnMulliganEndClick;
    }
}