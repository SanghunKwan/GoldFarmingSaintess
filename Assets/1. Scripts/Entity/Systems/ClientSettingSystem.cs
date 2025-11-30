using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientSettingSystem : ISystem
{


    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UserSettingProtocol>();
    }


    
    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, setting, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<UserSettingProtocol>>().WithEntityAccess())
        {
            LoginSceneManager.Instance.NickNameDetermined(setting.ValueRO);
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
