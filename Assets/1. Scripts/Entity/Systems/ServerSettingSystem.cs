using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerSettingSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UserSettingProtocol>();
    }


    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (request, settingData, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<UserSettingProtocol>>().WithEntityAccess())
        {
            if (GFSManager.IsValidNickName(settingData.ValueRO._nickName.ToString()))
                state.EntityManager.Broadcast(settingData.ValueRO, request.ValueRO.SourceConnection);
            else
                state.EntityManager.Broadcast(new ErrorProtocol { _errorType = ErrorType.NickNameInvalid }, request.ValueRO.SourceConnection);

            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }

}
