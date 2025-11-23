using Unity.Collections;
using Unity.NetCode;




namespace GFSUtilities.Protocol
{
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
    }

    public struct MatchingProtocol : IRpcCommand
    {
        public bool _isMatching;
    }
    public struct MatchingStatusProtocol : IRpcCommand
    {
        public int _currentMatchingCount;
    }
}