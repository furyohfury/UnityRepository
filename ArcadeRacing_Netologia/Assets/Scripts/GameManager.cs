using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Text;

namespace Cars
{
    public class GameManager : MonoBehaviour
    {
        #region Fields
        public static GameManager Instance;


        [SerializeField]
        private FinishLine _finishLine;
        [SerializeField]
        private PlayerInputController _player;
        [SerializeField]
        private Transform _playerCamera;
        [SerializeField]
        private TextMeshProUGUI _countdown;
        [SerializeField]
        private TextMeshProUGUI _timer;
        [SerializeField]
        private GameObject _speedometer;


        [SerializeField]
        private GameObject _leaderboardCanvas;
        [SerializeField]
        private TextMeshProUGUI _leaderboardList;
        [SerializeField]
        private TMP_InputField _playerNameInputField;


        private float _time;
        private float _finishTime;
        private string[] _leaders;
        #endregion
        #region Unity_Methods
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }
        private void OnEnable()
        {
            _finishLine.OnFinish += Finishing;
        }
        private void OnDisable()
        {
            _finishLine.OnFinish -= Finishing;
        }
        private void Start()
        {
            StartCoroutine(Countdown());
            // todo unlock Cursor.visible = false;
        }
        private IEnumerator Countdown()
        {
            _countdown.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _countdown.text = "2";
            yield return new WaitForSeconds(1f);
            _countdown.text = "1";
            yield return new WaitForSeconds(1f);
            _countdown.text = "GO!";
            // Активация управления игрока, таймера и спидометра
            _player.ActivatePlayerControls(true);            
            StartCoroutine(Timer());
            _speedometer.SetActive(true);
            yield return new WaitForSeconds(1f);
            _countdown.gameObject.SetActive(false);
        }
        private IEnumerator Timer()
        {
            _time = Time.time;
            while (true)
            {
                TimeSpan ts = TimeSpan.FromSeconds(Time.time - _time);
                _timer.text = "Time:\n" + ts.ToString(@"mm\:ss\:f");//ts.Minutes + ":" + ts.Seconds + ":" + ts.Milliseconds;
                yield return null;
            }            
        }
        #endregion
        private void Finishing()
        {
            StopCoroutine(Timer());
            _finishTime = Time.time - _time;
            _playerCamera.parent = null;
            _player.ActivatePlayerControls(false);
            _leaderboardCanvas.SetActive(true);
            Cursor.visible = true;
        }
        private void SetLeaderboardOnStart()
        {
            // Setup on first launch
            if (!PlayerPrefs.HasKey("Leader1"))
            {
                for (int i = 0; i < 10; i++)
                {
                    PlayerPrefs.SetString("Leader" + i, "_");
                    _leaders[i] = " ";
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    _leaders[i] = PlayerPrefs.GetString("Leader" + i);
                }
            }
        }
        private void ShowLeaderboardOnFinish()
        {
            string[] leadersShown = _leaders.OrderBy(p => p, new LeadersTimeComparer()).ToArray();
            StringBuilder sb = new();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(leadersShown[i]);
            }
            _leaderboardList.text = sb.ToString();
        }
        public void OnInputNameValueChange_Editor(string s)
        {
            if (_playerNameInputField.text.Contains("_")) _playerNameInputField.text = _playerNameInputField.text.Replace("_", "");
        }
        public void OnEnterNameButton_Editor()
        {

        }
    }
    public class LeadersTimeComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            string[] xStringTime = x.Split("_");
            float xTime = float.Parse(xStringTime[1]);
            string[] yStringTime = y.Split("_");
            float yTime = float.Parse(yStringTime[1]);
            if (xTime < yTime) return -1;
            else if (xTime > yTime) return 1;
            else return 0;
        }
    }
}