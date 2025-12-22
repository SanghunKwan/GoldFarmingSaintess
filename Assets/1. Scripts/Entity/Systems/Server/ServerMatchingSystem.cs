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
        _matchingLookup = SystemAPI.GetBufferLookup<MatchedEntityBuffer>();
        state.EntityManager.AddSharedComponent(_matchingEntity, new MatchedGroupIndex { _groupIndex = _matchedPartyCount++ });

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

        _matchingLookup.Update(ref state);

        buffer = _matchingLookup[_matchingEntity];

        commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        if (buffer.Length >= _matchCount)
        {
            var newMatchedEntity = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(newMatchedEntity, typeof(MatchedGroupIndex));
            commandBuffer.AddComponent(newMatchedEntity, typeof(MatchedEntityBuffer));
            commandBuffer.AddSharedComponent(newMatchedEntity, new MatchedGroupIndex { _groupIndex = _matchedPartyCount });

            state.EntityManager.Broadcast(new SetHostProtocol { _matchingSize = _matchCount, _groupIndex = _matchedPartyCount }, buffer[0]._matchedConnection);

            StringBuilder strbuilder = new StringBuilder();
            for (int i = 0; i < _matchCount; i++)
            {
                strbuilder.Append(state.EntityManager.GetComponentData<UserSettingData>(buffer[i]._matchedConnection)._nickName);
                strbuilder.Append(' ');
                //state.EntityManager.Broadcast(new GameStartProtocol { _nickNames = strbuilder.ToString(), _index = i + 1 }, _matchingList[i]);
                commandBuffer.AppendToBuffer(newMatchedEntity, new MatchedEntityBuffer { _matchedConnection = buffer[i]._matchedConnection });
                commandBuffer.AddComponent(buffer[i]._matchedConnection, new DisconnectCleanUp { _bufferEntityShared = _matchedPartyCount });
            }
            _matchedPartyCount++;
            strbuilder.Remove(strbuilder.Length - 1, 1);

            buffer.RemoveRange(0, _matchCount);
        }

        commandBuffer.Playback(state.EntityManager);
    }
}
