using Unity.Entities;
using UnityEngine;


public struct PlayerProtocolSpawn : IComponentData
{
    public Entity prefab;
}


public class PlayerProtocolSpawnAuthoring : MonoBehaviour
{

    public GameObject _prefab;

    class Baker : Baker<PlayerProtocolSpawnAuthoring>
    {
        public override void Bake(PlayerProtocolSpawnAuthoring authoring)
        {
            var data = new PlayerProtocolSpawn { prefab = GetEntity(authoring._prefab, TransformUsageFlags.None) };

            AddComponent(GetEntity(TransformUsageFlags.None), data);

        }
    }
}
