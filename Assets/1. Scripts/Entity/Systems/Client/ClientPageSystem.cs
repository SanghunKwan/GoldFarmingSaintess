using GFSUtilities;
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
        var entity = SystemAPI.GetSingletonEntity<PageProtocol>();
        var data = state.EntityManager.GetComponentData<PageProtocol>(entity);

        LoginSceneManager manager = LoginSceneManager.Instance;

        manager.ServerLinkSuccss(data);

        state.EntityManager.Broadcast(new PlayerData { _playerId = manager.GetPlayerId(), _ticketId = manager.GetTicketId() });

        state.EntityManager.DestroyEntity(entity);
    }

}
