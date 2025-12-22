using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateBefore(typeof(ServerTimerActiveSystem))]
public partial struct HostGameComplete : ISystem
{
    BufferLookup<ClientsIdentifyingData> _lookup;
    BufferLookup<RaceResultBuffer> _bufferLookup;


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameComplete>();
        _lookup = state.GetBufferLookup<ClientsIdentifyingData>();
        _bufferLookup = state.GetBufferLookup<RaceResultBuffer>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        _lookup.Update(ref state);
        Entity dataEntity = SystemAPI.GetSingletonEntity<ClientsIdentifyingData>();
        Entity bufferEntity = state.EntityManager.CreateEntity(typeof(RaceResultBuffer));
        foreach (var (data, ghost, entity) in SystemAPI.Query<RefRO<PlayerProtocol>, RefRO<GhostOwner>>().WithEntityAccess())
        {
            int playerIndex = _lookup[dataEntity][ghost.ValueRO.NetworkId - 1]._beforeIndex;
            commandBuffer.AppendToBuffer(bufferEntity,
                new RaceResultBuffer { _beforeIndex = playerIndex, _gold = data.ValueRO._gold });
        }
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();


        _bufferLookup.Update(ref state);
        _bufferLookup[bufferEntity].AsNativeArray().Sort();

        GameResult result = new GameResult();
        _bufferLookup.Update(ref state);
        result.CopyDynamicBuffer(_bufferLookup[bufferEntity]);

        state.EntityManager.Broadcast(result);
        _bufferLookup[bufferEntity].Clear();

        state.Enabled = false;
    }
}