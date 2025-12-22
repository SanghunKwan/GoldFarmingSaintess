using GFSUtilities.ResourcesData;
using Unity.Entities;




[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct HostGhostPrefabInstantiateSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HostNeedGhostPrefab>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var prefab = SystemAPI.GetSingletonRW<PlayerProtocolSpawn>();
        var dataEntity = SystemAPI.GetSingletonEntity<HostNeedGhostPrefab>();
        var data = SystemAPI.GetComponent<HostNeedGhostPrefab>(dataEntity);

        var em = state.EntityManager;
        switch (data._type)
        {
            case PrefabGhostType.Timer:
                em.Instantiate(prefab.ValueRO.prefabTimer);
                break;
            case PrefabGhostType.DisturbCounter:
                em.Instantiate(prefab.ValueRO.prefabDisturbCounter);
                break;
        }
        em.DestroyEntity(dataEntity);
    }
}
