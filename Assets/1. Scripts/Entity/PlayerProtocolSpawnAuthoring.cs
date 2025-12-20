using Unity.Entities;
using UnityEngine;


public struct PlayerProtocolSpawn : IComponentData
{
    public Entity prefab;
    public Entity prefabTimer;
    public Entity prefabDisturbCounter;
}


public class PlayerProtocolSpawnAuthoring : MonoBehaviour
{

    public GameObject _prefab;
    public GameObject _prefabTimer;
    public GameObject _prefabDisturbCounter;

    class Baker : Baker<PlayerProtocolSpawnAuthoring>
    {
        public override void Bake(PlayerProtocolSpawnAuthoring authoring)
        {
            var data = new PlayerProtocolSpawn { prefab = GetEntity(authoring._prefab, TransformUsageFlags.None), prefabTimer = GetEntity(authoring._prefabTimer, TransformUsageFlags.None), prefabDisturbCounter = GetEntity(authoring._prefabDisturbCounter, TransformUsageFlags.None) };

            AddComponent(GetEntity(TransformUsageFlags.None), data);
        }
    }
}
