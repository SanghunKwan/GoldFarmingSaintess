using UnityEngine;
using GFSUtilities;
using System.Collections.Generic;
using GFSUtilities.Unit;
using System;
using GFSUtilities.UI;
using Random = UnityEngine.Random;

namespace GFSManagers
{
    public class SelectManager
    {
        BGManager _bgManager;

        List<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> _allyBattleLists;
        List<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> _enemyBattleLists;
        List<BattleCondition> _conditions;


        int _maxCost;
        int _minCost;

        int _battleCount;

        int[] _costArray;

        BattleCondition _battle;
        public ref readonly BattleCondition _Battle => ref _battle;

        SelectWindow _window;

        public void InitManager(BGManager bgManager)
        {
            _bgManager = bgManager;

            SelectDataScriptableObject data = GameManager.Instance._SelectDataScriptableObject;

            _minCost = data._minCost;
            _maxCost = data._maxCost;
            _battleCount = data._battleCount;

            _allyBattleLists = new List<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>>(_battleCount);
            _enemyBattleLists = new List<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>>(_battleCount);
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

            _window = GameManager.Instance.InstantiatePrefab(UIType.Select, _bgManager.transform).GetComponent<SelectWindow>();
            _window.SetValue(_allyBattleLists, _enemyBattleLists, _conditions);
        }
        void DivideCost(int cost, List<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> lists)
        {
            Dictionary<KeyValuePair<StarCount, UnitTypes>, int> dic = new Dictionary<KeyValuePair<StarCount, UnitTypes>, int>();
            while (cost > 0)
            {
                int usableCostIndex = Array.BinarySearch<int>(_costArray, cost);
                usableCostIndex = (usableCostIndex < 0) ? ~usableCostIndex : usableCostIndex + 1;

                int type = Random.Range(0, (int)UnitTypes.Count) + 1;
                int usingCostIndex = Random.Range(1, usableCostIndex);
                var tempKey = new KeyValuePair<StarCount, UnitTypes>((StarCount)usingCostIndex, (UnitTypes)type);
                if (dic.ContainsKey(tempKey))
                    dic[tempKey]++;
                else
                    dic.Add(tempKey, 1);

                int usableCost = _costArray[usingCostIndex - 1];

                cost -= usableCost;

            }
            lists.Add(dic);
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