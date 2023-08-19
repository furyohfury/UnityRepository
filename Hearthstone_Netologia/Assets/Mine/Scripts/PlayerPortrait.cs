using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace Cards
{
    public class PlayerPortrait : MonoBehaviour
    {
        [field: SerializeField]
        public SideType Hero { get; private set; }
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
        // Start is called before the first frame update
        void Start()
        {            
            Health = _maxHP;
            _hpText.text = _maxHP + "";
        }
        public void ChangePlayerHealth(int delta, bool isHealing = true)
        {
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
                //todo анимация взрыв портрета героя и победа
                StartCoroutine(LoseAnimation());

            }
        }
        private IEnumerator LoseAnimation()
        {
            Instantiate(_explosion, transform.position + Vector3.up * 10, Quaternion.Euler(new Vector3(90, 0, 0)));
            _audioSource.PlayOneShot(_loseSound);
            yield return new WaitForSeconds(2f);
            OnLost?.Invoke(this);
#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
        }
        public delegate void Lost(PlayerPortrait portrait);
        public event Lost OnLost;
    }
}