using GFSUtilities.Unit;
using UnityEngine;

namespace GFSUtilities.UI
{


    #region enum
    public enum UIType
    {
        Settle = 0,
        Select,
        Training,
        Turn,

        Logo,
        Login,
        Setting,

        MessageBox,
        PlayersUI,
    }
    public enum SettlementVariableType
    {
        AidGold = 0,
        BattleRewards,
        BattleDistributeRate,
        LeftHealCount,
        Victory,
        SpecialCondition
    }
    public enum SettlementCalculatedType
    {
        DefaultGold = 0,
        AidGold,
        VictoryGold,
        HarassGold,
        ResultGold
    }
    public enum SettlementGraphicGroupType
    {
        Title = 0,
        VariableNames,
        VariableValues,
        CalculatedNames,
        CalculatedValues,
        Line,
        ResultName,
        ResultValue
    }

    public enum UIResourceType
    {
        SelectSlot = 0,
        SelectStar,
        TrainingSlot,

        PlayersUI_Slot,

    }

    public enum SelectGraphicGroupType
    {
        Main,
        Aid,
        Rate,
        PageButtons

    }

    public enum TurnGraphicGroupType
    {
        BG,
        TurnText,
        NoneChangeSubText,
        CurrentTurnText,
        ShiningEffect
    }

    public enum LogoGraphicGroupType
    {
        UnityChan,
    }
    public enum MessageBoxType
    {
        Proceed = 0,
        Alert,
        Check,
    }
    #endregion enum
}
