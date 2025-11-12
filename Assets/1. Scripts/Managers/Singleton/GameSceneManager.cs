using GFSBattle;
using GFSUtilities;
using GFSUtilities.Unit;
using GFSUtilities.Upgrade;
using System.Collections.Generic;
using UnityEngine;

namespace GFSManagers
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        BattleManager _battleManager;
        [SerializeField] UpgradeScriptableObjects upgradeObjects;

        LinkedList<BaseUnit> _ally;
        LinkedList<BaseUnit> _enemy;



        float GetsqrDistance(in Vector3 vec1, in Vector3 vec2)
            => (vec1 - vec2).sqrMagnitude;



        private void Awake()
        {
            Instance = this;
            _battleManager = new BattleManager();

            int[] upgrades = GameManager.Instance._healUpgrade;
            _battleManager.InitManger(GetUpgradeCalculated(upgrades, UpgradeType.HealAmount)
                                    , GetUpgradeCalculated(upgrades, UpgradeType.HealCount));


            _ally = new LinkedList<BaseUnit>();
            _enemy = new LinkedList<BaseUnit>();
        }


        public LinkedListNode<BaseUnit> EnrollUnit(BaseUnit unit)
        {
            return (unit._force == Force.Ally) ? _ally.AddLast(unit) : _enemy.AddLast(unit);
        }
        public void UnenrollUnit(LinkedListNode<BaseUnit> node)
        {
            Debug.Log("등록 해제");

            LinkedList<BaseUnit> tempList = node.List;
            tempList.Remove(node);

            if (tempList.Count <= 0)
                BattleEndCall((tempList != _ally) ? _ally : _enemy);
        }


        public bool FindTarget(BaseUnit unit, out BaseUnit target)
        {
            LinkedList<BaseUnit> list = (unit._force == Force.Ally) ? _enemy : _ally;
            target = null;
            if (list.Count == 0) return false;

            target = FindCloseNode(list, unit);
            return true;
        }
        BaseUnit FindCloseNode(LinkedList<BaseUnit> list, BaseUnit unit)
        {
            float minDistance = float.MaxValue;
            BaseUnit target = null;
            foreach (var node in list)
            {
                float distance = GetsqrDistance(node.transform.position, unit.transform.position);
                if (distance >= minDistance) continue;

                minDistance = distance;
                target = node;
            }

            return target;
        }

        public void BattleStart(float second)
        {
            foreach (var item in _ally)
                item.BattleStart(second);
            foreach (var item in _enemy)
                item.BattleStart(second);
        }

        void BattleEndCall(LinkedList<BaseUnit> leftList)
        {
            StartCoroutine(GFSManager.WaitForSecond(2, () =>
            {
                foreach (var item in leftList)
                {
                    item.ClearInAlive();
                }
            }));
        }
        int GetUpgradeCalculated(in int[] upgrades, UpgradeType type)
        {
            int typeIndex = (int)type;

            int defaultValue = upgradeObjects.defaultArray[typeIndex];
            int addByUpgrades = Mathf.CeilToInt(upgradeObjects.addArray[typeIndex] * upgrades[typeIndex]);

            return defaultValue + addByUpgrades;
        }

        #region BattleManager Transfer
        public void Attack(BaseUnit attacker, BaseUnit defender)
        {
            _battleManager.CalculateDamage(attacker, defender, (int)attacker._force);
        }
        public void ClickUnit(BaseUnit target)
        {
            _battleManager.HealUnit(target);
        }
        #endregion BattleManager Transfer
    }
}

