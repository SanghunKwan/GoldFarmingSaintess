using UnityEngine;
using GFSUtilities.Unit;
using GFSUtilities;
using GFSManager;
using System.Collections.Generic;
using System;

namespace GFSBattle
{
    public class BaseUnit : MonoBehaviour
    {
        [SerializeField] StatusScriptableObject _originalStat;
        [SerializeField] EffectScriptableObject _effect;
        protected UnitMove _unitMove;

        public float attackableTime { get; protected set; }



        public UnitTypes _type { get; private set; }
        public Force _force { get; private set; }
        public int _starCount { get; private set; }
        Status _stat;
        Status _currentStat;
        public Status _Stat => _currentStat;
        public ref readonly Status _RefStat => ref _currentStat;
        public bool _IsAttackable => Time.time >= attackableTime;
        public bool _IsDead { get; protected set; }

        public LinkedListNode<BaseUnit> _sceneNode { get; private set; }

        LinkedList<Action> _dieEventList;

        public void InitUnit()
        {
            _force = _effect._force;
            _type = _originalStat._type;
            _stat = _originalStat._stat[(int)_force];
            _currentStat = _stat;

            _unitMove = GetComponent<UnitMove>();
            _unitMove.InitMove(this);

            _sceneNode = GameSceneManager.Instance.EnrollUnit(this);

            _dieEventList = new LinkedList<Action>();
        }

        public void Attack(BaseUnit target)
        {
            Debug.Log(_currentStat._attack + "로 공격했다!");
            GameSceneManager.Instance.Attack(this, target);
            attackableTime = Time.time + (10 / _currentStat._atkSpeed);
        }
        public void HittByEnemy(int damage, BaseUnit attacker)
        {
            if (_IsDead) return;

            Damaged(damage);

            if (_currentStat._hp <= 0)
                Die();
            else
            {
                if (!_unitMove._IsInDistance)
                    _unitMove.SetTarget(attacker);
            }
        }

        public void Damaged(int damage)
        {
            _currentStat._hp = _currentStat._hp - damage;
            Debug.Log(damage + "만큼 데미지를 입었다!  남은 체력:" + _currentStat._hp);

        }
        public void Die()
        {
            Debug.Log("유닛이 사망했다!");

            GameSceneManager.Instance.UnenrollUnit(_sceneNode);
            _sceneNode = null;

            _IsDead = true;

            foreach (Action action in _dieEventList)
            {
                action();
            }
            _dieEventList.Clear();

            _unitMove.StopMove();
        }


        #region Targetting
        public void NewTargetting()
        {
            _unitMove.FindTarget();
        }
        public LinkedListNode<Action> EnrollTarget(in Action action)
        {
            return _dieEventList.AddLast(action);
        }
        public void UnenrollTarget(LinkedListNode<Action> node)
        {
            if (node.List != _dieEventList) return;

            _dieEventList.Remove(node);
            Debug.Log("노드 삭제");
        }
        #endregion Targetting

        public void StarEffect(BaseUnit target)
        {

        }

    }
}

