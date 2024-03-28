using UnityEngine;
namespace Pasians
{
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Collider2D _collider;
        [SerializeField]
        private Sprite _sprite;

        public void MoveToPosition(Vector3 pos)
            => transform.position = pos;

        public void OnBeginDrag(PointerEventData eventData)
        {
            // turn on sfx, visual highlight
        }

        public void OnDrag(PointerEventData eventData)
            => MoveToPosition(eventData.position);

        public void OnEndDrag(PointerEventData eventData)
        {
            // turn off sfx, visual highlight
            // check where by rays
        }

        public void OnPointerEnter(PointerEventData eventData){}

        public void OnPointerExit(PointerEventData eventData){}
        
    }
}