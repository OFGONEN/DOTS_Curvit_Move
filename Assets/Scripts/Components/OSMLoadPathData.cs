using Unity.Collections;
using Unity.Entities;

namespace Curvit.Demos.DOTS_Move
{
    public struct OSMLoadPathData : IComponentData
    {
        public FixedString512Bytes OSMLoadPath;
    }
}
