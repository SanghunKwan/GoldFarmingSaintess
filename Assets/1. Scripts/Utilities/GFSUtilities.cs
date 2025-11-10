using System;
using UnityEngine;

namespace GFSUtilities
{
    #region struct
    [Serializable]
    public struct Status
    {
        public int _hp;
        public int _attack;
        public int _defend;
        public float _range;
        public float _atkSpeed;
        public float _movSpeed;

        public void Multiply(float num)
        {
            _hp = Mathf.CeilToInt(_hp * num);
            _attack *= Mathf.CeilToInt(_attack * num);
            _defend *= Mathf.CeilToInt(_defend * num);
            _range *= num;
            _atkSpeed *= num;
            _movSpeed *= num;
        }
    }
    #endregion struct



    #region
    public static class GFSManager
    {
    }

    #endregion
}
