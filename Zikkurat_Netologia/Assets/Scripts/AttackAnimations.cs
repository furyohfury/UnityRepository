using System.Collections;
using UnityEngine;

namespace Zikkurat
{
    public class AttackAnimations : MonoBehaviour
    {
        public static IEnumerator Attacking(Unit unit, float attackTime)
        {
            unit._isAttacking = true;
            switch (unit.UnitRace)
            {
                case Race.Terran:
                    {
                        float currentTime = 0;
                        float turningSpeed = 90 / (attackTime / 4);
                        while (unit != null && currentTime < attackTime / 4)
                        {
                            unit.gameObject.transform.rotation *= Quaternion.Euler(turningSpeed * Time.deltaTime * new Vector3(0, 1, 0));
                            currentTime += Time.deltaTime;
                            yield return null;
                        }
                        turningSpeed = 180 / (attackTime * 0.75f);
                        while (unit != null && currentTime < attackTime)
                        {
                            unit.gameObject.transform.rotation *= Quaternion.Euler(turningSpeed * Time.deltaTime * new Vector3(0, -1, 0));
                            currentTime += Time.deltaTime;
                            yield return null;
                        }
                        if (unit != null && unit.Target != null) unit.transform.LookAt(unit.Target.transform);
                        yield return new WaitForSeconds(attackTime);
                        unit._isAttacking = false;
                        break;
                    }
                case Race.Zerg:
                    {
                        float currentTime = 0;
                        float chargingSpeed = unit.UnitStats._attackRange / (attackTime / 2);
                        while (unit != null && currentTime < attackTime / 2)
                        {
                            unit.Move(chargingSpeed * unit.transform.forward);
                            currentTime += Time.deltaTime;
                            yield return null;
                        }
                        while (unit != null && currentTime < attackTime)
                        {
                            unit.Move(-chargingSpeed * unit.transform.forward);
                            currentTime += Time.deltaTime;
                            yield return null;
                        }
                        if (unit != null && unit.Target != null) unit.transform.LookAt(unit.Target.transform);
                        yield return new WaitForSeconds(attackTime);
                        unit._isAttacking = false;
                        break;
                    }
                case Race.Protoss:
                    {
                        float currentTime = 0;
                        float turningSpeed = 360 / attackTime;
                        while (unit != null && currentTime < attackTime)
                        {
                            unit.gameObject.transform.rotation *= Quaternion.Euler(turningSpeed * Time.deltaTime * new Vector3(0, 1, 0));
                            currentTime += Time.deltaTime;
                            yield return null;
                        }
                        if (unit != null && unit.Target != null) unit.transform.LookAt(unit.Target.transform);
                        yield return new WaitForSeconds(attackTime);
                        unit._isAttacking = false;
                        break;
                    }
            }
        }
    }
}