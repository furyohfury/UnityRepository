using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace Cards
{
    public class PlayerPortrait : MonoBehaviour
    {
        [field: SerializeField]
        public SideType Hero { get; private set; }
        [field: SerializeField]
        public PlayerSide Player { get; private set; }
        [field : SerializeField]
        public int Health { get; private set; } = 10;
        private int _maxHP;
        [SerializeField]
        private TextMeshPro _hpText;
        // Start is called before the first frame update
        void Start()
        {
            Health = 10;
            _maxHP = 10;
            _hpText.text = _maxHP + "";
        }
        public void ChangePlayerHealth(int delta, bool isHealing = true)
        {
            if (isHealing)
            {
                if (Health + delta <= _maxHP) Health += delta;
                else Health = _maxHP;
            }
            else
            {
                Health += delta;
                _maxHP += delta;
            }
            _hpText.text = Health + "";
            if (Health <= 0)
            {
                //todo анимация взрыв портрета героя и победа
#if UNITY_EDITOR
                EditorApplication.isPaused = true;
#endif                
            }
        }
    }
}