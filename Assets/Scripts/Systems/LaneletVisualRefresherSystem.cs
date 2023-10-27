using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(WayVisualRefresherSystem))]
public partial struct LaneletVisualRefresherSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SelectedTag>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var laneletData in
                 SystemAPI
                     .Query<RefRO<LaneletData>>()
                     .WithNone<BuildVisualTag>())
        {
            var leftWayNodeDataBuffer = SystemAPI.GetBuffer<NodeReferenceBufferData>(laneletData.ValueRO.LeftWay);
            var rightWayNodeDataBuffer = SystemAPI.GetBuffer<NodeReferenceBufferData>(laneletData.ValueRO.RightWay);
            
            NativeList<float3> leftWayPositions =
                new NativeList<float3>(leftWayNodeDataBuffer.Length, Allocator.Temp);
            NativeList<float3> rightWayPositions =
                new NativeList<float3>(rightWayNodeDataBuffer.Length, Allocator.Temp);

            for (int i = 0; i < leftWayNodeDataBuffer.Length; i++)
                leftWayPositions.Add(SystemAPI.GetComponentRO<NodeData>(leftWayNodeDataBuffer[i].NodeEntity).ValueRO
                    .Position);
            
            for (int i = 0; i < rightWayNodeDataBuffer.Length; i++)
                rightWayPositions.Add(SystemAPI.GetComponentRO<NodeData>(rightWayNodeDataBuffer[i].NodeEntity).ValueRO
                    .Position);

            StaticResources.LaneletToMeshDictionary.TryGetValue(laneletData.ValueRO.ID, out var laneletMeshReference);
            MeshExtensions.SetMeshVertices(leftWayPositions, rightWayPositions, laneletMeshReference.Mesh);
        }
    }
}