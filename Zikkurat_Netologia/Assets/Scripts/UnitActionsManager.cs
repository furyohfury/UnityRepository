using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Zikkurat
{
    public class UnitActionsManager : MonoBehaviour
    {
        public static UnitActionsManager UnitActionsSingleton;
        public List<Unit> _units;
        private void Awake()
        {
            if (UnitActionsSingleton != null) Destroy(this);
            else UnitActionsSingleton = this;
        }
        private void Update()
        {
            UnitsLogic();
        }
        private void UnitsLogic()
        {
            foreach (var unit in _units.ToList())
            {
                if (unit.Target == null)
                {
                    List<Unit> enemies = _units.Where(enemy => enemy.UnitRace != unit.UnitRace).ToList();
                    if (enemies.Count > 0)
                    {
                        unit.SetTarget(FindClosestEnemy(unit, enemies));
                    }
                    else
                    {
                        OnWander(unit);
                    }
                }
                else
                {
                    if (TargetInAttackRange(unit))
                    {
                        OnAttack(unit);
                    }
                    else
                    {
                        OnSeek(unit);
                    }
                }
                DeathCheck(unit);

            }
        }
        private Unit FindClosestEnemy(Unit unit, List<Unit> enemies)
        {
            return enemies.First(enemy => Vector3.Distance(unit.transform.position, enemy.transform.position) - enemies.Min(enemy => Vector3.Distance(unit.transform.position, enemy.transform.position)) < 0.01);
        }
        private bool TargetInAttackRange(Unit unit)
        {
            return Vector3.Distance(unit.transform.position, unit.Target.transform.position) < unit.UnitStats._attackRange;
        }
        private void DeathCheck(Unit unit)
        {
            if (unit.UnitStats._HP <= 0)
            {
                foreach (var hasUnitAsTarget in _units.Where(un => un.Target == unit))
                {
                    hasUnitAsTarget.SetTarget(null);
                }
                _units.Remove(unit);
                // Debug.Log(unit.gameObject.name + " painfully died");
                Destroy(unit.gameObject);
            }
        }
        private void OnSeek(Unit unit)
        {
            var desired_velocity = (unit.Target.transform.position - unit.transform.position).normalized * unit.UnitStats._moveSpeed;
            var steering = desired_velocity - unit.GetVelocity();
            steering = Vector3.ClampMagnitude(steering, unit.UnitStats._moveSpeed); //делить на массу
            var velocity = Vector3.ClampMagnitude(unit.GetVelocity() + steering, unit.UnitStats._moveSpeed);
            unit.transform.LookAt(unit.Target.transform);
            unit.Move(velocity);
        }
        private void OnWander(Unit unit)
        {
            var center = unit.GetVelocity().normalized * unit.GetWanderData.WanderCenterDistance;

            var displacement = Vector3.zero;
            displacement.x = Mathf.Cos(unit.GetWanderAngle() * Mathf.Deg2Rad);
            displacement.z = Mathf.Sin(unit.GetWanderAngle() * Mathf.Deg2Rad);
            displacement = displacement.normalized * unit.GetWanderData.WanderRadius;
            unit.SetWanderAngle(unit.GetWanderAngle() + Random.Range(-unit.GetWanderData.WanderAngleRange, unit.GetWanderData.WanderAngleRange));
            var desired_velocity = center + displacement;
            var steering = desired_velocity - unit.GetVelocity();
            steering = Vector3.ClampMagnitude(steering, unit.UnitStats._maxVelocity); //делить на массу
            var velocity = Vector3.ClampMagnitude(unit.GetVelocity() + steering, unit.UnitStats._moveSpeed);
            unit.transform.LookAt(velocity);
            unit.Move(velocity);
        }
        public void OnAttack(Unit unit)
        {
            if (!unit._isAttacking)
            {
                (int attackDamage, float attackTime) attackResults = unit.AttackFastHeavyResult();
                unit._currentAttackDamage = attackResults.attackDamage;
                StartCoroutine(AttackAnimations.Attacking(unit, attackResults.attackTime));
            }
        }
    }
}