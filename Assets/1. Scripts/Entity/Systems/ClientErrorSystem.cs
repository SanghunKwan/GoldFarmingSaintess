using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientErrorSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ErrorProtocol>();
    }


    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, page, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ErrorProtocol>>().WithEntityAccess())
        {
            Debug.Log("에러 발생");
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
