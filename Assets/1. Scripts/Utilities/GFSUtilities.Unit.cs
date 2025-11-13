using UnityEngine;

namespace GFSUtilities.Unit
{
    #region Enum
    public enum UnitTypes
    {
        None = 0,

        Sword,
        Axe,
        Bow,
        Magic,

        Count = Magic           //마지막
    }
    public enum Force
    {
        None = 0,

        Enemy,
        Ally,

        Count = Ally         //마지막
    }
    public enum StarCount
    {
        None = 0,

        Beginner,
        Advanced,
        Expert,

        Count = Expert          //마지막
    }




    public enum UnitEffectType
    {
        None = 0,

        BaseEffect,
        HealEffect,
        WeaponEffect,

        Count = WeaponEffect            //마지막
    }
    #endregion Enum
}
