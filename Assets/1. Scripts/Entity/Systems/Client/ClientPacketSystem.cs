using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Entities;
using Unity.NetCode;




[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientPacketSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LinkPacket>();
    }



    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (packet, request, entity) in SystemAPI.Query<LinkPacket, ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }
}
