using Unity.Entities;

namespace Curvit.Demos.DOTS_Move
{
    [InternalBufferCapacity(8)]
    public struct NodeReferenceBufferData : IBufferElementData
    {
        public Entity NodeEntity;
    }
}
