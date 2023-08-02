using UnityEngine;

namespace Zikkurat.CameraControl
{
    public class CameraControl : MonoBehaviour
    {
        private CameraInput _cameraInput;
        [SerializeField]
        private float _cameraMoveSpeed = 25;
        [SerializeField]
        private float _cameraRotationSpeed = 10;
        private void Awake()
        {
            _cameraInput = new();
        }
        private void OnEnable()
        {
            _cameraInput.Enable();
        }
        private void OnDisable()
        {
            _cameraInput.Disable();
        }
        void Update()
        {
            CameraMovement();
        }
        private void CameraMovement()
        {
            var _cameraWASD = _cameraInput.CameraMap.CameraActions.ReadValue<Vector2>();
            var _cameraMouse = _cameraInput.CameraMap.CameraMouse.ReadValue<Vector2>();
            transform.position += _cameraMoveSpeed * Time.deltaTime * _cameraWASD.y * transform.forward + _cameraMoveSpeed * Time.deltaTime * _cameraWASD.x * transform.right;
            // transform.rotation *= Quaternion.Euler(_cameraRotationSpeed * Time.deltaTime * new Vector3(-_cameraMouse.y, _cameraMouse.x, 0));
            transform.eulerAngles += _cameraRotationSpeed * Time.deltaTime * new Vector3(-_cameraMouse.y, _cameraMouse.x, 0);
        }
    }
}