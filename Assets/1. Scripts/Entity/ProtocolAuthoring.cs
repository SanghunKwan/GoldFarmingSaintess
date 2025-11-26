using Unity.Entities;
using UnityEngine;




public struct Protocol : IComponentData
{
    public Entity prefab;
}
public class ProtocolAuthoring : MonoBehaviour
{
    public GameObject _prefab;

    class Baker : Baker<ProtocolAuthoring>
    {
        public override void Bake(ProtocolAuthoring authoring)
        {
            var data = new Protocol { prefab = GetEntity(authoring._prefab, TransformUsageFlags.None) };

            AddComponent(GetEntity(TransformUsageFlags.None), data);

        }
    }
}
