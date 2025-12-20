using GFSBattle;
using GFSManagers;
using GFSUtilities;
using GFSUtilities.Effect;
using GFSUtilities.Unit;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager
{
    Transform _unitFolder;
    PlaneManager _planeManager;

    UnitData[] _unitData;
    public void InitManager(Transform unitFolder, PlaneManager planeManager)
    {
        _unitFolder = unitFolder;
        _planeManager = planeManager;
        _unitData = GameManager.Instance._UnitStatScriptableObject.stat;
    }
    public void SpawnUnit(in IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> unitData, Force force)
    {
        int count = _planeManager._SlotCount;
        int currentCount;
        int forceIndex;
        PlaneSlotEffectType type;

        if (force == Force.Ally)
        {
            currentCount = 0;
            type = PlaneSlotEffectType.Occupied;
            forceIndex = (int)UnitTypes.Count;
        }
        else
        {
            currentCount = count / 2;
            type = PlaneSlotEffectType.EnemyOccupied;
            forceIndex = 0;
        }

        GameManager manager = GameManager.Instance;

        foreach (var item in unitData)
        {
            for (int i = 0; i < item.Value; i++)
            {
                GameObject spawnedObject = manager.InstantiateCharacterPrefab(item.Key.Value, force, _unitFolder);

                var unit = spawnedObject.GetComponent<BaseUnit>();

                unit.InitUnit(_unitData[(int)item.Key.Value + forceIndex - 1], item.Key.Key);

                _planeManager.SelectSlot(unit, currentCount++, type);
            }
        }
    }

    void ShuffleArrange(in LinkedList<BaseUnit> list, int inclusiveMinIndex, int exclusiveMaxIndex)
    {
        foreach (var unit in list)
            _planeManager.SwapSlot(unit, Random.Range(inclusiveMinIndex, exclusiveMaxIndex), _planeManager.GetIndex(unit.transform.position), PlaneSlotEffectType.EnemyOccupied, PlaneSlotEffectType.Useless);
    }
    public void ShuffleEnemy(in LinkedList<BaseUnit> list)
    {
        int count = _planeManager._SlotCount;
        ShuffleArrange(list, count / 2, count);
    }
    public void DisturbSpawnUnit(StarCount star, EffectManager effectManager)
    {
        UnitTypes type = (UnitTypes)Random.Range(0, (int)UnitTypes.Count) + 1;
        GameObject spawnedObject = GameManager.Instance.InstantiateCharacterPrefab(type, Force.Enemy, _unitFolder);
        spawnedObject.transform.position = Vector3.zero;
        var unit = spawnedObject.GetComponent<BaseUnit>();
        unit.InitUnit(_unitData[(int)type - 1], star);
        unit.PlayInvasion();
        unit.BattleStart(1);

        GameObject portalObject = Object.Instantiate(effectManager._PlaneEffects[(int)PlaneResourceType.Portal], Vector3.up * 0.1f, Quaternion.identity, null);
        Object.Destroy(portalObject, 1.5f);
    }
}
