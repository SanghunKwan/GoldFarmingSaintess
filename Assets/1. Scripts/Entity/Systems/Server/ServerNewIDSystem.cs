using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using GFSUtilities.ResourcesData;

#if !UNITY_SERVER
using UnityEngine.SceneManagement;
#endif


[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerNewIDSystem : ISystem
{
#if !UNITY_SERVER
    EntityQuery _networkQuery;
#endif
    EntityQuery _newNetworkIdQuery;

    BufferLookup<ConnectionCleanupEntityData> _lookUp;
    Entity _ccEntity;


    public void OnCreate(ref SystemState state)
    {
        using var builder = new EntityQueryBuilder(Allocator.Temp);
        _newNetworkIdQuery = state.GetEntityQuery(builder.WithAll<NetworkId>().WithNone<InitializedClient>());
        state.RequireForUpdate(_newNetworkIdQuery);

#if !UNITY_SERVER
        _networkQuery = state.GetEntityQuery(typeof(RoomFull));
#endif
        _ccEntity = state.EntityManager.CreateEntity(typeof(ConnectionCleanupEntityData));
        _lookUp = SystemAPI.GetBufferLookup<ConnectionCleanupEntityData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        _lookUp.Update(ref state);
        var buffer = _lookUp[_ccEntity];

        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
        {
            Entity cleanUpEntity = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(entity, new InitializedClient { _cleanUpEntity = cleanUpEntity });
            state.EntityManager.BroadcastMessage("Client connect with id = " + id.ValueRO.Value);

            commandBuffer.AppendToBuffer(_ccEntity, new ConnectionCleanupEntityData { _connection = entity, _cleanUp = cleanUpEntity });

#if UNITY_SERVER
            state.EntityManager.Broadcast(new PageProtocol { _pageType = PageType.Login, _id = id.ValueRO.Value }, entity);
#else
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                state.EntityManager.Broadcast(new PageProtocol { _pageType = PageType.Login, _id = id.ValueRO.Value }, entity);

            }
            else
            {
                if (_networkQuery.IsEmpty)
                    state.EntityManager.Broadcast(new HostLinkSuccess { _linkedIndex = id.ValueRO.Value }, entity);
                else
                {
                    state.EntityManager.Broadcast(new ErrorProtocol { _errorType = ErrorType.RoomFull }, entity);
                    commandBuffer.AddComponent<NetworkStreamRequestDisconnect>(entity);
                    UnityEngine.Debug.Log("꽉 찬 방에 추가로 인원 들어옴");
                }
            }
#endif
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
