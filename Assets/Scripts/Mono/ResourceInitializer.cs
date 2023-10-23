using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public class ResourceInitializer : MonoBehaviour
{
    public float NodeEntityScale = 1;
    
    private void Awake()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var nodeEntityPrefab = CreateNodeEntityPrefab(entityManager);
        var wayEntityPrefab = CreateWayEntityPrefab(entityManager);
        var laneletEntityPrefab = CreateLaneletEntityPrefab(entityManager);

        var curvitPrefabData = entityManager.CreateEntity();
        entityManager.AddComponentData<CurvitPrefabData>(curvitPrefabData, new CurvitPrefabData
        {
            NodeEntityPrefab = nodeEntityPrefab,
            WayEntityPrefab = wayEntityPrefab,
            LaneletEntityPrefab = laneletEntityPrefab,
            NodeEntityScale = NodeEntityScale
        });
    }

    Entity CreateNodeEntityPrefab(EntityManager entityManager)
    {
        //Rendering
        var description = new RenderMeshDescription(ShadowCastingMode.Off, false, MotionVectorGenerationMode.ForceNoMotion);
        var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
        
        //Resource
        var mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        var material = Resources.Load<Material>("Materials/MAT_Node");
        
        var nodeEntityPrefab = entityManager.CreateEntity();

        RenderMeshUtility.AddComponents(
            nodeEntityPrefab,
            entityManager,
            in description,
            new RenderMeshArray(new[] { material }, new[] { mesh }),
            materialMeshInfo
        );

        entityManager.AddComponentData(nodeEntityPrefab, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = NodeEntityScale
            }
        );

        entityManager.AddComponent<Prefab>(nodeEntityPrefab);
        return nodeEntityPrefab;
    }

    Entity CreateWayEntityPrefab(EntityManager entityManager)
    {
        var wayEntityPrefab = entityManager.CreateEntity();

        entityManager.AddComponent<Prefab>(wayEntityPrefab);
        return wayEntityPrefab;
    }

    Entity CreateLaneletEntityPrefab(EntityManager entityManager)
    {
        //Rendering
        var description = new RenderMeshDescription(ShadowCastingMode.Off, false, MotionVectorGenerationMode.ForceNoMotion);
        var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
        
        //Resource
        var mesh = Resources.GetBuiltinResource<Mesh>("Plane.fbx");
        var material = Resources.Load<Material>("Materials/MAT_Lanelet");
        
        var laneletEntityPrefab = entityManager.CreateEntity();

        RenderMeshUtility.AddComponents(
            laneletEntityPrefab,
            entityManager,
            in description,
            new RenderMeshArray(new[] { material }, new[] { mesh }),
            materialMeshInfo
        );

        entityManager.AddComponentData(laneletEntityPrefab, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1
            }
        );

        entityManager.AddComponent<Prefab>(laneletEntityPrefab);
        return laneletEntityPrefab;
    }
}
