using Unity.Entities;
using UnityEngine;

public class OSMPrefabAuthoring : MonoBehaviour
{
    public GameObject OSMNodePrefab;
    
    class OSMPrefabBaker : Baker<OSMPrefabAuthoring>
    {
        public override void Bake(OSMPrefabAuthoring authoring)
        {
            //Prefab Entities
            var NodePrefabEntity = GetEntity(authoring.OSMNodePrefab, TransformUsageFlags.Dynamic);
            
            var osmPrefabPropertyEntity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(osmPrefabPropertyEntity, new OSMPrefabProperties
            {
                OSMNodePrefabEntity = NodePrefabEntity
            });
        }
    }
}