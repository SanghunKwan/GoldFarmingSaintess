using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using System;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.NetCode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



[UpdateAfter(typeof(ServerMatchingSystem))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerHostingReadySystem : ISystem
{
    int _matchingCount;
    EntityQuery _matchedQuery;
    EntityQuery _matchWaitQuery;


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HostingReadyProtocol>();
        _matchingCount = GameManager.Instance._ServerScriptableObject._matchingCount;

        _matchedQuery = state.EntityManager.CreateEntityQuery(typeof(MatchedGroupIndex), typeof(MatchedEntityBuffer));
        _matchWaitQuery = state.EntityManager.CreateEntityQuery(typeof(MatchedGroupIndex), typeof(MatchedEntityBuffer));
        _matchWaitQuery.AddSharedComponentFilter(new MatchedGroupIndex { _groupIndex = 0 });
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (host, rpc, entity) in SystemAPI.Query<HostingReadyProtocol, ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            _matchedQuery.SetSharedComponentFilter(new MatchedGroupIndex { _groupIndex = host._groupIndex });


            commandBuffer.DestroyEntity(entity);
            if (!_matchedQuery.TryGetSingletonEntity<MatchedEntityBuffer>(out var bufferEntity)) continue;


            var buffer = state.EntityManager.GetBuffer<MatchedEntityBuffer>(bufferEntity);

            StringBuilder strbuilder = new StringBuilder();

            if (buffer.Length == _matchingCount)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    strbuilder.Append(state.EntityManager.GetComponentData<UserSettingData>(buffer[i]._matchedConnection)._nickName);
                    strbuilder.Append(' ');
                }
                strbuilder.Remove(strbuilder.Length - 1, 1);
                string nickNames = strbuilder.ToString();

                for (int i = 0; i < buffer.Length; i++)
                {
                    state.EntityManager.BroadcastMessage("게임시작");
                    state.EntityManager.Broadcast(new GameStartProtocol { _nickNames = nickNames, _index = i + 1, _joinCode = host._joinCode }, buffer[i]._matchedConnection);
                }
            }
            else
            {
                //매칭 중단
                var originalBuffer = _matchWaitQuery.GetSingletonBuffer<MatchedEntityBuffer>();
                originalBuffer.AddRange(buffer.AsNativeArray());
                commandBuffer.DestroyEntity(bufferEntity);
            }
        }
        commandBuffer.Playback(state.EntityManager);
    }
}
