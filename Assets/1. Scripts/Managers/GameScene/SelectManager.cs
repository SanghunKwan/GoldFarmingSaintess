using GFSUtilities;
using GFSUtilities.UI;
using GFSUtilities.Unit;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static SelectWindow;
using Random = UnityEngine.Random;

namespace GFSManagers
{
    public class SelectManager : BaseBGWindowManager<SelectWindow, SelectManager, BGManager>
    {
        IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>[] _allyBattleArrays;
        IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>[] _enemyBattleArrays;
        BattleCondition[] _conditions;

        int _maxCost;
        int _minCost;

        int _battleCount;

        int[] _costArray;

        int[] _allyPayment;
        int[] _enemyLoot;

        public float[] _additionalWeightArray { get; private set; }

        Func<string>[] _explainArray;
        UnitData[] _unitData;
        public int _selectedIndex { get; private set; }

        public BattleCondition _Battle => new BattleCondition
        {
            _headCount = _conditions[_selectedIndex]._headCount,
            _huntingRate = _conditions[_selectedIndex]._huntingRate * _additionalWeightArray[(int)SelectWeightType.HuntingRewardRate],
            _huntingGold = _conditions[_selectedIndex]._huntingGold,
            _participationAidGold = Mathf.FloorToInt(_conditions[_selectedIndex]._participationAidGold * _additionalWeightArray[(int)SelectWeightType.ParticipationAidGold])
        };
        public ref readonly IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> _AllyUnits
            => ref _allyBattleArrays[_selectedIndex];
        public ref readonly IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> _EnemyUnits
           => ref _enemyBattleArrays[_selectedIndex];

        public bool _IsSelecting { get; private set; }


        public ExplainManager _explainManager { get; set; }




        public override void InitManager(BGManager bgManager)
        {
            base.InitManager(bgManager);

            GameManager manager = GameManager.Instance;

            SelectDataScriptableObject data = manager._SelectDataScriptableObject;

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

            _explainArray = new Func<string>[]
            {
                GetPrepaymentExplain,
                GetLootExplain
            };

            _additionalWeightArray = new float[(int)SelectWeightType.Max];
            ClearAdditionalWeight();

            _IsSelecting = false;

            _allyPayment = data._allyPayment;
            _enemyLoot = data._enemyLoot;

            _unitData = manager._UnitStatScriptableObject.stat;
        }

        public void MakeBattle()
        {
            _IsSelecting = true;
            for (int i = 0; i < _battleCount; i++)
            {
                int cost = Random.Range(_minCost, _maxCost + 1);
                DivideCost(cost, _allyBattleArrays, i);
                DivideCost(cost, _enemyBattleArrays, i);
            }
            CalculateCondition();

            if (_window == null)
            {
                _window = GameManager.Instance.InstantiatePrefab(UIType.Select, _bgManager.transform).GetComponent<SelectWindow>();
                _window.InitWindow(this);
            }
            _bgManager.CallUI(1, _window);
            _window.SetValue(_allyBattleArrays, _enemyBattleArrays, _conditions);

            _maxCost += 1;
            _minCost += 1;
        }

        void DivideCost(int cost, IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>[] lists, int index)
        {
            Dictionary<KeyValuePair<StarCount, UnitTypes>, int> dic = new Dictionary<KeyValuePair<StarCount, UnitTypes>, int>();
            while (cost > 0)
            {
                int usableCostIndex = Array.BinarySearch(_costArray, cost);
                usableCostIndex = (usableCostIndex < 0) ? ~usableCostIndex : usableCostIndex + 1;

                int type = Random.Range(0, (int)UnitTypes.Count) + 1;
                int usingCostIndex = Random.Range(1, usableCostIndex + 1);
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
                    condition._huntingGold += dic.Value * GetEnemyReward(dic.Key.Value, dic.Key.Key);
                }

                int groupCount = 1;
                foreach (var dic in _allyBattleArrays[i])
                {
                    groupCount += dic.Value * ((_costArray[(int)dic.Key.Key - 1] + 1) / 2);
                    condition._participationAidGold += dic.Value * _allyPayment[(int)dic.Key.Value - 1];
                }

                condition._headCount = groupCount;
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
            _IsSelecting = false;
            _bgManager.ReleaseUI();
        }

        public void ChooseCurrentData()
        {
            _window.FadeOut();
        }

        public void CallExplain(WindowExplainType type)
            => ShowExplain((ExplainType)type, _explainArray[(int)type]());
        public void CallExplain(UnitTypes unitType, StarCount starCount, Force force)
            => ShowExplain(ExplainType.UnitSlotAlly - (int)force + 2, GetStatExplain(unitType, starCount, force));

        void ShowExplain(ExplainType explainType, in string details)
        {
            _explainManager.ShowAtMousPosition(explainType, details);
        }

        string GetPrepaymentExplain()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var dic in _AllyUnits)
            {
                sb.Append(dic.Key.Value.ToString());
                sb.Append(" ");
                sb.Append((int)dic.Key.Key);
                sb.Append("성   + ");
                sb.Append(dic.Value * GetAllyPrepayment(dic.Key.Value));
                sb.Append(" Gold");
            }

            return sb.ToString();
        }
        string GetLootExplain()
        {
            StringBuilder sb = new StringBuilder();
            ref readonly BattleCondition condition = ref _conditions[_selectedIndex];
            sb.AppendLine("분배 비율");
            foreach (var dic in _AllyUnits)
            {
                sb.Append(dic.Key.Value.ToString());
                sb.Append(" ");
                sb.Append((int)dic.Key.Key);
                sb.Append("성   : ");
                sb.Append((dic.Value * 100f * GetAllyContribution(dic.Key.Key) / condition._headCount).ToString("N2"));
                sb.AppendLine(" %");
            }
            sb.AppendLine("\n전리품");
            foreach (var dic in _EnemyUnits)
            {
                sb.Append(dic.Key.Value.ToString());
                sb.Append(" ");
                sb.Append((int)dic.Key.Key);
                sb.Append("성   + ");
                sb.Append(dic.Value * GetEnemyReward(dic.Key.Value, dic.Key.Key));
                sb.Append(" Gold");
            }
            return sb.ToString();
        }
        public int GetEnemyReward(UnitTypes type, StarCount starCount)
            => _enemyLoot[(int)type - 1] * _costArray[(int)starCount - 1];
        int GetAllyPrepayment(UnitTypes type)
            => _allyPayment[(int)type - 1];
        int GetAllyContribution(StarCount starCount)
            => (_costArray[(int)starCount - 1] + 1) / 2;
        string GetStatExplain(UnitTypes type, StarCount starCount, Force force)
        {
            StringBuilder sb = new StringBuilder();
            int index = (int)type - 1 + ((force == Force.Enemy) ? 0 : (int)UnitTypes.Count);
            ref readonly UnitData data = ref _unitData[index];

            sb.Append(type.ToString());
            sb.Append(" ");
            sb.Append((int)starCount);
            sb.AppendLine("성 스탯");

            sb.Append("체력 : ");
            sb.AppendLine(Mathf.CeilToInt(GetGrowStat(data.stat._hp, data, starCount)).ToString());


            sb.Append("공격력 : ");
            sb.AppendLine(Mathf.CeilToInt(GetGrowStat(data.stat._attack, data, starCount)).ToString());

            sb.Append("방어력 : ");
            sb.AppendLine(Mathf.CeilToInt(GetGrowStat(data.stat._defend, data, starCount)).ToString());

            sb.Append("사거리 : ");
            sb.AppendLine(GetGrowStat(data.stat._range, data, starCount).ToString("N2"));

            sb.Append("공격속도 : ");
            sb.AppendLine(GetGrowStat(data.stat._attack, data, starCount).ToString("N2"));

            sb.Append("이동 속도: ");
            sb.AppendLine(GetGrowStat(data.stat._movSpeed, data, starCount).ToString("N2"));

            if (force == Force.Enemy)
            {
                sb.Append("처치 보상 : ");
                sb.Append(GetEnemyReward(type, starCount));
                sb.AppendLine(" Gold");
            }
            else
            {
                sb.Append("계약금 : ");
                sb.Append(GetAllyPrepayment(type));
                sb.AppendLine(" Gold");

                sb.Append("배분치 : ");
                sb.Append(GetAllyContribution(starCount));
            }

            return sb.ToString();
        }
        float GetGrowStat(float statValue, in UnitData data, StarCount starCount)
            => statValue * (1 + data.growthRate * ((int)starCount - 1));

        public void ClearAdditionalWeight()
        {
            Array.Fill(_additionalWeightArray, 1f);
        }
    }
}