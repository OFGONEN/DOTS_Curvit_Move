using System.Collections.Generic;
using System.Xml;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
public partial struct OSMLoaderSystem : ISystem
{
    public NativeHashMap<uint, Entity> nodeHashMap;
    public NativeHashMap<uint, Entity> wayHashMap;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OSMLoadPathData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var osmLoadPathData_Entity = SystemAPI.GetSingletonEntity<OSMLoadPathData>();
        var osmLoadPathData_Singleton = state.EntityManager.GetComponentData<OSMLoadPathData>(osmLoadPathData_Entity);
        
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(osmLoadPathData_Singleton.OSMLoadPath.ToString());
        
        var osmNode = xmlDocument.SelectSingleNode("osm");
        var nodeList = osmNode.SelectNodes("node");
        var wayList = osmNode.SelectNodes("way");
        var laneletList = osmNode.SelectNodes("relation");

        var ECB = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var CurvitPrefabProperties = SystemAPI.GetSingleton<CurvitPrefabProperties>();
        
        CreateNodeEntities(ECB, nodeList, CurvitPrefabProperties.NodeEntityPrefab, CurvitPrefabProperties.NodeEntityScale);
        CreateWayEntities(ECB, wayList, CurvitPrefabProperties.WayEntityPrefab);
        CreateLaneletEntities(ECB, laneletList, CurvitPrefabProperties.LaneletEntityPrefab);

        ECB.DestroyEntity(osmLoadPathData_Entity);
    }
    
    void CreateNodeEntities(EntityCommandBuffer ecb, XmlNodeList nodeList, Entity nodeEntityPrefab, float nodeScale)
    {
        nodeHashMap = new NativeHashMap<uint, Entity>(nodeList.Count, Allocator.Persistent);
        
        for (int i = 0; i < nodeList.Count; i++)
        {
            var nodeXML = nodeList[i];
            var id = uint.Parse(nodeXML.Attributes["id"].Value);
        
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

            var nodeEntity = ecb.Instantiate(nodeEntityPrefab);
            ecb.AddBuffer<WayReferenceBufferData>(nodeEntity);
            ecb.AddComponent(nodeEntity, new NodeData
            {
                ID = id,
                Position = position
            });
            
            ecb.SetComponent(nodeEntity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = nodeScale
            });
            
            nodeHashMap.Add(id, nodeEntity);
        }
    }
    
    void CreateWayEntities(EntityCommandBuffer ecb, XmlNodeList wayList, Entity wayEntityPrefab)
    {
        wayHashMap = new NativeHashMap<uint, Entity>(wayList.Count, Allocator.Persistent);
        StaticResources.WayToLineRendererDictionary =
            new Dictionary<uint, LineRendererWayEntityReference>(wayList.Count);

        for (int i = 0; i < wayList.Count; i++)
        {
            XmlNode xmlNode_Way = wayList[i];
            uint id = uint.Parse(xmlNode_Way.Attributes["id"].Value);
            var xmlNodeList_NodeRef = xmlNode_Way.SelectNodes("nd");

            var wayEntity = ecb.Instantiate(wayEntityPrefab);
            ecb.AddComponent<WayData>(wayEntity, new WayData
            {
                ID = id
            });

            ecb.AddBuffer<LaneletReferenceBufferData>(wayEntity);
            var nodeReferenceBuffer = ecb.AddBuffer<NodeReferenceBufferData>(wayEntity);
            nodeReferenceBuffer.Length = xmlNodeList_NodeRef.Count;

            for (int j = 0; j < xmlNodeList_NodeRef.Count; j++)
            {
                var nodeID = uint.Parse(xmlNodeList_NodeRef[j].Attributes["ref"].Value);
                Entity nodeEntity;
                nodeHashMap.TryGetValue(nodeID, out nodeEntity);

                nodeReferenceBuffer[j] = new NodeReferenceBufferData { NodeEntity = nodeEntity };
                ecb.AppendToBuffer(nodeEntity, new WayReferenceBufferData{ WayEntity = wayEntity });
            }

            ecb.AddComponent<BuildVisualTag>(wayEntity);
            wayHashMap.Add(id, wayEntity);
        }
    }

    void CreateLaneletEntities(EntityCommandBuffer ecb, XmlNodeList laneletList, Entity laneletEntityPrefab)
    {
        StaticResources.LaneletToMeshDictionary =
            new Dictionary<uint, LaneletMeshReference>(laneletList.Count);
        
        for (int i = 0; i < laneletList.Count; i++)
        {
            var xmlLaneletNode = laneletList[i];
            var id = uint.Parse(xmlLaneletNode.Attributes["id"].Value);

            var laneletEntity = ecb.Instantiate(laneletEntityPrefab);
            ecb.AddBuffer<LeftWayNodePositionData>(laneletEntity);
            ecb.AddBuffer<RightWayNodePositionData>(laneletEntity);
            
            var laneletData = new LaneletData();
            laneletData.ID = id;

            foreach (XmlNode member in xmlLaneletNode.SelectNodes("member"))
            {
                if (member.Attributes["type"].Value == "way")
                {
                    var wayID = uint.Parse(member.Attributes["ref"].Value);

                    switch (member.Attributes["role"].Value)
                    {
                        case "left":
                            Entity leftWay;
                            wayHashMap.TryGetValue(wayID, out leftWay);
                            laneletData.LeftWay = leftWay;
                            ecb.AppendToBuffer(leftWay, new LaneletReferenceBufferData{ LaneletEntity = laneletEntity });
                            break;
                        case "right":
                            Entity rightWay;
                            wayHashMap.TryGetValue(wayID, out rightWay);
                            laneletData.RightWay = rightWay;
                            ecb.AppendToBuffer(rightWay, new LaneletReferenceBufferData{ LaneletEntity = laneletEntity });
                            break;
                    }
                }
            }
            
            ecb.AddComponent(laneletEntity, laneletData);
            ecb.AddComponent<BuildVisualTag>(laneletEntity);
        }
    }
}