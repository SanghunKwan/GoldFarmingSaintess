using Unity.Entities;
using UnityEngine;

public struct CubeSpawn : IComponentData
{
    public Entity _prefabEntity;
}

public class CubeSpawnAuthoring : MonoBehaviour
{
    public GameObject _prefab;
    class Baker : Baker<CubeSpawnAuthoring>
    {
        public override void Bake(CubeSpawnAuthoring authoring)
        {
            var entity = GetEntity(authoring._prefab, TransformUsageFlags.Renderable);

            var data = new CubeSpawn
            {
                _prefabEntity = entity
            };

            AddComponent(GetEntity(TransformUsageFlags.None), data);
            Debug.Log("½ÇÇàµÊ");
        }
    }
}
