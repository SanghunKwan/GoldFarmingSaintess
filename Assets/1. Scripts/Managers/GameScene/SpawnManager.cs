using GFSBattle;
using GFSManagers;
using GFSUtilities.Effect;
using GFSUtilities.Unit;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager
{
    Transform _unitFolder;
    PlaneManager _planeManager;

    public void InitManager(Transform unitFolder, PlaneManager planeManager)
    {
        _unitFolder = unitFolder;
        _planeManager = planeManager;
    }
    public void SpawnUnit(in IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> unitData, Force force)
    {
        int count = _planeManager._SlotCount;
        int currentCount = 0;
        foreach (var item in unitData)
        {
            for (int i = 0; i < item.Value; i++)
            {
                GameObject spawnedObject = GameManager.Instance.InstantiateCharacterPrefab(item.Key.Value, force, _unitFolder);

                var unit = spawnedObject.GetComponent<BaseUnit>();
                unit.InitUnit(item.Key.Key);

                if (force == Force.Ally)
                    _planeManager.SelectSlot(unit, currentCount, PlaneSlotEffectType.Occupied);
                else
                {
                    int index = currentCount + count / 2;
                    _planeManager.SelectSlot(unit, index, PlaneSlotEffectType.EnemyOccupied);
                    _planeManager.SwapSlot(unit, Random.Range(count / 2, count), index, PlaneSlotEffectType.EnemyOccupied, PlaneSlotEffectType.Useless);
                }

                ++currentCount;
            }
        }

    }

}
