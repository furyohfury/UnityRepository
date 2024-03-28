using UnityEngine;
namespace Pasians
{
    public class Card
    {
        private CardView _cardView;

        private Suit _suit;
        private Cost _cost;

        public Card(CardView view, Suit suit, Cost cost)
        {
            _cardView = view;
            _suit = suit;
            _cost = cost;
        }
    }
}