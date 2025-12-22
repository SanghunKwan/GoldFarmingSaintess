using GFSManagers;
using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientGetDisturbedSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(state.GetEntityQuery(typeof(ClientDisturbRPC), typeof(ReceiveRpcCommandRequest)));
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, disturb, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientDisturbRPC>>().WithEntityAccess())
        {
            GameSceneManager.Instance.TransferDisturb(disturb.ValueRO);

            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }
}
