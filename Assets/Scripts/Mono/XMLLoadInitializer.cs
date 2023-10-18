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
    private NativeArray<OSMLaneletData> LaneletOsmDataArray;
    private NativeList<int> WayOsmNodeRefDataList; // Node ID Reference List for Ways

    private XmlNode XMLNode_OSM;
    private XmlNodeList XMLNodeList_Node;
    private XmlNodeList XMLNodeList_Way;
    private XmlNodeList XMLNodeList_Lanelet;

    private void OnDisable()
    {
        NodeOsmDataArray.Dispose();
        WayOsmDataArray.Dispose();
        LaneletOsmDataArray.Dispose();
        WayOsmNodeRefDataList.Dispose();
    }

    [ContextMenu("Load XML")]
    private void InitializeXMLLoad()
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(OsmPath);
        XMLNode_OSM = xmlDocument.SelectSingleNode("osm");
        
        ReadOSMNodeData();
        ReadOSMWayData();
        ReadLaneletData();
        
        //Spawn Entity
        var OsmLoadComponent = new OSMLoadComponent
        {
            OSMNodeDataArray = this.NodeOsmDataArray,
            OSMWayDataArray = this.WayOsmDataArray,
            OSMLaneletDataArray = this.LaneletOsmDataArray,
            OSMWayNodeRefDataList = this.WayOsmNodeRefDataList
        };
        
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();

        entityManager.AddComponentData(entity, OsmLoadComponent);
    }

    private void ReadOSMNodeData()
    {
        XMLNodeList_Node = XMLNode_OSM.SelectNodes("node");
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
        XMLNodeList_Way = XMLNode_OSM.SelectNodes("way");
        
        WayOsmDataArray = new NativeArray<OSMWayData>(XMLNodeList_Way.Count, Allocator.Persistent);
        WayOsmNodeRefDataList = new NativeList<int>(XMLNodeList_Way.Count * 12, Allocator.Persistent);

        int nodeRefSliceCounter = 0;

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
                NodeRefSlice_Start = nodeRefSliceCounter,
                NodeRefSlice_End = WayOsmNodeRefDataList.Length - 1
            };

            nodeRefSliceCounter = WayOsmNodeRefDataList.Length;
        }
    }

    private void ReadLaneletData()
    {
        XMLNodeList_Lanelet = XMLNode_OSM.SelectNodes("relation");
        
        LaneletOsmDataArray = new NativeArray<OSMLaneletData>(XMLNodeList_Lanelet.Count, Allocator.Persistent);

        for (int i = 0; i < XMLNodeList_Lanelet.Count; i++)
        {
            var xmlLaneletNode = XMLNodeList_Lanelet[i];
            
            OSMLaneletData osmLaneletData= new OSMLaneletData();
            osmLaneletData.ID = int.Parse(xmlLaneletNode.Attributes["id"].Value);
            
            foreach (XmlNode tag in xmlLaneletNode.SelectNodes("tag"))
            {
                switch (tag.Attributes["k"].Value)
                {
                    case "subtype":
                        switch (tag.Attributes["v"].Value)
                        {
                            case "bicycle_lane":
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.TypeBicyclelane;
                                break;
                            case "crosswalk":
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.TypeCrosswalk;
                                break;
                            default:
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.TypeRoad;
                                break;
                        }
                        break;
                    case "turn_direction":
                        switch (tag.Attributes["v"].Value)
                        {
                            case "straight":
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.TurnStraight;
                                break;
                            case "left":
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.TurnLeft;
                                break;
                            case "right":
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.TurnRight;
                                break;
                        }
                        break;
                    case "reverse_line":
                        switch (tag.Attributes["v"].Value)
                        {
                            case "left":
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.ReverseLeft;
                                break;
                            case "right":
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.ReverseRight;
                                break;
                            default:
                                osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.ReverseNone;
                                break;
                        }
                        break;
                    case "speed_limit":
                        osmLaneletData.OsmLaneletDataFlag |= OSMLaneletDataFlag.SpeedLimitTrue;
                        osmLaneletData.SpeedLimit = int.Parse(tag.Attributes["v"].Value);
                        break;
                }
            }

            foreach (XmlNode member in xmlLaneletNode.SelectNodes("member"))
            {
                if (member.Attributes["type"].Value == "way")
                {
                    var wayID = int.Parse(member.Attributes["ref"].Value);

                    osmLaneletData.WayReference_Middle = -1;

                    switch (member.Attributes["role"].Value)
                    {
                        case "left":
                            osmLaneletData.WayReference_Left = wayID;
                            break;
                        case "right":
                            osmLaneletData.WayReference_Right = wayID;
                            break;
                        case "centerline":
                            osmLaneletData.WayReference_Middle = wayID;
                            break;
                    }
                }
            }

            LaneletOsmDataArray[i] = osmLaneletData;
        }
    }
}