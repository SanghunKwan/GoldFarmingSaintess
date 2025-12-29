using GFSUtilities.ResourcesData;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




namespace GFSUtilities.Protocol
{
    #region RpcCommand
    public struct MessageRpcCommand : IRpcCommand
    {
        public FixedString64Bytes _text;
    }
    public struct PageProtocol : IRpcCommand
    {
        public PageType _pageType;
        public int _id;
    }

    public enum PageType
    {
        Disconnect = 0,
        Login,
        Match,
    }

    public struct PlayerData : IRpcCommand
    {
        public FixedString64Bytes _playerId;
    }

    public struct UserSettingProtocol : IRpcCommand
    {
        public FixedString64Bytes _nickName;
    }
    public struct ErrorProtocol : IRpcCommand
    {
        public ErrorType _errorType;
    }
    public enum ErrorType
    {
        NickNameInvalid = 0,
        IdInvalid,
        RoomFull
    }
    public struct MatchingProtocol : IRpcCommand
    {
        public bool _isMatching;
    }
    public struct MatchingStatusProtocol : IRpcCommand
    {
        public int _matchingCount;
    }
    public struct SetHostProtocol : IRpcCommand
    {
        public int _matchingSize;
        public uint _groupIndex;
    }
    public struct GameStartProtocol : IRpcCommand
    {
        public FixedString64Bytes _nickNames;
        public FixedString64Bytes _joinCode;
        public int _index;
    }
    public struct HostLinkSuccess : IRpcCommand
    {
        public int _linkedIndex;
    }
    public struct HostClientIdentify : IRpcCommand
    {
        public int _beforeIndex;
        public int _currentIndex;
    }
    public struct HostSendGoIn : IRpcCommand
    {

    }
    public struct AllClientReady : IRpcCommand
    {
        public int playerCount;
        public int maxRound;
        public int defaultGold;

        public FixedList32Bytes<int> beforeIndexBuffer;

        public void CopyDynamicBuffer(in DynamicBuffer<ClientsIdentifyingData> buffer)
        {
            beforeIndexBuffer = new FixedList32Bytes<int>();

            for (int i = 0; i < buffer.Length; i++)
                beforeIndexBuffer.AddNoResize(buffer[i]._beforeIndex);
        }
    }
    public struct ClientGamePhase : IRpcCommand
    {
        public GamePhaseType _nextPhase;
    }

    public struct ClientDisturbRPC : IRpcCommand
    {
        public DisturbType _type;
        public int _intensity;
    }
    public enum DisturbType
    {
        MonsterSpawn,
        MonsterBuff,
        HeroHurt,

        Max
    }


    public struct WaitOtherEndPhase : IRpcCommand
    {
        public GamePhaseType _currentPhase;
    }

    public struct ServerVoteIndex : IRpcCommand
    {
        public int _index;
    }

    public struct AnonymousVote : IRpcCommand
    {
        public int _value;
    }

    public struct VoteResult : IRpcCommand
    {
        public FixedList32Bytes<AnonymousVoteBuffer> beforeIndexBuffer;

        public void CopyDynamicBuffer(in DynamicBuffer<AnonymousVoteBuffer> buffer)
        {
            beforeIndexBuffer = new FixedList32Bytes<AnonymousVoteBuffer>();

            for (int i = 0; i < buffer.Length; i++)
                beforeIndexBuffer.AddNoResize(buffer[i]);
        }
    }

    public struct GameResult : IRpcCommand
    {
        public FixedList32Bytes<RaceResultBuffer> beforeIndexBuffer;

        public void CopyDynamicBuffer(in DynamicBuffer<RaceResultBuffer> buffer)
        {
            beforeIndexBuffer = new FixedList32Bytes<RaceResultBuffer>();

            for (int i = 0; i < buffer.Length; i++)
                beforeIndexBuffer.AddNoResize(buffer[i]);
        }
    }

    public struct HostingReadyProtocol : IRpcCommand
    {
        public FixedString32Bytes _joinCode;
        public uint _groupIndex;
        public int _size;
    }
    public struct LinkPacket : IRpcCommand
    {

    }
    #endregion RpcCommand

    #region CommandData

    public struct PlayersUIDataSpawnCommand : ICommandData
    {



        public NetworkTick Tick { get; set; }
    }
    #endregion CommandData


}