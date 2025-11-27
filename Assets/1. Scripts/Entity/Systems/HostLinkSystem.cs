using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct HostLinkSystem : ISystem
{
    NativeArray<Entity> _linkedPlayers;
    EntityQuery _networkQuery;
    int _linkedCount;


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HostClientIdentify>();
        _linkedPlayers = new NativeArray<Entity>(GameManager.Instance._SceneChangeDataScriptableObject._size, Allocator.Persistent);
        _networkQuery = state.GetEntityQuery(typeof(NetworkStreamDriver));
        _linkedCount = 0;
    }


    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, identify, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<HostClientIdentify>>().WithEntityAccess())
        {
            int index = identify.ValueRO._index - 1;

            if (index < _linkedPlayers.Length && _linkedPlayers[index] == default)
            {
                _linkedPlayers[index] = request.ValueRO.SourceConnection;

                if (SystemAPI.TryGetSingleton<PlayerProtocolSpawn>(out var prefab))
                {
                    Entity protocolEntity = commandBuffer.Instantiate(prefab.prefab);
                    commandBuffer.SetComponent(protocolEntity, new PlayerProtocol { _type = ProtocolType.None });
                    //commandBuffer.SetComponentEnabled(protocolEntity, typeof(PlayerProtocol), false);

                    commandBuffer.SetComponent(protocolEntity, new GhostOwner { NetworkId = state.EntityManager.GetComponentData<NetworkId>(request.ValueRO.SourceConnection).Value });
                    commandBuffer.AppendToBuffer(request.ValueRO.SourceConnection, new LinkedEntityGroup { Value = protocolEntity });
                }

                if ((++_linkedCount) == _linkedPlayers.Length)
                {
                    //모든 플레이어 연결됨.
                    var networkDriver = _networkQuery.GetSingletonEntity();
                    commandBuffer.AddComponent(networkDriver, typeof(RoomFull));
                    commandBuffer.SetComponent(networkDriver, new RoomFull { });

                    GameSceneManager.Instance.ReadyToStart();
                    _linkedPlayers.Dispose();
                }
            }
            else
            {
                state.EntityManager.Broadcast(new ErrorProtocol { _errorType = ErrorType.IdInvalid });
            }
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

    public void OnDestroy(ref SystemState state)
    {
        if (_linkedPlayers.IsCreated)
            _linkedPlayers.Dispose();
    }
}
