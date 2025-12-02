using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct PlayerTimer : IComponentData
{
    [GhostField] public double _startTime;
    [GhostField] public float _waitTime;
}

public class PlayerTimerAuthoring : MonoBehaviour
{
    public ProtocolType _type;

    class Baker : Baker<PlayerTimerAuthoring>
    {
        public double _startTime;
        public float _waitTime;


        public override void Bake(PlayerTimerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            var data = new PlayerTimer { _startTime = _startTime, _waitTime = _waitTime };

            AddComponent(entity, data);
        }
    }

}
