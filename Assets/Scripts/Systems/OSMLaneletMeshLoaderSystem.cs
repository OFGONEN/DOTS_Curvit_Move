using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[BurstCompile]
public partial struct OSMLaneletMeshLoaderSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OSMLoadComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var material = Resources.Load<Material>("Materials/MAT_Lanelet");
        var desc = new RenderMeshDescription(ShadowCastingMode.Off, false, MotionVectorGenerationMode.ForceNoMotion);
        var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
        
        var query = SystemAPI.QueryBuilder().WithAll<LaneletComponent>().Build();
        var entities = query.ToEntityArray(Allocator.Temp);

        var osmLoadComponent = SystemAPI.GetSingleton<OSMLoadComponent>();

        var FirstLanenetID = osmLoadComponent.OSMLaneletDataArray[0].ID;
        var FirstWayID = osmLoadComponent.OSMWayDataArray[0].Id;

        NativeList<float3> leftVertices = new NativeList<float3>(64,Allocator.TempJob);
        NativeList<float3> rightVertices = new NativeList<float3>(64,Allocator.TempJob);

        foreach (var entity in entities)
        {
            leftVertices.Clear();
            rightVertices.Clear();
            
            var laneletComponent = state.EntityManager.GetComponentData<LaneletComponent>(entity);
            
            var leftWay = osmLoadComponent.OSMWayDataArray[
                osmLoadComponent.OSMLaneletDataArray[laneletComponent.ID - FirstLanenetID].WayReference_Left - FirstWayID
            ];

            var rightWay = osmLoadComponent.OSMWayDataArray[
                osmLoadComponent.OSMLaneletDataArray[laneletComponent.ID - FirstLanenetID].WayReference_Right - FirstWayID
            ];
            
            for (int i = 0; i < leftWay.NodeRefCount; i++)
                leftVertices.Add(
                    osmLoadComponent.OSMNodeDataArray[osmLoadComponent.OSMWayNodeRefDataList[leftWay.NodeRefSlice_Start + i] - 1].Position
                );
            
            for (int i = 0; i < rightWay.NodeRefCount; i++)
                rightVertices.Add(
                    osmLoadComponent.OSMNodeDataArray[osmLoadComponent.OSMWayNodeRefDataList[rightWay.NodeRefSlice_Start + i] - 1].Position
                );

            var mesh = MeshExtensions.BuildMeshForLanelet(leftVertices, rightVertices);
            var meshArray = new RenderMeshArray(new[] { material }, new[] { mesh });
            RenderMeshUtility.AddComponents(entity, state.EntityManager, desc, meshArray, materialMeshInfo);
        }
    }
}
