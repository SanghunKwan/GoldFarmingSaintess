using Unity.Entities;
using GFSManagers;



[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientTimerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTimer>();
        state.Enabled = false;
    }


    public void OnUpdate(ref SystemState state)
    {
        ref var timer = ref SystemAPI.GetSingletonRW<PlayerTimer>().ValueRW;

        GameSceneManager.Instance.SetTimer(timer._decreasingTime);

        if (timer._decreasingTime < 0)
        {
            GameSceneManager.Instance.AlarmTimer();
            state.Enabled = false;
        }
    }

}
