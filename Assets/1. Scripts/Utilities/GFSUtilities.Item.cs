using UnityEngine;

namespace GFSUtilities.Item
{
    public class Item
    {
        public int _id;
        public ItemType _type;
    }



    #region enum
    public enum ItemType
    {
        InBattle,
        RewardIncrease

    }
    #endregion enum
}
