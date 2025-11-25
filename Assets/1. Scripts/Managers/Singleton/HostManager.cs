using GFSManagers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public class HostManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var sceneChangeData = GameManager.Instance._SceneChangeDataScriptableObject;
        var serverData = GameManager.Instance._ServerScriptableObject;

        World tempWorld;
        if (ClientServerBootstrap.ServerWorld != null)
        {
            tempWorld = ClientServerBootstrap.ServerWorld;

            var endPoint = NetworkEndpoint.AnyIpv4.WithPort(serverData._port);

            using var query = tempWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(endPoint);
        }

        if (ClientServerBootstrap.ClientWorld != null)
        {
            tempWorld = ClientServerBootstrap.ClientWorld;

            var endPoint = NetworkEndpoint.Parse(serverData._ipv4, serverData._port);

            using var query = tempWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(tempWorld.EntityManager, endPoint);
        }
    }

}
