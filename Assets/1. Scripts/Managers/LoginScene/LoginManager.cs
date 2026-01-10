using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

namespace GFSManagers
{
    public class LoginManager : BaseBGWindowManager<LoginWindow, LoginManager, LoginNoneBGManager>
    {
        int _currentPage;

        public string _nickName { get; private set; }
        public int _id { get; private set; }

        public string _ticketId { get; private set; }
        public string _playerId { get; private set; }

        event Action _serverLinkEvent;

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

        public void LinkServer(in MultiplayAssignment assignment)
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
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, NetworkEndpoint.Parse(assignment.Ip, (ushort)assignment.Port));
            //query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, NetworkEndpoint.Parse(_ip, _port));

            Debug.Log("클라 연결");
            _window.SendPacketServer(clientWorld);
        }

        public void ServerLinkSuccss(in PageProtocol pageProtocol)
        {
            _CurrentPage = pageProtocol._pageType;
            _id = pageProtocol._id;

            _serverLinkEvent?.Invoke();
            _serverLinkEvent = null;
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
        public void TransferMatchingStatus(in MatchingStatusProtocol protocol)
        {
            _window.ShowCurrentMatchingPlayer(protocol._matchingCount);
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
        public void UpdateMatchingStatus(float endTime)
        {
            _window.UpdateMatchingData(endTime);
        }
        public void MatchingComplete(in GameStartProtocol pageProtocol)
        {
            string nickNames = pageProtocol._nickNames.ToString();

            GameManager manager = GameManager.Instance;

            manager.InstantiatePrefab(UIType.PlayersUI, _bgManager.transform).GetComponent<PlayersUI>()
                .InitUI(nickNames, pageProtocol._index, out var nicks, manager._PlayerColorScriptableObject._color, false);
            _messageBox.SetBox(MessageBoxType.Time);
            _window.FadeOut();
            int index = pageProtocol._index;
            _window.StartCoroutine(GFSManager.WaitForSecond(0.1f, ClearWorld));

            var data = manager._SceneChangeDataScriptableObject;
            data._names = nickNames;
            data._nameIndex = index;
            data._size = nicks.Length;
            data._joinCode = pageProtocol._joinCode.ToString();
        }
        void ClearWorld()
        {
            string str = ClientServerBootstrap.ClientWorld.Name;
            ClientServerBootstrap.ClientWorld.Dispose();
            ClientServerBootstrap.CreateClientWorld(str);
        }

        public void AutoLogin(string nickName)
        {
            Authen();
            _serverLinkEvent += () => SendProtocol(new UserSettingProtocol { _nickName = nickName });
        }

        public async void Authentication()
        {
            _window.ButtonInteractableFalse();

            await GFSManager.UnityServiceInitialize(1000);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Authen();
        }
        async void Authen()
        {
            _playerId = AuthenticationService.Instance.PlayerId;
            MultiplayAssignment assignment = null;
            do
            {
                var ticket = await MatchmakerService.Instance.CreateTicketAsync(new List<Player>
            {
                new Player(_playerId)
            }, new CreateTicketOptions());

                _ticketId = ticket.Id;

                Debug.Log(_ticketId);
                Debug.Log(_playerId);

                assignment = await PullMatchMaking();
            } while (assignment.Status != MultiplayAssignment.StatusOptions.Found);

            LinkServer(assignment);
        }

        async Task<MultiplayAssignment> PullMatchMaking()
        {
            MultiplayAssignment assignment = null;
            bool gotAssignment = false;
            do
            {
                //Rate limit delay
                await Task.Delay(TimeSpan.FromSeconds(1f));

                // Poll ticket
                var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(_ticketId);
                if (ticketStatus == null)
                {
                    continue;
                }

                //Convert to platform assignment data (IOneOf conversion)
                if (ticketStatus.Type == typeof(MultiplayAssignment))
                {
                    assignment = ticketStatus.Value as MultiplayAssignment;
                }

                switch (assignment?.Status)
                {
                    case MultiplayAssignment.StatusOptions.Found:
                        gotAssignment = true;
                        break;
                    case MultiplayAssignment.StatusOptions.InProgress:
                        //...
                        break;
                    case MultiplayAssignment.StatusOptions.Failed:
                        gotAssignment = true;
                        Debug.LogError("Failed to get ticket status. Error: " + assignment.Message);
                        break;
                    case MultiplayAssignment.StatusOptions.Timeout:
                        gotAssignment = true;
                        Debug.LogError("Failed to get ticket status. Ticket timed out.");
                        break;
                    default:
                        throw new InvalidOperationException();
                }

            } while (!gotAssignment);

            return assignment;
        }

        public void HostingReady(in string joinCode, uint groupIndex)
        {
            SendProtocol(new HostingReadyProtocol { _joinCode = joinCode, _groupIndex = groupIndex });
        }
    }
}


