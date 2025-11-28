using GFSManagers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public void ASDF()
    {
        var _endPoint = NetworkEndpoint.AnyIpv4.WithPort(GameManager.Instance._ServerScriptableObject._port);
        Debug.Log(GameManager.Instance._ServerScriptableObject._port);

        World serverWorld = ClientServerBootstrap.ServerWorld;

        using var query = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(_endPoint);
    }
}
