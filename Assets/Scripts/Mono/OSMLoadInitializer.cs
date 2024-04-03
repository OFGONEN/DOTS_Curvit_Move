using System;
using System.IO;
using System.Xml;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Curvit.Demos.DOTS_Move
{
    public class OSMLoadInitializer : MonoBehaviour
    {
        public string OSMFilePath;

        private void Start()
        {
            InitializeLoadingOSM();
        }

        [ContextMenu("Initialize Loading OSM")]
        public void InitializeLoadingOSM()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();
            var path = Path.Combine(Application.dataPath, "dots_move_test.osm");

            entityManager.AddComponentData<OSMLoadPathData>(entity, new OSMLoadPathData
            {
                OSMLoadPath = new FixedString512Bytes(path)
            });
        }
    }
}
