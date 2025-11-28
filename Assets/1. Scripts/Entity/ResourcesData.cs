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


    [GhostComponent()]
    public struct GoldInputData : IInputComponentData
    {
        public int gold;
    }
}
