using Unity.Entities;

namespace Curvit.Demos.DOTS_Move
{
    [InternalBufferCapacity(4)]
    public struct WayReferenceBufferData : IBufferElementData
    {
        public Entity WayEntity;
    }
}
