using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units
{
    public class TriggerComponent : MonoBehaviour
    {
        private Collider _collider;
        [SerializeField]
        private int _id = 0;
        private Unit _unit;
        private UnitStatsComponent _stats;

        public int GetID => _id;

        public bool Enable
        {
            get => _collider.enabled;
            set => _collider.enabled = value;
        }
        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            _collider.isTrigger = true;
        }
        public void Construct(Unit unit, UnitStatsComponent stats)
        {
            _unit = unit; _stats = stats;
        }
        private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponent<UnitStatsComponent>();

            if (unit == null) return;
            unit.CurrentHealth -= 5f;

            if (_id == 115)
            {
                var body = other.GetComponent<Rigidbody>();
                if (body == null) return;
                body.constraints = RigidbodyConstraints.None;
                other.transform.LookAt(_unit.transform);
                body.AddForce(-other.transform.forward * 1000f, ForceMode.Impulse);
            }

            Debug.Log(other.name + " has been hit");

            if (unit.CurrentHealth <= 0f) Destroy(unit.gameObject);
        }
    }
}