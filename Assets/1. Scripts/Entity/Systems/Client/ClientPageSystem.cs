using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;




[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientPageSystem : ISystem
{


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PageProtocol>();
    }



    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var entity = SystemAPI.GetSingletonEntity<PageProtocol>();
        var data = state.EntityManager.GetComponentData<PageProtocol>(entity);

        LoginSceneManager.Instance.ServerLinkSuccss(data);
        state.EntityManager.DestroyEntity(entity);
    }

}
