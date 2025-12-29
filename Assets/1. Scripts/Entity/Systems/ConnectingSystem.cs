using GFSUtilities.Protocol;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;




[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ConnectingSystem : ISystem
{


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkId>();
    }


    public void OnUpdate(ref SystemState state)
    {

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SendMeessageRpc("¾È³çÇÏ¼¼¿ä ¼±»ý´Ô", state.EntityManager);
        }

    }
    void SendMeessageRpc(in string text, in EntityManager manager)
    {
        var entity = manager.CreateEntity(typeof(SendRpcCommandRequest), typeof(MessageRpcCommand));
        manager.SetComponentData(entity, new MessageRpcCommand { _text = text });
    }
}
