using GFSUtilities.ResourcesData;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerDisconnectedInMatchingSystem : ISystem
{
    EntityQuery _matchedQuery;

    public void OnCreate(ref SystemState state)
    {
        var qb = SystemAPI.QueryBuilder().WithAll<InitializedClient, DisconnectCleanUp>().WithNone<NetworkId>().Build();

        state.RequireForUpdate(qb);

        _matchedQuery = state.GetEntityQuery(typeof(MatchedGroupIndex));

    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (clean, init, playerId, disconnectEntity) in SystemAPI.Query<RefRO<DisconnectCleanUp>, InitializedClient, RefRO<PlayerCleanUp>>().WithNone<NetworkId>().WithEntityAccess())
        {
            _matchedQuery.SetSharedComponentFilter(new MatchedGroupIndex { _groupIndex = clean.ValueRO._bufferEntityShared });

            var entity = _matchedQuery.GetSingletonEntity();
            var buffer = state.EntityManager.GetBuffer<MatchedEntityBuffer>(entity);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i]._matchedConnection != disconnectEntity) continue;

                buffer.RemoveAt(i);
                break;
            }
            ServerSceneManager.Instance.ExitClient(playerId.ValueRO._playerId, playerId.ValueRO._ticketId);
            commandBuffer.DestroyEntity(disconnectEntity);
        }

        commandBuffer.Playback(state.EntityManager);
    }
}
