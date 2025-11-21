using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public partial struct ListeningSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamDriver>();
    }
    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.HasSingleton<NetworkStreamDriver>())
        {
            Debug.Log("접속 성공");
            state.Enabled = false;
        }

    }

}

