using GFSManagers;
using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientReadySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AllClientReady>();
    }

    public void OnUpdate(ref SystemState state)
    {

        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (request, readyData, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<AllClientReady>>().WithEntityAccess())
        {
            GameSceneManager.Instance.ReadyToStart(readyData.ValueRO);

            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }


}
