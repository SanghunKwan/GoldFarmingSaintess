using GFSUtilities;
using GFSUtilities.Protocol;
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

        int _currentPage;

        public string _nickName { get; private set; }
        public int _id { get; private set; }

        MessageBox _messageBox;
        SettingWindow _settingWindow;

        public PageType _CurrentPage
        {
            get => (PageType)_currentPage;
            set
            {
                _window.SetPage(_currentPage, false);
                _currentPage = (int)value;
                _window.SetPage(_currentPage, true);
            }
        }

        public override void InitManager(LoginNoneBGManager bgManager)
        {
            base.InitManager(bgManager);

            ServerDataScriptableObject data = GameManager.Instance._ServerScriptableObject;

            _ip = data._ipv4;
            _port = data._port;

            _currentPage = 0;
        }

        public void MakeWindow()
        {
            GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Login, _bgManager.transform);
            _window = go.GetComponent<LoginWindow>();
            _window.InitWindow(this);

            go = GameManager.Instance.InstantiatePrefab(UIType.MessageBox, _window.transform);
            _messageBox = go.GetComponent<MessageBox>();
            _messageBox.InitBox();

        }

        public void CallWindow()
        {
            _window.FadeIn();
        }

        public void LinkServer()
        {
            World clientWorld = World.DefaultGameObjectInjectionWorld = ClientServerBootstrap.ClientWorld;

            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }

            using var query = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, NetworkEndpoint.Parse(_ip, _port));

            Debug.Log("클라 연결");
        }

        public void ServerLinkSuccss(in PageProtocol pageProtocol)
        {
            _CurrentPage = pageProtocol._pageType;
            _id = pageProtocol._id;
        }
        public void SubmitNickName(in string nickName)
        {
            if (!CheckValid(nickName)) return;


            _nickName = nickName;
            _messageBox.SetBox(MessageBoxType.Check);
            _messageBox.SetText(string.Format("'{0}'으로 진행하시겠습니까?", _nickName));


            _messageBox.OnButtonClickDispose += (buttonIndex) =>
            {
                if (buttonIndex != 0) return;

                SendProtocol(new UserSettingProtocol { _nickName = _nickName });
            };
        }
        void SendProtocol<T>(in T protocol) where T : unmanaged, IComponentData
        {
            var query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(T));
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(query, protocol);
        }
        bool CheckValid(in string nickName)
        {
            if (string.IsNullOrEmpty(nickName))
            {
                _messageBox.SetBox(MessageBoxType.Alert);
                _nickName = null;
                return false;
            }

            if (!GFSManager.CheckKorean(nickName))
            {
                _messageBox.SetBox(MessageBoxType.Alert);
                _messageBox.SetText(0);
                _nickName = null;
                return false;
            }

            return true;
        }

        void MakeSettingWindow()
        {
            GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Setting, _bgManager.transform);
            _settingWindow = go.GetComponent<SettingWindow>();
            _settingWindow.InitWindow(this);

        }
        public void CallSettingWindow()
        {
            if (_settingWindow == null)
                MakeSettingWindow();

            _settingWindow.ToggleWindow();
        }

        public void NickNameDetermined(in UserSettingProtocol settingProtocol)
        {
            _nickName = settingProtocol._nickName.ToString();
            _CurrentPage = PageType.Match;
            _window.FadeInOrder(LogingraphicGroupType.MatchWindowButtons);
        }

        public void SettingCancel()
        {
            _messageBox.transform.SetParent(_settingWindow.transform);
            _messageBox.SetBox(MessageBoxType.Check);
            _messageBox.SetText(1);

            _messageBox.OnButtonClickDispose += (buttonIndex) =>
            {
                if (buttonIndex != 0) return;
                _settingWindow.DisactiveWindow();
            };
        }
        public void SubmitSetting(in string newNickName)
        {
            _messageBox.transform.SetParent(_settingWindow.transform);
            if (!CheckValid(newNickName)) return;

            _messageBox.SetBox(MessageBoxType.Check);
            _messageBox.SetText(2);
            _nickName = newNickName;

            _messageBox.OnButtonClickDispose += (buttonIndex) =>
            {
                if (buttonIndex != 0) return;

                SendProtocol(new UserSettingProtocol { _nickName = _nickName });
                _settingWindow.DisactiveWindow();
            };
        }
        public void SetMatching(bool isOn)
        {
            SendProtocol(new MatchingProtocol { _isMatching = isOn });
        }
        public void UpdateMatchingStatus(in MatchingStatusProtocol matchingStatusProtocol)
        {
            _window.UpdateMatchingData(matchingStatusProtocol._currentMatchingCount);
        }
        public void MatchingComplete(in GameStartProtocol pageProtocol)
        {
            GameManager.Instance.InstantiatePrefab(UIType.PlayersUI, _bgManager.transform).GetComponent<PlayersUI>().InitUI(pageProtocol._nickNames, out var nicks, false);
            _messageBox.SetBox(MessageBoxType.Time);
            _window.FadeOut();
            _window.StartCoroutine(GFSManager.WaitForSecond(0.1f, ClearWorld));

            var data = GameManager.Instance._SceneChangeDataScriptableObject;
            data._names = pageProtocol._nickNames.ToString();
            data._nameIndex = pageProtocol._index;
            data._size = nicks.Length;

            var connectData = GameManager.Instance._ServerScriptableObject;
            connectData._port = 2;
        }
        void ClearWorld()
        {
            string str = ClientServerBootstrap.ClientWorld.Name;
            ClientServerBootstrap.ClientWorld.Dispose();
            ClientServerBootstrap.CreateClientWorld(str);

#if UNITY_EDITOR
            if (ClientServerBootstrap.ServerWorld != null)
            {
                str = ClientServerBootstrap.ServerWorld.Name;
                ClientServerBootstrap.ServerWorld.Dispose();
                ClientServerBootstrap.CreateServerWorld(str);
            }

#endif
        }
    }
}

