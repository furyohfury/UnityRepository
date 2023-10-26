using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.Player
{
    public class WeaponSetComponent : MonoBehaviour
    {
        private WeaponType _type;

        [SerializeField]
        private WeaponSet[] _weapons;

        public WeaponType WeaponType
        {
            get => _type;
            set
            {
                ChangeSet(_type, value);
                _type = value;
            }
        }

        private void ChangeSet(WeaponType disableType, WeaponType enableType)
        {
            foreach(var weapon in _weapons)
            {
                if (weapon.Type == disableType) weapon.Object.SetActive(false);
                else if (weapon.Type == enableType) weapon.Object.SetActive(true);
            }
        }

        [System.Serializable]
        internal struct WeaponSet
        {
            public WeaponType Type;
            public GameObject Object;
        }
    }
}