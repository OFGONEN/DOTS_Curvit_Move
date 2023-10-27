using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct LaneletData : IComponentData
{
    public uint ID;
    public Entity LeftWay;
    public Entity RightWay;
}
