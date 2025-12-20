using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


[BurstCompile]
public struct DisturbCounter : IComponentData
{
    [GhostField] public int _monsterSpawnCount;
    [GhostField] public int _monsterBuffCount;
    [GhostField] public int _heroHurtCount;
}

public class DisturbCounterAuthoring : MonoBehaviour
{
    class Baker : Baker<DisturbCounterAuthoring>
    {
        public int _monsterSpawnCount;
        public int _monsterBuffCount;
        public int _heroHurtCount;


        public override void Bake(DisturbCounterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var data = new DisturbCounter
            {
                _monsterSpawnCount = _monsterSpawnCount,
                _heroHurtCount = _heroHurtCount,
                _monsterBuffCount = _monsterBuffCount
            };

            AddComponent(entity, data);
        }
    }

}
