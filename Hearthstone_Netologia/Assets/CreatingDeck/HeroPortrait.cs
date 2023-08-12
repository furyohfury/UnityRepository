using System.Collections;
using System.Collections.Generic;
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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public delegate void HeroChosen(SideType hero);
        public event HeroChosen OnHeroChosen;
    }
}