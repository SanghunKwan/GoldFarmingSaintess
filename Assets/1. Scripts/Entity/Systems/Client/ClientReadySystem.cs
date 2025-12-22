using GFSManagers;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientReadySystem : ISystem
{


    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AllClientReady>();
    }



    public void OnUpdate(ref SystemState state)
    {

        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (request, readyData, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<AllClientReady>>().WithEntityAccess())
        {
            GameSceneManager.Instance.ReadyToStart(readyData.ValueRO);

            //readyData.ValueRO._beforeIndexBuffer
            var bufferEntity = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(bufferEntity, typeof(ClientsIdentifyingData));
            var buffer = commandBuffer.SetBuffer<ClientsIdentifyingData>(bufferEntity);

            ref readonly var fixedList = ref readyData.ValueRO.beforeIndexBuffer;
            for (int i = 0; i < fixedList.Length; i++)
                buffer.Add(new ClientsIdentifyingData { _beforeIndex = fixedList[i] });

            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
    }


}
