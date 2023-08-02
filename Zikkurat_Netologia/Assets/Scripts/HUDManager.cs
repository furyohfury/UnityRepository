using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



namespace Zikkurat
{
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager HUDSingleton;
        [SerializeField]
        private Button _hideExtendButton;
        [SerializeField]
        private RectTransform _panel;
        private bool _hidden = false;
        private float _panelSizeY;
        private float _panelScaleY;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private TextMeshProUGUI _name;
        [SerializeField]
        private TMP_InputField _HP;
        [SerializeField]
        private Slider _moveSpeed;
        private TMP_Text _moveSpeedText;
        [SerializeField]
        private TMP_InputField _fastAttackDamage;
        [SerializeField]
        private TMP_InputField _fastAttackTime;
        [SerializeField]
        private TMP_InputField _heavyAttackDamage;
        [SerializeField]
        private TMP_InputField _heavyAttackTime;
        [SerializeField]
        private Slider _missProbability;
        private TMP_Text _missProbabilityText;
        [SerializeField]
        private Slider _doubleDamageProbability;
        private TMP_Text _doubleDamageProbabilityText;
        [SerializeField]
        private Slider _fastHeavyProbability;
        private TMP_Text _fastHeavyProbabilityText;
        [SerializeField]
        private TMP_Dropdown _race;
        [SerializeField]
        private TMP_InputField _frequencyOfSpawn;
        [SerializeField]
        private Button _setSpawnPointButton;

        public Building SelectedBuilding { get; private set; } = null;
        public Unit SelectedUnit { get; private set; } = null;
        private Building[] _buildings;
        public Unit NewUnit { get; private set; }
        private Selected? _selected = null;
        public bool SettingSpawn { get; private set; } = false;
        #region Incapsulation_Stuff
        public void SetSelectedBuilding(Building building)
        {
            SelectedBuilding = building;
        }
        public void SetSelectedUnit(Unit unit)
        {
            SelectedUnit = unit;
        }
        public void SetNewUnit(Unit unit)
        {
            NewUnit = unit;
        }
        #endregion
        #region Unity_Methods
        private void Awake()
        {
            if (HUDSingleton != null) Destroy(this);
            else HUDSingleton = this;
            _buildings = FindObjectsOfType<Building>();
            _panelSizeY = _panel.anchoredPosition.y;
            _panelScaleY = _panel.localScale.y;
        }
        private void OnEnable()
        {
            foreach (var building in _buildings)
            {
                building.OnSelectingBuilding += SelectBuilding;
                building.OnSelectingBuilding += ChangeBackGround;
            }
            var floor = FindObjectOfType<Floor>();
            floor.OnNewSpawnPos += SetSpawnFloor;
        }
        private void OnDisable()
        {
            foreach (var building in _buildings)
            {
                building.OnSelectingBuilding -= SelectBuilding;
                building.OnSelectingBuilding -= ChangeBackGround;
            }
            var floor = FindObjectOfType<Floor>();
            if (floor != null) floor.OnNewSpawnPos -= SetSpawnFloor;
        }
        private void Start()
        {
            _moveSpeedText = _moveSpeed.transform.parent.GetComponent<TMP_Text>();
            _missProbabilityText = _missProbability.transform.parent.GetComponent<TMP_Text>();
            _doubleDamageProbabilityText = _doubleDamageProbability.transform.parent.GetComponent<TMP_Text>();
            _fastHeavyProbabilityText = _fastHeavyProbability.transform.parent.GetComponent<TMP_Text>();
        }
        #endregion
        #region Selecting
        public void SelectUnit()
        {
            if (SelectedUnit == null) return;
            _setSpawnPointButton.gameObject.SetActive(false);
            _frequencyOfSpawn.gameObject.SetActive(false);
            _selected = Selected.Unit;
            _name.text = SelectedUnit.gameObject.name;
            _HP.text = SelectedUnit.UnitStats._HP + "";
            _moveSpeed.value = SelectedUnit.UnitStats._moveSpeed;
            _moveSpeedText.text = "Move Speed = " + _moveSpeed.value;
            _fastAttackDamage.text = SelectedUnit.UnitStats._fastAttackDamage + "";
            _fastAttackTime.text = SelectedUnit.UnitStats._fastAttackTime + "";
            _heavyAttackDamage.text = SelectedUnit.UnitStats._heavyAttackDamage + "";
            _heavyAttackTime.text = SelectedUnit.UnitStats._heavyAttackTime + "";
            _missProbability.value = SelectedUnit.UnitStats._missProbability;
            _missProbabilityText.text = "Miss Probability = " + _missProbability.value;
            _doubleDamageProbability.value = SelectedUnit.UnitStats._doubleDamageProbability;
            _doubleDamageProbabilityText.text = "Double Damage Probability = " + _doubleDamageProbability.value;
            _fastHeavyProbability.value = SelectedUnit.UnitStats._fastHeavyProbability;
            _fastHeavyProbabilityText.text = "Fast/Heavy Probability = " + _fastHeavyProbability.value;
            _race.value = (int)SelectedUnit.UnitRace;
        }
        public void SelectBuilding()
        {
            if (SelectedBuilding == null) return;
            _setSpawnPointButton.gameObject.SetActive(true);
            _frequencyOfSpawn.gameObject.SetActive(true);
            _selected = Selected.Building;
            _name.text = SelectedBuilding.gameObject.name;
            _HP.text = SelectedBuilding.UnitConfig._HP + "";
            _moveSpeed.value = SelectedBuilding.UnitConfig._moveSpeed;
            _moveSpeedText.text = "Move Speed = " + _moveSpeed.value;
            _fastAttackDamage.text = SelectedBuilding.UnitConfig._fastAttackDamage + "";
            _fastAttackTime.text = SelectedBuilding.UnitConfig._fastAttackTime + "";
            _heavyAttackDamage.text = SelectedBuilding.UnitConfig._heavyAttackDamage + "";
            _heavyAttackTime.text = SelectedBuilding.UnitConfig._heavyAttackTime + "";
            _missProbability.value = SelectedBuilding.UnitConfig._missProbability;
            _missProbabilityText.text = "Miss Probability = " + _missProbability.value;
            _doubleDamageProbability.value = SelectedBuilding.UnitConfig._doubleDamageProbability;
            _doubleDamageProbabilityText.text = "Double Damage Probability = " + _doubleDamageProbability.value;
            _fastHeavyProbability.value = SelectedBuilding.UnitConfig._fastHeavyProbability;
            _fastHeavyProbabilityText.text = "Fast/Heavy Probability = " + _fastHeavyProbability.value;
            _race.value = (int)SelectedBuilding.GetBuildingRace();
            _frequencyOfSpawn.text = "" + SelectedBuilding.FrequencyOfSpawning;
        }
        #endregion
        #region ChangePanelBackground
        public void ChangeBackGround()
        {
            if (_selected == Selected.Building)
            {
                _background.sprite = Resources.Load<Sprite>(ReturnLogo(SelectedBuilding.GetBuildingRace()));
            }
            else if (_selected == Selected.Unit)
            {
                _background.sprite = Resources.Load<Sprite>(ReturnLogo(SelectedUnit.UnitRace));
            }
        }

        private string ReturnLogo(Race race)
        {
            switch (race)
            {
                case Race.Terran:
                    return "terranlogo";
                case Race.Zerg:
                    return "zerglogo";
                case Race.Protoss:
                    return "protosslogo";
                default:
                    return "";
            }
        }
        #endregion
        #region HP_InputField
        public void ChangeHP_EditorValue(string hp)
        {
            if (hp.Contains('-'))
            {
                //_HP.text = " " + Mathf.Abs(float.Parse(hp));
                _HP.text = _HP.text.Replace("-", "");
            }
        }
        public void ChangeHP_EditorEnd(string hp)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.HP, int.Parse(hp));
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.HP, int.Parse(hp));
            }
        }
        #endregion
        #region MoveSpeed_Slider
        public void ChangeMoveSpeed_Editor(float ms)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.MoveSpeed, ms);
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.MoveSpeed, ms);
            }
            _moveSpeedText.text = string.Format("Move Speed = {0:0.##}", ms);
        }
        #endregion
        #region FastAttackDamage_InputField
        public void ChangeFastAttackDamage_EditorValue(string fad)
        {
            if (fad.Contains('-'))
            {
                _fastAttackDamage.text = _fastAttackDamage.text.Replace("-", "");
            }
        }
        public void ChangeFastAttackDamage_EditorEnd(string fad)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.FastAttackDamage, int.Parse(fad));
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.FastAttackDamage, int.Parse(fad));
            }
        }
        #endregion
        #region FastAttackTime_InputField
        public void ChangeFastAttackTime_EditorValue(string fat)
        {
            if (fat.Contains('-'))
            {
                _fastAttackTime.text = _fastAttackTime.text.Replace("-", "");
            }
        }
        public void ChangeFastAttackTime_EditorEnd(string fat)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.FastAttackTime, float.Parse(fat, System.Globalization.NumberStyles.Float));
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.FastAttackTime, float.Parse(fat, System.Globalization.NumberStyles.Float));
            }
        }
        #endregion
        #region HeavyAttackDamage_InputField
        public void ChangeHeavyAttackDamage_EditorValue(string had)
        {
            if (had.Contains('-'))
            {
                _heavyAttackDamage.text = _heavyAttackDamage.text.Replace("-", "");
            }
        }
        public void ChangeHeavyAttackDamage_EditorEnd(string had)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.HeavyAttackDamage, int.Parse(had));
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.HeavyAttackDamage, int.Parse(had));
            }
        }
        #endregion
        #region HeavyAttackTime_InputField
        public void ChangeHeavyAttackTime_EditorValue(string hat)
        {
            if (hat.Contains('-'))
            {
                _heavyAttackTime.text = _heavyAttackTime.text.Replace("-", "");
            }
        }
        public void ChangeHeavyAttackTime_EditorEnd(string hat)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.HeavyAttackTime, float.Parse(hat, System.Globalization.NumberStyles.Float));
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.HeavyAttackTime, float.Parse(hat, System.Globalization.NumberStyles.Float));
            }
        }
        #endregion
        #region MissProbability_Slider
        public void ChangeMissProbability_Editor(float missprob)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.MissProbability, missprob);
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.MissProbability, missprob);
            }
            _missProbabilityText.text = string.Format("Miss Probability = {0:0.##}", missprob);
        }
        #endregion
        #region DoubleDamageProbability_Slider
        public void ChangeDoubleDamageProbability_Editor(float ddprob)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.DoubleDamageProbability, ddprob);
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.DoubleDamageProbability, ddprob);
            }
            _doubleDamageProbabilityText.text = string.Format("Double Damage Probability = {0:0.##}", ddprob);
        }
        #endregion
        #region FastHeavyProbability_Slider
        public void ChangeFastHeavyProbability_Editor(float fhprob)
        {
            if (_selected == null) return;
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetUnitConfig(UnitStat.FastHeavyProbability, fhprob);
            }
            else
            {
                SelectedUnit.SetUnitStats(UnitStat.FastHeavyProbability, fhprob);
            }
            _fastHeavyProbabilityText.text = string.Format("Fast/Heavy Probability = {0:0.##}", fhprob);
        }
        #endregion
        #region Race
        public void ChangeRace_Editor(int option)
        {
            if (_selected == Selected.Building && (Race)option != SelectedBuilding.GetBuildingRace())
            {
                SelectedBuilding.SetNewBuildingRace((Race)option);
            }
        }
        #endregion
        #region FrequencyOfSpawn_InputField
        public void ChangeFrequencyOfSpawn_EditorValue(string fos)
        {
            if (fos.Contains('-'))
            {
                _frequencyOfSpawn.text = _frequencyOfSpawn.text.Replace("-", "");
            }
        }
        public void ChangeFrequencyOfSpawn_EditorEnd(string fos)
        {
            if (_selected == Selected.Building)
            {
                SelectedBuilding.SetFrequencyOfSpawning(float.Parse(fos, System.Globalization.NumberStyles.Float));
            }
        }
        #endregion
        #region ExpandHide
        public void HUDExpandHide_Editor()
        {
            StartCoroutine(ExpandHideHUDCoroutine());
        }
        private IEnumerator ExpandHideHUDCoroutine()
        {
            _hideExtendButton.interactable = false;
            if (!_hidden)
            {
                float speedY = -_panelSizeY;
                float speedScale = _panel.localScale.y;
                while (_panel.anchoredPosition.y < 0 && _panel.localScale.y > 0)
                {
                    _panel.anchoredPosition += new Vector2(0, speedY) * Time.deltaTime;
                    _panel.localScale -= new Vector3(0, speedScale, 0) * Time.deltaTime;
                    yield return null;
                }
                _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x, 0);
                _panel.localScale = new Vector3(_panel.localScale.x, 0, _panel.localScale.z);
                _hidden = true;
                _panel.gameObject.SetActive(false);
            }
            else
            {
                _panel.gameObject.SetActive(true);
                float speedY = -_panelSizeY;
                float speedScale = _panelScaleY;
                while (_panel.anchoredPosition.y > _panelSizeY && _panel.localScale.y < 1)
                {
                    _panel.anchoredPosition -= new Vector2(0, speedY) * Time.deltaTime;
                    _panel.localScale += new Vector3(0, speedScale, 0) * Time.deltaTime;
                    yield return null;
                }
                _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x, _panelSizeY);
                _panel.localScale = new Vector3(_panel.localScale.x, _panelScaleY, _panel.localScale.z);
                _hidden = false;
            }
            _hideExtendButton.interactable = true;
        }
        #endregion
        #region SpawnPoint
        public void SetSpawnPoint_Editor()
        {
            if (_selected != Selected.Building) return;
            SettingSpawn = true;
        }
        private void SetSpawnFloor(Vector3 point)
        {
            SelectedBuilding.SetSpawnPoint(point);
            SettingSpawn = false;
        }
        #endregion        
        public void AttackedSelectedUnit()
        {
            _HP.text = "" + SelectedUnit.UnitStats._HP;
        }
    }
}
