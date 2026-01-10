using Unity.Entities;
using GFSUtilities.Protocol;



[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientHostCancelSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HostingStopProtocol>();
    }


    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;
        var entity = SystemAPI.GetSingletonEntity<HostingStopProtocol>();
        LoginSceneManager.Instance.CancelHost();

        em.DestroyEntity(entity);
    }

}
