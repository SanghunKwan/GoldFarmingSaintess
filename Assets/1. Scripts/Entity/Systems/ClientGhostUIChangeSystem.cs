using GFSManagers;
using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[BurstCompile]
public partial struct ClientGhostUIChangeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        using var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<GoldInputData, GhostOwnerIsLocal>();
        state.RequireForUpdate(state.GetEntityQuery(builder));
    }
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (data, local, entity) in SystemAPI.Query<RefRW<GoldInputData>, RefRO<GhostOwnerIsLocal>>().WithEntityAccess())
        {
            var job = new ClientGhostUIChangeJob
            {
                detlaTime = SystemAPI.Time.DeltaTime
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
}

[BurstCompile]
public partial struct ClientGhostUIChangeJob : IJobEntity
{
    public float detlaTime;
    public void Execute(ref PlayerProtocol player, GoldInputData input)
    {
        player._gold = input.gold;
    }
}
