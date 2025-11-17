using UnityEngine;
using GFSUtilities;
using System.Collections.Generic;
using GFSUtilities.Unit;
using System;
using GFSUtilities.UI;
using Random = UnityEngine.Random;

namespace GFSManagers
{
    public class SelectManager : BaseBGWindowManager<SelectWindow, SelectManager>
    {
        BGManager _bgManager;

        IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>[] _allyBattleArrays;
        IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>[] _enemyBattleArrays;
        BattleCondition[] _conditions;

        int _maxCost;
        int _minCost;

        int _battleCount;

        int[] _costArray;

        int _selectedIndex;

        public ref readonly BattleCondition _Battle => ref _conditions[_selectedIndex];
        public ref readonly IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> _AllyUnits
            => ref _allyBattleArrays[_selectedIndex];
        public ref readonly IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> _EnemyUnits
           => ref _enemyBattleArrays[_selectedIndex];

        public void InitManager(BGManager bgManager)
        {
            _bgManager = bgManager;

            SelectDataScriptableObject data = GameManager.Instance._SelectDataScriptableObject;

            _minCost = data._minCost;
            _maxCost = data._maxCost;
            _battleCount = data._battleCount;

            _allyBattleArrays = new IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>[_battleCount];
            _enemyBattleArrays = new IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>[_battleCount];
            _conditions = new BattleCondition[_battleCount];

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
                DivideCost(cost, _allyBattleArrays, i);
                DivideCost(cost, _enemyBattleArrays, i);
            }
            CalculateCondition();

            _window = GameManager.Instance.InstantiatePrefab(UIType.Select, _bgManager.transform).GetComponent<SelectWindow>();
            _window.InitWindow(this);
            _window.SetValue(_allyBattleArrays, _enemyBattleArrays, _conditions);
        }

        void DivideCost(int cost, IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>[] lists, int index)
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
            lists[index] = dic;
        }

        void CalculateCondition()
        {
            for (int i = 0; i < _battleCount; i++)
            {
                BattleCondition condition = new BattleCondition();

                foreach (var dic in _enemyBattleArrays[i])
                {
                    condition._huntingGold += dic.Value * 10;
                }

                int groupCount = 1;
                foreach (var dic in _allyBattleArrays[i])
                {
                    groupCount += dic.Value;
                    condition._participationAidGold += dic.Value * 10;
                }

                condition._huntingRate = 1f / groupCount;
                _conditions[i] = condition;
            }
        }
        public void SetBattleIndex(int index)
        {
            _selectedIndex = index;
        }
        public void EndSelect()
        {
            GameSceneManager.Instance.EndSelect();
        }
    }
}