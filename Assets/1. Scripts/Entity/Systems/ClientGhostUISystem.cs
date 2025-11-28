using GFSManagers;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


[BurstCompile]
[UpdateInGroup(typeof(GhostInputSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientGhostUISystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GoldInputData>();
    }
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (data, local, entity) in SystemAPI.Query<RefRW<GoldInputData>, RefRO<GhostOwnerIsLocal>>().WithEntityAccess())
        {
            data.ValueRW.gold = GameSceneManager.Instance.GetMoney;
        }

        foreach (var (data, ghost, entity) in SystemAPI.Query<RefRO<PlayerProtocol>, RefRO<GhostOwner>>().WithEntityAccess())
        {
            
            GameSceneManager.Instance.SetMoney(data.ValueRO._gold, ghost.ValueRO.NetworkId);
        }
    }

}
