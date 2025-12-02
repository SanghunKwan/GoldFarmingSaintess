using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[BurstCompile]
public partial struct ClientGhostUIChangeSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(state.GetEntityQuery(typeof(GoldInputData), typeof(GhostOwnerIsLocal)));
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new ClientGhostUIChangeJob { };
        state.Dependency = job.ScheduleParallel(state.Dependency);

        //foreach (var (data, local, entity) in SystemAPI.Query<RefRW<GoldInputData>, RefRO<GhostOwnerIsLocal>>().WithEntityAccess())
        //{
        //state.Dependency = job.ScheduleParallel(state.Dependency);
        //}
    }
}

[BurstCompile]
public partial struct ClientGhostUIChangeJob : IJobEntity
{


    [BurstCompile]
    public void Execute(ref PlayerProtocol player, in GoldInputData input, in GhostOwnerIsLocal owner)
    {
        player._gold = input.gold;
    }
}
