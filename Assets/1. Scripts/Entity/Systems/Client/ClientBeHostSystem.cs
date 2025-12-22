using Unity.Entities;
using GFSUtilities.Protocol;



[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientBeHostSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SetHostProtocol>();
    }


    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        var entity = SystemAPI.GetSingletonEntity<SetHostProtocol>();
        SetHostProtocol protocol = state.EntityManager.GetComponentData<SetHostProtocol>(entity);

        LoginSceneManager.Instance.SetHost(protocol);

        em.DestroyEntity(entity);
    }

}
