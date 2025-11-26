using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientHostLinkSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HostLinkSuccess>();
    }


    public void OnUpdate(ref SystemState state)
    {
        UnityEngine.Debug.Log("»Æ¿Œ");
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var data = GameManager.Instance._SceneChangeDataScriptableObject;
        foreach (var (request, link, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<HostLinkSuccess>>().WithEntityAccess())
        {
            if (data._nameIndex == -1)
                data._nameIndex = link.ValueRO._linkedIndex;

            state.EntityManager.Broadcast(new HostClientIdentify { _index = data._nameIndex }, request.ValueRO.SourceConnection);

            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
