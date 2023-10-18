using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct OSMPrefabProperties : IComponentData
{
    public Entity OSMNodePrefabEntity;
    // public Entity OSMLaneletPrefabEntity;
    public float NodeSize;
}