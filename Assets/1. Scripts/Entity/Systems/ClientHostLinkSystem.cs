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
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        ref int nameIndex = ref GameManager.Instance._SceneChangeDataScriptableObject._nameIndex;
        foreach (var (request, link, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<HostLinkSuccess>>().WithEntityAccess())
        {
            if (nameIndex == -1)
            {
                nameIndex = link.ValueRO._linkedIndex;
                //Debug.Log("새로운 인덱스 부여" + data._nameIndex);
            }


            state.EntityManager.Broadcast(new HostClientIdentify { _beforeIndex = nameIndex, _currentIndex = link.ValueRO._linkedIndex }, request.ValueRO.SourceConnection);

            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
