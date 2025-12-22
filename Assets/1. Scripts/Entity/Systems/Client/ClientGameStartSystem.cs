using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;




[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientGameStartSystem : ISystem
{


    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameStartProtocol>();
    }


    
    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, matching, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<GameStartProtocol>>().WithEntityAccess())
        {
            LoginSceneManager.Instance.GameStart(matching.ValueRO);
            Debug.Log(matching.ValueRO._nickNames);
            commandBuffer.DestroyEntity(entity);
        }


        commandBuffer.Playback(state.EntityManager);

    }
}
