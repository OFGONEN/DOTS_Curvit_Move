using Unity.Entities;

[InternalBufferCapacity(4)]
public struct WayReferenceBufferData : IBufferElementData
{
    public Entity WayEntity;
}