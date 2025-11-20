using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct CubeSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CubeSpawn>();
    }
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var cube in SystemAPI.Query<CubeSpawn>())
        {
            for (int i = 0; i < 10; i++)
            {
                Entity entity = ecb.Instantiate(cube._prefabEntity);
                ecb.SetComponent(entity, new LocalTransform
                {
                    Position = new float3(i * 2, 0, 0),
                    Rotation = quaternion.identity,
                    Scale = 1
                });
            }
        }

        ecb.Playback(state.EntityManager);

    }

}

