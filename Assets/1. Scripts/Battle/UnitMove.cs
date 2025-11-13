using GFSUtilities;
using GFSManagers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GFSBattle
{
    [RequireComponent(typeof(BaseUnit), typeof(NavMeshAgent))]
    public class UnitMove : MonoBehaviour
    {
        float _speed;
        float _angularSpeed;
        float _range;

        bool _isRunning;
        bool _isGrabbed;

        NavMeshAgent _navAgent;
        CapsuleCollider _collider;
        Animator _anim;
        [SerializeField] Transform _meshTransform;

        public BaseUnit _unit { get; private set; }
        public BaseUnit _targetUnit { get; private set; }
        bool _IsInDistance =>
            Vector3.Distance(_targetUnit.transform.position, transform.position) <= _range + _targetUnit._Radius;
        public bool _NeedChangeNode(BaseUnit target) => (target != _targetUnit) && (!_IsInDistance);

        public bool _IsInSight => _CosEnemyAngle > BattleConstant._inSight;
        bool _IsOutRunTurning => _CosEnemyAngle > BattleConstant._inRunTurning;
        float _CosEnemyAngle => Vector3.Dot(transform.forward, _TargetDirection);

        Vector3 _TargetDirection => (_targetUnit.transform.position - transform.position).normalized;

        LinkedListNode<Action> _targetDieEventNode;

        private void Update()
        {
            if (_IsInDistance)
            {
                TurnToAttack();
                if (_IsInSight)
                    PlayAttack();
            }
            else
            {
                MoveToAttack();
            }
        }

        public void InitMove(BaseUnit unit)
        {
            _unit = unit;
            float radius = _unit._Radius;

            _navAgent = GetComponent<NavMeshAgent>();
            _navAgent.radius = radius;

            _collider = GetComponent<CapsuleCollider>();
            _collider.radius = radius;

            _anim = _meshTransform.GetChild(0).GetComponent<Animator>();
            _anim.SetFloat(HashId.f_Type, (float)_unit._type);
            _targetDieEventNode = null;

            ref readonly Status stat = ref _unit._RefStat;

            _range = stat._range;
            _speed = stat._movSpeed;
            _angularSpeed = _navAgent.angularSpeed;

            _navAgent.speed = _speed;
            _navAgent.stoppingDistance = _range;
        }


        #region Tagetting
        public void SetTarget(BaseUnit target)
        {
            if (_targetDieEventNode != null)
                _targetUnit.UnenrollTarget(_targetDieEventNode);

            _targetUnit = target;
            _navAgent.stoppingDistance = _range + _targetUnit._Radius;
            _targetDieEventNode = _targetUnit.EnrollTarget(OnTargetDie);
        }
        public void FindTarget()
        {
            if (GameSceneManager.Instance.FindTarget(_unit, out BaseUnit target))
            {
                enabled = true;
                SetTarget(target);
            }
            else
            {
                StopMove();
                _navAgent.isStopped = true;
            }
        }
        void OnTargetDie()
        {
            OnTargetDisable();
            _unit.NewTargetting();
        }
        void OnTargetDisable()
        {
            _targetUnit = null;
            _targetDieEventNode = null;
            _navAgent.stoppingDistance = _range;
        }
        #endregion Tagetting

        #region Action
        void MoveToAttack()
        {
            _navAgent.SetDestination(_targetUnit.transform.position);
            SetRunning(true);
        }
        void TurnToAttack()
        {
            if (_IsOutRunTurning) SetRunning(false);
            else
            {
                SetRunning(true);
                Quaternion destination = Quaternion.LookRotation(_TargetDirection, Vector3.up);

                transform.rotation
                    = Quaternion.RotateTowards(transform.rotation, destination, _angularSpeed * Time.deltaTime);
            }
        }
        void SetAnimBool(bool isOn, ref bool animBool, int hashName)
        {
            if (isOn == animBool) return;

            animBool = isOn;
            _anim.SetBool(hashName, animBool);
        }
        void SetRunning(bool onOff) => SetAnimBool(onOff, ref _isRunning, HashId.b_Run);
        void SetGrabbed(bool onOff) => SetAnimBool(onOff, ref _isGrabbed, HashId.b_Grabbed);


        void PlayAttack()
        {
            if (!_unit._IsAttackable) return;

            _unit.Attack(_targetUnit);
            _anim.SetTrigger(HashId.t_Attack);

        }
        public void PlayDead()
        {
            StopMove();
            _anim.SetTrigger(HashId.t_Dead);
            _navAgent.enabled = false;
        }
        public void StopMove()
        {
            SetRunning(false);
            enabled = false;
            OnTargetDisable();
        }
        public void SetGrabbedByPlayer(bool isOn)
        {
            SetGrabbed(isOn);
        }
        #endregion Action


        public void BattleStart()
        {
            _anim.SetBool(HashId.b_OnBattle, true);
        }
        public void BattleEnd()
        {
            _anim.SetBool(HashId.b_OnBattle, false);
            gameObject.AddComponent<BattleEndMove>().InitMove(_angularSpeed / 3, 3);
        }
    }
}

