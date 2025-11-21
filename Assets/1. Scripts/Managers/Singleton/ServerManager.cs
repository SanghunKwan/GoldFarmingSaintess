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

        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        em.AddComponentData(em.CreateEntity(), new NetworkStreamRequestListen
        {
            Endpoint = _endPoint
        });
        Debug.Log("서버 대기중");
    }

}
