using Unity.Entities;
using UnityEngine;

public struct UIData : IComponentData
{
    public Entity prefab;
}

public class UIDataAuthoring : MonoBehaviour
{
    public GameObject _prefab;

    class Baker : Baker<UIDataAuthoring>
    {
        public override void Bake(UIDataAuthoring authoring)
        {
            var data = new UIData { prefab = GetEntity(authoring._prefab, TransformUsageFlags.None) };

            AddComponent(GetEntity(TransformUsageFlags.None), data);

        }
    }
}
