using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;



[UpdateBefore(typeof(ServerMatchingSystem))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerDisconnectedSystem : ISystem
{
    BufferLookup<MatchedEntityBuffer> _matchingLookup;

    EntityQuery _matchingFirstQuery;

    public void OnCreate(ref SystemState state)
    {
        var qb = SystemAPI.QueryBuilder().WithAll<InitializedClient>().WithNone<NetworkId, DisconnectCleanUp>().Build();


        state.RequireForUpdate(qb);

        _matchingLookup = SystemAPI.GetBufferLookup<MatchedEntityBuffer>();

        _matchingFirstQuery = state.GetEntityQuery(typeof(MatchedEntityBuffer), typeof(MatchedGroupIndex));
        _matchingFirstQuery.SetSharedComponentFilter(new MatchedGroupIndex { _groupIndex = 0 });
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        _matchingLookup.Update(ref state);

        var entity = _matchingFirstQuery.GetSingletonEntity();
        var buffer = _matchingLookup[entity];

        foreach (var (init, playerId, disconnectEntity) in SystemAPI.Query<InitializedClient, RefRO<PlayerCleanUp>>().WithNone<NetworkId, DisconnectCleanUp>().WithEntityAccess())
        {
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
        state.EntityManager.Broadcast(new MatchingStatusProtocol { _matchingCount = buffer.Length });
    }
}
