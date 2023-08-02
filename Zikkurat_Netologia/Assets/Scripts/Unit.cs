using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static Zikkurat.HUDManager;

namespace Zikkurat
{
    [RequireComponent(typeof(CharacterController))]
    public class Unit : MonoBehaviour, IPointerClickHandler
    {
        public UnitStats UnitStats { get; private set; }
        public Race UnitRace { get; private set; }
        [HideInInspector]
        public Unit Target { get; private set; }
        private CharacterController _charController;
        public WanderData GetWanderData { get; private set; } = new(30, 5, 10);
        [Range(-90f, 90f)]
        private float WanderAngle = 0;
        public bool _isAttacking = false;
        public int _currentAttackDamage = 0;
        private Material _defaultMaterial;
        private Material _damagedMaterial;
        #region Incapsulation_Stuff
        public void SetTarget(Unit target)
        {
            Target = target;
        }
        public void SetUnitRace(Race race)
        {
            UnitRace = race;
        }
        public Vector3 GetVelocity()
        {
            return _charController.velocity;
        }
        public float GetWanderAngle()
        {
            return WanderAngle;
        }
        public void SetWanderAngle(float wa)
        {
            WanderAngle = wa;
        }
        public void Move(Vector3 velocity)
        {
            _charController.SimpleMove(velocity);
        }
        public void SetUnitStats(UnitStat stat, float value)
        {
            switch (stat)
            {
                case UnitStat.HP:
                    {
                        UnitStats local = UnitStats;
                        local._HP = (int)value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.MoveSpeed:
                    {
                        UnitStats local = UnitStats;
                        local._moveSpeed = value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.MaxVelocity:
                    {
                        UnitStats local = UnitStats;
                        local._maxVelocity = value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.FastAttackDamage:
                    {
                        UnitStats local = UnitStats;
                        local._fastAttackDamage = (int)value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.FastAttackTime:
                    {
                        UnitStats local = UnitStats;
                        local._fastAttackTime = value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.HeavyAttackDamage:
                    {
                        UnitStats local = UnitStats;
                        local._heavyAttackDamage = (int)value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.HeavyAttackTime:
                    {
                        UnitStats local = UnitStats;
                        local._heavyAttackTime = value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.MissProbability:
                    {
                        UnitStats local = UnitStats;
                        local._missProbability = value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.DoubleDamageProbability:
                    {
                        UnitStats local = UnitStats;
                        local._doubleDamageProbability = value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.FastHeavyProbability:
                    {
                        UnitStats local = UnitStats;
                        local._fastHeavyProbability = value;
                        UnitStats = local;
                    }
                    break;
                case UnitStat.AttackRange:
                    {
                        UnitStats local = UnitStats;
                        local._attackRange = value;
                        UnitStats = local;
                    }
                    break;
            }
        }
        public void SetUnitStats(UnitStats stats)
        {
            UnitStats = stats;
        }
        public float GetUnitStats(UnitStat stat)
        {
            switch (stat)
            {
                case UnitStat.HP: return UnitStats._HP;
                case UnitStat.MoveSpeed: return UnitStats._moveSpeed;
                case UnitStat.MaxVelocity: return UnitStats._maxVelocity;
                case UnitStat.FastAttackDamage: return UnitStats._fastAttackDamage;
                case UnitStat.FastAttackTime: return UnitStats._fastAttackTime;
                case UnitStat.HeavyAttackDamage: return UnitStats._heavyAttackDamage;
                case UnitStat.HeavyAttackTime: return UnitStats._heavyAttackTime;
                case UnitStat.MissProbability: return UnitStats._missProbability;
                case UnitStat.DoubleDamageProbability: return UnitStats._doubleDamageProbability;
                case UnitStat.FastHeavyProbability: return UnitStats._fastHeavyProbability;
                case UnitStat.AttackRange: return UnitStats._attackRange;
                default: return 0;
            }
        }
        #endregion
        #region Unity_Methods
        private void Awake()
        {
            HUDSingleton.SetNewUnit(this);
            _charController = GetComponent<CharacterController>();

            _defaultMaterial = GetComponentInChildren<MeshRenderer>().material;
            _damagedMaterial = (Material)Resources.Load(ModelsPaths._damagedMaterialPath);
        }
        private void OnEnable()
        {
            OnSelectingUnit += HUDSingleton.SelectUnit;
            OnSelectingUnit += HUDSingleton.ChangeBackGround;
            OnAttackedUnit += HUDSingleton.AttackedSelectedUnit;
        }
        private void OnDisable()
        {
            OnSelectingUnit -= HUDSingleton.SelectUnit;
            OnSelectingUnit -= HUDSingleton.ChangeBackGround;
            OnAttackedUnit -= HUDSingleton.AttackedSelectedUnit;
        }
        private void OnDestroy()
        {
            if (HUDSingleton.SelectedUnit == this) HUDSingleton.SetSelectedUnit(null);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            HUDSingleton.SetSelectedUnit(this);
            OnSelectingUnit?.Invoke();
            // Debug.Log("Clicked unit " + gameObject);
        }
        #endregion        
        #region Attack_Data
        private bool AttackMissResult()
        {
            var hit = Random.Range(0, 1f);
            if (hit < UnitStats._missProbability) return true;
            else return false;
        }
        private bool AttackDoubleDamageResult()
        {
            var ddhit = Random.Range(0, 1f);
            if (ddhit < UnitStats._doubleDamageProbability) return true;
            else return false;
        }
        public (int attackDamage, float attackTime) AttackFastHeavyResult()
        {
            var attackType = Random.Range(0, 1f);
            if (AttackMissResult())
            {
                if (attackType < UnitStats._fastHeavyProbability) return (0, UnitStats._fastAttackTime);
                else return (0, UnitStats._heavyAttackTime);
            }
            else if (AttackDoubleDamageResult())
            {
                if (attackType < UnitStats._fastHeavyProbability) return (2 * UnitStats._fastAttackDamage, UnitStats._fastAttackTime);
                else return (2 * UnitStats._heavyAttackDamage, UnitStats._heavyAttackTime);
            }
            else
            {
                if (attackType < UnitStats._fastHeavyProbability) return (UnitStats._fastAttackDamage, UnitStats._fastAttackTime);
                else return (UnitStats._heavyAttackDamage, UnitStats._heavyAttackTime);
            }
        }
        #endregion
        #region Got_Attacked
        private void OnTriggerEnter(Collider other)
        {
            GotAttacked(other);
        }
        private void GotAttacked(Collider other)
        {
            if (other.transform.root.gameObject.TryGetComponent(out Unit hitByUnit) && hitByUnit.Target == this && hitByUnit._isAttacking)
            {
                SetUnitStats(UnitStat.HP, GetUnitStats(UnitStat.HP) - hitByUnit._currentAttackDamage);
                if (hitByUnit._currentAttackDamage > 0) StartCoroutine(GotAttackedColorChange());
                if (HUDSingleton.SelectedUnit == this)
                {
                    OnAttackedUnit?.Invoke();
                }
            }
        }
        private IEnumerator GotAttackedColorChange()
        {
            var meshRend = GetComponentInChildren<MeshRenderer>();
            meshRend.material = _damagedMaterial;
            meshRend.material.color = new Color(1, 0, 0);
            yield return new WaitForSeconds(0.5f);
            meshRend.material = _defaultMaterial;
        }
        #endregion

        public delegate void UnitHUDChange();
        public event UnitHUDChange OnSelectingUnit;
        public event UnitHUDChange OnAttackedUnit;
    }
}