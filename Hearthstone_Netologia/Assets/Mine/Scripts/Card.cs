using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Cards {
    public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected static GameObject _draggingCard;
        private Camera _camera;
        [SerializeField]
        private GameObject _image;
        [SerializeField]
        private TextMeshPro _cost;
        [SerializeField]
        private TextMeshPro _name;
        [SerializeField]
        private TextMeshPro _attack;
        [SerializeField]
        private TextMeshPro _health;
        [SerializeField]
        private TextMeshPro _type;
        [SerializeField]
        private TextMeshPro _description;
        public void OnBeginDrag(PointerEventData eventData)
        {
            _draggingCard = gameObject;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_draggingCard == null) return;
            Vector3 pos = new(_camera.ScreenToWorldPoint(Input.mousePosition).x, _draggingCard.transform.position.y, _camera.ScreenToWorldPoint(Input.mousePosition).z);
            _draggingCard.transform.position = pos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // CarPosition - 6 слой
            LayerMask handposition = LayerMask.GetMask("Hand", "Board");
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 20, handposition))
            {                
                _draggingCard.transform.position = new Vector3(hit.collider.transform.position.x, _draggingCard.transform.position.y, hit.collider.transform.position.z);
            }
            _draggingCard = null;
        }
        private void Awake()
        {
            _camera = Camera.main;
        }
        void Start()
        {

        }

        void Update()
        {

        }
    }
}