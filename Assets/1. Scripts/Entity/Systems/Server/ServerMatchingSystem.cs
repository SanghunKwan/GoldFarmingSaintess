using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;



[UpdateAfter(typeof(ServerDisconnectedInMatchingSystem))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerMatchingSystem : ISystem
{
    Entity _matchingEntity;

    BufferLookup<MatchedEntityBuffer> _matchingLookup;

    int _matchCount;
    uint _matchedPartyCount;

    public void OnCreate(ref SystemState state)
    {
        state.RequireAnyForUpdate(state.EntityManager.CreateEntityQuery(typeof(MatchingProtocol)));

        _matchCount = GameManager.Instance._ServerScriptableObject._matchingCount;
        _matchedPartyCount = 0;

        _matchingEntity = state.EntityManager.CreateEntity(typeof(MatchedEntityBuffer), typeof(MatchedGroupIndex));
        state.EntityManager.AddSharedComponent(_matchingEntity, new MatchedGroupIndex { _groupIndex = _matchedPartyCount++ });

        _matchingLookup = SystemAPI.GetBufferLookup<MatchedEntityBuffer>();

    }

    public void OnUpdate(ref SystemState state)
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        _matchingLookup.Update(ref state);

        var buffer = _matchingLookup[_matchingEntity];
        foreach (var (request, matching, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<MatchingProtocol>>().WithEntityAccess())
        {

            if (matching.ValueRO._isMatching)
            {
                //새 플레이어 매칭 시작.
                commandBuffer.AppendToBuffer(_matchingEntity, new MatchedEntityBuffer { _matchedConnection = request.ValueRO.SourceConnection });
                Debug.Log("매칭 시작");
            }
            else
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i]._matchedConnection != request.ValueRO.SourceConnection) continue;

                    buffer.RemoveAt(i);
                    break;
                }
            }

            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();

        state.EntityManager.Broadcast(new MatchingStatusProtocol { _matchingCount = buffer.Length });


        _matchingLookup.Update(ref state);

        buffer = _matchingLookup[_matchingEntity];

        if (buffer.Length >= _matchCount)
        {
            var newMatchedEntity = state.EntityManager.CreateEntity(typeof(MatchedEntityBuffer));
            state.EntityManager.AddSharedComponent(newMatchedEntity, new MatchedGroupIndex { _groupIndex = _matchedPartyCount });
            var matchedBuffer = state.EntityManager.GetBuffer<MatchedEntityBuffer>(newMatchedEntity);


            state.EntityManager.Broadcast(new SetHostProtocol { _matchingSize = _matchCount, _groupIndex = _matchedPartyCount }, buffer[0]._matchedConnection);

            for (int i = 0; i < _matchCount; i++)
            {
                matchedBuffer.Add(new MatchedEntityBuffer { _matchedConnection = buffer[i]._matchedConnection });
                var cleanupEntity = SystemAPI.GetComponentRO<InitializedClient>(buffer[i]._matchedConnection).ValueRO._cleanUpEntity;
                state.EntityManager.AddComponentData(cleanupEntity, new DisconnectCleanUp { _bufferEntityShared = _matchedPartyCount });
            }
            _matchedPartyCount++;

            buffer.RemoveRange(0, _matchCount);
        }
    }
}
