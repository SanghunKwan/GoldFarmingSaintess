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
        var entity = SystemAPI.GetSingletonEntity<GameStartProtocol>();
        var data = state.EntityManager.GetComponentData<GameStartProtocol>(entity);

        LoginSceneManager.Instance.GameStart(data);

        Debug.Log(data._nickNames);
        state.EntityManager.DestroyEntity(entity);
    }
}
