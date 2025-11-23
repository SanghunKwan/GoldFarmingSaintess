using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerMatchingSystem : ISystem
{
    NativeList<Entity> _matchingList;
    int _matchCount;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MatchingProtocol>();
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
                _matchingList.RemoveAt(_matchingList.BinarySearch(request.ValueRO.SourceConnection));
                //플레이어 매칭 취소.
            }

            commandBuffer.DestroyEntity(entity);
        }

        if (_matchingList.Length >= _matchCount)
        {
            for (int i = 0; i < _matchCount; i++)
                state.EntityManager.BroadcastMessage("게임시작");
            _matchingList.RemoveRange(0, _matchCount);
        }



        foreach (var target in _matchingList)
        {
            state.EntityManager.Broadcast(new MatchingStatusProtocol { _currentMatchingCount = _matchingList.Length }, target);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
