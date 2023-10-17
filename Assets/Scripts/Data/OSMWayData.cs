using System;
using Unity.Collections;
using UnityEngine;

public struct OSMWayData
{
    public int Id;
    public OSMWayDataFlag OSMWayDataFlag;
    public NativeArray<OSMNodeData> OSMNodeDataArray;
}

[Flags]
public enum OSMWayDataFlag
{
    None,
    Solid,
    Dashed,
    BiDirectional
}