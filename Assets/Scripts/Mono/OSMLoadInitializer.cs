using System;
using System.Xml;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class OSMLoadInitializer : MonoBehaviour
{
    public string OSMFilePath;

    [ContextMenu("Initialize Loading OSM")]
    public void InitializeLoadingOSM()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();
        
        entityManager.AddComponentData<OSMLoadPathData>(entity, new OSMLoadPathData
        {
            OSMLoadPath = new FixedString512Bytes(OSMFilePath)
        });
    }
}