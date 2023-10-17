using Unity.Entities;

public struct WayComponent : IComponentData
{
    public int ID;
    public OSMWayDataFlag OsmWayDataFlag;
}