using System.Collections;
using UnityEngine;
using TMPro;

namespace Cards
{
    public class PlayerPortrait : MonoBehaviour
    {
        [field: SerializeField]
        public SideType Hero { get; set; }
        [field: SerializeField]
        public PlayerSide Player { get; private set; }
        public int Health { get; private set; }
        [SerializeField]
        private int _maxHP = 10;
        [SerializeField]
        private TextMeshPro _hpText;
        [SerializeField]
        private GameObject _explosion;
        [SerializeField]
        private AudioClip _loseSound;
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private GameObject _damageMask;
        void Start()
        {
            Health = _maxHP;
            _hpText.text = _maxHP + "";
        }
        public void ChangePlayerHealth(int delta, bool isHealing = false)
        {
            if (delta < 0)
            {
                StartCoroutine(DamageReceived());
            }
            if (isHealing)
            {
                if (Health + delta <= _maxHP) Health += delta;
                else Health = _maxHP;
            }
            else
            {
                Health += delta;
                _maxHP += delta;
            }
            _hpText.text = Health + "";
            if (Health <= 0)
            {
                StartCoroutine(LoseAnimation());
            }
        }
        private IEnumerator LoseAnimation()
        {
            Instantiate(_explosion, transform.position + Vector3.up * 10, Quaternion.Euler(new Vector3(90, 0, 0)));
            _audioSource.PlayOneShot(_loseSound);
            yield return new WaitForSeconds(2f);
            OnLost?.Invoke(this);
        }
        private IEnumerator DamageReceived()
        {
            _damageMask.SetActive(true);
            yield return new WaitForSeconds(1f);
            _damageMask.SetActive(false);
        }
        public delegate void Lost(PlayerPortrait portrait);
        public event Lost OnLost;
    }
}