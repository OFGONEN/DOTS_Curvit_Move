using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct OSMLoadComponent : IComponentData
{
    public NativeArray<OSMNodeData> OSMNodeDataArray;
}