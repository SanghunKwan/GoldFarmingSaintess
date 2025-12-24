using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using GFSUtilities.ResourcesData;



[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerMatchIDSystem : ISystem
{


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (data, request, entity) in SystemAPI.Query<RefRO<PlayerData>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
        {
            ServerSceneManager.Instance.NewClient(data.ValueRO._ticketId);
            commandBuffer.AddComponent(request.ValueRO.SourceConnection, new PlayerCleanUp { _playerId = data.ValueRO._playerId, _ticketId = data.ValueRO._ticketId });
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
