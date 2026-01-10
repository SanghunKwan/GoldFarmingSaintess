using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct HostLinkSystem : ISystem
{
    BufferLookup<ClientsIdentifyingData> _lookup;
    Entity _bufferEntity;
    int _length;
    int _currentCount;

    EntityQuery _netDriverQuery;


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(state.GetEntityQuery(typeof(HostClientIdentify), typeof(ReceiveRpcCommandRequest)));

        //state.EntityManager.CreateEntity(typeof(DynamicBuffer<ClientsIdentifyingData>));와 동일
        _bufferEntity = state.EntityManager.CreateEntity(typeof(ClientsIdentifyingData));
        _lookup = state.GetBufferLookup<ClientsIdentifyingData>();

        _currentCount = 0;
        _length = GameManager.Instance._SceneChangeDataScriptableObject._size;
        using var tempArray = new NativeArray<ClientsIdentifyingData>(_length, Allocator.Temp);
        _lookup[_bufferEntity].AddRange(tempArray);

        _netDriverQuery = state.GetEntityQuery(typeof(NetworkStreamDriver));
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        _lookup.Update(ref state);
        foreach (var (request, identify, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<HostClientIdentify>>().WithEntityAccess())
        {
            UnityEngine.Debug.Log("연결");
            ref var element = ref _lookup[_bufferEntity].ElementAt(identify.ValueRO._currentIndex - 1);

            if (element._beforeIndex != 0)
            {
                //이미 수정됨. 버그.
                state.EntityManager.Broadcast(new ErrorProtocol { _errorType = ErrorType.IdInvalid });
            }
            else
            {
                element._beforeIndex = identify.ValueRO._beforeIndex;
                var queueNodeEntity = commandBuffer.CreateEntity();
                commandBuffer.AddComponent(queueNodeEntity, new HostPlayerProtocolSpawnQueue
                {
                    _beforeIndex = element._beforeIndex,
                    _currentIndex = identify.ValueRO._currentIndex,
                    _requestTarget = request.ValueRO.SourceConnection
                });

                if ((++_currentCount) == _length)
                {
                    //모든 플레이어 연결됨.

                    var networkDriver = _netDriverQuery.GetSingletonEntity();
                    commandBuffer.AddComponent(networkDriver, typeof(RoomFull));
                    commandBuffer.SetComponent(networkDriver, new RoomFull { });

                    AllClientReady data = new AllClientReady
                    {
                        defaultGold = 100,
                        maxRound = GameManager.Instance._TurnScriptableObject._maxTurn,
                        playerCount = _length
                    };
                    data.CopyDynamicBuffer(_lookup[_bufferEntity]);

                    state.EntityManager.Broadcast(data);
                }
            }
            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }
}