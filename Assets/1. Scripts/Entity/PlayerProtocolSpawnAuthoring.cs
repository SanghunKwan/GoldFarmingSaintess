using Unity.Entities;
using UnityEngine;


public struct PlayerProtocolSpawn : IComponentData
{
    public Entity prefab;
    public Entity prefabTimer;
}


public class PlayerProtocolSpawnAuthoring : MonoBehaviour
{

    public GameObject _prefab;
    public GameObject _prefabTimer;

    class Baker : Baker<PlayerProtocolSpawnAuthoring>
    {
        public override void Bake(PlayerProtocolSpawnAuthoring authoring)
        {
            var data = new PlayerProtocolSpawn { prefab = GetEntity(authoring._prefab, TransformUsageFlags.None), prefabTimer = GetEntity(authoring._prefabTimer, TransformUsageFlags.None) };

            AddComponent(GetEntity(TransformUsageFlags.None), data);
        }
    }
}
