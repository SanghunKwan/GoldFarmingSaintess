using GFSManagers;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Services.Core;
using Unity.Services.Matchmaker.Models;

using Unity.Services.Matchmaker;
using System.Linq;
using GFSUtilities;
using System.Collections;
using System;




#if UNITY_SERVER
using Unity.Services.Multiplay;
#endif
using UnityEngine;



public class ServerManager : MonoBehaviour
{
    string _ip;
    ushort _portNum;
    string _ticketId;
    List<Player> _players;
    BackfillTicket _ticket;
    Queue<Action> _playerChangeBuffer;


    private void OnEnable()
    {
        StartCoroutine(ApproveBackFillCoroutine());
    }
    IEnumerator ApproveBackFillCoroutine()
    {
        var wait = new WaitForSeconds(1);
        while (true)
        {
            ApproveBackFill();
            yield return wait;
        }
    }

    public async void InitManager()
    {
#if UNITY_SERVER
        _players = new List<Player>();
        _playerChangeBuffer = new Queue<Action>();

        await UnityServices.InitializeAsync();

        var data = MultiplayService.Instance.ServerConfig;
        _ip = data.IpAddress;
        _portNum = data.Port;

        var _endPoint = NetworkEndpoint.AnyIpv4.WithPort(_portNum);

        World serverWorld = ClientServerBootstrap.ServerWorld;
        using var query = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(_endPoint);
        await MultiplayService.Instance.ReadyServerForPlayersAsync();

        CreateBackFillTicket();
#endif
    }

    public async void CreateBackFillTicket()
    {
        // Set the Match Properties. These properties can also be found in the Allocation Payload (cf Allocation Payload)
        List<string> names = new List<string>(_players.Count);
        for (int i = 0; i < names.Count; i++)
            names[i] = _players[i].Id;


        var teams = new List<Team>
        {
                    new Team( "Saintess", "9c8e302e-9cf3-4ad6-a005-b2604e6851e3", names)
        };

        // Define the Players of the match with their data.
        var matchProperties = new MatchProperties(teams, _players);
        var backfillTicketProperties = new BackfillTicketProperties(matchProperties);
        // Set options for matchmaking
        var options = new CreateBackfillTicketOptions("DefaultQueue", _ip + ":" + _portNum, null, backfillTicketProperties);
        // Create backfill ticket
        _ticketId = await MatchmakerService.Instance.CreateBackfillTicketAsync
        (options);
        // Print the created ticket id
        Debug.Log(_ticketId);
    }



    public async void ApproveBackFill()
    {
        _ticket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(_ticketId);

        while (_playerChangeBuffer.Count > 0)
            _playerChangeBuffer.Dequeue()();

        await MatchmakerService.Instance.UpdateBackfillTicketAsync(_ticketId, _ticket);
    }

    public void TicketEnter(string playerId)
    {
        _playerChangeBuffer.Enqueue(() =>
        {
            _ticket.Properties.MatchProperties.Teams[0].PlayerIds.Add(playerId);
            _ticket.Properties.MatchProperties.Players.Add(new Player(playerId));
        });
    }
    public void TicketExit(string playerId, string ticketId)
    {
        _playerChangeBuffer.Enqueue(() =>
        {
            _ticket.Properties.MatchProperties.Teams[0].PlayerIds.Remove(playerId);
            var removePlayer = _ticket.Properties.MatchProperties.Players.FirstOrDefault(p => p.Id.Equals(playerId));
            _ticket.Properties.MatchProperties.Players.Remove(removePlayer);

            MatchmakerService.Instance.DeleteTicketAsync(ticketId);
        });
    }

    public async void DeleteBackFillTicket()
    {
        await MatchmakerService.Instance.DeleteBackfillTicketAsync(_ticketId);
    }
}
