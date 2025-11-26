using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;




[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerDisconnectedSystem : ISystem
{
    EntityQuery _connections;

    NativeHashSet<Entity> _beforeConnections;
    NativeHashSet<Entity> _currentConnections;

    public void OnCreate(ref SystemState state)
    {
        _connections = state.GetEntityQuery(typeof(NetworkId), typeof(InitializedClient));
        _beforeConnections = new NativeHashSet<Entity>(0, Allocator.Persistent);
        _currentConnections = new NativeHashSet<Entity>(0, Allocator.Persistent);
    }


    public void OnUpdate(ref SystemState state)
    {
        using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        using var tempList = _connections.ToEntityArray(Allocator.Temp);
        foreach (var e in tempList)
            _currentConnections.Add(e);

        foreach (var connection in _beforeConnections)
        {
            if (!_currentConnections.Contains(connection))
            {
                var entity = state.EntityManager.CreateEntity(typeof(DisconnectedPlayer));
                commandBuffer.AddComponent(entity, new DisconnectedPlayer { disconnectedSource = connection });
            }
        }

        _beforeConnections.Clear();

        var tempSet = _beforeConnections;
        _beforeConnections = _currentConnections;
        _currentConnections = tempSet;
    }
    public void OnDestroy(ref SystemState state)
    {
        _beforeConnections.Clear();
        _beforeConnections.Dispose();

        _currentConnections.Clear();
        _currentConnections.Dispose();
    }
}
