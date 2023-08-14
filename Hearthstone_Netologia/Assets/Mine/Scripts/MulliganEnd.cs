using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Cards.MulliganManager;
namespace Cards
{
    public class MulliganEnd : MonoBehaviour, IPointerClickHandler
    {
        private PlayerSide _currentPlayer = PlayerSide.One;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!MulliganSingleton.Input) return;
            Debug.Log("Mulligan End clicked");
            OnMulliganEndClick?.Invoke(_currentPlayer);
            _currentPlayer = PlayerSide.Two;
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