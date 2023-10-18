
using System;

public struct OSMLaneletData
{
    public int ID;
    public OSMLaneletDataFlag OsmLaneletDataFlag;
    public int SpeedLimit;
    //Fcking C#, Can't have a fixed sized array inside the struct
    public int WayReference_Left;
    public int WayReference_Right;
    public int WayReference_Middle;
}

[Flags]
public enum OSMLaneletDataFlag
{
    None,
    //Lanelet Type
    TypeRoad,
    TypeCrosswalk,
    TypeBicyclelane,
    //Turn Direction
    TurnStraight,
    TurnLeft,
    TurnRight,
    //Reverse
    ReverseNone,
    ReverseLeft,
    ReverseRight,
    SpeedLimitTrue
}