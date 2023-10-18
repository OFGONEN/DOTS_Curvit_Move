using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public struct LaneletInstantiateParallelJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<OSMNodeData> OsmNodeDataArray;
    [ReadOnly] public NativeArray<OSMWayData> OsmWayDataArray;
    [ReadOnly] public NativeArray<OSMLaneletData> OsmLaneletDataArray;
    [ReadOnly] public NativeList<int> OsmWayNodeRefDataList;
    [ReadOnly] public Entity LaneletEntityPrefab;
    [ReadOnly] public int sortKey;
    [ReadOnly] public int FirstWayId;
    
    public EntityCommandBuffer.ParallelWriter ECB;
    
    [BurstCompile]
    public void Execute(int index)
    {
        var entity = ECB.Instantiate(sortKey, LaneletEntityPrefab);
        var laneletData = OsmLaneletDataArray[index];
        
        ECB.AddComponent<LaneletComponent>(sortKey, entity, new LaneletComponent
        {
            ID = laneletData.ID,
            OsmLaneletDataFlag = laneletData.OsmLaneletDataFlag,
            SpeedLimit = laneletData.SpeedLimit,
            WayReference_Left = laneletData.WayReference_Left,
            WayReference_Right = laneletData.WayReference_Right,
            WayReference_Middle = laneletData.WayReference_Middle
        });
        
        //Cache Way Data
        var leftWay = OsmWayDataArray[laneletData.WayReference_Left - FirstWayId];
        var rightWay = OsmWayDataArray[laneletData.WayReference_Right - FirstWayId];

        //Cache Node Datas
        NativeArray<OSMNodeData> leftNodeDataArray = new NativeArray<OSMNodeData>(leftWay.NodeRefCount, Allocator.Temp);
        NativeArray<OSMNodeData> rightNodeDataArray = new NativeArray<OSMNodeData>(rightWay.NodeRefCount, Allocator.Temp);
        
        for (int i = 0; i < leftNodeDataArray.Length; i++)
            leftNodeDataArray[i] = OsmNodeDataArray[OsmWayNodeRefDataList[leftWay.NodeRefSlice_Start + i] - 1];
        
        for (int i = 0; i < rightNodeDataArray.Length; i++)
            rightNodeDataArray[i] = OsmNodeDataArray[OsmWayNodeRefDataList[rightWay.NodeRefSlice_Start + i] - 1];
        
        //Create Mesh
        
        //Assign Mesh
    }
}