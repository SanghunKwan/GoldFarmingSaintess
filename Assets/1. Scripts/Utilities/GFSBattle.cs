using UnityEngine;

namespace GFSBattle
{
    class HashId
    {
        public static readonly int t_Drag = Animator.StringToHash("Drag");
        public static readonly int t_Drop = Animator.StringToHash("Drop");
        public static readonly int t_Attack = Animator.StringToHash("Attack");
        public static readonly int b_Run = Animator.StringToHash("Run");
        public static readonly int t_Dead = Animator.StringToHash("Dead");
        public static readonly int b_OnBattle = Animator.StringToHash("OnBattle");
    }
    class BattleConstant
    {
        public static readonly float _inSight = Mathf.Cos(70 * Mathf.Deg2Rad);
        public static readonly float _inRunTurning = Mathf.Cos(5 * Mathf.Deg2Rad);
    }
}
