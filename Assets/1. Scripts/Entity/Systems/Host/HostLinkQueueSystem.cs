using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct HostLinkQueueSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProtocolSpawn>();
        state.RequireForUpdate<HostPlayerProtocolSpawnQueue>();
    }

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

            state.EntityManager.BroadcastZoroSize<HostSendGoIn>();

            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }
}
