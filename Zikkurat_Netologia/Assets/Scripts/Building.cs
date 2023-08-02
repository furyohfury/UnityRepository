using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static Zikkurat.HUDManager;
using static Zikkurat.UnitActionsManager;

namespace Zikkurat
{
    public class Building : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Race BuildingRace;
        public UnitStats UnitConfig { get; private set; }
        private Vector3 _spawnPoint;
        public float FrequencyOfSpawning { get; private set; } = 10;
        private GameObject _terran;
        private GameObject _zerg;
        private GameObject _protoss;
        private GameObject _spawnPointFlag;
        private MeshRenderer _spawnPoingFlagMesh;
        private FlagPlank _flagComponent;

        #region Incapsulation_Stuff
        public void SetSpawnPoint(Vector3 point)
        {
            _spawnPoint = point;
            _spawnPointFlag.transform.position = point;
        }
        public Race GetBuildingRace()
        {
            return BuildingRace;
        }
        public void SetNewBuildingRace(Race race)
        {
            BuildingRace = race;

            UnitConfig = new UnitStats(race);
            switch (race)
            {
                case Race.Terran:
                    {
                        var newBase = Instantiate((GameObject)Resources.Load(ModelsPaths._terranBasePath), transform.position, transform.rotation);
                        ChangingSelectedInHUDAndSpawnFlagDestroying(newBase);
                        break;
                    }
                case Race.Zerg:
                    {
                        var newBase = Instantiate((GameObject)Resources.Load(ModelsPaths._zergBasePath), transform.position, transform.rotation);
                        ChangingSelectedInHUDAndSpawnFlagDestroying(newBase);
                        break;
                    }
                case Race.Protoss:
                    {
                        var newBase = Instantiate((GameObject)Resources.Load(ModelsPaths._protossBasePath), transform.position, transform.rotation);
                        ChangingSelectedInHUDAndSpawnFlagDestroying(newBase);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        private void ChangingSelectedInHUDAndSpawnFlagDestroying(GameObject newBase)
        {
            HUDSingleton.SetSelectedBuilding(newBase.GetComponent<Building>());
            Destroy(_flagComponent.gameObject);
            Destroy(gameObject);
            HUDSingleton.SelectBuilding();
        }
        public void SetFrequencyOfSpawning(float fr)
        {
            FrequencyOfSpawning = fr;
        }
        public void SetUnitConfig(UnitStat stat, float value)
        {
            switch (stat)
            {
                case UnitStat.HP:
                    {
                        UnitStats local = UnitConfig;
                        local._HP = (int)value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.MoveSpeed:
                    {
                        UnitStats local = UnitConfig;
                        local._moveSpeed = value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.MaxVelocity:
                    {
                        UnitStats local = UnitConfig;
                        local._maxVelocity = value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.FastAttackDamage:
                    {
                        UnitStats local = UnitConfig;
                        local._fastAttackDamage = (int)value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.FastAttackTime:
                    {
                        UnitStats local = UnitConfig;
                        local._fastAttackTime = value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.HeavyAttackDamage:
                    {
                        UnitStats local = UnitConfig;
                        local._heavyAttackDamage = (int)value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.HeavyAttackTime:
                    {
                        UnitStats local = UnitConfig;
                        local._heavyAttackTime = value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.MissProbability:
                    {
                        UnitStats local = UnitConfig;
                        local._missProbability = value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.DoubleDamageProbability:
                    {
                        UnitStats local = UnitConfig;
                        local._doubleDamageProbability = value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.FastHeavyProbability:
                    {
                        UnitStats local = UnitConfig;
                        local._fastHeavyProbability = value;
                        UnitConfig = local;
                    }
                    break;
                case UnitStat.AttackRange:
                    {
                        UnitStats local = UnitConfig;
                        local._attackRange = value;
                        UnitConfig = local;
                    }
                    break;
            }
        }
        #endregion
        #region Unity_Methods
        private void Start()
        {
            _terran = Resources.Load<GameObject>(ModelsPaths._terranPath);
            _zerg = Resources.Load<GameObject>(ModelsPaths._zergPath);
            _protoss = Resources.Load<GameObject>(ModelsPaths._protossPath);
            UnitConfig = new(BuildingRace);

            //Флаг спауна
            var flag = Resources.Load<GameObject>(ModelsPaths._spawnPointPath);
            _spawnPointFlag = Instantiate(flag);
            _spawnPointFlag.name = BuildingRace.ToString() + "_SpawnPoint";
            var flagComponent = _spawnPointFlag.GetComponent<FlagPlank>();
            var buildingmaterial = gameObject.GetComponentInChildren<MeshRenderer>().material;
            flagComponent._banner.material = buildingmaterial;

            SetSpawnPoint(new Vector3((transform.position + transform.forward * 10).x, 0, (transform.position + transform.forward * 10).z));
            _spawnPoingFlagMesh = _spawnPointFlag.GetComponent<MeshRenderer>();
            _flagComponent = flagComponent;
            SetFlagMeshVisibility(false);
            StartCoroutine(UnitsSpawning());
        }
        #endregion

        #region Spawning_Units
        public void SpawnUnit()
        {
            GameObject unit;
            switch (BuildingRace)
            {
                case Race.Terran:
                    {
                        unit = Instantiate(_terran, _spawnPoint, Quaternion.identity);
                        break;
                    }
                case Race.Zerg:
                    {
                        unit = Instantiate(_zerg, _spawnPoint, Quaternion.identity);
                        break;
                    }
                case Race.Protoss:
                    {
                        unit = Instantiate(_protoss, _spawnPoint, Quaternion.identity);
                        break;
                    }
                default:
                    {
                        unit = null;
                        break;
                    }
            }
            var unitComponent = unit.AddComponent<Unit>();
            unitComponent.SetUnitStats(UnitConfig);
            unitComponent.SetUnitRace(BuildingRace);
            UnitActionsSingleton._units.Add(unitComponent);
        }
        private IEnumerator UnitsSpawning()
        {
            while (true)
            {
                SpawnUnit();
                yield return new WaitForSeconds(FrequencyOfSpawning);
            }
        }
        #endregion
        public void SetFlagMeshVisibility(bool enable)
        {
            _spawnPoingFlagMesh.enabled = enable;
            _flagComponent._banner.enabled = enable;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Флаг прошлого выбранного здания скрыть
            if (HUDSingleton.SelectedBuilding != null)
            {
                HUDSingleton.SelectedBuilding.SetFlagMeshVisibility(false);
            }
            //Флаг этого здания показать
            SetFlagMeshVisibility(true);
            HUDSingleton.SetSelectedBuilding(this);
            OnSelectingBuilding?.Invoke();
            // Debug.Log("Clicked " + gameObject.name);
        }

        public delegate void SelectingBuilding();
        public event SelectingBuilding OnSelectingBuilding;
    }
}