using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;




namespace GFSUtilities.Protocol
{
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
    }
}