using Unity.Entities;
using UnityEngine;


public struct PlayerProtocol : IComponentData, IEnableableComponent
{
    public ProtocolType _type;
}

public enum ProtocolType
{
    None = 0,

}


public class PlayerProtocolAuthoring : MonoBehaviour
{
    public ProtocolType _type;

    class Baker : Baker<PlayerProtocolAuthoring>
    {
        public override void Bake(PlayerProtocolAuthoring authoring)
        {
            var data = new PlayerProtocol { _type = authoring._type };


            AddComponent(GetEntity(TransformUsageFlags.None), data);

        }
    }

}
