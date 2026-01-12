using Unity.NetCode;
using UnityEngine.Scripting;


[Preserve]
public class CustomBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        //return base.Initialize(defaultWorldName);

        if (!DetermineIfBootstrappingEnabled())
            return false;

        CreateDefaultClientServerWorlds();

        return true;
    }
    protected override void CreateDefaultClientServerWorlds()
    {
        //base.CreateDefaultClientServerWorlds();
#if UNITY_SERVER
        if (RequestedPlayType == PlayType.Server || RequestedPlayType == PlayType.ClientAndServer)
            CreateServerWorld("ServerWorld");
#endif
        if (RequestedPlayType == PlayType.Client || RequestedPlayType == PlayType.ClientAndServer)
            CreateClientWorld("ClientWorld");

        //ServerDataScriptableObject data = GameManager.Instance._ServerScriptableObject;



        //if (ServerWorld != null)
        //{
        //    var endPoint = NetworkEndpoint.AnyIpv4.WithPort(data._port);

        //    using var query = ServerWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        //    query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(endPoint);
        //}
        //if (ClientWorld != null)
        //{
        //    var endPoint = NetworkEndpoint.Parse(data._ipv4, data._port);

        //    using var query = ClientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        //    query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(ClientWorld.EntityManager, endPoint);
        //}
    }
}
