using GFSBattle;
using System.Collections.Generic;
using UnityEngine;
using GFSUtilities.Unit;
using GFSUtilities.UI;
using GFSUtilities;
using System;


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
        bool _isExterminated;
        bool? _isSpecialConditionCompleted;

        float _huntingRewardRate;
        float _deadReductionRate;
        float _specialConditionRate;

        public NoneBGManager _noneBGManager { get; set; }
        public HostManager _hostManager { get; set; }
        public SelectManager _selectManager { get; set; }

        bool _isActive;

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
        public void ShowWindow(in Action action)
        {
            _bgManager.CallUI(2f, _window);
            _window.FadeEvent += action;
            _isActive = true;
        }
        public void HideWindow()
        {
            if (!_isActive) return;

            _bgManager.ReleaseUI();
            _window.FadeOut();
            _isActive = false;
        }
        public void SendWait()
        {
            _hostManager.SendWaitPhaseEnd(GFSUtilities.ResourcesData.GamePhaseType.Settlement);
        }

        public void DisposeFormerTurn()
        {
            GameSceneManager.Instance.DisposeFormerList(_deadAlly);
            GameSceneManager.Instance.DisposeFormerList(_deadEnemy);
        }

        #region SetValue
        public void SetData(in BattleCondition battleData)
        {
            _participationAidGold = battleData._participationAidGold;
            _huntingRewardRate = battleData._huntingRate;
        }
        public void SetData(in BattleResult battleData)
        {
            _healCount = battleData._leftHealCount;
            _isSpecialConditionCompleted = battleData._isSpecialConditionCompleted;
            _isWin = battleData._isWin;
            _isExterminated = battleData._isExterminated;
        }


        public void CalculateSettlement()
        {
            if (_window == null)
            {
                _window = GameManager.Instance.InstantiatePrefab(UIType.Settle, _bgManager.transform).GetComponent<SettlementWindow>();
                _window.InitWindow(this);
            }
            CalculateRewardVariables();
            SetWindowVariables();
            SetWindowCalculated();
            //leftHealGold + aidGold + huntingGold              총계 보상

        }
        void CalculateRewardVariables()
        {
            _huntingGold = 0;
            foreach (var unit in _deadEnemy)
            {
                Debug.Log("더함");
                _huntingGold += _selectManager.GetEnemyReward(unit._type, unit._starCount);
            }

            _huntingRewardRate = _isExterminated ? 0 :
                Mathf.Max(0, _huntingRewardRate * (1 - (_deadReductionRate * _deadAlly.Count)));
        }
        void SetWindowVariables()
        {
            _window.SetValues(_participationAidGold.ToString("N"), SettlementVariableType.AidGold);
            _window.SetValues(_huntingGold.ToString("N"), SettlementVariableType.BattleRewards);
            _window.SetValues(_huntingRewardRate.ToString("P"), SettlementVariableType.BattleDistributeRate);
            _window.SetValues(_healCount.ToString("N"), SettlementVariableType.LeftHealCount);
            _window.SetValues(_isWin ? "승리" : (_isExterminated ? "패배" : "후퇴"), SettlementVariableType.Victory);
            _window.SetValues(_isSpecialConditionCompleted.HasValue ?
                              (_isSpecialConditionCompleted.Value ? "성공" : "실패") : "없음",
                              SettlementVariableType.SpecialCondition);
        }
        void SetWindowCalculated()
        {
            int leftHealGold = _healFee * _healCount;
            int aidGold
                = Mathf.FloorToInt(_participationAidGold * (_specialConditionRate + (1 - _specialConditionRate)));
            int huntingGold = Mathf.FloorToInt(_huntingGold * _huntingRewardRate);
            int result = leftHealGold + aidGold + huntingGold;

            _window.SetValues(leftHealGold.ToString("N"),
                               SettlementCalculatedType.DefaultGold);
            _window.SetValues(aidGold.ToString("N"),
                               SettlementCalculatedType.AidGold);
            _window.SetValues(huntingGold.ToString("N"),
                               SettlementCalculatedType.VictoryGold);
            _window.SetValues(result.ToString("N"),
                               SettlementCalculatedType.ResultGold);

            _noneBGManager._CurrentGold += leftHealGold + aidGold + huntingGold;
        }
        #endregion SetValue

        public int GetDeadEnemy(out int[] disturbRates)
        {
            disturbRates = new int[(int)StarCount.Count];

            foreach (var enemy in _deadEnemy)
                disturbRates[(int)enemy._starCount - 1]++;

            return _deadEnemy.Count;
        }
    }
}