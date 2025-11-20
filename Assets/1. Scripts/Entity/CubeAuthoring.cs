using Unity.Entities;
using UnityEngine;

public struct Cube : IComponentData
{

}

public class CubeAuthoring : MonoBehaviour
{


    class Baker : Baker<CubeAuthoring>
    {
        public override void Bake(CubeAuthoring authoring)
        {
            var data = new Cube();

            AddComponent(GetEntity(TransformUsageFlags.Renderable), data);
        }
    }


}
