using GFSManagers;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Services.Core;
using Unity.Services.Matchmaker.Models;

using Unity.Services.Matchmaker;
using GFSUtilities;
using System.Collections;
using System.Threading.Tasks;




#if UNITY_SERVER
using Unity.Services.Multiplay;
#endif
using UnityEngine;



public class ServerManager : MonoBehaviour
{
    string _ip;
    ushort _portNum;
    string _ticketId;
    int _serverSize;
    const float _waitTime = 30;

    HashSet<string> _linkedId = new HashSet<string>();

    Dictionary<string, float> _waitForConnect = new Dictionary<string, float>();
    Dictionary<string, float> _waitForApprove = new Dictionary<string, float>();

    Dictionary<string, float> _kickList = new Dictionary<string, float>();

    Queue<string> _playerExitBuffer = new Queue<string>();


    private void OnEnable()
    {
        StartCoroutine(UpdateBackFillCoroutine());
    }
    IEnumerator UpdateBackFillCoroutine()
    {
        var wait = new WaitForSecondsRealtime(1);
        int i = 0;
        yield return wait;
        while (enabled)
        {
            //ApproveBackfill 및 UpdateBackfill 호출
            var task = UpdateBackFill();
            yield return new WaitUntil(() => task.IsCompleted);
            if (i == 0)
                //연결 실패한 클라이언트 데이터 삭제
                ClearKickList();

            yield return wait;
            i = (i + 1) % 10;
        }
    }

    public async void InitManager()
    {
        _serverSize = GameManager.Instance._ServerScriptableObject._serverSize;
        await GFSManager.UnityServiceInitialize(100);
#if UNITY_SERVER
        var data = MultiplayService.Instance.ServerConfig;
        _ip = data.IpAddress;
        _portNum = data.Port;

        var _endPoint = NetworkEndpoint.AnyIpv4.WithPort(_portNum);
        World serverWorld = ClientServerBootstrap.ServerWorld;
        using var query = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(_endPoint);


        await MultiplayService.Instance.ReadyServerForPlayersAsync();
        await CreateBackFillTicket();

        enabled = true;
    }

    public async Task CreateBackFillTicket()
    {

        BackfillTicketProperties payload = null;
        for (int i = 0; i <= 3; i++)
        {
            try
            {
                payload = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<BackfillTicketProperties>(false);
                Debug.Log("payload 생성 성공");


                if (payload != null && payload.MatchProperties.Teams.Count == 1 && payload.MatchProperties.Teams[0].PlayerIds.Count == 1)
                    break;
            }
            catch
            {
                Debug.LogError("Allocation 생성 실패");
            }

            if (i == 3)
                Application.Quit();
        }

        // Set the Match Properties. These properties can also be found in the Allocation Payload (cf Allocation Payload)
        // Set options for matchmaking
        var options = new CreateBackfillTicketOptions("DefaultQueue", _ip + ":" + _portNum, null, payload);
        // Create backfill ticket
        _ticketId = await MatchmakerService.Instance.CreateBackfillTicketAsync(options);
#endif
    }

    public async Task UpdateBackFill()
    {

        var backticket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(_ticketId);
        ClientServerBootstrap.ServerWorld.EntityManager.BroadcastMessage(backticket.Properties.MatchProperties.Players.Count.ToString());
        ClientServerBootstrap.ServerWorld.EntityManager.BroadcastMessage("링크id : " + _linkedId.Count + "   approve : " + _waitForApprove.Count + "    connect : " + _waitForConnect.Count);
        var ids = backticket.Properties.MatchProperties.Teams[0].PlayerIds;

        string tempId;
        bool needUpdate = false;
        //연결이 끊어진 클라이언트 정보 정리
        for (int i = 0; i < ids.Count; i++)
        {
            tempId = ids[i];
            if (_linkedId.Contains(tempId)) continue;
            needUpdate = true;
            if (_kickList.TryGetValue(tempId, out float expire))
            {
                if (expire >= Time.unscaledTime)
                {
                    ExitId(tempId, backticket);
                    continue;
                }
                else
                    _kickList.Remove(tempId);
            }

            if (_waitForApprove.ContainsKey(tempId))
            {
                _linkedId.Add(tempId);
                _waitForApprove.Remove(tempId);
                continue;
            }

            if (!_waitForConnect.TryGetValue(tempId, out float expire2))
            {
                ClientServerBootstrap.ServerWorld.EntityManager.BroadcastMessage(tempId);
                _waitForConnect.Add(tempId, Time.unscaledTime + _waitTime);
                continue;
            }

            if (expire2 >= Time.unscaledTime) continue;

            _waitForConnect.Remove(tempId);
            ExitId(tempId, backticket);
            _kickList.Add(tempId, Time.unscaledTime + _waitTime);
        }

        bool isExit = false;
        //연결이 끊어진 클라이언트 BackfillTicket에 반영
        while (_playerExitBuffer.Count > 0)
        {
            needUpdate = true;
            isExit = true;
            string exitId = _playerExitBuffer.Dequeue();
            ExitId(exitId, backticket);
            _linkedId.Remove(exitId);
        }

        if (isExit && _waitForApprove.Count + _waitForConnect.Count + _linkedId.Count == 0)
        {
            enabled = false;
            DeleteBackFillTicket();
            needUpdate = false;
        }

        if (!needUpdate) return;
        await MatchmakerService.Instance.UpdateBackfillTicketAsync(_ticketId, backticket);
    }
    void ExitId(string tempId, in BackfillTicket backticket)
    {
        backticket.Properties.MatchProperties.Teams[0].PlayerIds.Remove(tempId);
        var player = backticket.Properties.MatchProperties.Players.Find((p) => p.Id == tempId);
        backticket.Properties.MatchProperties.Players.Remove(player);
    }

    public bool TicketEnter(string playerId)
    {
        ClientServerBootstrap.ServerWorld.EntityManager.BroadcastMessage(playerId);
        if (_kickList.TryGetValue(playerId, out float expire))
        {
            if (expire >= Time.unscaledTime) return false;

            _kickList.Remove(playerId);
        }

        if (_waitForConnect.ContainsKey(playerId))
        {
            _waitForConnect.Remove(playerId);
            _linkedId.Add(playerId);
        }
        else
            _waitForApprove.Add(playerId, Time.unscaledTime + _waitTime);

        return true;
    }
    public void TicketExit(string playerId)
    {
        _playerExitBuffer.Enqueue(playerId);

        if (_waitForApprove.Count + _waitForConnect.Count + _linkedId.Count < _serverSize)
            enabled = true;
    }

    public async void DeleteBackFillTicket()
    {
#if UNITY_SERVER
        await MultiplayService.Instance.UnreadyServerAsync();
        await MatchmakerService.Instance.DeleteBackfillTicketAsync(_ticketId);
        Application.Quit();
#endif
    }

    void ClearKickList()
    {
        List<string> removeBuffer = new List<string>();
        foreach (var item in _kickList)
        {
            if (item.Value >= Time.unscaledTime) continue;

            removeBuffer.Add(item.Key);
        }

        for (int i = 0; i < removeBuffer.Count; i++)
            _kickList.Remove(removeBuffer[i]);
    }
}
