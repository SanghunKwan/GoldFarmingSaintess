using GFSBattle;
using GFSManagers;
using GFSUtilities.Unit;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager
{
    Transform _unitFolder;

    public void InitManager(Transform unitFolder)
    {
        _unitFolder = unitFolder;
    }
    public void SpawnUnit(in IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> unitData, Force force)
    {
        foreach (var item in unitData)
        {
            for (int i = 0; i < item.Value; i++)
            {
                GameObject spawnedObject = GameManager.Instance.InstantiateCharacterPrefab(item.Key.Value, force, _unitFolder);
                spawnedObject.GetComponent<BaseUnit>().InitUnit(item.Key.Key);
            }
        }

    }

}
