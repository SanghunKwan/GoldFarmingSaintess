using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




namespace GFSUtilities.Protocol
{
    #region RpcCommand
    [BurstCompile]
    public struct MessageRpcCommand : IRpcCommand
    {
        public FixedString64Bytes _text;
    }
    [BurstCompile]
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
    [BurstCompile]
    public struct UserSettingProtocol : IRpcCommand
    {
        public FixedString64Bytes _nickName;
    }
    [BurstCompile]
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
    [BurstCompile]
    public struct MatchingProtocol : IRpcCommand
    {
        public bool _isMatching;
    }
    [BurstCompile]
    public struct MatchingStatusProtocol : IRpcCommand
    {
        public int _currentMatchingCount;
        public float _time;
    }
    [BurstCompile]
    public struct GameStartProtocol : IRpcCommand
    {
        public FixedString512Bytes _nickNames;
        public int _index;
    }
    [BurstCompile]
    public struct HostLinkSuccess : IRpcCommand
    {
        public int _linkedIndex;
    }
    [BurstCompile]
    public struct HostClientIdentify : IRpcCommand
    {
        public int _beforeIndex;
        public int _currentIndex;
    }
    [BurstCompile]
    public struct HostSendGoIn : IRpcCommand
    {

    }
    [BurstCompile]
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
    [BurstCompile]
    public struct ClientGamePhase : IRpcCommand
    {
        public GamePhaseType _nextPhase;
    }
    [BurstCompile]
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

    [BurstCompile]
    public struct WaitOtherEndPhase : IRpcCommand
    {
        public GamePhaseType _currentPhase;
    }
    [BurstCompile]
    public struct ServerVoteIndex : IRpcCommand
    {
        public int _index;
    }
    [BurstCompile]
    public struct AnonymousVote : IRpcCommand
    {
        public int _value;
    }
    [BurstCompile]
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
    [BurstCompile]
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
    #endregion RpcCommand

    #region CommandData
    [BurstCompile]
    public struct PlayersUIDataSpawnCommand : ICommandData
    {



        public NetworkTick Tick { get; set; }
    }
    #endregion CommandData
}