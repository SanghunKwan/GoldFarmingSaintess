using GFSUtilities.UI;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace GFSManagers
{
    public class LoginManager : BaseBGWindowManager<LoginWindow, LoginManager, LoginNoneBGManager>
    {

        string _ip;
        ushort _port;


        public override void InitManager(LoginNoneBGManager bgManager)
        {
            base.InitManager(bgManager);

            ServerDataScriptableObject data = GameManager.Instance._ServerScriptableObject;


        }

        public void MakeWindow()
        {
            GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Login, _bgManager.transform);
            _window = go.GetComponent<LoginWindow>();
            _window.InitWindow(this);
        }

        public void CallWindow()
        {
            _window.FadeIn();
        }

        public void LinkServer()
        {
            //var server = ClientServerBootstrap.CreateServerWorld("ServerWorld");

            //if (World.DefaultGameObjectInjectionWorld == null)
            //    World.DefaultGameObjectInjectionWorld = server;

            //foreach (var world in World.All)
            //{
            //    if (world.Flags == WorldFlags.Game)
            //    {
            //        world.Dispose();
            //        break;
            //    }
            //}
            ServerDataScriptableObject data = GameManager.Instance._ServerScriptableObject;
            var endPoint = NetworkEndpoint.Parse(data._ipv4, _port);

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            em.AddComponentData(em.CreateEntity(), new NetworkStreamRequestConnect
            {
                Endpoint = endPoint
            });

            Debug.Log("클라 접속");
        }

        void ServerLink()
        {
            var endPoint = NetworkEndpoint.AnyIpv4.WithPort(_port);


        }
        void ClientLink()
        {
            var endPoint = NetworkEndpoint.Parse(_ip, _port);
        }
    }
}

