using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


namespace GFSUtilities.ResourcesData
{
    [BurstCompile]
    public struct InitializedClient : IComponentData
    {

    }
    [BurstCompile]
    public struct DisconnectedPlayer : IComponentData
    {
        public Entity disconnectedSource;
    }
    [BurstCompile]
    public struct UserSettingData : IComponentData
    {
        public Entity _user;
        public FixedString64Bytes _nickName;
    }
    [BurstCompile]
    public struct MatchingConditionData : IComponentData
    {
        //public FixedString64Bytes _nickName;
    }
    [BurstCompile]
    public struct RoomFull : IComponentData
    {
        //public bool _isRoomFull;
    }
    [BurstCompile]
    public struct ClientIdentifyData : IComponentData
    {
        public int _beforeIndex;
        public int _currentIndex;
    }
    [BurstCompile]
    public struct HostPlayerProtocolSpawnQueue : IComponentData
    {
        public int _beforeIndex;
        public int _currentIndex;
        public Entity _requestTarget;
    }
    [BurstCompile]
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
    [BurstCompile]
    public struct HostNeedGhostPrefab : IComponentData
    {
        public PrefabGhostType _type;
    }
    public enum PrefabGhostType
    {
        Timer = 0,
        DisturbCounter,
    }
    [BurstCompile]
    public struct ClientPhaseEnd : IComponentData
    {
        public GamePhaseType _currentPhase;
    }
    [BurstCompile]
    public struct ClientsIdentifyingData : IBufferElementData
    {
        public int _beforeIndex;
    }
    [BurstCompile]
    public struct ClientVoteIndex : IComponentData
    {
        public int _inclusiveBottom;
        public int _exclusiveTop;
    }
    [BurstCompile]
    public struct VoteWait : IComponentData
    {
    }
    [BurstCompile]
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
        BidSubmit
    }
    [BurstCompile]
    public struct AnonymousVoteBuffer : IBufferElementData, IComparable<AnonymousVoteBuffer>
    {
        public int _beforeIndex;
        public int _value;


        public int CompareTo(AnonymousVoteBuffer other)
        {
            return _value.CompareTo(other._value);
        }
    }
    [BurstCompile]
    public struct RaceResultBuffer : IBufferElementData, IComparable<RaceResultBuffer>
    {
        public int _beforeIndex;
        public int _gold;


        public int CompareTo(RaceResultBuffer other)
        {
            return -_gold.CompareTo(other._gold);
        }
    }
}
