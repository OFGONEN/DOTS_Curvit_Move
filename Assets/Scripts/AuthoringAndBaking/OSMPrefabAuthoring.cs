using Unity.Entities;
using UnityEngine;

public class OSMPrefabAuthoring : MonoBehaviour
{
    public GameObject OSMNodePrefab;
    public GameObject OSMLaneletPrefab;

    public float NodeSize;
    
    class OSMPrefabBaker : Baker<OSMPrefabAuthoring>
    {
        public override void Bake(OSMPrefabAuthoring authoring)
        {
            //Dependencies
            DependsOn(authoring.OSMNodePrefab);
            DependsOn(authoring.OSMLaneletPrefab);


            if (authoring.OSMNodePrefab == null || authoring.OSMLaneletPrefab == null)
            {
                Debug.Log("Prefab References Are NULL");
                return;
            }
            
            //Prefab Entities
            var NodePrefabEntity = GetEntity(authoring.OSMNodePrefab, TransformUsageFlags.Dynamic);
            var LaneletPrefabEntity = GetEntity(authoring.OSMLaneletPrefab, TransformUsageFlags.Renderable);
            
            //Instantiated Entities
            var osmPrefabPropertyEntity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(osmPrefabPropertyEntity, new OSMPrefabProperties
            {
                OSMNodePrefabEntity = NodePrefabEntity,
                OSMLaneletPrefabEntity = LaneletPrefabEntity,
                NodeSize = authoring.NodeSize
            });
        }
    }
}