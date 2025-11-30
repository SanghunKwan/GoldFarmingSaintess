using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using GFSUtilities.ResourcesData;

#if !UNITY_SERVER
using UnityEngine.SceneManagement;
#endif


[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerNewIDSystem : ISystem
{
    EntityQuery _networkQuery;
    EntityQuery _newNetworkIdQuery;
    
    
    public void OnCreate(ref SystemState state)
    {
        using var builder = new EntityQueryBuilder(Allocator.Temp);
        _newNetworkIdQuery = state.GetEntityQuery(builder.WithAll<NetworkId>().WithNone<InitializedClient>());
        state.RequireForUpdate(_newNetworkIdQuery);
        
        _networkQuery = state.GetEntityQuery(typeof(RoomFull));
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
        {
            commandBuffer.AddComponent<InitializedClient>(entity);
            state.EntityManager.BroadcastMessage("Client connect with id = " + id.ValueRO.Value);

#if UNITY_SERVER
            var successProtocol = new PageProtocol { _pageType = PageType.Login, _id = id.ValueRO.Value };
            state.EntityManager.Broadcast(successProtocol, entity);
#else
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                var successProtocol = new PageProtocol { _pageType = PageType.Login, _id = id.ValueRO.Value };
                state.EntityManager.Broadcast(successProtocol, entity);

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
