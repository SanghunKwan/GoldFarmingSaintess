using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;



[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerTimerActiveSystem : ISystem
{
    int _accumulatedEventFailCount;
    float _failAdventage;
    GamePhaseType _currentPhase;


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HostPhaseEnd>();
        _accumulatedEventFailCount = 0;
        _currentPhase = GamePhaseType.Turn;
        _failAdventage = GameManager.Instance._TurnScriptableObject._eventFailAdventage;
    }


    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (phase, entity) in SystemAPI.Query<RefRO<HostPhaseEnd>>().WithEntityAccess())
        {
            if (_currentPhase == phase.ValueRO._phase)
            {
                int nextIndex = ((int)_currentPhase + 1 + PassEvent(_currentPhase)) % (int)GamePhaseType.Max;

                var timer = SystemAPI.GetSingletonRW<PlayerTimer>();
                timer.ValueRW._decreasingTime = GameManager.Instance._TurnScriptableObject._times[nextIndex];
                _currentPhase = (GamePhaseType)nextIndex;
                state.EntityManager.Broadcast(new ClientGamePhase { _nextPhase = _currentPhase });

                foreach (var (request, disturb, identity) in SystemAPI.Query<RefRO<NetworkId>, RefRO<ClientPhaseEnd>>().WithEntityAccess())
                {
                    commandBuffer.RemoveComponent<ClientPhaseEnd>(identity);
                }
            }

            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }
    int PassEvent(GamePhaseType type)
    {
        if (type != GamePhaseType.Turn) return 0;

        if (Random.Range(0f, 1f) > _failAdventage * _accumulatedEventFailCount)
        {
            _accumulatedEventFailCount++;
            return 1;
        }
        else
        {
            _accumulatedEventFailCount = 1;
            return 0;
        }
    }
}
