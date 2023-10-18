using System;

public struct OSMWayData
{
    public int Id;
    public OSMWayDataFlag OSMWayDataFlag;
    public int NodeRefSlice_Start;
    public int NodeRefSlice_End;

    public int NodeRefCount => (NodeRefSlice_End - NodeRefSlice_Start) + 1;
}

[Flags]
public enum OSMWayDataFlag
{
    None,
    //Way Type
    Solid,
    Dashed,
    //Direction
    Bidirectional
}