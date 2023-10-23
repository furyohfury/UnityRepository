using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPG.Units;

namespace RPG.UI
{
    public class FocusUI : MonoBehaviour
    {
        private Image _image;
        [SerializeField]
        private Color _friendlyColor;
        [SerializeField]
        private Color _defaultColor;
        [SerializeField]
        private Color _enemyColor;

        [Space, SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _healthText;

        [Space, SerializeField, Range(10f, 500f)]
        private float _maxDistance = 500f;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }
        private void LateUpdate()
        {
            var ray = Camera.main.ScreenPointToRay(transform.position);
            UnitStatsComponent stats;
            
            if (Physics.Raycast(ray, out var hit, _maxDistance))
            {
                stats = hit.transform.GetComponent<UnitStatsComponent>();
                if (stats == null)
                {
                    ClearFocus();
                    return;
                }
            }
            else
            {
                ClearFocus();
                return;
            }
            _nameText.text = stats.Name;
            _healthText.text = string.Concat(stats.CurrentHealth, '/', stats.MaxHealth);

            switch (stats.SideType)
            {
                case SideType.Friendly:
                    _image.color = _friendlyColor;
                    _nameText.color = _friendlyColor;
                    break;
                case SideType.Enemy:
                    _image.color = _enemyColor;
                    _nameText.color = _enemyColor;
                    break;
            }
        }
        private void ClearFocus()
        {
            _image.color = _nameText.color = _defaultColor;
            _nameText.text = _healthText.text = string.Empty;
        }
    }
}