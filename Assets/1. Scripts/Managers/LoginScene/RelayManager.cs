using System.Threading.Tasks;
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
            _allocation = await RelayService.Instance.CreateAllocationAsync(alloSize);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);

            ClientServerBootstrap.CreateServerWorld("ServerWorld");

            return joinCode;
        }

        public void LinkRelay()
        {
            var em = ClientServerBootstrap.ServerWorld.EntityManager;

            var driverStore = new NetworkDriverStore();
            var netDebug = em.CreateEntityQuery(typeof(NetDebug)).GetSingleton<NetDebug>();
            var ipcSettings = DefaultDriverBuilder.GetNetworkServerSettings();
            var relay = AllocationUtils.ToRelayServerData(_allocation, "dtls");
            DefaultDriverBuilder.RegisterServerIpcDriver(ClientServerBootstrap.ServerWorld, ref driverStore, netDebug, ipcSettings);
            var relaySettings = DefaultDriverBuilder.GetNetworkServerSettings();
            relaySettings.WithRelayParameters(ref relay);
            DefaultDriverBuilder.RegisterServerUdpDriver(ClientServerBootstrap.ServerWorld, ref driverStore, netDebug, relaySettings);
            var networkStreamDriver = em.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingleton<NetworkStreamDriver>();
            networkStreamDriver.ResetDriverStore(ClientServerBootstrap.ServerWorld.Unmanaged, ref driverStore);
            
            networkStreamDriver.Listen(NetworkEndpoint.AnyIpv4);
        }
    }
}