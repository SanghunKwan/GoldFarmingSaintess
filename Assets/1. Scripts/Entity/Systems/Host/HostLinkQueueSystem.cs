using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct HostLinkQueueSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProtocolSpawn>();
        state.RequireForUpdate<HostPlayerProtocolSpawnQueue>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (queue, entity) in SystemAPI.Query<RefRO<HostPlayerProtocolSpawnQueue>>().WithEntityAccess())
        {
            var prefab = SystemAPI.GetSingleton<PlayerProtocolSpawn>();

            Entity protocolEntity = commandBuffer.Instantiate(prefab.prefab);
            commandBuffer.SetComponent(protocolEntity, new PlayerProtocol { _gold = 100, _emotionType = EmotionType.None });

            commandBuffer.SetComponent(protocolEntity, new GhostOwner { NetworkId = queue.ValueRO._currentIndex });
            commandBuffer.AppendToBuffer(queue.ValueRO._requestTarget, new LinkedEntityGroup { Value = protocolEntity });

            commandBuffer.AddComponent<NetworkStreamInGame>(queue.ValueRO._requestTarget);

            var send = commandBuffer.CreateEntity();
            commandBuffer.AddComponent<HostSendGoIn>(send);
            commandBuffer.AddComponent<SendRpcCommandRequest>(send);

            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }
}
