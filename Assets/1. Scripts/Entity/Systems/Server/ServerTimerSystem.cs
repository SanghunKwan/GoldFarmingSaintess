using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Entities;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerTimerSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTimer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var timer = SystemAPI.GetSingletonRW<PlayerTimer>();

        ref var value = ref timer.ValueRW;
        float calTime = value._decreasingTime - SystemAPI.Time.DeltaTime;
        value._decreasingTime = calTime;
    }

}
