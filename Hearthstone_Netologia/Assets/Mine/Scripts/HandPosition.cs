using UnityEngine;

namespace Cards
{
    public class HandPosition : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerSide Player { get; private set; }
        [field: SerializeField]
        public Card LinkedCard { get; private set; }
        public void SetLinkedCard(Card card) => LinkedCard = card;

    }
}