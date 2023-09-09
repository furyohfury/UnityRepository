using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using static Network.GameManager;
using UnityEditor;

namespace Network {
    public class Player : MonoBehaviour
    {
        private PlayerControls _controls;
        private CharacterController _charController;
        [SerializeField]
        private Camera _camera;
        private GameObject _bullet;
        [SerializeField, Range(1, 10)]
        private float _cameraSpeed = 7;
        [field: SerializeField]
        public int GetHealth { get; private set; } = 10;
        [SerializeField, Range(1, 100)]
        private float _moveSpeed = 5;
        private void Awake()
        {
            _controls = new PlayerControls();
            _controls.Enable();
            _charController = GetComponent<CharacterController>();
            _bullet = Resources.Load<GameObject>("Prefabs/Bullet");

        }
        private void Start()
        {            
            
        }
        private void OnEnable()
        {
            _controls.ActionMap.Fire.performed += OnFire;
            _controls.ActionMap.Quit.performed += OnQuit;
        }
        private void OnFire(CallbackContext context)
        {
            Debug.Log("FIRE");
            GameObject bullet = Instantiate(_bullet, transform.position + new Vector3(0.8f, 0, 2), transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
            Bullet bulletComp = bullet.GetComponent<Bullet>();
            if (bulletComp == null) Debug.Log("null");
            Manager.AddBullet(bulletComp);
        }
        private void OnQuit(CallbackContext context)
        {
            Debug.Log("Quit");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;

#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN
            Application.Quit();
#endif
        }
        private void Update()
        {
            Movement();
            CameraMovement();
            // Debug.Log(movement);
        }
        private void OnDisable()
        {
            _controls.ActionMap.Fire.performed -= OnFire;
            _controls.ActionMap.Quit.performed -= OnQuit;
        }
        private void OnDestroy()
        {
            _controls.Disable();
        }
        private void CameraMovement()
        {
            Vector2 cameraMove = _controls.ActionMap.CameraMove.ReadValue<Vector2>();
            transform.rotation *= Quaternion.Euler(new Vector3(0, cameraMove.x * Time.deltaTime * _cameraSpeed, 0));
        }
        private void Movement()
        {
            float RotationInRads = (float)(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            Vector2 movement = _moveSpeed * _controls.ActionMap.Movement.ReadValue<Vector2>();
            movement = new Vector2(movement.x * (float)System.Math.Cos(RotationInRads) + movement.y * (float)System.Math.Sin(RotationInRads), -movement.x * (float)System.Math.Sin(RotationInRads) + movement.y * (float)System.Math.Cos(RotationInRads));
            _charController.SimpleMove(new Vector3(movement.x, 0, movement.y));
        }
    }
}