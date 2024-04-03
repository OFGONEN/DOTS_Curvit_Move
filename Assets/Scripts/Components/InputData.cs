using Unity.Entities;
using Unity.Mathematics;

namespace Curvit.Demos.DOTS_Move
{
    public struct InputData : IComponentData
    {
        public float2 InputDirection;
    }
}
