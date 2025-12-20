using GFSManagers;
using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientAgendaSystem : ISystem
{



    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ServerVoteIndex>();
    }



    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, setting, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ServerVoteIndex>>().WithEntityAccess())
        {
            GameSceneManager.Instance.SetAgenda(setting.ValueRO._index);
            
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
