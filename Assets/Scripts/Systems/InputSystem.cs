using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Curvit.Demos.DOTS_Move
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
    [UpdateBefore(typeof(OSMLoaderSystem))]
    public partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var inputEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(inputEntity, new InputData());
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<NodeData>()
                    .WithNone<BuildVisualTag, SelectedTag>()
                    .Build(state.EntityManager);

                var entityArray = query.ToEntityArray(Allocator.Temp);
                var ECB = new EntityCommandBuffer(Allocator.Temp);

                for (int i = 0; i < entityArray.Length; i++)
                {
                    ECB.AddComponent<SelectedTag>(entityArray[i]);
                }

                ECB.Playback(state.EntityManager);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<NodeData, SelectedTag>()
                    .Build(state.EntityManager);

                var entityArray = query.ToEntityArray(Allocator.Temp);
                var ECB = new EntityCommandBuffer(Allocator.Temp);

                for (int i = 0; i < entityArray.Length; i++)
                {
                    ECB.RemoveComponent<SelectedTag>(entityArray[i]);
                }

                ECB.Playback(state.EntityManager);
            }
            else
            {
                var vertical = Input.GetAxis("Vertical");
                var horizontal = Input.GetAxis("Horizontal");

                state.EntityManager.SetComponentData(
                    SystemAPI.GetSingletonEntity<InputData>(),
                    new InputData { InputDirection = new float2(horizontal, vertical) }
                );
            }
        }
    }
}
