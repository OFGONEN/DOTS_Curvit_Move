using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct MoveSelectedNodeJob : IJobEntity
{
    [ReadOnly]
    public float3 MoveDelta;
    void Execute(ref NodeData nodeData, ref LocalTransform localTransform)
    {
        nodeData.Position += MoveDelta;
        localTransform.Position = nodeData.Position;
    }
}