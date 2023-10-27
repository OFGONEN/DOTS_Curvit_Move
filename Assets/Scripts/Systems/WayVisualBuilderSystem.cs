using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct WayVisualBuilderSystem : ISystem
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
        var ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var lineRendererForWayPrefab = Resources.Load<LineRendererWayEntityReference>("Prefabs/WayLineRenderer");

        foreach (var (wayData, nodeReferenceBuffer, wayEntity) in SystemAPI
                     .Query<RefRO<WayData>, DynamicBuffer<NodeReferenceBufferData>>()
                     .WithAll<BuildVisualTag>().WithEntityAccess())
        {
            var lineRendererForWay = GameObject.Instantiate(lineRendererForWayPrefab);
            lineRendererForWay.SetPositionCount(nodeReferenceBuffer.Length);

            for (int i = 0; i < nodeReferenceBuffer.Length; i++)
                lineRendererForWay.SetPosition(i,
                    SystemAPI.GetComponent<NodeData>(nodeReferenceBuffer[i].NodeEntity).Position);

            LineRendererWayEntityReference.ReferenceDictionary.Add(wayData.ValueRO.ID, lineRendererForWay);
            ECB.RemoveComponent<BuildVisualTag>(wayEntity);
        }
    }
}