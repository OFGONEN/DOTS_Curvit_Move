using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct NodeMovementSystem : ISystem
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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var inputData = SystemAPI.GetSingleton<InputData>().InputDirection;
        var deltaTime = SystemAPI.Time.DeltaTime;

        var delta = new float3(inputData.x * deltaTime, 0, inputData.y * deltaTime);
        
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<NodeData, LocalTransform>()
            .WithAll<SelectedTag>()
            .Build(state.EntityManager);

        var nodeMoveJob = new MoveSelectedNodeJob
        {
            MoveDelta = delta
        }.ScheduleParallel(query, state.Dependency);

        state.Dependency = nodeMoveJob;
        state.Dependency.Complete();
    }
}
