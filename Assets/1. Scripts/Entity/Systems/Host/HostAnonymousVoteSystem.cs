using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct HostAnonymousVoteSystem : ISystem
{
    BufferLookup<AnonymousVoteBuffer> _lookUp;
    Entity _lookupEntity;

    EntityQuery _waitQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(state.GetEntityQuery(typeof(AnonymousVote), typeof(ReceiveRpcCommandRequest)));
        _lookupEntity = state.EntityManager.CreateEntity(typeof(AnonymousVoteBuffer));
        _lookUp = state.GetBufferLookup<AnonymousVoteBuffer>();
        _lookUp[_lookupEntity].EnsureCapacity(GameManager.Instance._SceneChangeDataScriptableObject._size);

        using var builder = new EntityQueryBuilder(Allocator.Temp);
        _waitQuery = builder.WithAll<NetworkId>().WithNone<VoteWait>().Build(ref state);
    }

    public void OnUpdate(ref SystemState state)
    {
        _lookUp.Update(ref state);
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var buffer = SystemAPI.GetSingletonBuffer<ClientsIdentifyingData>();

        foreach (var (request, vote, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<AnonymousVote>>().WithEntityAccess())
        {
            var targetEntity = request.ValueRO.SourceConnection;
            commandBuffer.AddComponent<VoteWait>(targetEntity);
            var data = state.EntityManager.GetComponentData<NetworkId>(targetEntity);

            commandBuffer.AppendToBuffer(_lookupEntity, new AnonymousVoteBuffer { _beforeIndex = buffer[data.Value - 1]._beforeIndex, _value = vote.ValueRO._value });
            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();

        if (!_waitQuery.IsEmpty) return;

        commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, disturb, entity) in SystemAPI.Query<RefRO<NetworkId>, RefRO<VoteWait>>().WithEntityAccess())
        {
            commandBuffer.RemoveComponent<VoteWait>(entity);
        }
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();

        _lookUp.Update(ref state);
        _lookUp[_lookupEntity].AsNativeArray().Sort();
        VoteResult result = new VoteResult();
        _lookUp.Update(ref state);
        result.CopyDynamicBuffer(_lookUp[_lookupEntity]);

        state.EntityManager.Broadcast(result);
        _lookUp[_lookupEntity].Clear();

    }
}