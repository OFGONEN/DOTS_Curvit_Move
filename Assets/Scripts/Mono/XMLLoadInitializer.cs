using System;
using System.Xml;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class XMLLoadInitializer : MonoBehaviour
{
    public string OsmPath;
    
    private NativeArray<OSMNodeData> NodeOsmDataArray;

    private XmlNodeList XMLNodeList_Node;

    private void OnDisable()
    {
        NodeOsmDataArray.Dispose();
    }

    [ContextMenu("Load XML")]
    public void InitializeXMLLoad()
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(OsmPath);
            
        var xmlNodeOsm = xmlDocument.SelectSingleNode("osm");
        XMLNodeList_Node = xmlNodeOsm.SelectNodes("node");
        
        ReadOSMNodeData();
        
        //Spawn Entity
        var OsmLoadComponent = new OSMLoadComponent
        {
            OSMNodeDataArray = this.NodeOsmDataArray
        };
        
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();

        entityManager.AddComponentData(entity, OsmLoadComponent);
    }

    public void ReadOSMNodeData()
    {
        NodeOsmDataArray = new NativeArray<OSMNodeData>(XMLNodeList_Node.Count, Allocator.Persistent);

        for (int i = 0; i < XMLNodeList_Node.Count; i++)
        {
            var nodeXML = XMLNodeList_Node[i];
            var id = nodeXML.Attributes["id"].Value;
        
            float3 position = new float3();
        
            foreach (XmlNode tag in nodeXML.SelectNodes("tag"))
            {
                switch (tag.Attributes["k"].Value)
                {
                    case "ele":
                        position.y = float.Parse(tag.Attributes["v"].Value);
                        break;
                    case "local_x":
                        position.x = float.Parse(tag.Attributes["v"].Value);
                        break;
                    case "local_y":
                        position.z = float.Parse(tag.Attributes["v"].Value);
                        break;
                }
            }
        
            NodeOsmDataArray[i] = new OSMNodeData
            {
                Id = int.Parse(id),
                Position = position
            };    
        }
    }
}