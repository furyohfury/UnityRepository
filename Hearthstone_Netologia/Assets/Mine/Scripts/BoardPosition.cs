using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class BoardPosition : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerSide Player { get; private set; }
        [field: SerializeField]
        public Card LinkedCard { get; private set; }
        // Start is called before the first frame update
        public void SetLinkedCard(Card card) => LinkedCard = card;
    }
}