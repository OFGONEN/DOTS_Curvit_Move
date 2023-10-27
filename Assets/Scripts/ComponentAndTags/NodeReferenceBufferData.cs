using Unity.Entities;

[InternalBufferCapacity(8)]
public struct NodeReferenceBufferData : IBufferElementData
{
    public Entity NodeEntity;
}