using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Curvit.Demos.DOTS_Move
{
    public static class StaticResources
    {
        public static Dictionary<uint, LineRendererWayEntityReference> WayToLineRendererDictionary;
        public static Dictionary<uint, LaneletMeshReference> LaneletToMeshDictionary;
    }

    public struct LaneletMeshReference
    {
        public Mesh Mesh;
        public BatchMeshID MeshBatchID;

        public LaneletMeshReference(Mesh mesh, BatchMeshID batchID)
        {
            Mesh = mesh;
            MeshBatchID = batchID;
        }
    }
}
