using GFSBattle;
using System.Collections.Generic;
using UnityEngine;
using GFSUtilities.Unit;
using GFSUtilities;

public class SettlementManager
{
    LinkedList<BaseUnit> _deadAlly;
    LinkedList<BaseUnit> _deadEnemy;

    int _participationAidGold;
    int _huntingGold;
    int _healFee;

    float _huntingRewardRate;
    float _deadReductionRate;
    float _specialConditionRate;


    public SettlementManager(SettlementScriptableObject dataObject)
    {
        _deadAlly = new LinkedList<BaseUnit>();
        _deadEnemy = new LinkedList<BaseUnit>();

        _healFee = dataObject._healFee;
        _deadReductionRate = dataObject._deadReductionRate;
        _specialConditionRate = dataObject._specialConditionRate;
    }

    public void InitManager(int participationAidGold, int huntingGold, float huntingRate)
    {
        _participationAidGold = participationAidGold;

        _huntingGold = huntingGold;
        _huntingRewardRate = huntingRate;
    }

    public void AddInSettle(LinkedListNode<BaseUnit> node)
    {
        LinkedList<BaseUnit> list = node.Value._force == Force.Ally ? _deadAlly : _deadEnemy;

        list.AddLast(node);
    }


    public void CallSettlement(int healCount, bool isSpecialConditionCompleted, bool isWin)
    {
        int winConst = isWin ? 1 : 0;

        int leftHealGold = _healFee * healCount;

        int aidGold
            = Mathf.FloorToInt(_participationAidGold * (_specialConditionRate + (1 - _specialConditionRate) * winConst));

        int huntingGold = (_deadAlly.Count == 0) ? Mathf.CeilToInt(_huntingGold * _huntingRewardRate * winConst)
                          : Mathf.Max(Mathf.FloorToInt(_huntingGold * _huntingRewardRate * winConst * (1 - _deadReductionRate)), 0);

        //leftHealGold + aidGold + huntingGold              ÃÑ°è º¸»ó
    }
}
