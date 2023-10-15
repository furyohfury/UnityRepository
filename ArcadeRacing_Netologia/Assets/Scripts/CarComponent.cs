using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cars
{
    public class CarComponent : MonoBehaviour
    {
        private WheelsComponent _wheels;
        private BaseInputController _input;
        private Rigidbody _rigidBody;

        [SerializeField, Range(5f, 60f)]
        private float _maxSteerAngle = 25f;
        /// <summary>
        /// Крутящий момент Н/м
        /// </summary>
        [SerializeField]
        private float _torque = 2500f;
        [SerializeField, Range(0f, float.MaxValue)]
        private float _handBrakeTorque = float.MaxValue;
        [SerializeField]
        private Vector3 _centerOfMass;

        private void FixedUpdate()
        {
            _wheels.UpdateVisual(_input.Rotate * _maxSteerAngle);
            // Делится на 2 из-за того что момент идет на 2 колеса
            var torque = _input.Acceleration * _torque / 2f;
            foreach (var wheel in _wheels.GetFrontWheels)
                wheel.motorTorque = torque;
        }
        private void Start()
        {
            _wheels = GetComponent<WheelsComponent>();
            _input = GetComponent<BaseInputController>();
            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.centerOfMass = _centerOfMass;
            _input.OnHandBrakeEvent += OnHandBrake;
        }
        private void OnDestroy()
        {
            _input.OnHandBrakeEvent -= OnHandBrake;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.TransformPoint(_centerOfMass), .4f);
        }
        private void OnHandBrake(bool value)
        {
            if (value)
            {
                foreach (var wheel in _wheels.GetRearWheels)
                {
                    wheel.brakeTorque = _handBrakeTorque;
                    wheel.motorTorque = 0f;
                }
                    
            }
            else
            {
                foreach (var wheel in _wheels.GetRearWheels)
                    wheel.brakeTorque = 0f;
            }
        }
    }
}