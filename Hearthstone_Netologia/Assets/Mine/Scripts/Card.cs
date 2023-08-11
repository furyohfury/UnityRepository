using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Cards {
    public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        protected static GameObject _draggingCard;
        private Camera _camera;
        [SerializeField]
        private MeshRenderer _image;
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
        private CardPropertyData _cardData;
        private void Awake()
        {
            _camera = Camera.main;
        }
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
            // CarPosition - 6 ����
            LayerMask handposition = LayerMask.GetMask("Hand", "Board");
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 20, handposition))
            {                
                _draggingCard.transform.position = new Vector3(hit.collider.transform.position.x, _draggingCard.transform.position.y, hit.collider.transform.position.z);
            }
            _draggingCard = null;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.localScale *= 1.2f;
            // gameObject.transform.position += Vector3.up * 2;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.localScale /= 1.2f;
            // gameObject.transform.position -= Vector3.up * 2;
        }
        public void SetCardDataAndVisuals(Texture image, int cost, string name, int attack, int health, CardUnitType type, string description)
        {
            _image.material.mainTexture = image;
            _image.material.mainTextureScale = new Vector2(1, 1);
            _cardData._image = image;
            _cost.text = "" + cost;
            _cardData._cost = cost;
            _name.text = name;
            _cardData._name = name;
            _attack.text = "" + attack;
            _cardData._attack = attack;
            _health.text = "" + health;
            _cardData._health = health;
            _type.text = "" + type;
            _cardData._type = type;
            _description.text = description;
            _cardData._description = description;
        }
        /* public void SetCardData(int cost, Texture image, string name, string description, int attack, int health, CardUnitType type)
        {
            _cardData._cost = 
        } */
        

    }
}