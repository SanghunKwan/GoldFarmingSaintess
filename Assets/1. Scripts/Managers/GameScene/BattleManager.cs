using GFSManagers;
using GFSUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace GFSBattle
{
    public enum ItemAutoUseType
    {
        Potion = 0,
        Shield,
        HealCount,

        Max
    }

    public class BattleManager
    {
        public float _enemyAttackBuff { get; private set; }
        public int _maxHealCount { get; private set; }
        public int _leftHealCount { get; private set; }
        public int _healAmount { get; private set; }
        public bool? _isSpecialConditionCompleted { get; private set; }

        public bool _isBattleEnd { get; set; }

        Queue<int>[] _inventoryItemSlots;

        public InventoryManager _inventoryManager { get; set; }

        public void InitManager()
        {
            _enemyAttackBuff = 1;
            _isBattleEnd = false;
            _inventoryItemSlots = new Queue<int>[(int)ItemAutoUseType.Max];
            for (int i = 0; i < _inventoryItemSlots.Length; i++)
                _inventoryItemSlots[i] = new Queue<int>();
        }

        public void UpdateManager(int healCount, int healAmount)
        {
            _maxHealCount = healCount;
            _healAmount = healAmount;

            _leftHealCount = _maxHealCount;
        }
        public void CalculateDamage(BaseUnit attacker, BaseUnit defender, int minValue)
        {
            ref readonly Status currStat = ref defender._RefStat;

            int damage = Mathf.FloorToInt(attacker._RefStat._attack * _enemyAttackBuff);
            int calculated = Mathf.Max(damage - currStat._defend, minValue);

            if (calculated < currStat._hp)
                defender.HittByEnemy(calculated, attacker);
            else
            {
                if (TrySurvive(defender))
                {
                    defender.HittByEnemyNoneDamage(attacker);
                    return;
                }

                defender.HittByEnemy(calculated, attacker);
                defender.Die();
            }
        }

        public bool HealUnit(BaseUnit target)
        {
            if (_leftHealCount <= 0)
            {
                Debug.Log("ÀÜ¿© Èú ¾øÀ½");
                if (TryUseItem(ItemAutoUseType.HealCount))
                    _leftHealCount = _maxHealCount;

                return false;
            }

            target.HpChangeAction(_healAmount);
            target.ShowMyEffect(GFSUtilities.Unit.UnitEffectType.HealEffect, 2);
            _leftHealCount--;
            return true;
        }

        public BattleResult GetResult(bool isWin, bool isExterminated)
            => new BattleResult
            {
                _leftHealCount = _leftHealCount,
                _isSpecialConditionCompleted = _isSpecialConditionCompleted,
                _isWin = isWin,
                _isExterminated = isExterminated
            };

        public int GetPercent(BaseUnit defender, float percent)
            => Mathf.FloorToInt(defender._RefFullStat._hp * percent);

        public void DamagePercent(LinkedList<BaseUnit> list, int intensity, GameObject effect)
        {
            float percent = intensity / 100f;
            foreach (BaseUnit unit in list)
            {
                unit.HpChangeAction(GetPercent(unit, percent));
                unit.ShowEffect(effect, GFSUtilities.Unit.UnitEffectType.WeaponEffect, 1);
            }
        }
        public void ReinforceDamage(LinkedList<BaseUnit> list, int intensity, GameObject effect)
        {
            _enemyAttackBuff += intensity / 100f;

            foreach (BaseUnit unit in list)
                unit.ShowEffect(effect, GFSUtilities.Unit.UnitEffectType.WeaponEffect, 1);
        }
        public void RegisterBattleItems(ItemAutoUseType type, int slotIndex)
        {
            _inventoryItemSlots[(int)type].Enqueue(slotIndex);
            Debug.Log(type);
            Debug.Log(_inventoryItemSlots[(int)type].Count);
        }

        bool TryUseItem(ItemAutoUseType type)
        {
            Debug.Log("Ã¼Å©" + type);
            int queueIndex = (int)type;
            if (_inventoryItemSlots[queueIndex].Count > 0)
            {
                int slotIndex = _inventoryItemSlots[queueIndex].Dequeue();
                _inventoryManager.RemoveItem(slotIndex);

                return true;
            }

            return false;
        }
        bool TrySurvive(BaseUnit unit)
        {
            if (unit._force == GFSUtilities.Unit.Force.Enemy) return false;

            if (TryUseItem(ItemAutoUseType.Potion))
            {
                unit.HpChangeAction(Mathf.CeilToInt(unit._RefFullStat._hp * 0.2f));
                return true;
            }

            if (TryUseItem(ItemAutoUseType.Shield))
            {
                unit.StatDefendChange(10000, 3);
                return true;
            }

            return false;
        }
    }
}

