using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cars
{
    public class WheelsComponent : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _frontTransforms;
        [SerializeField]
        private Transform[] _rearTransforms;

        [Space, SerializeField]
        private WheelCollider[] _frontWheels;
        [SerializeField]
        private WheelCollider[] _rearWheels;
        [SerializeField]
        private WheelCollider[] _allWheels;

        public WheelCollider[] GetFrontWheels => _frontWheels;
        public WheelCollider[] GetRearWheels => _rearWheels;

        public WheelCollider[] GetAllWheels => _allWheels;

        public void UpdateVisual(float angle)
        {
            for (int i = 0; i < _frontTransforms.Length; i++)
            {
                _frontWheels[i].steerAngle = angle;
                _frontWheels[i].GetWorldPose(out Vector3 pos, out Quaternion rot);
                _frontTransforms[i].position = pos;
                _frontTransforms[i].rotation = rot;

                _rearWheels[i].GetWorldPose(out pos, out rot);
                _rearTransforms[i].SetPositionAndRotation(pos, rot);
            }
        }

        private void Start()
        {
            _allWheels = new WheelCollider[] { _frontWheels[0], _frontWheels[1], _rearWheels[0], _rearWheels[1] };

        }

    }
}