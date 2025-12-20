using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct PlayerTimer : IComponentData
{
    [GhostField] public float _decreasingTime;
}

public class PlayerTimerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerTimerAuthoring>
    {
        public float _decreasingTime;


        public override void Bake(PlayerTimerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var data = new PlayerTimer { _decreasingTime = _decreasingTime };

            AddComponent(entity, data);
        }
    }

}
