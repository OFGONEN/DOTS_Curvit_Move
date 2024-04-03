using Unity.Entities;
using Unity.Mathematics;

namespace Curvit.Demos.DOTS_Move
{
    public struct NodeData : IComponentData
    {
        public uint ID;
        public float3 Position;
    }
}
