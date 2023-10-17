using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
public partial struct OSMLoaderSystem : ISystem
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
        var ECBParalel = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var osmLoadComponent = SystemAPI.GetSingleton<OSMLoadComponent>();
        var osmLoadComponentEntity = SystemAPI.GetSingletonEntity<OSMLoadComponent>();
        
        var osmPrefabProperties = SystemAPI.GetSingleton<OSMPrefabProperties>();

        ECBParalel.DestroyEntity(0, osmLoadComponentEntity);
        
        var nodeInstantiateParallelJobHANDLE = new NodeInstantiateParallelJob
        {
            sortKey = 0,
            ECB = ECBParalel,
            NodeEntityPrefab = osmPrefabProperties.OSMNodePrefabEntity,
            OsmNodeDataArray = osmLoadComponent.OSMNodeDataArray
        }.Schedule(osmLoadComponent.OSMNodeDataArray.Length, osmLoadComponent.OSMNodeDataArray.Length / 4,
            state.Dependency);

        state.Dependency = nodeInstantiateParallelJobHANDLE;
    }
}