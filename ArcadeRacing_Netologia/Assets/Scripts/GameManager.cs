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
        private Coroutine _timerCoroutine;


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
        // private string[] _leaders;
        private (string Leader, string Name, float Time)[] _leaders = new(string Leader, string Name, float Time)[10];
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
            SetLeaderboardOnStart();
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
            // ��������� ���������� ������, ������� � ����������
            _player.ActivatePlayerControls(true);            
            _timerCoroutine = StartCoroutine(Timer());
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
            StopCoroutine(_timerCoroutine);
            _finishTime = Time.time - _time;
            _playerCamera.parent = null;
            _player.ActivatePlayerControls(false);
            _leaderboardCanvas.SetActive(true);
            ShowLeaderboardOnFinish();
            Cursor.visible = true;
        }
        private void SetLeaderboardOnStart()
        {
            // Setup on first launch
            if (!PlayerPrefs.HasKey("Leader1"))
            {
                for (int i = 0; i < 10; i++)
                {
                    PlayerPrefs.SetString("Leader" + i, "None_0");
                    _leaders[i] = ("Leader" + i, "None", 0f);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    string[] nameAndTime = PlayerPrefs.GetString("Leader" + i).Split("_");
                    _leaders[i] = ("Leader" + i, nameAndTime[0], float.Parse(nameAndTime[1]));
                }
            }
        }
        private void ShowLeaderboardOnFinish()
        {
            // (string, string, float)[] leadersShown = _leaders.OrderBy(p => p.Item3).ToArray();
            _leaders = _leaders.OrderBy(p => p.Time).ToArray();
            StringBuilder sb = new();
            for (int i = 0; i < 10; i++)
            {
                if (_leaders[i].Time < 0.1f) continue;
                TimeSpan ts = TimeSpan.FromSeconds(_leaders[i].Time);
                sb.Append(_leaders[i].Name + "\t" + ts.ToString(@"mm\:ss\:f") + "\n");
            }
            _leaderboardList.text = sb.ToString();
        }
        public void OnInputNameValueChange_Editor(string _)
        {
            if (_playerNameInputField.text.Contains("_")) _playerNameInputField.text = _playerNameInputField.text.Replace("_", "");
        }
        public void OnEnterNameButton_Editor()
        {
            // Check first empty position
            (string, string, float) r = _leaders.FirstOrDefault(p => p.Name == "None" && p.Time == 0f);
            if (r != default)
            {
                int ind = Array.IndexOf(_leaders, r);
                _leaders[ind].Name = _playerNameInputField.text;
                _leaders[ind].Time = _finishTime;
                ShowLeaderboardOnFinish();
                PlayerPrefs.SetString(_leaders[ind].Leader, _leaders[ind].Name + "_" + _leaders[ind].Time);
                return;
            }
            // If no empty positions but there are slower ones
            r = _leaders.LastOrDefault(p => p.Time > _finishTime && Array.IndexOf(_leaders, p) != 9);
            if (r != default)
            {
                int ind = Array.IndexOf(_leaders, r);
                _leaders[ind].Name = _playerNameInputField.text;
                _leaders[ind].Time = _finishTime;
                ShowLeaderboardOnFinish();
                PlayerPrefs.SetString(_leaders[ind].Leader, _leaders[ind].Name + "_" + _leaders[ind].Time);
                return;
            }
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