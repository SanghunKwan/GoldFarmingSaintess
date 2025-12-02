using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[BurstCompile]
public struct PlayerProtocol : IComponentData
{
    [GhostField] public int _type;
    [GhostField] public int _gold;
}

public enum ProtocolType
{
    None = 0,

}


public class PlayerProtocolAuthoring : MonoBehaviour
{
    public ProtocolType _type;

    class Baker : Baker<PlayerProtocolAuthoring>
    {
        public override void Bake(PlayerProtocolAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            var data = new PlayerProtocol { _type = (int)authoring._type, _gold = 100 };

            AddComponent(entity, data);
            AddComponent<GoldInputData>(entity);
        }
    }

}
