using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Entities;
using UnityEngine;


[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerBroadcastRandomSystem : ISystem
{


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ClientVoteIndex>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var entity = SystemAPI.GetSingletonEntity<ClientVoteIndex>();

        var condition = state.EntityManager.GetComponentData<ClientVoteIndex>(entity);
        int agenda = Random.Range(condition._inclusiveBottom, condition._exclusiveTop);

        state.EntityManager.Broadcast(new ServerVoteIndex { _index = agenda });
        state.EntityManager.DestroyEntity(entity);
    }

}
