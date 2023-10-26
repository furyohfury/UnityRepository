using RPG.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace RPG.Units
{
    public abstract class Unit : MonoBehaviour
    {
        private bool _inAnimation;
        private Animator _animator;
        protected UnitInputComponent _inputs;
        protected UnitStatsComponent Stats;
        public UnitStatsComponent GetStats => Stats;

        protected TriggerComponent[] _colliders;

        public SimpleHandle OnTargetLostHandler;

        [SerializeField]
        protected Transform _targetPoint;
        public Unit Target { get; protected set; }
        public Transform GetTargetPoint => _targetPoint;


        [Inject]
        protected UnitManager _unitManager;
        [SerializeField, SQRFloat,Tooltip(" вадрат макс рассто€ни€")]
        protected float _sqrFindTargetDistance = 500f;
        private void OnValidate()
        {
            if (_targetPoint != null) return;
            _targetPoint = GetComponentsInChildren<Transform>().First(t => name == EditorConstants.FocusTargetPointName);
        }
        protected virtual void Start()
        {
            _animator = this.FindComponent<Animator>();
            _inputs = this.FindComponent<UnitInputComponent>();
            Stats = this.FindComponent<UnitStatsComponent>();

            _colliders = GetComponentsInChildren<TriggerComponent>();
            if (_colliders.Length == 0) Debug.LogWarning(gameObject.name + " doesnt have triggers");

            foreach (var collider in _colliders) collider.Construct(this, Stats);


            BindingEvents();
            
        }                
        protected virtual void Update()
        {
            OnMove();
            // OnRotate();
        }
        private void OnDisable()
        {
            BindingEvents(true);
        }
        protected abstract void FindNewTarget();
        protected void BindingEvents(bool unbind = false)
        {
#if UNITY_EDITOR
            if (_inputs == null)
            {
                Debug.LogWarning("No binds for " + gameObject.name);
                return;
            }
#endif
            if (unbind)
            {
                _inputs.MainEventHandler -= OnMainAction;
                _inputs.AdditionalEventHandler -= OnAdditionalAction;
                _inputs.TargetEventHandler -= OnTargetUpdate;
                return;
            }
            else
            {
                _inputs.MainEventHandler += OnMainAction;
                _inputs.AdditionalEventHandler += OnAdditionalAction;
                _inputs.TargetEventHandler += OnTargetUpdate;
            }            
        }
        protected virtual void OnMove()
        {
            if (_inAnimation) return;
            ref var movement = ref _inputs.MoveDirection;

            _animator.SetFloat("ForwardMove", movement.z);
            _animator.SetFloat("SideMove", movement.x);

            if (movement.x == 0f && movement.z == 0f)
            {
                _animator.SetBool("Moving", false);
            }
            else
            {
                _animator.SetBool("Moving", true);
                transform.position += transform.TransformVector(movement) * Stats.MoveSpeed * Time.deltaTime;
            }
        }

        // protected abstract void OnRotate();
        private void OnMainAction()
        {
            if (_inAnimation) return;
            _animator.SetTrigger("SwordAttack");
            _inAnimation = true;
        }
        private void OnAdditionalAction()
        {
            if (_inAnimation || Stats._currentCooldown > 0f) return;
            _animator.SetTrigger("ShieldAttack");
            _inAnimation = true;
            Stats._currentCooldown = Stats._cooldownShieldAttack;
        }
        private void OnTargetUpdate()
        {
            if (Target != null)
            {
                Target = null;
                OnTargetLostHandler?.Invoke(); //todo
                return;
            }
            /* var units = FindObjectsOfType<UnitStatsComponent>(); //todo fix (get from list, check distance and walls)

            var distance = float.MaxValue;
            UnitStatsComponent target = null;
            foreach (var unit in units)
            {
                if (unit.SideType == Stats.SideType) continue;

                var currentDistance = (unit.transform.position - transform.position).sqrMagnitude;
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    target = unit;
                }
            }
            if (target == null) Debug.LogError("No bots found");
            else
            {
                Debug.Log("Locked on " + target.gameObject.name);
                Target = target.GetComponent<Unit>(); 

            }*/
            FindNewTarget();
        }
        private void OnAnimationEnded_UnityEvent(AnimationEvent data)
        {
            _inAnimation = false;
        }
        private void OnCollider_UnityEvent(AnimationEvent data)
        {
            var collider = _colliders.FirstOrDefault(t => t.GetID == data.intParameter);

#if UNITY_EDITOR
            if (collider == null)
            {
                Debug.LogError($"collider with ID {data.intParameter} not found");
                UnityEditor.EditorApplication.isPaused = true;
            }
#endif
            collider.Enable = data.floatParameter == 1f;
            
        }
    }
}