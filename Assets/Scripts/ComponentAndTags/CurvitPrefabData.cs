using Unity.Entities;
using UnityEngine;

public struct CurvitPrefabData : IComponentData
{
    public Entity NodeEntityPrefab;
    public Entity WayEntityPrefab;
    public Entity LaneletEntityPrefab;

    public float NodeEntityScale;
}