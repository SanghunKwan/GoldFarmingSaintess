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
        //대문자 시작 프로퍼티는 외부 호출용.
        [SerializeField] StatusScriptableObject _originalStat;
        [SerializeField] ForceScriptableObject _originalForce;
        protected UnitMove _unitMove;

        public float _attackableTime { get; protected set; }
        Transform[] _baseEffectTr;

        public UnitTypes _type { get; private set; }
        public Force _force { get; private set; }
        public StarCount _starCount { get; private set; }
        Status _stat;
        Status _currentStat;
        public ref readonly Status _RefStat => ref _currentStat;
        public bool _IsAttackable => Time.time >= _attackableTime;
        public bool _IsDead { get; protected set; }
        [SerializeField] float _radius;
        public float _Radius => _radius;


        public LinkedListNode<BaseUnit> _sceneNode { get; private set; }

        LinkedList<Action> _dieEventList;

        public void InitUnit(StarCount starCount)
        {
            _force = _originalForce._force;
            _type = _originalStat._type;
            _starCount = starCount;

            _stat = _originalStat._stat[(int)_force - 1];

            _baseEffectTr = new Transform[(int)UnitEffectType.Count];
            for (int i = 0; i < _baseEffectTr.Length; i++)
            {
                _baseEffectTr[i] = transform.Find(((UnitEffectType)i + 1).ToString());
            }
            EffectTransformByType(UnitEffectType.BaseEffect).localScale = _radius / 3 * Vector3.one;
            EffectTransformByType(UnitEffectType.HealEffect).localScale = _radius * 1.3f * Vector3.one;

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
            _attackableTime = Time.time + (10 / _currentStat._atkSpeed);
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
        public void Healing(int healAmount)
        {
            ChangeHp(healAmount);
            GameObject effect = GetEffect(UnitEffectType.HealEffect);

            Destroy(effect, 2);
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
            if (_starCount <= StarCount.Beginner) return;

            ChangeStatusByStarCount();
            GetEffect(UnitEffectType.BaseEffect);
        }
        void ChangeStatusByStarCount()
        {
            _stat.Multiply((int)_starCount * _originalForce._statGrowthRate);
        }

        GameObject GetEffect(UnitEffectType type)
        {
            GameObject effectPrefab = GameSceneManager.Instance.GetEffect(this, type);

            return Instantiate(effectPrefab, EffectTransformByType(type));
        }
        Transform EffectTransformByType(UnitEffectType type) => _baseEffectTr[(int)type - 1];
        #endregion Init
        #region Transfer
        public void BattleStart(float second)
        {
            SetGrab(false);
            _unitMove.BattleStart();
            StartCoroutine(GFSManager.WaitForSecond(second, NewTargetting));
        }

        public void SetGrab(bool isOn)
        {
            _unitMove.SetGrabbedByPlayer(isOn);
            //마우스 위치로 이동.

        }
        #endregion Transfer

        #region Mouse Interact
        public void OnDragStart()
        {
            if (_force != Force.Ally) return;

            GameSceneManager.Instance.DragInUnit(this);
        }
        public void OnDragEnd()
        {
            if (_force != Force.Ally) return;

            GameSceneManager.Instance.DragOutUnit(this);
        }
        public void OnClick()
        {
            GameSceneManager.Instance.ClickUnit(this);
        }
        #endregion Mouse Interact
    }
}

