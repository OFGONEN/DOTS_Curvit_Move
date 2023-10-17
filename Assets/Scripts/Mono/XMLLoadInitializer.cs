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
    private NativeArray<OSMWayData> WayOsmDataArray;
    private NativeList<int> WayOsmNodeRefDataList;

    private XmlNodeList XMLNodeList_Node;
    private XmlNodeList XMLNodeList_Way;

    private void OnDisable()
    {
        NodeOsmDataArray.Dispose();
    }

    [ContextMenu("Load XML")]
    private void InitializeXMLLoad()
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(OsmPath);
            
        var xmlNodeOsm = xmlDocument.SelectSingleNode("osm");
        XMLNodeList_Node = xmlNodeOsm.SelectNodes("node");
        XMLNodeList_Way = xmlNodeOsm.SelectNodes("way");
        
        ReadOSMNodeData();
        ReadOSMWayData();
        
        //Spawn Entity
        var OsmLoadComponent = new OSMLoadComponent
        {
            OSMNodeDataArray = this.NodeOsmDataArray,
            OSMWayDataArray = this.WayOsmDataArray,
            OSMWayNodeRefDataList = this.WayOsmNodeRefDataList
        };
        
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();

        entityManager.AddComponentData(entity, OsmLoadComponent);
    }

    private void ReadOSMNodeData()
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

    private void ReadOSMWayData()
    {
        WayOsmDataArray = new NativeArray<OSMWayData>(XMLNodeList_Way.Count, Allocator.Persistent);
        WayOsmNodeRefDataList = new NativeList<int>(XMLNodeList_Way.Count * 12, Allocator.Persistent);

        int nodeRefSlicCounter = 0;

        for (int i = 0; i < XMLNodeList_Way.Count; i++)
        {
            XmlNode xmlNode_Way = XMLNodeList_Way[i];

            int wayID = int.Parse(xmlNode_Way.Attributes["id"].Value);

            var xmlNodeList_NodeRef = xmlNode_Way.SelectNodes("nd");

            for (int j = 0; j < xmlNodeList_NodeRef.Count; j++)
            {
                var nodeID = int.Parse(xmlNodeList_NodeRef[j].Attributes["ref"].Value);
                WayOsmNodeRefDataList.Add(nodeID);
            }

            OSMWayDataFlag osmWayDataFlag = OSMWayDataFlag.None;

            //Assign WayDataFlag Values
            foreach (XmlNode xmlTag in xmlNode_Way.SelectNodes("tag"))
            {
                switch (xmlTag.Attributes["k"].Value)
                {
                    case "subtype":
                        switch (xmlTag.Attributes["v"].Value)
                        {
                            case "solid":
                                osmWayDataFlag |= OSMWayDataFlag.Solid;
                                break;
                            case "dashed":
                                osmWayDataFlag |= OSMWayDataFlag.Dashed;
                                break;
                            default:
                                Debug.Log("Unsupported Way subtype value: " + xmlTag.Attributes["v"].Value);
                                break;
                        }
                        break; 
                    case "bidirectional":
                        switch (xmlTag.Attributes["v"].Value)
                        {
                            case "true":
                                osmWayDataFlag |= OSMWayDataFlag.Bidirectional;
                                break;
                            case "false":
                                break;
                            default:
                                Debug.Log("Unsupported Way bidirectional value: " + xmlTag.Attributes["v"].Value);
                                break;
                        }
                        break;
                }
            }
            
            WayOsmDataArray[i] = new OSMWayData
            {
                Id = wayID,
                OSMWayDataFlag = osmWayDataFlag,
                NodeRefSlice_Start = nodeRefSlicCounter,
                NodeRefSlice_End = WayOsmNodeRefDataList.Length - 1
            };

            nodeRefSlicCounter = WayOsmNodeRefDataList.Length;
        }
    }
}