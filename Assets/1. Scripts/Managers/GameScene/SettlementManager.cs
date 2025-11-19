using GFSBattle;
using System.Collections.Generic;
using UnityEngine;
using GFSUtilities.Unit;
using GFSUtilities.UI;
using GFSUtilities;


namespace GFSManagers
{
    public class SettlementManager : BaseBGWindowManager<SettlementWindow, SettlementManager, BGManager>
    {
        LinkedList<BaseUnit> _deadAlly;
        LinkedList<BaseUnit> _deadEnemy;

        int _participationAidGold;
        int _huntingGold;
        int _healFee;
        int _healCount;
        bool _isWin;
        bool? _isSpecialConditionCompleted;

        float _huntingRewardRate;
        float _deadReductionRate;
        float _specialConditionRate;

        public override void InitManager(BGManager bgManager)
        {
            base.InitManager(bgManager);

            _deadAlly = new LinkedList<BaseUnit>();
            _deadEnemy = new LinkedList<BaseUnit>();

            SettlementScriptableObject dataObject = GameManager.Instance._SettlementScriptableObject;

            _healFee = dataObject._healFee;
            _deadReductionRate = dataObject._deadReductionRate;
            _specialConditionRate = dataObject._specialConditionRate;
        }

        public void AddInSettle(LinkedListNode<BaseUnit> node)
        {
            LinkedList<BaseUnit> list = node.Value._force == Force.Ally ? _deadAlly : _deadEnemy;

            list.AddLast(node);
        }
        public void ShowWindow()
        {
            _bgManager.CallUI(3f, _window);
        }
        public void HideWindow()
        {
            _bgManager.ReleaseUI();
            _window.FadeOut();
        }


        #region SetValue
        public void SetData(in BattleCondition battleData)
        {
            _participationAidGold = battleData._participationAidGold;
            _huntingGold = battleData._huntingGold;
            _huntingRewardRate = battleData._huntingRate;
        }
        public void SetData(in BattleResult battleData)
        {
            _healCount = battleData._leftHealCount;
            _isSpecialConditionCompleted = battleData._isSpecialConditionCompleted;
            _isWin = battleData._isWin;
        }


        public void CalculateSettlement()
        {
            if (_window == null)
            {
                _window = GameManager.Instance.InstantiatePrefab(UIType.Settle, _bgManager.transform).GetComponent<SettlementWindow>();
                _window.InitWindow(this);
            }

            SetWindowVariables();
            SetWindowCalculated();
            //leftHealGold + aidGold + huntingGold              총계 보상
        }
        void SetWindowVariables()
        {
            _window.SetValues(_participationAidGold.ToString("N"), SettlementVariableType.AidGold);
            _window.SetValues(_huntingGold.ToString("N"), SettlementVariableType.BattleRewards);
            _window.SetValues(_huntingRewardRate.ToString("P"), SettlementVariableType.BattleDistributeRate);
            _window.SetValues(_healCount.ToString("N"), SettlementVariableType.LeftHealCount);
            _window.SetValues(_isWin ? "승리" : "패배", SettlementVariableType.Victory);
            _window.SetValues(_isSpecialConditionCompleted.HasValue ?
                              (_isSpecialConditionCompleted.Value ? "성공" : "실패") : "없음",
                              SettlementVariableType.SpecialCondition);
        }
        void SetWindowCalculated()
        {
            int winConst = _isWin ? 1 : 0;

            int leftHealGold = _healFee * _healCount;
            int aidGold
                = Mathf.FloorToInt(_participationAidGold * (_specialConditionRate + (1 - _specialConditionRate) * winConst));
            int huntingGold = (_deadAlly.Count == 0) ? Mathf.CeilToInt(_huntingGold * _huntingRewardRate * winConst)
                              : Mathf.Max(Mathf.FloorToInt(_huntingGold * _huntingRewardRate * winConst * (1 - _deadReductionRate)), 0);
            int result = leftHealGold + aidGold + huntingGold;

            _window.SetValues(leftHealGold.ToString("N"),
                               SettlementCalculatedType.DefaultGold);
            _window.SetValues(aidGold.ToString("N"),
                               SettlementCalculatedType.AidGold);
            _window.SetValues(huntingGold.ToString("N"),
                               SettlementCalculatedType.VictoryGold);
            _window.SetValues(0.ToString("N"),
                               SettlementCalculatedType.HarassGold);
            _window.SetValues(result.ToString("N"),
                               SettlementCalculatedType.ResultGold);
        }
        #endregion SetValue
    }
}