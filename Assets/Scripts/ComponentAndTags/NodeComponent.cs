using Unity.Entities;
using Unity.Mathematics;

public struct NodeComponent : IComponentData
{
    public int Id;
    public float3 Position;
}