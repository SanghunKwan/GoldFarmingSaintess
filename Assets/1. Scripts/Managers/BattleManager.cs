using UnityEngine;


namespace GFSBattle
{
    public class BattleManager
    {
        public void CalculateDamage(BaseUnit attacker, BaseUnit defender, int minValue)
        {
            int damage = Mathf.Max(attacker._RefStat._attack - defender._RefStat._defend, minValue);
            defender.HittByEnemy(damage, attacker);
        }


    }
}

