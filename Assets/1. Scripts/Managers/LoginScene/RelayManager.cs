using System.Collections;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;





namespace GFSManagers
{
    public class RelayManager
    {
        Allocation _allocation;

        public async Task<string> InitRelay(int alloSize)
        {
            Debug.Log(alloSize);
            _allocation = await RelayService.Instance.CreateAllocationAsync(alloSize);
            Debug.Log(_allocation.AllocationId);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);

            ClientServerBootstrap.CreateServerWorld("ServerWorld");

            return joinCode;
        }

        public void LinkRelay()
        {
            var em = ClientServerBootstrap.ServerWorld.EntityManager;

            var driverStore = new NetworkDriverStore();
            var netDebug = em.CreateEntityQuery(typeof(NetDebug)).GetSingleton<NetDebug>();
            //var ipcSettings = DefaultDriverBuilder.GetNetworkServerSettings();
            //DefaultDriverBuilder.RegisterServerIpcDriver(ClientServerBootstrap.ServerWorld, ref driverStore, netDebug, ipcSettings);


            var relay = _allocation.ToRelayServerData(Unity.Services.Multiplayer.RelayProtocol.DTLS);
            var relaySettings = DefaultDriverBuilder.GetNetworkServerSettings();
            relaySettings.WithRelayParameters(ref relay);
            DefaultDriverBuilder.RegisterServerUdpDriver(ClientServerBootstrap.ServerWorld, ref driverStore, netDebug, relaySettings);

            var networkStreamDriver = em.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingleton<NetworkStreamDriver>();
            networkStreamDriver.ResetDriverStore(ClientServerBootstrap.ServerWorld.Unmanaged, ref driverStore);
            for (int i = networkStreamDriver.DriverStore.FirstDriver; i < networkStreamDriver.DriverStore.LastDriver; i++)
            {
                Debug.Log($"driver {i} : type = {networkStreamDriver.DriverStore.GetDriverType(i)}");
            }
            Debug.Log("listen");
            networkStreamDriver.Listen(NetworkEndpoint.AnyIpv4);
        }
        public void CancelRelay()
        {
            var serverWorld = ClientServerBootstrap.ServerWorld;
            ResetStore(serverWorld.EntityManager);
            serverWorld.Dispose();
        }
        NetworkStreamDriver ResetStore(EntityManager em)
        {
            var networkStreamDriver = em.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingleton<NetworkStreamDriver>();
            ref var store = ref networkStreamDriver.DriverStore;
            networkStreamDriver.ResetDriverStore(ClientServerBootstrap.ServerWorld.Unmanaged, ref store);

            return networkStreamDriver;
        }
    }
}