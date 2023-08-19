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
            if (!MulliganSingleton.InputMull) return;
            // Debug.Log("Mulligan End clicked");
            OnMulliganEndClick?.Invoke(_currentPlayer);
            _currentPlayer = PlayerSide.Two;
        }
        public delegate void MulliganEndClick(PlayerSide player);
        public event MulliganEndClick OnMulliganEndClick;
    }
}