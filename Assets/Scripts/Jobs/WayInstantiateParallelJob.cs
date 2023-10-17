using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Rendering;

[BurstCompile]
public struct WayInstantiateParallelJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<OSMWayData> OsmWayDataArray;
    [ReadOnly] public NativeArray<OSMNodeData> OsmNodeDataArray;
    [ReadOnly] public NativeList<int> OsmWayNodeRefDataList;
    [ReadOnly] public int sortKey;
    public EntityCommandBuffer.ParallelWriter ECB;
    
    [BurstCompile]
    public void Execute(int index)
    {
        var entity = ECB.CreateEntity(sortKey);
        var wayNodeData = OsmWayDataArray[index];
        
        ECB.AddComponent<WayComponent>(sortKey, entity, new WayComponent
        {
            ID = wayNodeData.Id,
            OsmWayDataFlag = wayNodeData.OSMWayDataFlag
        });

        var wayNodeRefCount = wayNodeData.NodeRefCount;
        var bufferComponent = ECB.AddBuffer<NodeReferenceBufferComponent>(sortKey, entity);
        bufferComponent.Length = wayNodeRefCount;
        
        for (int i = 0; i < wayNodeRefCount; i++)
        {
            bufferComponent[i] = new NodeReferenceBufferComponent
            {
                OSMNodeData = OsmNodeDataArray[OsmWayNodeRefDataList[wayNodeData.NodeRefSlice_Start + i]]
            };
        }
    }
}