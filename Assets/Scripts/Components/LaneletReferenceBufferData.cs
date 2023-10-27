using Unity.Entities;

[InternalBufferCapacity(8)]
public struct LaneletReferenceBufferData : IBufferElementData
{
    public Entity LaneletEntity;
}