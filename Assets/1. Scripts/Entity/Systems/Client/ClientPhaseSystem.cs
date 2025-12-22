using GFSManagers;
using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;


[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientPhaseSystem : ISystem
{



    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ClientGamePhase>();
    }



    public void OnUpdate(ref SystemState state)
    {

        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (request, phase, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientGamePhase>>().WithEntityAccess())
        {
            GameSceneManager.Instance.SetPhase(phase.ValueRO._nextPhase);


            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }


}
