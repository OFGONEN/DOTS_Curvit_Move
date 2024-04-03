using Unity.Entities;
using UnityEngine;

namespace Curvit.Demos.DOTS_Move
{
    public struct CurvitPrefabProperties : IComponentData
    {
        public Entity NodeEntityPrefab;
        public Entity WayEntityPrefab;
        public Entity LaneletEntityPrefab;

        public float NodeEntityScale;
    }
}
