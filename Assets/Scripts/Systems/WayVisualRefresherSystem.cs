using Unity.Burst;
using Unity.Entities;

namespace Curvit.Demos.DOTS_Move
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(NodeMovementSystem))]
    public partial struct WayVisualRefresherSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectedTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (wayData, nodeReferenceBuffer) in SystemAPI
                         .Query<RefRO<WayData>, DynamicBuffer<NodeReferenceBufferData>>()
                         .WithNone<BuildVisualTag>())
            {
                StaticResources.WayToLineRendererDictionary.TryGetValue(wayData.ValueRO.ID, out var lineRenderer);

                for (int i = 0; i < nodeReferenceBuffer.Length; i++)
                    lineRenderer.SetPosition(i,
                        SystemAPI.GetComponentRO<NodeData>(nodeReferenceBuffer[i].NodeEntity).ValueRO.Position);
            }
        }
    }
}
