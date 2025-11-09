using GFSManager;
using GFSUtilities;
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
        float _range;

        NavMeshAgent _navAgent;

        public BaseUnit _unit { get; private set; }
        public BaseUnit _targetUnit { get; private set; }
        public bool _IsInDistance =>
            Vector3.Distance(_targetUnit.transform.position, transform.position) < _range;

        LinkedListNode<Action> _targetDieEventNode;

        private void Update()
        {
            if (_IsInDistance)
            {
                if (!_unit._IsAttackable) return;

                _unit.Attack(_targetUnit);
            }
            else
            {
                AttackMove();
            }
        }

        public void InitMove(BaseUnit unit)
        {
            _unit = unit;
            _navAgent = GetComponent<NavMeshAgent>();

            ref readonly Status stat = ref _unit._RefStat;

            _range = stat._range;
            _speed = stat._movSpeed;

            _navAgent.speed = _speed;
            _navAgent.stoppingDistance = _range;
        }


        #region Tagetting
        public void SetTarget(BaseUnit target)
        {
            if (_targetDieEventNode != null)
                _targetUnit.UnenrollTarget(_targetDieEventNode);

            _targetUnit = target;
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
        #endregion Tagetting

        void AttackMove()
        {
            _navAgent.SetDestination(_targetUnit.transform.position);
        }

        public void StopMove()
        {
            enabled = false;
            OnTargetDisable();
        }
        void OnTargetDisable()
        {
            _targetUnit = null;
            _targetDieEventNode = null;
        }
    }
}

