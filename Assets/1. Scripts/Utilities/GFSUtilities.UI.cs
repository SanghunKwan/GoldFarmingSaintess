using GFSUtilities.Unit;
using UnityEngine;

namespace GFSUtilities.UI
{


    #region enum
    public enum UIType
    {
        Settle,
    }
    public enum SettlementVariableType
    {
        AidGold,
        BattleRewards,
        BattleDistributeRate,
        LeftHealCount,
        Victory,
        SpecialCondition
    }
    public enum SettlementCalculatedType
    {
        DefaultGold,
        AidGold,
        VictoryGold,
        HarassGold,
        ResultGold
    }
    public enum SettlementGraphicGroupType
    {
        Title,
        VariableNames,
        VariableValues,
        CalculatedNames,
        CalculatedValues,
        Line,
        ResultName,
        ResultValue
    }
    #endregion enum
}
