using Unity.Entities;

[InternalBufferCapacity(32)]
public struct NodeReferenceBufferComponent : IBufferElementData
{
    public OSMNodeData OSMNodeData;
}
