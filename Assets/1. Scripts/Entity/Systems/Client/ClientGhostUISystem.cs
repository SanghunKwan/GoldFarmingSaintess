using GFSManagers;
using GFSUtilities.ResourcesData;
using Unity.Entities;
using Unity.NetCode;


[UpdateInGroup(typeof(GhostInputSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientGhostUISystem : ISystem
{
    BufferLookup<ClientsIdentifyingData> _lookup;


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GoldInputData>();
        state.RequireForUpdate<ClientsIdentifyingData>();
        _lookup = state.GetBufferLookup<ClientsIdentifyingData>();
    }


    public void OnUpdate(ref SystemState state)
    {
        foreach (var (data, local, entity) in SystemAPI.Query<RefRW<GoldInputData>, RefRO<GhostOwnerIsLocal>>().WithEntityAccess())
        {
            data.ValueRW.gold = GameSceneManager.Instance.GetMoney;
        }
        _lookup.Update(ref state);
        Entity dataEntity = SystemAPI.GetSingletonEntity<ClientsIdentifyingData>();
        foreach (var (data, ghost, entity) in SystemAPI.Query<RefRO<PlayerProtocol>, RefRO<GhostOwner>>().WithEntityAccess())
        {
            int playerIndex = _lookup[dataEntity][ghost.ValueRO.NetworkId - 1]._beforeIndex;
            GameSceneManager.Instance.TransferPlayerProtocols(data.ValueRO, playerIndex);
        }
    }

}
