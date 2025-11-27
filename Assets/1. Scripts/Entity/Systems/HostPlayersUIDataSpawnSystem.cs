using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct HostPlayersUIDataSpawnSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProtocol>();
    }
    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (protocol, owner, entity) in SystemAPI.Query<RefRO<PlayerProtocol>, RefRO<GhostOwner>>().WithEntityAccess())
        {
            Debug.Log(owner.ValueRO.NetworkId + " + " + protocol.ValueRO._type);


            commandBuffer.SetComponentEnabled(entity, typeof(PlayerProtocol), false);
        }
        commandBuffer.Playback(state.EntityManager);
    }

}
