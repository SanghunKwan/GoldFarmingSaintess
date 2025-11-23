using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientMatchingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MatchingStatusProtocol>();
    }
    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, matching, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<MatchingStatusProtocol>>().WithEntityAccess())
        {
            LoginSceneManager.Instance.UpdateMatchingStatus(matching.ValueRO);

            commandBuffer.DestroyEntity(entity);
        }


        commandBuffer.Playback(state.EntityManager);

    }
}
