using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerMessageSystem : ISystem
{
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MessageRpcCommand>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, message, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<MessageRpcCommand>>().WithEntityAccess())
        {
            //Debug.Log(message.ValueRO._text + "from clinet index " + request.ValueRO.SourceConnection.Index + "Version" + request.ValueRO.SourceConnection.Version);
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
