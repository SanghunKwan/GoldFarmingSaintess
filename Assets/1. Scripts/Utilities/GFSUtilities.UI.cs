
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
        TimerUI,

        Disturb,
        Bidding,
        Inventory,

        Explain,
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
        ItemSlot,

        PlayersUI_Slot,

        CharacterHPBar,

        FloatingImage,

        ShowEmotion,
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
        Time,

        Max
    }
    public enum LogingraphicGroupType
    {
        MatchWindowButtons,
        MatchingTexts
    }
    public enum ColorType
    {
        Red = 0,
        Blue,
        Green,
        Orange,
        Yellow,
    }
    public enum DisturbOptionType
    {
        FirstFree,
        AfterPriced,

        Max
    }

    public enum UISpriteType
    {
        PortraitHeroSword = 0,
        PortraitHeroAxe,
        PortraitHeroBow,
        PortraitHeroMagic,
        PortraitMonsterSword,
        PortraitMonsterAxe,
        PortraitMonsterBow,
        PortraitMonsterMagic,

        MessageBoxIconProceed = 8,
        MessageBoxIconAlert,
        MessageBoxIconCheck,
        MessageBoxIconTime,

        ItemCane = 12,
        ItemRing,
        ItemPaper,
        ItemPotion,
        ItemShield,
        ItemWater,

        EmotionAnnoying = 18,
        EmotionAnger,
        EmotionBidding,
        EmotionBidSubmit,
        EmotionSmile = 22,
        EmotionLaugh,
        EmotionExpressionless,
        EmotionSour,
        EmotionCry,
        EmotionSob,

    }
    public enum PlayerUISlotGroupType
    {
        SpeechBubble = 0,
        Emotion,
        CostVote,

        Max
    }

    public enum ExplainType
    {
        Prepayment,
        BattleReward,

        UnitSlotAlly,
        UnitSlotEnemy,

        Upgrade,

        DisturbSpawn,
        DisturbEnemyBuff,
        DisturbPlayerHurt,

        BiddingMoney,
        BiddingItem,

        InventoryItem
    }

    public enum SelectWeightType
    {
        HuntingRewardRate = 0,
        ParticipationAidGold,
        SpecialConditionComplete,

        Max
    }
    #endregion enum


}
