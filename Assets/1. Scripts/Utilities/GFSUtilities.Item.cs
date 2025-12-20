using System;
using UnityEngine;

namespace GFSUtilities.Item
{
    [Serializable]
    public struct Item
    {
        public int index;
        public int cost;
        public ItemType type;
        public string name;
        public string description;
    }


    #region enum
    public enum ItemType
    {
        결산,
        전투
    }
    #endregion enum
}
