using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPGMine
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private WeaponType _weaponType;
        private BoxCollider _weaponCollider;
        public int _weaponLightAttackDamage {get; private set;}
        public int _weaponHeavyAttackDamage {get; private set;}
        private void Awake()
        {
            _weaponCollider = GetComponent<BoxCollider>();
            _weaponCollider.isTrigger = true;
            _weaponCollider.enabled = false;
        }
        public void SetWeaponDamage(int lad, int had)
        {
            _weaponLightAttackDamage = lad;
            _weaponHeavyAttackDamage = had;
        }
        public void TurnOnWeaponTrigger_AnimationEvent()
        {
            _weaponCollider.enabled = true;
        }
        public void TurnOffWeaponTrigger_AnimationEvent()
        {
            _weaponCollider.enabled = false;
        }
    }
}