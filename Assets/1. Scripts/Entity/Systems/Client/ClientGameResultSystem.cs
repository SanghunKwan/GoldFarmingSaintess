using Unity.Entities;
using GFSManagers;
using GFSUtilities.Protocol;



[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientGameResultSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameResult>();
    }


    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        var entity = SystemAPI.GetSingletonEntity<GameResult>();
        var data = em.GetComponentData<GameResult>(entity);

        GameSceneManager.Instance.ShowGameResult(data.beforeIndexBuffer);

        em.DestroyEntity(entity);
    }

}
