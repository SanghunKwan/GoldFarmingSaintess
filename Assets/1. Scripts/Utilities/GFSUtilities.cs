using GFSUtilities.Protocol;
using System;
using System.Collections;
using Unity.Entities;
using Unity.NetCode;
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

        public static bool IsValidNickName(in string nickName)
        {
            if (string.IsNullOrEmpty(nickName)) return false;

            if (!CheckKorean(nickName)) return false;

            return true;
        }
        public static bool CheckKorean(in string nickName)
        {
            for (int i = 0; i < nickName.Length; i++)
            {
                char c = nickName[i];
                if ((c >= '\u1100' && c <= '\u11FF') ||   // Hangul Jamo
                (c >= '\u3130' && c <= '\u318F') ||   // Jamo compatibility
                (c >= '\uA960' && c <= '\uA97F') ||   // Extended A
                (c >= '\uD7B0' && c <= '\uD7FF'))
                    return false;

                if (c == ' ') return false;
            }
            return true;
        }
        public static void Broadcast<T>(this EntityManager manager, in T protocol, in Entity target = default)
        where T : unmanaged, IComponentData
        {
            var entity = manager.CreateEntity(typeof(SendRpcCommandRequest), typeof(T));
            manager.SetComponentData(entity, protocol);
            if (target == default) return;

            manager.SetComponentData(entity, new SendRpcCommandRequest { TargetConnection = target });
        }
        public static void BroadcastMessage(this EntityManager manager, in string text)
        {
            MessageRpcCommand protocol = new MessageRpcCommand { _text = text };
            manager.Broadcast(protocol);
        }
    }

    #endregion Static

    #region hash
    class UIHashID
    {
        public static readonly int b_IsBlack = Animator.StringToHash("isBlack");

        public static readonly int t_FadeIn = Animator.StringToHash("FadeIn");
        public static readonly int t_FadeOut = Animator.StringToHash("FadeOut");

        public static readonly int b_IsMatching = Animator.StringToHash("IsMatching");
        public static readonly int t_Matched = Animator.StringToHash("Matched");
    }

    #endregion hash
}
