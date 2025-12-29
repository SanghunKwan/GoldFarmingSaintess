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
        state.RequireForUpdate<ConnectionCleanupEntityData>();

        _matchedQuery = state.GetEntityQuery(typeof(MatchedEntityBuffer));
    }

    public void OnUpdate(ref SystemState state)
    {
        var cleanBuffer = SystemAPI.GetSingletonBuffer<ConnectionCleanupEntityData>();

        for (int i = 0; i < cleanBuffer.Length; i++)
        {
            if (SystemAPI.Exists(cleanBuffer[i]._connection)) continue;

            Entity destroyedConnection = cleanBuffer[i]._connection;

            var cleanUpEntity = cleanBuffer[i]._cleanUp;
            ServerSceneManager.Instance.ExitClient(
                SystemAPI.GetComponentRO<PlayerCleanUp>(cleanUpEntity).ValueRO._playerId);

            if (!SystemAPI.HasComponent<DisconnectCleanUp>(cleanUpEntity)) continue;

            _matchedQuery.SetSharedComponentFilter(new MatchedGroupIndex
            {
                _groupIndex
                = SystemAPI.GetComponentRO<DisconnectCleanUp>(cleanUpEntity).ValueRO._bufferEntityShared
            });

            var entity = _matchedQuery.GetSingletonEntity();
            var matchBuffer = state.EntityManager.GetBuffer<MatchedEntityBuffer>(entity);

            for (int j = 0; j < matchBuffer.Length; j++)
            {
                if (matchBuffer[j]._matchedConnection != destroyedConnection) continue;

                matchBuffer.RemoveAt(j);
                break;
            }
        }
    }
}
