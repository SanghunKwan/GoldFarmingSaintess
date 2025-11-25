using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using GFSUtilities.ResourcesData;

#if !UNITY_SERVER
using UnityEngine.SceneManagement;
#endif





[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerNewIDSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkId>();
    }


    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
        {
            commandBuffer.AddComponent<InitializedClient>(entity);
            state.EntityManager.BroadcastMessage("Client connect with id = " + id.ValueRO.Value);

#if UNITY_SERVER
            var successProtocol = new PageProtocol { _pageType = PageType.Login, _id = id.ValueRO.Value };
            state.EntityManager.Broadcast(successProtocol, entity);
#else
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                var successProtocol = new PageProtocol { _pageType = PageType.Login, _id = id.ValueRO.Value };
                state.EntityManager.Broadcast(successProtocol, entity);
            }
            else
            {
                state.EntityManager.Broadcast(new HostLinkSuccess { }, entity);
            }
#endif
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
