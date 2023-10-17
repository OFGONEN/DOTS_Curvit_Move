using Unity.Entities;
using UnityEngine;

public class OSMPrefabAuthoring : MonoBehaviour
{
    public GameObject OSMNodePrefab;
    
    class OSMPrefabBaker : Baker<OSMPrefabAuthoring>
    {
        public override void Bake(OSMPrefabAuthoring authoring)
        {
            //Dependencies
            DependsOn(authoring.OSMNodePrefab);
            
            
            if(authoring.OSMNodePrefab == null)
                return;
            
            //Prefab Entities
            var NodePrefabEntity = GetEntity(authoring.OSMNodePrefab, TransformUsageFlags.Dynamic);
            
            //Instantiated Entities
            var osmPrefabPropertyEntity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(osmPrefabPropertyEntity, new OSMPrefabProperties
            {
                OSMNodePrefabEntity = NodePrefabEntity
            });
        }
    }
}