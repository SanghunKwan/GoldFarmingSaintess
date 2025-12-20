using UnityEngine;

namespace GFSUtilities.Effect
{
    #region Enums
    public enum PlaneResourceType
    {
        PlaneSlot = 0,
        Portal
    }
    public enum PlaneSlotEffectType
    {
        Useless = 0,

        Occupied,
        Usable,
        EnemyOccupied,
        HighLight,
    }
    public enum CamEffectType
    {
        Explode = 0,
        Leak,
    }
    #endregion Enums
}
