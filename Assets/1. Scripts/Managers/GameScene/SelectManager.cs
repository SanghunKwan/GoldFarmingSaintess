using UnityEngine;
using GFSUtilities;
using System.Collections.Generic;
using GFSUtilities.Unit;
using System;
using Random = UnityEngine.Random;

namespace GFSManagers
{
    public class SelectManager
    {
        List<List<KeyValuePair<StarCount, UnitTypes>>> _allyBattleLists;
        List<List<KeyValuePair<StarCount, UnitTypes>>> _enemyBattleLists;
        List<BattleCondition> _conditions;
        int _maxCost;
        int _minCost;

        int _battleCount;

        int[] _costArray;

        BattleCondition _battle;
        public ref readonly BattleCondition _Battle => ref _battle;

        SelectWindow _window;

        public void InitManager()
        {
            SelectDataScriptableObject data = GameManager.Instance._SelectDataScriptableObject;

            _minCost = data._minCost;
            _maxCost = data._maxCost;
            _battleCount = data._battleCount;

            _allyBattleLists = new List<List<KeyValuePair<StarCount, UnitTypes>>>(_battleCount);
            _enemyBattleLists = new List<List<KeyValuePair<StarCount, UnitTypes>>>(_battleCount);
            _conditions = new List<BattleCondition>(_battleCount);

            _costArray = new int[(int)StarCount.Count];
            _costArray[(int)StarCount.Beginner - 1] = 1;
            _costArray[(int)StarCount.Advanced - 1] = data._advencedCost;
            _costArray[(int)StarCount.Expert - 1] = data._expertCost;
        }

        public void MakeBattle()
        {
            for (int i = 0; i < _battleCount; i++)
            {
                int cost = Random.Range(_minCost, _maxCost + 1);
                DivideCost(cost, _allyBattleLists);
                DivideCost(cost, _enemyBattleLists);
            }
            CalculateCondition();
            _window = new SelectWindow();
            _window.SetValue(_allyBattleLists, _enemyBattleLists);
        }
        void DivideCost(int cost, List<List<KeyValuePair<StarCount, UnitTypes>>> lists)
        {
            List<KeyValuePair<StarCount, UnitTypes>> list = new List<KeyValuePair<StarCount, UnitTypes>>();
            while (cost > 0)
            {
                int usableCostIndex = Array.BinarySearch<int>(_costArray, cost);
                usableCostIndex = (usableCostIndex < 0) ? ~usableCostIndex : usableCostIndex + 1;

                int type = Random.Range(0, (int)UnitTypes.Count) + 1;
                list.Add(new KeyValuePair<StarCount, UnitTypes>((StarCount)usableCostIndex, (UnitTypes)type));

                int usableCost = _costArray[Random.Range(0, usableCostIndex)];
                cost -= usableCost;
            }
            lists.Add(list);
        }

        void CalculateCondition()
        {
            for (int i = 0; i < _battleCount; i++)
            {
                BattleCondition condition = new BattleCondition();
                condition._huntingRate = 100f / (_allyBattleLists[i].Count + 1);
                for (int j = 0; j < _enemyBattleLists[i].Count; j++)
                {
                    condition._huntingGold += 10;
                }
                for (int j = 0; j < _allyBattleLists[i].Count; j++)
                {
                    condition._participationAidGold += 10;
                }
                _conditions.Add(condition);
            }
        }
    }
}