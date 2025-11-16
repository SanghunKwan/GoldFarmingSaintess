using GFSUtilities.Unit;
using GFSUtilities.Upgrade;
using System;
using System.Collections;
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
            _hp += Mathf.CeilToInt(_hp * num);
            _attack += Mathf.CeilToInt(_attack * num);
            _defend += Mathf.CeilToInt(_defend * num);
            _atkSpeed += num;
            _movSpeed += num;
        }
    }

    public struct BattleCondition
    {
        public int _participationAidGold;
        public int _huntingGold;
        public float _huntingRate;
    }
    public struct BattleResult
    {
        public int _leftHealCount;
        public bool _isWin;
        public bool? _isSpecialConditionCompleted;
    }
    #endregion struct



    #region Static
    public static class GFSManager
    {
        public static IEnumerator WaitForSecond(float second, Action action)
        {
            yield return new WaitForSeconds(second);
            action();
        }
        public static IEnumerator ActionInOrder(int length, Action<int> action, float waitSecond)
        {
            for (int i = 0; i < length; i++)
            {
                action(i);
                yield return new WaitForSeconds(waitSecond);
            }
        }
    }
    #endregion Static

    #region hash
    class UIHashID
    {
        public static readonly int b_IsBlack = Animator.StringToHash("isBlack");

        public static readonly int t_FadeIn = Animator.StringToHash("FadeIn");
        public static readonly int t_FadeOut = Animator.StringToHash("FadeOut");
    }

    #endregion hash
}
