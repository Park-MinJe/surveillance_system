using Microsoft.VisualBasic;
using OpenTK.Core.Native;
using OpenTK.Graphics.ES11;
using Silk.NET.Core.Native;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Xml;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        /** 230727 박민제
             * StreetMap 플러그인 'OSMFile' 참고
             * osm 파일로부터 도로, 건물, cctv 정보 읽어옴.
             */
        /** Types of ways */
        public enum EOSMWayType
        {
            ///
            /// ROADS
            ///

            /** A restricted access major divided highway, normally with 2 or more running lanes plus emergency hard shoulder. Equivalent to the Freeway, Autobahn, etc. */
            Motorway,

            /** The link roads (sliproads/ramps) leading to/from a motorway from/to a motorway or lower class highway. Normally with the same motorway restrictions. */
            Motorway_Link,

            /** The most important roads in a country's system that aren't motorways. (Need not necessarily be a divided highway.) */
            Trunk,

            /** The link roads (sliproads/ramps) leading to/from a trunk road from/to a trunk road or lower class highway. */
            Trunk_Link,

            /** The next most important roads in a country's system. (Often link larger towns.) */
            Primary,

            /** The link roads (sliproads/ramps) leading to/from a primary road from/to a primary road or lower class highway. */
            Primary_Link,

            /** The next most important roads in a country's system. (Often link smaller towns and villages.) */
            Secondary,

            /** The link roads (sliproads/ramps) leading to/from a secondary road from/to a secondary road or lower class highway. */
            Secondary_Link,

            /** The next most important roads in a country's system. */
            Tertiary,

            /** The link roads (sliproads/ramps) leading to/from a tertiary road from/to a tertiary road or lower class highway. */
            Tertiary_Link,

            /** Roads which are primarily lined with and serve as an access to housing. */
            Residential,

            /** For access roads to, or within an industrial estate, camp site, business park, car park etc. */
            Service,

            /** The least most important through roads in a country's system, i.e. minor roads of a lower classification than tertiary, but which serve a purpose other than access to properties. */
            Unclassified,


            ///
            /// NON-ROADS
            ///

            /** Residential streets where pedestrians have legal priority over cars, speeds are kept very low and where children are allowed to play on the street. */
            Living_Street,

            /** For roads used mainly/exclusively for pedestrians in shopping and some residential areas which may allow access by motorised vehicles only for very limited periods of the day. */
            Pedestrian,

            /** Roads for agricultural or forestry uses etc, often rough with unpaved/unsealed surfaces, that can be used only by off-road vehicles (4WD, tractors, ATVs, etc.) */
            Track,

            /** A busway where the vehicle guided by the way (though not a railway) and is not suitable for other traffic. */
            Bus_Guideway,

            /** A course or track for (motor) racing */
            Raceway,

            /** A road where the mapper is unable to ascertain the classification from the information available. */
            Road,


            ///
            /// PATHS
            ///

            /** For designated footpaths; i.e., mainly/exclusively for pedestrians. This includes walking tracks and gravel paths. */
            Footway,

            /** For designated cycleways. */
            Cycleway,

            /** Paths normally used by horses */
            Bridleway,

            /** For flights of steps (stairs) on footways. */
            Steps,

            /** A non-specific path. */
            Path,


            ///
            /// LIFECYCLE
            ///

            /** For planned roads, use with proposed=* and also proposed=* with a value of the proposed highway value. */
            Proposed,

            /** For roads under construction. */
            Construction,


            ///
            /// BUILDINGS
            ///

            /** Default type of building.  A general catch-all. */
            Building,


            ///
            /// Target Traces
            ///

            /** Trace log of Surveilance Target */
            TargetTrace,


            ///
            /// UNSUPPORTED
            /// 

            /** Currently unrecognized type */
            Other,
        };

        static public string[] EOSMWayTypeName =
        {
                ///
                /// ROADS
                ///

                /** A restricted access major divided highway, normally with 2 or more running lanes plus emergency hard shoulder. Equivalent to the Freeway, Autobahn, etc. */
                "Motorway",

                /** The link roads (sliproads/ramps) leading to/from a motorway from/to a motorway or lower class highway. Normally with the same motorway restrictions. */
                "Motorway_Link",

                /** The most important roads in a country's system that aren't motorways. (Need not necessarily be a divided highway.) */
                "Trunk",

                /** The link roads (sliproads/ramps) leading to/from a trunk road from/to a trunk road or lower class highway. */
                "Trunk_Link",

                /** The next most important roads in a country's system. (Often link larger towns.) */
                "Primary",

                /** The link roads (sliproads/ramps) leading to/from a primary road from/to a primary road or lower class highway. */
                "Primary_Link",

                /** The next most important roads in a country's system. (Often link smaller towns and villages.) */
                "Secondary",

                /** The link roads (sliproads/ramps) leading to/from a secondary road from/to a secondary road or lower class highway. */
                "Secondary_Link",

                /** The next most important roads in a country's system. */
                "Tertiary",

                /** The link roads (sliproads/ramps) leading to/from a tertiary road from/to a tertiary road or lower class highway. */
                "Tertiary_Link",

                /** Roads which are primarily lined with and serve as an access to housing. */
                "Residential",

                /** For access roads to, or within an industrial estate, camp site, business park, car park etc. */
                "Service",

                /** The least most important through roads in a country's system, i.e. minor roads of a lower classification than tertiary, but which serve a purpose other than access to properties. */
                "Unclassified",


                ///
                /// NON-ROADS
                ///

                /** Residential streets where pedestrians have legal priority over cars, speeds are kept very low and where children are allowed to play on the street. */
                "Living_Street",

                /** For roads used mainly/exclusively for pedestrians in shopping and some residential areas which may allow access by motorised vehicles only for very limited periods of the day. */
                "Pedestrian",

                /** Roads for agricultural or forestry uses etc, often rough with unpaved/unsealed surfaces, that can be used only by off-road vehicles (4WD, tractors, ATVs, etc.) */
                "Track",

                /** A busway where the vehicle guided by the way (though not a railway) and is not suitable for other traffic. */
                "Bus_Guideway",

                /** A course or track for (motor) racing */
                "Raceway",

                /** A road where the mapper is unable to ascertain the classification from the information available. */
                "Road",


                ///
                /// PATHS
                ///

                /** For designated footpaths; i.e., mainly/exclusively for pedestrians. This includes walking tracks and gravel paths. */
                "Footway",

                /** For designated cycleways. */
                "Cycleway",

                /** Paths normally used by horses */
                "Bridleway",

                /** For flights of steps (stairs) on footways. */
                "Steps",

                /** A non-specific path. */
                "Path",


                ///
                /// LIFECYCLE
                ///

                /** For planned roads, use with proposed=* and also proposed=* with a value of the proposed highway value. */
                "Proposed",

                /** For roads under construction. */
                "Construction",


                ///
                /// BUILDINGS
                ///

                /** Default type of building.  A general catch-all. */
                "Building",


                ///
                /// Target Traces
                ///

                /** Trace log of Surveilance Target */
                "TargetTrace",


                ///
                /// UNSUPPORTED
                /// 

                /** Currently unrecognized type */
                "Other",
            };

        /** 230704 박민제
        * osm 데이터의 감시자원 정보
        */
        public class FOSMSurvInfo
        {
            public double Direction { set; get; }
            public string MountType { set; get; }
            public string CameraType { set; get; }
            public int Height { set; get; }
            public string Loc { set; get; }
            public string SurvType { set; get; }
            public string SurvZone { set; get; }

            public override string ToString()
            {
                string rt = "camera:direction - " + Convert.ToString(Direction);
                rt += "\ncamera:mount - " + MountType;
                rt += "\ncamera:type - " + CameraType;
                rt += "\nheight - " + Convert.ToString(Height);
                rt += "\nsurveillance - " + Loc;
                rt += "\nsurveillance:type - " + SurvType;
                rt += "\nsurveillance:zone - " + SurvZone;

                return rt;
            }
        }

        public class FOSMTargetInfo
        {
            public string Type { set; get; }
            public double Width { set; get; }
            public double Length { set; get; }
            public double Height { set; get; }
        }

        public class FOSMWayRef
        {
            // Way that we're referencing at this node
            public FOSMWayInfo Way { set; get; }

            // Index of the node in the way's array of nodes
            public int NodeIndex { set; get; }

            public override string ToString()
            {
                string rt = "Way\n==========>\n" + Way.ToString();
                rt += "\nNodeIndex - " + Convert.ToString(NodeIndex);

                return rt;
            }
        }

        public class FOSMNodeInfo
        {
            public double Latitude { set; get; }
            public double Longitude { set; get; }
            public List<FOSMWayRef> WayRefs { set; get; }

            /** 230710 박민제
            * Node 하위의 tag 중 감시 자원에 대한 정보
            * SurvInfoExist: 감시 자원 태그의 존재 여부
            */
            public FOSMSurvInfo SurvInfo { set; get; }
            public bool SurvInfoExist { set; get; } = false;

            public FOSMNodeInfo()
            {
                WayRefs = new List<FOSMWayRef>();
            }

            public override string ToString()
            {
                string rt = "Latitude - " + Convert.ToString(Latitude);
                rt += "\nLongitude - " + Convert.ToString(Longitude);
                rt += "\nWayRefs Count - " + Convert.ToString(WayRefs.Count);
                if (SurvInfoExist)
                {
                    rt += "\nSurvInfo\n==========>\n";
                    rt += SurvInfo.ToString();
                }

                return rt;
            }
        }

        public class FOSMWayInfo
        {
            public string Name { set; get; }
            public string Ref { set; get; }
            public List<FOSMNodeInfo> Nodes { set; get; }
            public EOSMWayType WayType { set; get; }
            public double Height { set; get; }
            public int BuildingLevels { set; get; }

            // If true, way is only traversable in the order the nodes are listed in the Nodes list
            public bool bIsOneWay { set; get; } = false;

            // If way is about target trace log
            public FOSMTargetInfo TargetInfo { set; get; }
            public bool TargetInfoExist { set; get; } = false;

            public FOSMWayInfo()
            {
                Nodes = new List<FOSMNodeInfo>();
            }

            public override string ToString()
            {
                string rt = "Way Name - " + Name;
                rt += "\nWay Ref - " + Ref;
                //rt += "\nWay Nodes Count - " + Convert.ToString(Nodes.Count);
                rt += "\nWay Nodes\n==========>";
                for (int i = 0; i < Nodes.Count; i++)
                {
                    rt += "\n\n" + Nodes[i].ToString();
                }
                if (WayType == EOSMWayType.Building)
                {
                    rt += "\nHeight - " + Convert.ToString(Height);
                    rt += "\nBuilding Levels - " + Convert.ToString(BuildingLevels);
                }

                return rt;
            }
        }

        public class OsmReader
        {
            string fileName;
            XmlDocument xdoc;
            double minlat, minlon, maxlat, maxlon;
            XmlNodeList nodes;
            XmlNodeList ways;
            XmlNodeList relations;

            // Average Latitude (roughly the center of the map)
            public double AverageLatitude { set; get; } = 0.0;
            public double AverageLongitude { set; get; } = 0.0;

            // All ways we've parsed
            public List<FOSMWayInfo> Ways { set; get; } = new List<FOSMWayInfo>();

            // Maps node IDs to info about each node
            public Dictionary<long, FOSMNodeInfo> NodeMap { set; get; } = new Dictionary<long, FOSMNodeInfo>();

            // 230704 박민제 존재하는 감시 자원
            public Dictionary<long, FOSMSurvInfo> SurveillancesMap { set; get; } = new Dictionary<long, FOSMSurvInfo>();

            public void setOsmReader(string fileName)
            {
                this.fileName = fileName;
                xdoc = new XmlDocument();
                xdoc.Load(@fileName);

                XmlNode lat_lon = xdoc.SelectSingleNode("/osm/bounds");

                minlat = Convert.ToDouble(lat_lon.Attributes["minlat"].Value);
                minlon = Convert.ToDouble(lat_lon.Attributes["minlon"].Value);
                maxlat = Convert.ToDouble(lat_lon.Attributes["maxlat"].Value);
                maxlon = Convert.ToDouble(lat_lon.Attributes["maxlon"].Value);

                nodes = xdoc.SelectNodes("/osm/node");
                ways = xdoc.SelectNodes("/osm/way");
                relations = xdoc.SelectNodes("/osm/relation");

                // parse nodes from osm file
                for(int i = 0; i < nodes.Count; i++)
                {
                    XmlNode node = nodes.Item(i);
                    FOSMNodeInfo CurrentNode = new FOSMNodeInfo();
                    long CurrentNodeId = 0;

                    // 기본 설정: node에 감시 자원이 없다.
                    CurrentNode.SurvInfoExist = false;
                    // node id
                    if (node.Attributes["id"].Value != null)
                    {
                        CurrentNodeId = Convert.ToInt64(node.Attributes["id"].Value);
                    }
                    // node lat
                    if (node.Attributes["lat"].Value != null)
                    {
                        CurrentNode.Latitude = Convert.ToDouble(node.Attributes["lat"].Value.ToString());

                        AverageLatitude += CurrentNode.Latitude;

                        if(CurrentNode.Latitude < minlat) { minlat = CurrentNode.Latitude; }
                        if(CurrentNode.Latitude > maxlat) { maxlat = CurrentNode.Latitude; }
                    }
                    // node lon
                    if (node.Attributes["lon"].Value != null)
                    {
                        CurrentNode.Longitude = Convert.ToDouble(node.Attributes["lon"].Value.ToString());

                        AverageLongitude += CurrentNode.Longitude;

                        if (CurrentNode.Longitude < minlon) { minlon = CurrentNode.Longitude; }
                        if (CurrentNode.Longitude > maxlon) { maxlon = CurrentNode.Longitude; }
                    }

                    // Validate tags under current node
                    XmlNodeList tags = node.SelectNodes("tag");
                    // Is there surveillance at this node?
                    CurrentNode.SurvInfoExist = isSurvExist(tags);
                    if (CurrentNode.SurvInfoExist)
                    {
                        CurrentNode.SurvInfo = getSurveillance(tags);

                        SurveillancesMap.Add(CurrentNodeId, CurrentNode.SurvInfo);
                    }

                    NodeMap.Add(CurrentNodeId, CurrentNode);
                }

                AverageLatitude /= nodes.Count;
                AverageLongitude /= nodes.Count;

                // parse ways from osm file
                for(int i = 0; i < ways.Count; i++)
                {
                    FOSMWayInfo CurrentWayInfo = new FOSMWayInfo();

                    XmlNode way = ways.Item(i);
                    
                    // read way ref data
                    XmlNodeList nds = way.SelectNodes("nd");
                    for(int j = 0; j < nds.Count; j++)
                    {
                        XmlNode nd = nds.Item(j);
                        FOSMNodeInfo refNode = NodeMap[Convert.ToInt64(nd.Attributes["ref"].Value)];
                        int NewNodeIndex = Ways.Count;
                        CurrentWayInfo.Nodes.Add(refNode);

                        // Update the node with information about the way that is referencing it
                        FOSMWayRef NewWayRef = new FOSMWayRef();
                        NewWayRef.Way = CurrentWayInfo;
                        NewWayRef.NodeIndex = NewNodeIndex;
                        refNode.WayRefs.Add(NewWayRef);
                    }

                    // read way tag data
                    XmlNodeList tags = way.SelectNodes("tag");
                    for(int j = 0; j < tags.Count; j++)
                    {
                        XmlNode tag = tags.Item(j);
                        string tag_k = tag.Attributes["k"].Value;
                        string tag_v = tag.Attributes["v"].Value;
                        if (tag_k.Equals("name"))
                        {
                            if (tag_v != null) CurrentWayInfo.Name = tag_v;
                            else CurrentWayInfo.Name = "NotDefined";
                        }
                        else if (tag_k.Equals("ref"))
                        {
                            if (tag_v != null) CurrentWayInfo.Ref = tag_v;
                            else CurrentWayInfo.Ref = "NotDefined";
                        }
                        else if (tag_k.Equals("highway"))
                        {
                            CurrentWayInfo.WayType = EOSMWayType.Other;
                            if (tag_v.Equals("motorway")) CurrentWayInfo.WayType = EOSMWayType.Motorway;
                            else if (tag_v.Equals("motorway_link")) CurrentWayInfo.WayType = EOSMWayType.Motorway_Link;
                            else if (tag_v.Equals("trunk")) CurrentWayInfo.WayType = EOSMWayType.Trunk;
                            else if (tag_v.Equals("trunk_link")) CurrentWayInfo.WayType = EOSMWayType.Trunk_Link;
                            else if (tag_v.Equals("primary")) CurrentWayInfo.WayType = EOSMWayType.Primary;
                            else if (tag_v.Equals("primary_link")) CurrentWayInfo.WayType = EOSMWayType.Primary_Link;
                            else if (tag_v.Equals("secondary")) CurrentWayInfo.WayType = EOSMWayType.Secondary;
                            else if (tag_v.Equals("secondary_link")) CurrentWayInfo.WayType = EOSMWayType.Secondary_Link;
                            else if (tag_v.Equals("tertiary")) CurrentWayInfo.WayType = EOSMWayType.Tertiary;
                            else if (tag_v.Equals("tertiary_link")) CurrentWayInfo.WayType = EOSMWayType.Tertiary_Link;
                            else if (tag_v.Equals("residential")) CurrentWayInfo.WayType = EOSMWayType.Residential;
                            else if (tag_v.Equals("service")) CurrentWayInfo.WayType = EOSMWayType.Service;
                            else if (tag_v.Equals("unclassified")) CurrentWayInfo.WayType = EOSMWayType.Unclassified;
                            else if (tag_v.Equals("living_street")) CurrentWayInfo.WayType = EOSMWayType.Living_Street;
                            else if (tag_v.Equals("pedestrian")) CurrentWayInfo.WayType = EOSMWayType.Pedestrian;
                            else if (tag_v.Equals("track")) CurrentWayInfo.WayType = EOSMWayType.Track;
                            else if (tag_v.Equals("bus_guideway")) CurrentWayInfo.WayType = EOSMWayType.Bus_Guideway;
                            else if (tag_v.Equals("raceway")) CurrentWayInfo.WayType = EOSMWayType.Raceway;
                            else if (tag_v.Equals("road")) CurrentWayInfo.WayType = EOSMWayType.Road;
                            else if (tag_v.Equals("footway")) CurrentWayInfo.WayType = EOSMWayType.Footway;
                            else if (tag_v.Equals("cycleway")) CurrentWayInfo.WayType = EOSMWayType.Cycleway;
                            else if (tag_v.Equals("bridleway")) CurrentWayInfo.WayType = EOSMWayType.Bridleway;
                            else if (tag_v.Equals("steps")) CurrentWayInfo.WayType = EOSMWayType.Steps;
                            else if (tag_v.Equals("path")) CurrentWayInfo.WayType = EOSMWayType.Path;
                            else if (tag_v.Equals("steps")) CurrentWayInfo.WayType = EOSMWayType.Steps;
                            else if (tag_v.Equals("path")) CurrentWayInfo.WayType = EOSMWayType.Path;
                            else if (tag_v.Equals("proposed")) CurrentWayInfo.WayType = EOSMWayType.Proposed;
                            else if (tag_v.Equals("construction")) CurrentWayInfo.WayType = EOSMWayType.Construction;
                        }
                        else if (tag_k.Equals("building"))
                        {
                            CurrentWayInfo.WayType = EOSMWayType.Building;

                            if (tag_v.Equals("yes")) CurrentWayInfo.WayType = EOSMWayType.Building;
                        }
                        else if (tag_k.Equals("height"))
                        {
                            if (tag_v != null && Convert.ToDouble(tag_v) > 0.0)
                                CurrentWayInfo.Height = Convert.ToDouble(tag_v);
                            else CurrentWayInfo.Height = -1;
                        }
                        else if (tag_k.Equals("building:levels"))
                        {
                            if (tag_v != null && Convert.ToInt32(tag_v) > 0)
                                CurrentWayInfo.BuildingLevels = Convert.ToInt32(tag_v);
                            else CurrentWayInfo.BuildingLevels = -1;
                        }
                        else if (tag_k.Equals("oneway"))
                        {
                            if (tag_v.Equals("yes"))
                                CurrentWayInfo.bIsOneWay = true;
                            else
                                CurrentWayInfo.bIsOneWay = false;
                        }
                    }

                    if (CurrentWayInfo.Name == null) CurrentWayInfo.Name = "NotDefined";
                    if (CurrentWayInfo.Ref == null) CurrentWayInfo.Ref = "NotDefined";
                    if(CurrentWayInfo.WayType == EOSMWayType.Building)
                    {
                        if (CurrentWayInfo.Height >= 0.0) CurrentWayInfo.Height = -1;
                        if (CurrentWayInfo.BuildingLevels >= 0) CurrentWayInfo.BuildingLevels = -1;
                    }

                    Ways.Add(CurrentWayInfo);
                }
            }

            public bool isSurvExist(XmlNodeList pTags)
            {
                for (int j = 0; j < pTags.Count; j++)
                {
                    XmlNode tag = pTags.Item(j);
                    if (tag.Attributes["k"].Value.Equals("surveillance"))
                    {
                        // yes there is.
                        return true;
                    }
                }
                return false;
            }

            public FOSMSurvInfo getSurveillance(XmlNodeList pTags)
            {
                FOSMSurvInfo newSurvInfo = new FOSMSurvInfo();
                for (int j = 0; j < pTags.Count; j++)
                {
                    XmlNode tag = pTags.Item(j);
                    string tag_k = tag.Attributes["k"].Value;
                    string tag_v = tag.Attributes["v"].Value;

                    if (tag_k.Equals("camera:direction"))
                    {
                        if (tag_v != null)
                            newSurvInfo.Direction = Convert.ToDouble(tag_v);
                        else newSurvInfo.Direction = 360.0;
                    }
                    else if (tag_k.Equals("camera:mount"))
                    {
                        if (tag_v != null)
                            newSurvInfo.MountType = tag_v;
                        else newSurvInfo.MountType = "pole";
                    }
                    else if (tag_k.Equals("camera:type"))
                    {
                        if (tag_v != null)
                            newSurvInfo.CameraType = tag_v;
                        else newSurvInfo.CameraType = "fixed";
                    }
                    else if (tag_k.Equals("height"))
                    {
                        if (tag_v != null)
                            newSurvInfo.Height = Convert.ToInt32(tag_v);
                        else newSurvInfo.Height = 4;
                    }
                    else if (tag_k.Equals("surveillance"))
                    {
                        if (tag_v != null)
                            newSurvInfo.Loc = tag_v;
                        else newSurvInfo.Loc = "outdoor";
                    }
                    else if (tag_k.Equals("surveillance:type"))
                    {
                        if (tag_v != null)
                            newSurvInfo.SurvType = tag_v;
                        else newSurvInfo.SurvType = "camera";
                    }
                    else if (tag_k.Equals("surveillance:zone"))
                    {
                        if (tag_v != null)
                            newSurvInfo.SurvZone = tag_v;
                        else newSurvInfo.SurvZone = "building";
                    }
                }
                return newSurvInfo;
            }

            public double getMinlat() { return minlat; }
            public double getMinlon() { return minlon; }
            public double getMaxlat() { return maxlat; }
            public double getMaxlon() { return maxlon; }

            public Point getMapUpperCorner()
            {
                return new Point(this.maxlon, this.maxlat, 0);
            }
            public Point getMapLowerCorner()
            {
                return new Point(this.minlon, this.minlat, 0);
            }

            // checking OsmReader
            public void printNodesHead()
            {
                for (int i = 0; i < 5; i++)
                {
                    XmlNode tmp = this.nodes[i].SelectSingleNode("./tag[@k='name:en']");
                    Console.WriteLine("{0}", tmp.Attributes["v"].Value);
                }
            }
        }

        public class OsmWriter
        {
            // All ways we've parsed
            public Dictionary<long, FOSMWayInfo> WaysMap { set; get; } = new Dictionary<long, FOSMWayInfo>();

            // Maps node IDs to info about each node
            public Dictionary<long, FOSMNodeInfo> NodesMap { set; get; } = new Dictionary<long, FOSMNodeInfo>();

            // 230704 박민제 존재하는 감시 자원
            public Dictionary<long, FOSMSurvInfo> SurveillancesMap { set; get; } = new Dictionary<long, FOSMSurvInfo>();

            public void printAsOsm(SimulatorCore simCore, int cctvSetIdx, int simIdx)
            {
                // xml writer setting
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;

                string fn = "CctvSet" + cctvSetIdx + ".Sim" + simIdx + ".SimLog.osm";
                XmlWriter writer = XmlWriter.Create(fn, settings);

                WaysMap.Clear();
                NodesMap.Clear();
                SurveillancesMap.Clear();

                /** 230802 박민제
                 * get nodes for roads from simCore.map.nodes
                 * get roads from simCore.map.roads
                 */
                // node
                for(int i = 0; i < simCore.map.nodes.Count; i++)
                {
                    Node curNode = simCore.map.nodes[i];
                    Point convertTo4326 = indexOnProgToEpsg4326(new Point(curNode.x, curNode.y, 0), simCore.map.lowerCorner, simCore.map.upperCorner, simCore.map.X_mapSize, simCore.map.Y_mapSize);

                    // define node used for representing road
                    bool isNodeExist = false;
                    for (int nodeIdx = 0; nodeIdx < NodesMap.Count; nodeIdx++)
                    {
                        if (NodesMap[nodeIdx].Longitude == convertTo4326.x && NodesMap[nodeIdx].Latitude == convertTo4326.y)
                        {
                            isNodeExist = true;
                            break;
                        }
                    }
                    if (!isNodeExist)
                    {
                        FOSMNodeInfo newNode = new FOSMNodeInfo();
                        newNode.Latitude = convertTo4326.y;
                        newNode.Longitude = convertTo4326.x;

                        NodesMap.Add(NodesMap.Count, newNode);
                    }
                }
                // way
                for(int i = 0; i < simCore.map.roads.Count; i++)
                {
                    Road curRoad = simCore.map.roads[i];
                    FOSMWayInfo newWay = new FOSMWayInfo();

                    newWay.Name = curRoad.RoadName;
                    newWay.Ref = curRoad.RoadRef;
                    newWay.WayType = curRoad.EOSMWayType;
                    newWay.Height = -1;     // not defined
                    newWay.BuildingLevels = -1; // not defined
                    newWay.bIsOneWay = curRoad.bIsOneWay;

                    for(int j = 0; j < curRoad.NodeIndices.Count; j++)
                    {
                        Node tmpNode = simCore.map.nodes[curRoad.NodeIndices[j]];
                        Point convertTo4326 = indexOnProgToEpsg4326(new Point(tmpNode.x, tmpNode.y, 0), simCore.map.lowerCorner, simCore.map.upperCorner, simCore.map.X_mapSize, simCore.map.Y_mapSize);
                        
                        for(int k = 0; k < NodesMap.Count; k++)
                        {
                            if (NodesMap[k].Latitude == convertTo4326.y && NodesMap[k].Longitude == convertTo4326.x)
                            {
                                newWay.Nodes.Add(NodesMap[k]);
                                break;
                            }
                        }
                    }

                    WaysMap.Add(WaysMap.Count, newWay);
                }

                /** 230802 박민제
                 * get buildings from simCore.buildings
                 */
                for(int i = 0; i < simCore.buildings.Count(); i++)
                {
                    FOSMWayInfo newWayInfo = new FOSMWayInfo();
                    newWayInfo.Name = "NotDefined";
                    newWayInfo.Ref = "NotDefined";
                    newWayInfo.WayType = EOSMWayType.Building;
                    newWayInfo.Height = simCore.buildings[i].H;
                    newWayInfo.BuildingLevels = -1; // not defined

                    for(int j = 0; j < simCore.buildings[i].pointsOfBottom.Count(); j++)
                    {
                        Point tmpPoint = new Point(simCore.buildings[i].pointsOfBottom[j]);
                        Point convertTo4326 = indexOnProgToEpsg4326(tmpPoint, simCore.map.lowerCorner, simCore.map.upperCorner, simCore.map.X_mapSize, simCore.map.Y_mapSize);

                        bool isNodeExist = false;
                        for (int k = 0; k < NodesMap.Count; k++)
                        {
                            if (NodesMap[k].Latitude == convertTo4326.y && NodesMap[k].Longitude == convertTo4326.x)
                            {
                                isNodeExist = true;
                                newWayInfo.Nodes.Add(NodesMap[k]);
                                break;
                            }
                        }
                        if (!isNodeExist)
                        {
                            FOSMNodeInfo newNode = new FOSMNodeInfo();
                            newNode.Latitude = convertTo4326.y;
                            newNode.Longitude = convertTo4326.x;
                            NodesMap.Add(NodesMap.Count, newNode);

                            newWayInfo.Nodes.Add(newNode);
                        }
                    }

                    WaysMap.Add(WaysMap.Count, newWayInfo);
                }

                /** 230802 박민제
                 * get cctv data from simCore.cctvs
                 */
                for(int i = 0; i < simCore.cctvs.Count(); i++)
                {
                    Point cctvPoint = new Point(simCore.cctvs[i].X, simCore.cctvs[i].Y, simCore.cctvs[i].Z);
                    Point convertTo4326 = indexOnProgToEpsg4326(cctvPoint, simCore.map.lowerCorner, simCore.map.upperCorner, simCore.map.X_mapSize, simCore.map.Y_mapSize);

                    FOSMSurvInfo newSurv = new FOSMSurvInfo();
                    newSurv.Direction = simCore.cctvs[i].Direction;
                    newSurv.MountType = "pole";
                    newSurv.CameraType = "fixed";
                    newSurv.Height = simCore.cctvs[i].Z;
                    newSurv.Loc = "outdoor";
                    newSurv.SurvType = "camera";
                    newSurv.SurvZone = "building";

                    bool isNodeExist = false;
                    for (int k = 0; k < NodesMap.Count; k++)
                    {
                        if (NodesMap[k].Latitude == convertTo4326.y && NodesMap[k].Longitude == convertTo4326.x)
                        {
                            isNodeExist = true;
                            NodesMap[k].SurvInfoExist = true;
                            NodesMap[k].SurvInfo = newSurv;
                            break;
                        }
                    }
                    if (!isNodeExist)
                    {
                        FOSMNodeInfo newNode = new FOSMNodeInfo();
                        newNode.Latitude = convertTo4326.y;
                        newNode.Longitude = convertTo4326.x;

                        newNode.SurvInfoExist = true;
                        newNode.SurvInfo = newSurv;

                        NodesMap.Add(NodesMap.Count, newNode);
                    }

                    SurveillancesMap.Add(SurveillancesMap.Count, newSurv);
                }

                /** 230802 박민제
                 * get target movement log from csv file
                 * define used nodes at NodesMap
                 * define trace at WaysMap
                 */
                Dictionary<int, List<TargetLogCSVReader.TargetLog>> targetsLogDict = new Dictionary<int, List<TargetLogCSVReader.TargetLog>>();
                for(int i = 0; i < simCore.N_Target; i++)
                {
                    FOSMWayInfo newWayInfo = new FOSMWayInfo();

                    // basic info of target
                    newWayInfo.TargetInfoExist = true;
                    newWayInfo.TargetInfo = new FOSMTargetInfo();
                    if (i < simCore.N_Ped)
                    {
                        newWayInfo.TargetInfo.Type = "Pedestrian";
                        newWayInfo.TargetInfo.Width = simCore.peds[i].W / 1000;
                        newWayInfo.TargetInfo.Length = simCore.peds[i].W2 / 1000;
                        newWayInfo.TargetInfo.Height = simCore.peds[i].H / 1000;
                    }
                    else
                    {
                        newWayInfo.TargetInfo.Type = "Car";
                        newWayInfo.TargetInfo.Width = simCore.cars[i - simCore.N_Ped].W / 1000;
                        newWayInfo.TargetInfo.Length = simCore.cars[i - simCore.N_Ped].W2 / 1000;
                        newWayInfo.TargetInfo.Height = simCore.cars[i - simCore.N_Ped].H / 1000;
                    }

                    List<TargetLogCSVReader.TargetLog> targetLogs = new TargetLogCSVReader().TraceLogFromCSV(cctvSetIdx, simIdx, i);
                    foreach(TargetLogCSVReader.TargetLog log in targetLogs)
                    {

                        // Convert coordination to epsg 4326 from program local coordination.
                        Point convertTo4326 = indexOnProgToEpsg4326(new Point(log.xLog, log.yLog, 0), simCore.map.lowerCorner, simCore.map.upperCorner, simCore.map.X_mapSize, simCore.map.Y_mapSize);
                        log.xLog = convertTo4326.x;
                        log.yLog = convertTo4326.y;

                        // define node used for trace log
                        bool isNodeExist = false;
                        for(int nodeIdx = 0; nodeIdx < NodesMap.Count; nodeIdx++)
                        {
                            if (NodesMap[nodeIdx].Longitude == log.xLog && NodesMap[nodeIdx].Latitude == log.yLog)
                            {
                                isNodeExist = true;
                                newWayInfo.Nodes.Add(NodesMap[nodeIdx]);
                                break;
                            }
                        }
                        if (!isNodeExist)
                        {
                            FOSMNodeInfo newNode = new FOSMNodeInfo();
                            newNode.Latitude = log.yLog;
                            newNode.Longitude = log.xLog;

                            NodesMap.Add(NodesMap.Count, newNode);

                            newWayInfo.Nodes.Add(newNode);
                        }
                    }

                    newWayInfo.Name = "Target" + i;
                    newWayInfo.Ref = "TraceLog.Target" + i;
                    newWayInfo.WayType = EOSMWayType.TargetTrace;
                    newWayInfo.Height = -1;    // 높이가 존재하지 않는다.
                    newWayInfo.BuildingLevels = -1;    // 층 수가 존재하지 않는다.
                    newWayInfo.bIsOneWay = false;
                    WaysMap.Add(WaysMap.Count, newWayInfo);

                    targetsLogDict.Add(i, targetLogs);
                }

                /** 230802 박민제
                 * start writing osm file
                 */
                //~ "?xml" start
                {
                    //writer.WriteStartElement("/?xml");
                    //writer.WriteAttributeString("version", "1.0");
                    //writer.WriteAttributeString("encoding", "UTF-8");
                    //writer.WriteEndElement();   // end of "?xml"
                }
                //~ "?xml" end

                //~ "osm" start
                {
                    writer.WriteStartElement("osm");
                    writer.WriteAttributeString("version", "0.6");
                    writer.WriteAttributeString("generator", "CGImap 0.8.8 (2403364 spike-06.openstreetmap.org)");
                    writer.WriteAttributeString("copyright", "OpenStreetMap and contributors");
                    writer.WriteAttributeString("attribution", "http://www.openstreetmap.org/copyright");
                    writer.WriteAttributeString("license", "http://opendatacommons.org/licenses/odbl/1-0/");

                    //~ "bound" start
                    {
                        writer.WriteStartElement("bounds");
                        writer.WriteAttributeString("minlat", Convert.ToString(simCore.map.lowerCorner.y));
                        writer.WriteAttributeString("minlon", Convert.ToString(simCore.map.lowerCorner.x));
                        writer.WriteAttributeString("maxlat", Convert.ToString(simCore.map.upperCorner.y));
                        writer.WriteAttributeString("maxlon", Convert.ToString(simCore.map.upperCorner.x));
                        writer.WriteEndElement();   // end of "bound"
                    }
                    //~ "bound" end

                    //~ "node"s start
                    {
                        for (int i = 0; i < NodesMap.Count; i++)
                        {
                            long nodeId = NodesMap.ElementAt(i).Key;
                            FOSMNodeInfo node = NodesMap.ElementAt(i).Value;

                            writer.WriteStartElement("node");
                            writer.WriteAttributeString("id", Convert.ToString(nodeId));
                            writer.WriteAttributeString("visible", "true");
                            writer.WriteAttributeString("lat", Convert.ToString(Math.Round(node.Latitude, 7)));
                            writer.WriteAttributeString("lon", Convert.ToString(Math.Round(node.Longitude, 7)));

                            if (node.SurvInfoExist)
                            {
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "camera:direction");
                                writer.WriteAttributeString("v", Convert.ToString(node.SurvInfo.Direction));
                                writer.WriteEndElement();   // end of "tag"

                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "camera:mount");
                                writer.WriteAttributeString("v", node.SurvInfo.MountType);
                                writer.WriteEndElement();   // end of "tag"

                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "camera:type");
                                writer.WriteAttributeString("v", node.SurvInfo.CameraType);
                                writer.WriteEndElement();   // end of "tag"

                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "height");
                                writer.WriteAttributeString("v", Convert.ToString(node.SurvInfo.Height / 1000));
                                writer.WriteEndElement();   // end of "tag"

                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "surveillance");
                                writer.WriteAttributeString("v", node.SurvInfo.Loc);
                                writer.WriteEndElement();   // end of "tag"

                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "surveillance:type");
                                writer.WriteAttributeString("v", node.SurvInfo.SurvType);
                                writer.WriteEndElement();   // end of "tag"

                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "surveillance:zone");
                                writer.WriteAttributeString("v", node.SurvInfo.SurvZone);
                                writer.WriteEndElement();   // end of "tag"
                            }

                            writer.WriteEndElement();   // end of "node"
                        }
                    }
                    //~ "node"s end

                    //~ "way"s start
                    {
                        for (int i = 0; i < WaysMap.Count; i++)
                        {
                            long wayId = WaysMap.ElementAt(i).Key;
                            FOSMWayInfo way = WaysMap.ElementAt(i).Value;

                            writer.WriteStartElement("way");
                            writer.WriteAttributeString("id", Convert.ToString(wayId));

                            // nd
                            for(int j = 0; j < way.Nodes.Count; j++)
                            {
                                writer.WriteStartElement("nd");
                                long ndId = -1;
                                for(int k = 0; k < NodesMap.Count; k++)
                                {
                                    if (NodesMap[k].Latitude == way.Nodes[j].Latitude 
                                        && NodesMap[k].Longitude == way.Nodes[j].Longitude)
                                    {
                                        ndId = NodesMap.ElementAt(k).Key;
                                        break;
                                    }
                                }
                                writer.WriteAttributeString("ref", Convert.ToString(ndId));
                                writer.WriteEndElement();   // end of "nd"
                            }

                            // tag "name"
                            writer.WriteStartElement("tag");
                            writer.WriteAttributeString("k", "name");
                            writer.WriteAttributeString("v", way.Name);
                            writer.WriteEndElement();   // end of tag "name"

                            // tag "ref"
                            writer.WriteStartElement("tag");
                            writer.WriteAttributeString("k", "ref");
                            writer.WriteAttributeString("v", way.Ref);
                            writer.WriteEndElement();   // end of tag "ref"

                            // tag "highway"
                            if (way.WayType != EOSMWayType.Building
                                && way.WayType != EOSMWayType.TargetTrace)
                            {
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "highway");
                                writer.WriteAttributeString("v", EOSMWayTypeName[Convert.ToInt32(way.WayType)].ToLower());
                                writer.WriteEndElement();   // end of tag "highway"
                            }

                            // tag about surveillance target
                            if(way.WayType == EOSMWayType.TargetTrace)
                            {
                                // tag "target:trace"
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "target:trace");
                                writer.WriteAttributeString("v", "yes");
                                writer.WriteEndElement();   // end of tag "target:trace"

                                // tag "Type"
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "target:type");
                                writer.WriteAttributeString("v", way.TargetInfo.Type);
                                writer.WriteEndElement();   // end of tag "target:type"

                                // tag "Width"
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "target:width");
                                writer.WriteAttributeString("v", Convert.ToString(way.TargetInfo.Width));
                                writer.WriteEndElement();   // end of tag "target:width"

                                // tag "Length"
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "target:length");
                                writer.WriteAttributeString("v", Convert.ToString(way.TargetInfo.Length));
                                writer.WriteEndElement();   // end of tag "target:length"

                                // tag "Height"
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "target:height");
                                writer.WriteAttributeString("v", Convert.ToString(way.TargetInfo.Height));
                                writer.WriteEndElement();   // end of tag "target:height"
                            }

                            if(way.WayType == EOSMWayType.Building)
                            {
                                // tag "building"
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "building");
                                writer.WriteAttributeString("v", "yes");
                                writer.WriteEndElement();   // end of tag "building"

                                // tag "height"
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "height");
                                writer.WriteAttributeString("v", Convert.ToString(way.Height));
                                writer.WriteEndElement();   // end of tag "height"

                                if(way.BuildingLevels != -1)
                                {
                                    // tag "building:levels"
                                    writer.WriteStartElement("tag");
                                    writer.WriteAttributeString("k", "building:levels");
                                    writer.WriteAttributeString("v", Convert.ToString(way.BuildingLevels));
                                    writer.WriteEndElement();   // end of tag "building:levels"
                                }

                                // tag "oneway"
                                writer.WriteStartElement("tag");
                                writer.WriteAttributeString("k", "oneway");
                                if (way.bIsOneWay)
                                {
                                    writer.WriteAttributeString("v", "yes");
                                }
                                else
                                {
                                    writer.WriteAttributeString("v", "no");
                                }
                                writer.WriteEndElement();   // end of tag "oneway"
                            }

                            writer.WriteEndElement();   // end of "way"
                        }
                    }
                    //~ "way"s end

                    writer.WriteEndElement();   // end of "osm"
                }
                //~ "osm" end

                writer.Flush();
                writer.Close();
            }
        }
    }
}
