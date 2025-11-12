using UnityEngine;
using GFSUtilities.Unit;
using GFSUtilities;
using GFSManagers;
using System.Collections.Generic;
using System;

namespace GFSBattle
{
    public class BaseUnit : MonoBehaviour
    {
        //대분자 시작 프로퍼티는 외부 호출용.
        [SerializeField] StatusScriptableObject _originalStat;
        [SerializeField] EffectScriptableObject _effect;
        protected UnitMove _unitMove;

        public float attackableTime { get; protected set; }
        Transform _effectTr;


        public UnitTypes _type { get; private set; }
        public Force _force { get; private set; }
        public int _starCount { get; private set; }
        Status _stat;
        Status _currentStat;
        public Status _Stat => _currentStat;
        public ref readonly Status _RefStat => ref _currentStat;
        public bool _IsAttackable => Time.time >= attackableTime;
        public bool _IsDead { get; protected set; }
        [SerializeField] float _radius;
        public float _Radius => _radius;

        public LinkedListNode<BaseUnit> _sceneNode { get; private set; }

        LinkedList<Action> _dieEventList;

        public void InitUnit(int starCount)
        {
            _force = _effect._force;
            _type = _originalStat._type;
            _starCount = starCount;

            _stat = _originalStat._stat[(int)_force];
            _effectTr = transform.Find("Effect");
            CallStarInfluence();
            _currentStat = _stat;

            _unitMove = GetComponent<UnitMove>();
            _unitMove.InitMove(this);

            _sceneNode = GameSceneManager.Instance.EnrollUnit(this);

            _dieEventList = new LinkedList<Action>();
        }

        #region Action
        public void Attack(BaseUnit target)
        {
            Debug.Log(_currentStat._attack + "로 공격했다!");
            GameSceneManager.Instance.Attack(this, target);
            attackableTime = Time.time + (10 / _currentStat._atkSpeed);
        }
        public void HittByEnemy(int damage, BaseUnit attacker)
        {
            if (_IsDead) return;

            ChangeHp(-damage);

            if (_currentStat._hp <= 0)
                Die();
            else
            {
                //조건1 : attacker 현재 타겟이 아닐 경우
                //조건2 : 대상이 공격범위 바깥에 있을 경우.

                //현재 타겟이 아니고 현재 타겟이 바깥에 있을 경우.
                if (_unitMove._NeedChangeNode(attacker))
                    _unitMove.SetTarget(attacker);
            }
        }

        public void ChangeHp(int plus)
        {
            _currentStat._hp = _currentStat._hp + plus;
        }
        public void Die()
        {
            GameSceneManager.Instance.UnenrollUnit(_sceneNode);
            _sceneNode = null;

            _IsDead = true;

            foreach (Action action in _dieEventList)
            {
                action();
            }
            _dieEventList.Clear();

            _unitMove.PlayDead();
        }
        public void ClearInAlive()
        {
            _unitMove.BattleEnd();
        }
        #endregion Action

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
        #region Init
        void CallStarInfluence()
        {
            if (_starCount <= 1) return;

            ChangeStatusByStarCount();
            ActivateStarEffect();
        }
        void ChangeStatusByStarCount()
        {
            _stat.Multiply((_starCount - 1) * _effect._statGrowthRate);
        }

        void ActivateStarEffect()
        {
            Instantiate(_effect.effects[_starCount - 2], _effectTr);
        }
        #endregion Init
        #region Transfer
        public void BattleStart(float second)
        {
            _unitMove.BattleStart();
            StartCoroutine(GFSManager.WaitForSecond(second, NewTargetting));
        }
        #endregion Transfer

        private void OnMouseUpAsButton()
        {
            Debug.Log("클릭" + gameObject.name);
        }
    }
}

