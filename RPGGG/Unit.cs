using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPGMine
{
    public abstract class Unit
    {
        protected UnitStats _unitStats;        
        protected Animator _unitAnimator;
        protected CharacterController _charController;
        
        protected bool CanMove = true;
        protected bool CanAttack = true;
        public UnitStats GetUnitStats()
        {
            return _unitStats;
        }


        protected virtual void Awake()
        {
            _charController = GetComponent<CharacterController>();
            _unitAnimator = GetComponent<Animator>();
        }        
        protected virtual void OnEnable()
        {

        }
        protected virtual void OnDisable()
        {
            
        }
        protected virtual void FixedUpdate()
        {
            Move();
        }        
        protected virtual void OnCollisionEnter(Collision other)
        {
            if (other.transform.root.TryGetComponent(out Unit enemy)) GotHit(enemy);
        }
        protected void Move()
        {            
            Vector2 movement = GetMovement();
            if (movement == Vector2.zero || !CanMove)
            {
                _unitAnimator.SetBool("Moving", false);
                return;
            }
            _charController.SimpleMove(new Vector3(movement.x, 0, movement.y) * _unitStats.MoveSpeed);
            _unitAnimator.SetBool("Moving", true);
            _unitAnimator.SetFloat("ForwardMove", movement.y);
            _unitAnimator.SetFloat("SideMove", movement.x);
        }
        
        protected virtual void LightAttack()
        {
            _unitAnimator.SetTrigger("LightAttack");
            _unitStats.Damage = _unitStats.LightAttackDamage;
        }
        protected virtual void GotHit(Unit enemy)
        {
            _unitAnimator.SetTrigger("Hit"); //todo add animation and add triggers to stop/allow moevemnt there
            _unitStats.HP -= enemy.GetUnitStats().Damage;
            if (_unitStats.HP <= 0) Dead();
        }
        protected virtual void Dead()
        {
            _unitAnimator.SetTrigger("Death");
            // goes to DestroyUnit_AnimatorEvent
            //todo different deaths for player and npcs
        }        
        protected virtual void EquipWeapon(Weapon Weapon)
        {
            _unitStats.LightAttackDamage = weapon._weaponLightAttackDamage;
            _unitStats.HeavyAttackDamage = weapon._weaponHeavyAttackDamage;
        }
        protected virtual void AllowMovement_AnimatorEvent()
        {
            CanMove = true;
        }
        protected virtual void StopMovement_AnimatorEvent()
        {
            CanMove = false;
        }
        protected virtual void DestroyUnit_AnimatorEvent()
        {
            gameObject.enabled = false;
            Destroy(gameObject);
        }
        
        protected abstract Vector2 GetMovement();
        
    }
}