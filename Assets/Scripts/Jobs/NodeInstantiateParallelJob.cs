using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct NodeInstantiateParallelJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<OSMNodeData> OsmNodeDataArray;
    [ReadOnly] public Entity NodeEntityPrefab;
    public EntityCommandBuffer.ParallelWriter ECB;
    
    [BurstCompile]
    public void Execute(int index)
    {
        var entity = ECB.Instantiate(0, NodeEntityPrefab);
        var osmNodeData = OsmNodeDataArray[index];
        
        ECB.AddComponent<NodeComponent>(0, entity, new NodeComponent
        {
            Id = osmNodeData.Id,
            Position = osmNodeData.Position,
        });
        
        ECB.SetComponent(0, entity, new LocalTransform
        {
            Position = osmNodeData.Position,
            Rotation = quaternion.identity,
            Scale = 1
        });
    }
}