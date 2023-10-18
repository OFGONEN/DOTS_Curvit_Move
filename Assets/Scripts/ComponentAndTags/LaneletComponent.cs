using Unity.Entities;
using UnityEngine;

public struct LaneletComponent : IComponentData
{
    public int ID;
    public OSMLaneletDataFlag OsmLaneletDataFlag;
    public int SpeedLimit;
    public int WayReference_Left;
    public int WayReference_Right;
    public int WayReference_Middle;
}