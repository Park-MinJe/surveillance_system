using Microsoft.VisualBasic;
using OpenTK.Core.Native;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
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
        

        public class OsmReader
        {
            string fileName;
            XmlDocument xdoc;
            double minlat, minlon, maxlat, maxlon;
            XmlNodeList nodes;
            XmlNodeList ways;
            XmlNodeList relations;

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
		        /// UNSUPPORTED
		        /// 
		
		        /** Currently unrecognized type */
		        Other,
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
            public class FOSMWayRef
            {
                // Way that we're referencing at this node
                public FOSMWayInfo Way { set; get; }

                // Index of the node in the way's array of nodes
                public int NodeIndex { set; get; }
            };


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
                public bool SurvInfoExist { set; get; }
            };


            public class FOSMWayInfo
            {
                public string Name { set; get; }
                public string Ref { set; get; }
                public List<FOSMNodeInfo> Nodes { set; get; }
                public EOSMWayType WayType { set; get; }
                public double Height { set; get; }
                public int BuildingLevels { set; get; }

                // If true, way is only traversable in the order the nodes are listed in the Nodes list
                public bool bIsOneWay { set; get; }
            };

            // Average Latitude (roughly the center of the map)
            public double AverageLatitude { set; get; } = 0.0;
            public double AverageLongitude { set; get; } = 0.0;

            // All ways we've parsed
            public List<FOSMWayInfo> Ways { set; get; }

            // Maps node IDs to info about each node
            public Dictionary<long, FOSMNodeInfo> NodeMap { set; get; }

            // 230704 박민제 존재하는 감시 자원
            public Dictionary<long, FOSMSurvInfo> SurveillancesMap { set; get; }

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
                    if (node.Attributes["lon"].Value == null)
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

                        // Debug
                        Console.WriteLine("\nNodeId -> " + Convert.ToString(CurrentNodeId));
                        Console.WriteLine(CurrentNode.SurvInfo.ToString());
                    }

                    NodeMap.Add(CurrentNodeId, CurrentNode);
                }

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
                        if (tag_k.Equals("name")) CurrentWayInfo.Name = tag_v;
                        else if (tag_k.Equals("ref")) CurrentWayInfo.Ref = tag_v;
                        else if (tag_k.Equals("highway"))
                        {
                            EOSMWayType WayType = EOSMWayType.Other;
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
                    }
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

        public interface InputFromApi
        {
            public void setRequest(string methodname);
            public void setServiceKey(string key);
            public void setTypename(string typename);
            public void setBbox(string bbox);
            public void setSrsname(string srsname);
            public void setEndPointUrl();
            public void loadBuildingDataFromApiAsXml();
            public void readFeatureMembers();
            public void readPosLists();
            public void readBuildingHs();
            public Point getMapLowerCorner();
            public Point getMapUpperCorner();
            public int getFeatureMembersCnt();
            public double getBuildingHByIdx(int idx);
            public Point[] getPosListByIdx(int idx);
        }

        public class GisBuildingService : InputFromApi
        {
            private HttpClient client = new HttpClient();

            private string url;
            private string apiEndPoint = "http://apis.data.go.kr/1611000/nsdi/GisBuildingService";
            private string methodName = "";
            private string serviceKey = "?ServiceKey=";     // 필수
            private string typeName = "&typename=";
            private string bbox = "&bbox=";
            private string pnu = "&pnu=";
            private string maxFeature = "&maxFeatures=";
            private string resultType = "&resultType=";
            private string srsName = "&srsName=";

            private bool IsSetMethodNameCalled = false;
            private bool IsSetServiceKeyCalled = false;
            private bool IsSetTypeNameCalled = false;
            private bool IsSetBBoxCalled = false;
            private bool IsSetPnuCalled = false;
            private bool IsSetMaxFeatureCalled = false;
            private bool IsSetResultTypeCalled = false;
            private bool IsSetSrsNameCalled = false;

            // 데이터 저장 메모리
            // 데이터 베이스 또는 문서로 확장할까?
            XmlDocument xml;
            XmlNodeList gml_lowerCorner;
            XmlNodeList gml_upperCorner;
            XmlNodeList gml_featureMembers;
            XmlNodeList gml_posLists;
            XmlNodeList NSDI_BULD_HGs;

            /* --------------------------------------
             * setter
            -------------------------------------- */
            public void setRequest(string methodName)
            {
                this.IsSetMethodNameCalled = true;

                if (methodName == "국토교통부 GIS건물일반정보WMS조회")
                    this.methodName += "/wms/getGisGnrlBuildingWMS";
                else if (methodName == "국토교통부 GIS건물일반정보WFS조회")
                    this.methodName += "/wfs/getGisGnrlBuildingWFS";
                else if (methodName == "국토교통부 GIS건물집합정보WMS조회")
                    this.methodName += "/wms/getGisAggrBuildingWMS";
                else if (methodName == "국토교통부 GIS건물집합정보WFS조회")
                    this.methodName += "/wfs/getGisAggrBuildingWFS";
            }
            public void setServiceKey(string serviceKey)
            {
                this.IsSetServiceKeyCalled = true;
                this.serviceKey += serviceKey;
            }
            public void setTypename(string typeName)
            {
                this.IsSetTypeNameCalled = true;
                this.typeName += typeName;
            }
            public void setBbox(string bbox) 
            {
                this.IsSetBBoxCalled = true;
                this.bbox += bbox;
            }
            public void setPnu(string pnu)
            {
                this.IsSetPnuCalled = true;
                this.pnu += pnu;
            }
            public void setMaxFeature(string maxFeature) 
            {
                this.IsSetMaxFeatureCalled = true;
                this.maxFeature += maxFeature;
            }
            public void setResultType(string resultType) 
            {
                this.IsSetResultTypeCalled = true;
                this.resultType += resultType;
            }
            public void setSrsname(string srsName) 
            {
                this.IsSetSrsNameCalled = true;
                this.srsName += srsName; 
            }

            public void setEndPointUrl(string methodName, string serviceKey, string bbox, string pnu, string typeName = "F171", string maxFeature = "10", string resultType = "results", string srsName = "EPSG:5174")
            {
                if (methodName != "")
                    this.setRequest(methodName);
                if (serviceKey != "")
                    this.setServiceKey(serviceKey);
                if (bbox != "")
                    this.setBbox(bbox);
                if (pnu != "")
                    this.setPnu(pnu);
                if (typeName != "")
                    this.setTypename(typeName);
                if (maxFeature != "")
                    this.setMaxFeature(maxFeature);
                if (resultType != "")
                    this.setResultType(resultType);
                if (srsName != "")
                    this.setSrsname(srsName);

                this.url = this.apiEndPoint + this.methodName;
                if (this.IsSetServiceKeyCalled) this.url += this.serviceKey;
                if (this.IsSetTypeNameCalled) this.url += this.typeName;
                if (this.IsSetBBoxCalled) this.url += this.bbox;
                if (this.IsSetPnuCalled) this.url += this.pnu;
                if (this.IsSetMaxFeatureCalled) this.url += this.maxFeature;
                if (this.IsSetResultTypeCalled) this.url += this.resultType;
                if (this.IsSetSrsNameCalled) this.url += this.srsName;

                // debug;
                //Console.WriteLine(url);
            }

            public void setEndPointUrl()
            {
                this.url = this.apiEndPoint;
                if (this.IsSetMethodNameCalled) this.url += this.methodName;
                if (this.IsSetServiceKeyCalled) this.url += this.serviceKey;
                if (this.IsSetTypeNameCalled) this.url += this.typeName;
                if (this.IsSetBBoxCalled) this.url += this.bbox;
                if (this.IsSetPnuCalled) this.url += this.pnu;
                if (this.IsSetMaxFeatureCalled) this.url += this.maxFeature;
                if (this.IsSetResultTypeCalled) this.url += this.resultType;
                if (this.IsSetSrsNameCalled) this.url += this.srsName;

                // debug;
                //Console.WriteLine(url);
            }

            /* --------------------------------------
             * load data as xml
            -------------------------------------- */
            public void loadBuildingDataFromApiAsXml()
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                string results = string.Empty;
                HttpWebResponse response;
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    results = reader.ReadToEnd();
                }

                //Debug
                //Console.WriteLine(results);

                this.xml = new XmlDocument();
                xml.LoadXml(results);
            }

            /* --------------------------------------
             * read from xml
            -------------------------------------- */
            public void readLowerCorner()
            {
                gml_lowerCorner = xml.GetElementsByTagName("gml:lowerCorner");
            }

            public void readUpperCorner()
            {
                gml_upperCorner = xml.GetElementsByTagName("gml:upperCorner");
            }

            public void readFeatureMembers()
            {
                gml_featureMembers = xml.GetElementsByTagName("gml:featureMember");
            }

            public void readPosLists()
            {
                gml_posLists = xml.GetElementsByTagName("gml:posList");
            }

            public void readBuildingHs()
            {
                NSDI_BULD_HGs = xml.GetElementsByTagName("NSDI:BULD_HG");
            }

            /* --------------------------------------
             * get data from xml as local class
            -------------------------------------- */
            public Point getMapLowerCorner()
            {
                this.readLowerCorner();
                string lowerCorner = gml_lowerCorner[0].InnerText;
                string[] lowerCornerCoordinate = lowerCorner.Split(' ');
                Point rt = new Point(Convert.ToDouble(lowerCornerCoordinate[0]), Convert.ToDouble(lowerCornerCoordinate[1]), 0d);

                //Debug
                //Console.WriteLine("Raw Lower Corner");
                //rt.printString();

                return rt;
            }

            public Point getMapUpperCorner()
            {
                this.readUpperCorner();
                string upperCorner = gml_upperCorner[0].InnerText;
                string[] upperCornerCoordinate = upperCorner.Split(' ');
                Point rt = new Point(Convert.ToDouble(upperCornerCoordinate[0]), Convert.ToDouble(upperCornerCoordinate[1]), 0d);

                //Debug
                //Console.WriteLine("Raw Lower Corner");
                //rt.printString();

                return rt;
            }

            public int getFeatureMembersCnt()
            {
                return this.gml_featureMembers.Count;
            }

            public double getBuildingHByIdx(int idx)
            {
                return Convert.ToDouble(NSDI_BULD_HGs[idx].InnerText);
            }

            public Point[] getPosListByIdx(int idx)
            {
                string phrase = gml_posLists[idx].InnerText;
                string[] words = phrase.Split(" ");
                Point[] rt = new Point[words.Length / 2];
                for (int j = 0; j < words.Length / 2; j++)
                {
                    rt[j] = new Point(Convert.ToDouble(words[j * 2]), Convert.ToDouble(words[j * 2 + 1]), 0d);
                }

                return rt;
            }

            // debug
            /*public void testGisBuildingService(string methodName, string serviceKey, string bbox, string pnu, string typeName = "F171", string maxFeature = "10", string resultType = "results", string srsName = "EPSG:5174")
            {
                this.setEndPointUrl(methodName, serviceKey, bbox, pnu, typeName, maxFeature, resultType, srsName);

                this.loadBuildingDataFromApiAsXml();

                Point lowerCorner = this.getMapLowerCorner();

                Point upperCorner = this.getMapUpperCorner();

                Point transformedLowerCorner = TransformCoordinate(lowerCorner, 5174, 4326);
                Console.WriteLine("Transformed Lower Corner");
                transformedLowerCorner.printString();

                Point transformedUpperCorner = TransformCoordinate(upperCorner, 5174, 4326);
                Console.WriteLine("Transformed Upper Corner");
                transformedUpperCorner.printString();

                Console.WriteLine("가로 길이: {0}", getDistanceBetweenPointsOfepsg4326(transformedLowerCorner.x, transformedLowerCorner.y, transformedUpperCorner.x, transformedLowerCorner.y));
                Console.WriteLine("세로 길이: {0}", getDistanceBetweenPointsOfepsg4326(transformedLowerCorner.x, transformedLowerCorner.y, transformedLowerCorner.x, transformedUpperCorner.y));

                this.readFeatureMembers();
                this.readPosLists();
                this.readBuildingHs();

                List<Point[]> pls = new List<Point[]>();
                List<double> hs = new List<double>();

                for (int i = 0; i<this.getFeatureMembersCnt(); i++)
                {
                    double h = Convert.ToDouble(NSDI_BULD_HGs[i].InnerText);
                    if (h > 0)
                    {
                        hs.Add(h);

                        Point[] pl = this.getPosListByIdx(i);
                        Point[] transformedPl = TransformCoordinate(pl, 5174, 4326);

                        // 프로그램상의 좌표계로 변환
                        // 지도 범위의 왼쪽 위를 기준으로 한다.
                        Point[] plOnSystem = calcIndexOnProg(transformedPl, transformedLowerCorner, transformedUpperCorner);

                        pls.Add(plOnSystem);
                    }
                }

                buildings = new Building[pls.Count];
                for(int i = 0; i<pls.Count; i++)
                {
                    buildings[i] = new Building();
                    buildings[i].define_Building(pls[i], hs[i]);
                }
            }*/
        }

        public class VworldService : InputFromApi
        {
            private string url = "http://api.vworld.kr/req/wfs",
                request = "?request=",      //필수
                key = "&key=",              //필수
                typename = "&typename=",    //필수
                bbox = "&bbox=",            //필수
                srsname = "&srsname=";

            private bool isSetRequestCalled = false,
                isSetKeyCalled = false,
                isSetTypenameCalled = false,
                isSetBboxCalled = false,
                isSetSrsnameCalled = false;

            // 데이터 저장 메모리
            // 데이터 베이스 또는 문서로 확장할까?
            XmlDocument xml;
            XmlNodeList gml_featureMembers;
            XmlNodeList sop_ag_geom;
            XmlNodeList sop_heights;

            Point upperCorner, lowerCorner;

            /* --------------------------------------
             * setter
            -------------------------------------- */
            public void setRequest(string request) { this.request += request; this.isSetRequestCalled = true; }
            public void setServiceKey(string key) { this.key += key; this.isSetKeyCalled = true; }
            public void setTypename(string typename) { this.typename += typename; this.isSetTypenameCalled = true; }
            public void setBbox(string bbox) 
            { 
                this.bbox += bbox;
                this.isSetBboxCalled = true;

                string tmp_string = bbox;
                string[] tmp_word = bbox.Split(',');
                lowerCorner = new Point(Convert.ToDouble(tmp_word[0]), Convert.ToDouble(tmp_word[1]), 0);
                upperCorner = new Point(Convert.ToDouble(tmp_word[2]), Convert.ToDouble(tmp_word[3]), 0);
            }
            public void setSrsname(string srsname) { this.srsname += srsname; this.isSetSrsnameCalled = true; }

            public void setEndPointUrl()
            {
                if (isSetRequestCalled) url += request;
                if (isSetKeyCalled) url += key;
                if (isSetTypenameCalled) url += typename;
                if (isSetBboxCalled) url += bbox;
                if (isSetSrsnameCalled) url += srsname;

                //debug
                //Console.WriteLine(url);
            }

            /* --------------------------------------
             * load data as xml
            -------------------------------------- */
            public void loadBuildingDataFromApiAsXml()
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                string results = string.Empty;
                HttpWebResponse response;
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    results = reader.ReadToEnd();
                }

                //Debug
                //Console.WriteLine(results);

                this.xml = new XmlDocument();
                xml.LoadXml(results);
            }

            /* --------------------------------------
             * read from xml
            -------------------------------------- */
            public void readFeatureMembers()
            {
                gml_featureMembers = xml.GetElementsByTagName("gml:featureMember");
            }

            public void readPosLists()
            {
                sop_ag_geom = xml.GetElementsByTagName("sop:ag_geom");
            }

            public void readBuildingHs()
            {
                sop_heights = xml.GetElementsByTagName("sop:height");
            }

            /* --------------------------------------
             * get data from xml as local class
            -------------------------------------- */
            public Point getMapLowerCorner()
            {
                Point rt = lowerCorner;

                //Debug
                //Console.WriteLine("Raw Lower Corner");
                //rt.printString();

                return rt;
            }

            public Point getMapUpperCorner()
            {
                Point rt = upperCorner;

                //Debug
                //Console.WriteLine("Raw Lower Corner");
                //rt.printString();

                return rt;
            }

            public int getFeatureMembersCnt()
            {
                return this.gml_featureMembers.Count;
            }

            public double getBuildingHByIdx(int idx)
            {
                return Convert.ToDouble(sop_heights[idx].InnerText);
            }

            public Point[] getPosListByIdx(int idx)
            {
                string phrase = sop_ag_geom[idx].InnerText;
                string[] words = phrase.Split(" ");
                Point[] rt = new Point[words.Length];
                for (int i = 0; i < words.Length; i++)
                {
                    string[] coordinate = words[i].Split(",");
                    rt[i] = new Point(Convert.ToDouble(coordinate[0]), Convert.ToDouble(coordinate[1]), 0d);
                    //debug
                    //rt[i].printString();
                    //Console.WriteLine();
                }

                return rt;
            }
        }
    }
}
