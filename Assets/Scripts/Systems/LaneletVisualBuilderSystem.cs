using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Curvit.Demos.DOTS_Move
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(WayVisualBuilderSystem))]
    public partial struct LaneletVisualBuilderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildVisualTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ECB = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            var HybridRenderer = state.World.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();

            foreach (var (laneletData, materialMeshInfo, laneletEntity) in
                     SystemAPI
                         .Query<RefRO<LaneletData>, RefRW<MaterialMeshInfo>>()
                         .WithAll<BuildVisualTag>().WithEntityAccess())
            {
                var leftWayNodeDataBuffer = SystemAPI.GetBuffer<NodeReferenceBufferData>(laneletData.ValueRO.LeftWay);
                var rightWayNodeDataBuffer = SystemAPI.GetBuffer<NodeReferenceBufferData>(laneletData.ValueRO.RightWay);

                NativeList<float3> leftWayPositions =
                    new NativeList<float3>(leftWayNodeDataBuffer.Length, Allocator.Temp);
                NativeList<float3> rightWayPositions =
                    new NativeList<float3>(rightWayNodeDataBuffer.Length, Allocator.Temp);

                for (int i = 0; i < leftWayNodeDataBuffer.Length; i++)
                    leftWayPositions.Add(SystemAPI.GetComponentRO<NodeData>(leftWayNodeDataBuffer[i].NodeEntity).ValueRO
                        .Position);

                for (int i = 0; i < rightWayNodeDataBuffer.Length; i++)
                    rightWayPositions.Add(SystemAPI.GetComponentRO<NodeData>(rightWayNodeDataBuffer[i].NodeEntity).ValueRO
                        .Position);

                var mesh = MeshExtensions.BuildMeshForLanelet(leftWayPositions, rightWayPositions);
                var meshBatchID = HybridRenderer.RegisterMesh(mesh);
                var meshReference = new LaneletMeshReference(mesh, meshBatchID);

                materialMeshInfo.ValueRW.MeshID = meshBatchID;

                StaticResources.LaneletToMeshDictionary.Add(laneletData.ValueRO.ID, meshReference);

                ECB.RemoveComponent<BuildVisualTag>(laneletEntity);
            }
        }
    }
}
