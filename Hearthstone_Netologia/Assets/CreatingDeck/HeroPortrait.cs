using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    public class HeroPortrait : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private SideType _hero;
        public void OnPointerClick(PointerEventData eventData)
        {
            OnHeroChosen?.Invoke(_hero);
        }
        public delegate void HeroChosen(SideType hero);
        public event HeroChosen OnHeroChosen;
    }
}