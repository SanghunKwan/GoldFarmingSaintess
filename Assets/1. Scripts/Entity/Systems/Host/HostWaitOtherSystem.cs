using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateBefore(typeof(ServerTimerActiveSystem))]
public partial struct HostWaitOtherSystem : ISystem
{
    EntityQuery _inPhaseQuery;


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(state.GetEntityQuery(typeof(WaitOtherEndPhase), typeof(ReceiveRpcCommandRequest)));

        using var queryBuilder = new EntityQueryBuilder(Allocator.Temp);
        _inPhaseQuery = queryBuilder.WithAll<NetworkId>().WithNone<ClientPhaseEnd>().Build(ref state);
    }

    public void OnUpdate(ref SystemState state)
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        GamePhaseType type = GamePhaseType.Max;
        foreach (var (request, disturb, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<WaitOtherEndPhase>>().WithEntityAccess())
        {
            type = disturb.ValueRO._currentPhase;
            commandBuffer.AddComponent(request.ValueRO.SourceConnection, new ClientPhaseEnd { _currentPhase = type });
            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();

        if (!_inPhaseQuery.IsEmpty) return;

        var phaseEntity = state.EntityManager.CreateEntity(typeof(HostPhaseEnd));
        state.EntityManager.AddComponentData(phaseEntity, new HostPhaseEnd { _phase = type });

        commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, disturb, entity) in SystemAPI.Query<RefRO<NetworkId>, RefRO<ClientPhaseEnd>>().WithEntityAccess())
        {
            commandBuffer.RemoveComponent<ClientPhaseEnd>(entity);
        }

        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}