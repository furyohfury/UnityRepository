using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Units
{
    public class UnitStatsComponent : MonoBehaviour
    {
        [Range(0f, 10f)]
        public float MoveSpeed = 3f;

        public float CurrentHealth { get; set; }
        public SideType SideType;
        public string Name;

        public float _cooldownShieldAttack = 2f;
        public float _currentCooldown;

        public float MaxHealth { get; private set; } = 10f;
        private void Start()
        {
            CurrentHealth = MaxHealth;
        }
        private void Update()
        {
            if (_currentCooldown > 0f) _currentCooldown -= Time.deltaTime;
        }
    }
}