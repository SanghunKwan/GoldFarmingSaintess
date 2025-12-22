using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;




[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientMessageSystem: ISystem
{


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MessageRpcCommand>();
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, message, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<MessageRpcCommand>>().WithEntityAccess())
        {
            Debug.Log(message.ValueRO._text);
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
