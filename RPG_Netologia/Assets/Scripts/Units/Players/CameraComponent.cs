using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Units.Player
{
    public class CameraComponent : MonoBehaviour
    {
        private PlayerControls _controls;
        private Unit _target;

        public Transform PivotTransform { get; private set; }
        private Transform _camera;

        //������� ��������� �������� ������ ��� OX
        private float _angleX;
        //������� ��������� �������� ������ ��� OY
        private float _angleY;

        [SerializeField, Range(-90f, 0f), Tooltip("��� ������ ������ �� ���������")]
        private float _minY = -45f;
        [SerializeField, Range(0f, 90f), Tooltip("���� ������ ������ �� ���������")]
        private float _maxY = 30f;
        [Space, SerializeField, Range(0.5f, 10f)]
        private float _moveSpeed = 6f;
        [SerializeField, Range(0.1f, 1f)]
        private float _rotateSpeed = 0.1f;

        [SerializeField, Range(10f, 0.1f), Tooltip("����������� ��������")]
        private float _smoothing = 8f;
        [SerializeField, Range(0.1f, 10f)]
        private float _lockCameraSpeed = 1.5f;

        private Vector3 _pivotEulers;
        // ������� ������ �� ���������
        private Quaternion _pivotTargetRotation;
        // ������� ������ �� �����������
        private Quaternion _transformTargetRotation;
        /// <summary>
        /// ������� ������ ������������ ������ �� ���������
        /// </summary>
        private Quaternion _defaultCameraRotation;

        private void Awake()
        {
            _controls = new();
        }
        private void OnEnable()
        {
            _controls.Camera.Enable();
        }
        private void Start()
        {
            _target = transform.parent.GetComponent<Unit>();
            PivotTransform = transform.GetChild(0);
            _pivotEulers = PivotTransform.eulerAngles;
                        
            _camera = GetComponentInChildren<Camera>().transform;
            _defaultCameraRotation = _camera.localRotation;
            transform.parent = null;

            _target.OnTargetLostHandler += () =>  _camera.localRotation = _defaultCameraRotation; 
        }
        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _target.transform.position, Time.deltaTime * _moveSpeed);
            if (_target.Target == null) FreeCamera();
            else LockCamera();
        }
        private void OnDisable()
        {
            _controls.Camera.Disable();
        }
        
        private void FreeCamera()
        {
            var delta = _controls.Camera.Delta.ReadValue<Vector2>();

            _angleX += delta.x * _rotateSpeed;
            _angleY -= delta.y * _rotateSpeed;
            _angleY = Mathf.Clamp(_angleY, _minY, _maxY);

            _pivotTargetRotation = Quaternion.Euler(_angleY, _pivotEulers.y, _pivotEulers.z);
            _transformTargetRotation = Quaternion.Euler(0f, _angleX, 0f);

            PivotTransform.localRotation = Quaternion.Slerp(PivotTransform.localRotation, _pivotTargetRotation, _smoothing * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _transformTargetRotation, _smoothing * Time.deltaTime);
        }
        private void LockCamera()
        {
            var rotation = Quaternion.LookRotation(_target.Target.GetTargetPoint.position - _camera.position);
            _camera.rotation = Quaternion.Slerp(_camera.rotation, rotation, _lockCameraSpeed * Time.deltaTime);

            rotation = Quaternion.LookRotation(_target.Target.GetTargetPoint.position - PivotTransform.position);
            PivotTransform.rotation = Quaternion.Slerp(PivotTransform.rotation, rotation, _lockCameraSpeed * Time.deltaTime);

            // ������ ���
            // _camera.LookAt(_target.Target.transform.position, Vector3.up);
            // transform.LookAt(_target.Target.transform.position, Vector3.up);
        }
    }
}