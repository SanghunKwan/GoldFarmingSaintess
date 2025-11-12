using UnityEngine;


namespace GFSBattle
{
    public class BattleManager
    {
        public int _MaxHealCount { get; private set; }
        public int _LeftHealCount { get; private set; }
        public int _HealAmount { get; private set; }

        public void InitManger(int healCount, int healAmount)
        {
            _MaxHealCount = healCount;
            _HealAmount = healAmount;

            _LeftHealCount = _MaxHealCount;
        }
        public void CalculateDamage(BaseUnit attacker, BaseUnit defender, int minValue)
        {
            int damage = Mathf.Max(attacker._RefStat._attack - defender._RefStat._defend, minValue);
            defender.HittByEnemy(damage, attacker);
        }

        public void HealUnit(BaseUnit target)
        {
            if (_LeftHealCount <= 0)
            {
                Debug.Log("ÀÜ¿© Èú ¾øÀ½");
                return;
            }

            target.ChangeHp(_HealAmount);
            _LeftHealCount--;
        }

    }
}

