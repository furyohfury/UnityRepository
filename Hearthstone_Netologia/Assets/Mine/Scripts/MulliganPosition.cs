using UnityEngine;
using UnityEngine.EventSystems;
using static Cards.MulliganManager;

namespace Cards
{
    public class MulliganPosition : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField]
        public Card LinkedCard { get; private set; }
        public bool Change { get; private set; } = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!MulliganSingleton.InputMull) return;
            LinkedCard.transform.eulerAngles = Vector3.zero;
            Change = true;
        }

        public void SetLinkedCard(Card card) => LinkedCard = card;
        public void ResetMullliganPosition() => Change = false;
    }
}