using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace Network
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private PhotonView _bulletPhotonView;
        [SerializeField]
        private float _bulletsLifeTime = 5f;
        [SerializeField, Range(1, 10)]
        private float _bulletSpeed = 5;
        [field: SerializeField, Range(1, 10)]
        public int BulletDamage { get; private set; } = 5;
        private Coroutine _dying;
        private void Start()
        {
            _dying = StartCoroutine(BulletDying());
        }
        private void Update()
        {
            BulletMovement();
        }
        private void OnDestroy()
        {
            StopCoroutine(_dying);
        }
        private IEnumerator BulletDying()
        {
            yield return new WaitForSeconds(_bulletsLifeTime);
            if (gameObject != null && _bulletPhotonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        private void BulletMovement()
        {
            if (gameObject != null && _bulletPhotonView.IsMine)
            {
                transform.position += _bulletSpeed * Time.deltaTime * transform.up;
            }            
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<Player>(out _))
            {
                Debugger.Log("bullet collision");
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}