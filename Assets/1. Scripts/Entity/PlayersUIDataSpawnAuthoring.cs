using Unity.Entities;
using UnityEngine;

public struct PlayersUIDataSpawn : IComponentData
{
    public Entity prefab;
}

public class PlayersUIDataSpawnAuthoring : MonoBehaviour
{
    public GameObject _prefab;

    class Baker : Baker<PlayersUIDataSpawnAuthoring>
    {
        public override void Bake(PlayersUIDataSpawnAuthoring authoring)
        {
            var data = new PlayersUIDataSpawn { prefab = GetEntity(authoring._prefab, TransformUsageFlags.None) };

            AddComponent(GetEntity(TransformUsageFlags.None), data);

        }
    }
}
