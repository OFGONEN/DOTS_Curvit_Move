using Unity.Entities;
using Unity.Mathematics;

public struct NodeData : IComponentData
{
    public uint ID;
    public float3 Position;
}