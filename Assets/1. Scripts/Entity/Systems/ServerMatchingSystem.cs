using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerMatchingSystem : ISystem
{
    NativeList<Entity> _matchingList;
    int _matchCount;

    public void OnCreate(ref SystemState state)
    {
        state.RequireAnyForUpdate(state.EntityManager.CreateEntityQuery(typeof(MatchingProtocol)), state.EntityManager.CreateEntityQuery(typeof(DisconnectedPlayer)));

        _matchingList = new NativeList<Entity>(Allocator.Persistent);
        _matchCount = GameManager.Instance._ServerScriptableObject._matchingCount;
    }


    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, matching, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<MatchingProtocol>>().WithEntityAccess())
        {
            if (matching.ValueRO._isMatching)
            {
                //새 플레이어 매칭 시작.
                _matchingList.Add(request.ValueRO.SourceConnection);
            }
            else
            {
                int index = _matchingList.IndexOf(request.ValueRO.SourceConnection);
                _matchingList.RemoveAt(index);
                //플레이어 매칭 취소.
            }

            commandBuffer.DestroyEntity(entity);
        }

        if (_matchingList.Length >= _matchCount)
        {
            for (int i = 0; i < _matchCount; i++)
            {
                commandBuffer.AddComponent(_matchingList[i], new MatchingConditionData { });

                StringBuilder strbuilder = new StringBuilder();
                for (int j = 0; j < _matchCount; j++)
                {
                    strbuilder.Append(state.EntityManager.GetComponentData<UserSettingData>(_matchingList[j])._nickName);
                    strbuilder.Append(' ');
                }
                strbuilder.Remove(strbuilder.Length - 1, 1);
                state.EntityManager.Broadcast(new GameStartProtocol { _nickNames = strbuilder.ToString() }, _matchingList[i]);
            }

            state.EntityManager.BroadcastMessage("게임시작");
            _matchingList.RemoveRange(0, _matchCount);
        }

        foreach (var (disconnect, entity) in SystemAPI.Query<RefRO<DisconnectedPlayer>>().WithEntityAccess())
        {
            if (_matchingList.Contains(disconnect.ValueRO.disconnectedSource))
            {
                int index = _matchingList.IndexOf(disconnect.ValueRO.disconnectedSource);
                var indexEntity = _matchingList[index];

                _matchingList.RemoveAt(index);
                commandBuffer.DestroyEntity(indexEntity);
            }
            commandBuffer.DestroyEntity(entity);
        }

        foreach (var target in _matchingList)
        {
            state.EntityManager.Broadcast(new MatchingStatusProtocol { _currentMatchingCount = _matchingList.Length, _time = Time.time }, target);
        }

        commandBuffer.Playback(state.EntityManager);
    }


    public void OnDestory(ref SystemState state)
    {
        _matchingList.Clear();
        _matchingList.Dispose();
    }
}
