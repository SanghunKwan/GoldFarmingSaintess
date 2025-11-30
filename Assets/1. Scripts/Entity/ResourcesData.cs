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
    public struct ClientsIdentifyingData : IBufferElementData
    {
        public int _beforeIndex;
    }
    [GhostComponent()]
    public struct GoldInputData : IInputComponentData
    {
        public int gold;
    }
}
