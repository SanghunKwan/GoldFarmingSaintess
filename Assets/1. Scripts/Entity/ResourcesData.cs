using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


namespace GFSUtilities.ResourcesData
{

    public struct InitializedClient : ICleanupComponentData
    {

    }

    public struct UserSettingData : IComponentData
    {
        public Entity _user;
        public FixedString64Bytes _nickName;
    }

    public struct RoomFull : IComponentData
    {
        //public bool _isRoomFull;
    }

    public struct ClientIdentifyData : IComponentData
    {
        public int _beforeIndex;
        public int _currentIndex;
    }

    public struct HostPlayerProtocolSpawnQueue : IComponentData
    {
        public int _beforeIndex;
        public int _currentIndex;
        public Entity _requestTarget;
    }

    public struct HostPhaseEnd : IComponentData
    {
        public GamePhaseType _phase;
    }
    public enum GamePhaseType
    {
        Turn = 0,
        Event,
        Select,
        Place,
        Battle,
        Settlement,
        Bidding,
        BidCalculate,

        Max
    }

    public struct HostNeedGhostPrefab : IComponentData
    {
        public PrefabGhostType _type;
    }
    public enum PrefabGhostType
    {
        Timer = 0,
        DisturbCounter,
    }

    public struct ClientPhaseEnd : IComponentData
    {
        public GamePhaseType _currentPhase;
    }

    public struct ClientsIdentifyingData : IBufferElementData
    {
        public int _beforeIndex;
    }

    public struct ClientVoteIndex : IComponentData
    {
        public int _inclusiveBottom;
        public int _exclusiveTop;
    }

    public struct VoteWait : IComponentData
    {
    }

    public struct GameComplete : IComponentData
    {
    }

    [GhostComponent()]
    public struct GoldInputData : IInputComponentData
    {
        public int gold;
        public EmotionType emotionType;
        public int voteTarget;
    }
    public enum EmotionType
    {
        None = 0,

        Annoying,
        Anger,
        Bidding,
        BidSubmit,
        
        Smile = 5,
        Laugh,
        Expressionless,
        Sour,
        Cry,
        Sob,
    }

    public struct AnonymousVoteBuffer : IBufferElementData, IComparable<AnonymousVoteBuffer>
    {
        public int _beforeIndex;
        public int _value;


        public int CompareTo(AnonymousVoteBuffer other)
        {
            return _value.CompareTo(other._value);
        }
    }

    public struct RaceResultBuffer : IBufferElementData, IComparable<RaceResultBuffer>
    {
        public int _beforeIndex;
        public int _gold;


        public int CompareTo(RaceResultBuffer other)
        {
            return -_gold.CompareTo(other._gold);
        }
    }
    public struct MatchedEntityBuffer : IBufferElementData
    {
        public Entity _matchedConnection;
    }
    public struct MatchedGroupIndex : ISharedComponentData
    {
        public uint _groupIndex;
    }
    public struct MatchedBufferData : IComponentData
    {
        public Entity _bufferEntity;
    }
    public struct DisconnectCleanUp : ICleanupComponentData
    {
        public uint _bufferEntityShared;
    }

}
