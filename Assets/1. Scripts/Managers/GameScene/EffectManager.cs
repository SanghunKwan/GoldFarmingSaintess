using GFSBattle;
using GFSUtilities.Unit;
using UnityEngine;
using System.Collections.Generic;
using System;

public class EffectManager
{
    //force ฐüทร effect
    GameObject[] _healEffects;

    Dictionary<Force, IReadOnlyDictionary<StarCount, GameObject>> _baseEffects;
    Dictionary<Force, IReadOnlyDictionary<UnitTypes, GameObject>> _weaponEffect;

    public IReadOnlyList<GameObject> _HealEffects => _healEffects;
    public IReadOnlyDictionary<Force, IReadOnlyDictionary<StarCount, GameObject>> _BaseEffects => _baseEffects;

    public EffectManager(EffectScriptableObject originalEffect)
    {
        _healEffects = originalEffect.healEffects;

        GetArrayFromListCover(originalEffect.baseEffects, ref _baseEffects);
        GetArrayFromListCover(originalEffect.weaponEffects, ref _weaponEffect);
    }

    void GetArrayFromListCover<T1, T2>(in EffectScriptableObject.ListCover[] cover,
                                        ref Dictionary<T1, IReadOnlyDictionary<T2, GameObject>> effectVariable)
        where T1 : Enum where T2 : Enum
    {
        //forceCount T1
        //typeCount T2
        effectVariable = new Dictionary<T1, IReadOnlyDictionary<T2, GameObject>>(cover.Length);

        for (int i = 0; i < cover.Length; i++)
        {
            if (cover[i] == null || cover[i]._list == null || cover[i]._list.Length == 0) continue;

            int arrayLength = cover[i]._list.Length;

            T1 firstKey = (T1)Enum.ToObject(typeof(T1), i);
            Dictionary<T2, GameObject> tempDic = new Dictionary<T2, GameObject>(arrayLength);
            effectVariable.Add(firstKey, tempDic);

            for (int j = 0; j < arrayLength; j++)
            {
                if (cover[i]._list[j] == null) continue;

                tempDic.Add((T2)Enum.ToObject(typeof(T2), j), cover[i]._list[j]);
            }
        }
    }
}
