using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;




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
        UnityEngine.Debug.Log("확인");
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var data = GameManager.Instance._SceneChangeDataScriptableObject;
        foreach (var (request, link, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<HostLinkSuccess>>().WithEntityAccess())
        {
            if (data._nameIndex == -1)
            {
                data._nameIndex = link.ValueRO._linkedIndex;
                Debug.Log("새로운 인덱스 부여" + data._nameIndex);
            }


            state.EntityManager.Broadcast(new HostClientIdentify { _beforeIndex = data._nameIndex, _currentIndex = link.ValueRO._linkedIndex }, request.ValueRO.SourceConnection);

            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
