using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(OSMLoaderSystem))]
[BurstCompile]
public partial struct OSMWayLineLoaderSystem : ISystem
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
        var osmLoadComponent = SystemAPI.GetSingleton<OSMLoadComponent>();
        
        var LinePrefab = Resources.Load<LineRenderer>("Prefabs/Line");
        var mat_line_arrow_bidirectional_dashed = Resources.Load<Material>("Materials/Mat_Line_Arrow_Bidirectional_Dashed");
        var mat_line_arrow_bidirectional_solid = Resources.Load<Material>("Materials/Mat_Line_Arrow_Bidirectional_Solid");
        var mat_line_arrow_common_dashed = Resources.Load<Material>("Materials/Mat_Line_Arrow_Common_Dashed");
        var mat_line_arrow_common_solid = Resources.Load<Material>("Materials/Mat_Line_Arrow_Common_Solid");

        for (int i = 0; i < osmLoadComponent.OSMWayDataArray.Length; i++)
        {
            var wayData = osmLoadComponent.OSMWayDataArray[i];
            var LineRenderer = GameObject.Instantiate(LinePrefab);

            LineRenderer.positionCount = wayData.NodeRefCount;

            for (int j = 0; j < wayData.NodeRefCount; j++)
                LineRenderer.SetPosition(j, osmLoadComponent.OSMNodeDataArray[osmLoadComponent.OSMWayNodeRefDataList[wayData.NodeRefSlice_Start + j] - 1].Position);

            var isBidirectional =
                (wayData.OSMWayDataFlag & OSMWayDataFlag.Bidirectional) == OSMWayDataFlag.Bidirectional;

            var isDashed = (wayData.OSMWayDataFlag & OSMWayDataFlag.Dashed) == OSMWayDataFlag.Dashed;
            
            Color lineColor = isBidirectional
                ? Color.yellow
                : Color.white;

            LineRenderer.startColor = lineColor;
            LineRenderer.endColor = lineColor;

            if (isBidirectional)
            {
                if (isDashed)
                    LineRenderer.sharedMaterial = mat_line_arrow_bidirectional_dashed;
                else
                    LineRenderer.sharedMaterial = mat_line_arrow_bidirectional_solid;
            }
            else
            {
                if (isDashed)
                    LineRenderer.sharedMaterial = mat_line_arrow_common_dashed;
                else
                    LineRenderer.sharedMaterial = mat_line_arrow_common_solid; 
            }
        }
    }
}
