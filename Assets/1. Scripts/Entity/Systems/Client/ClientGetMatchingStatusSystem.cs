using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientGetMatchingStatusSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(state.GetEntityQuery(typeof(MatchingStatusProtocol), typeof(ReceiveRpcCommandRequest)));
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (protocol, request, entity) in SystemAPI.Query<RefRO<MatchingStatusProtocol>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
        {
            LoginSceneManager.Instance.ShowLinkStatus(protocol.ValueRO);

            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }
}
