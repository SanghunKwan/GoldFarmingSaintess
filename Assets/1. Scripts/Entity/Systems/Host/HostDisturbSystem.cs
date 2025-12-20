using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct HostDisturbSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(state.GetEntityQuery(typeof(ClientDisturbRPC), typeof(ReceiveRpcCommandRequest)));
    }

    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        ref var counter = ref SystemAPI.GetSingletonRW<DisturbCounter>().ValueRW;

        foreach (var (request, disturb, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientDisturbRPC>>().WithEntityAccess())
        {
            foreach (var (netId, idEntity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<ClientPhaseEnd>().WithEntityAccess())
            {
                //피해 생성
                state.EntityManager.Broadcast(disturb.ValueRO, idEntity);
            }

            switch (disturb.ValueRO._type)
            {
                case DisturbType.MonsterSpawn:
                    counter._monsterSpawnCount++;
                    break;
                case DisturbType.MonsterBuff:
                    counter._monsterBuffCount++;
                    break;
                case DisturbType.HeroHurt:
                    counter._heroHurtCount++;
                    break;
            }
            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }
}
