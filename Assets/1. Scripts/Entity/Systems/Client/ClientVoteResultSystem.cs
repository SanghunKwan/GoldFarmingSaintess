using Unity.Entities;
using GFSManagers;
using GFSUtilities.Protocol;



[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientVoteResultSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<VoteResult>();
    }


    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        var entity = SystemAPI.GetSingletonEntity<VoteResult>();
        var data = em.GetComponentData<VoteResult>(entity);

        GameSceneManager.Instance.ShowBidResult(data.beforeIndexBuffer);


        em.DestroyEntity(entity);
    }

}
