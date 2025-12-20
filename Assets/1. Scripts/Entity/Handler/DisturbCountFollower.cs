using GFSUtilities;
using GFSUtilities.Protocol;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class DisturbCountFollower : MonoBehaviour
{
    EntityQuery _disturbQuery;
    Action<DisturbType, int> _updateAction;

    public int[] _lastDisturbCount { get; private set; }



    public void InitFollower(in Action<DisturbType, int> action)
    {
        var em = ClientServerBootstrap.ClientWorld.EntityManager;
        _disturbQuery = em.CreateEntityQuery(typeof(DisturbCounter));

        _updateAction = action;
        _lastDisturbCount = new int[(int)DisturbType.Max];

        Update();
    }


    private void Update()
    {
        ref readonly var value = ref _disturbQuery.GetSingletonRW<DisturbCounter>().ValueRO;

        UpdateCheck(DisturbType.MonsterSpawn, value._monsterSpawnCount);
        UpdateCheck(DisturbType.MonsterBuff, value._monsterBuffCount);
        UpdateCheck(DisturbType.HeroHurt, value._heroHurtCount);
    }

    void UpdateCheck(DisturbType type, int updatedValue)
    {
        int index = (int)type;
        if (_lastDisturbCount[index] == updatedValue) return;

        _lastDisturbCount[index] = updatedValue;
        _updateAction(type, updatedValue);
    }


}
