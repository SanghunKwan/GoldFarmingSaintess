using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[BurstCompile]
public struct PlayerProtocol : IComponentData
{
    [GhostField] public int _gold;
    [GhostField] public EmotionType _emotionType;
    [GhostField] public int _vote;
}



public class PlayerProtocolAuthoring : MonoBehaviour
{

    class Baker : Baker<PlayerProtocolAuthoring>
    {
        public override void Bake(PlayerProtocolAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var data = new PlayerProtocol { _gold = 100, _emotionType = EmotionType.None, _vote = 0 };

            AddComponent(entity, data);
            AddComponent<GoldInputData>(entity);
        }
    }

}
