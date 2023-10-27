using Unity.Collections;
using Unity.Entities;

public struct OSMLoadPathData : IComponentData
{
    public FixedString512Bytes OSMLoadPath;
}
