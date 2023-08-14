using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Cards.MulliganManager;

namespace Cards
{
    public class MulliganPosition : MonoBehaviour, IPointerClickHandler
    {
        [field :SerializeField]
        public Card LinkedCard { get; private set; }
        public bool Change { get; private set; } = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!MulliganSingleton.Input) return;
            LinkedCard.transform.eulerAngles = Vector3.zero;
            Change = true;
            // if (!Changed) OnMulliganClick?.Invoke(_currentPlayer, this);
        }

        public void SetLinkedCard(Card card) => LinkedCard = card;
        public void ResetMullliganPosition() => Change = false;
        
        // public delegate void MulliganClick(PlayerSide player, MulliganPosition mulpos);
        // public event MulliganClick OnMulliganClick;
    }
}