using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;

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

        public class GisBuildingService
        {
            private HttpClient client = new HttpClient();

            private string url;
            private string apiEndPoint = "http://apis.data.go.kr/1611000/nsdi/GisBuildingService";
            private string gisMethodName;
            private string serviceKey = "?ServiceKey=";     // 필수
            private string typeName = "&typename=";
            private string bbox = "&bbox=";
            private string pnu = "&pnu=";
            private string maxFeature = "&maxFeatures=";
            private string resultType = "&resultType=";
            private string srsName = "&srsName=";

            private bool IsSetGisMethodNameCalled = false;
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
            public void setGisMethodName(string gisMethodName)
            {
                this.IsSetGisMethodNameCalled = true;

                if(gisMethodName == "국토교통부 GIS건물일반정보WMS조회")
                {
                    this.gisMethodName = "/wms/getGisGnrlBuildingWMS";
                }
                else if(gisMethodName == "국토교통부 GIS건물일반정보WFS조회")
                {
                    this.gisMethodName = "/wfs/getGisGnrlBuildingWFS";
                }
                else if(gisMethodName == "국토교통부 GIS건물집합정보WMS조회")
                {
                    this.gisMethodName = "/wms/getGisAggrBuildingWMS";
                }
                else if(gisMethodName == "국토교통부 GIS건물집합정보WFS조회")
                {
                    this.gisMethodName = "/wfs/getGisAggrBuildingWFS";
                }
            }
            public void setServiceKey(string serviceKey)
            {
                this.IsSetServiceKeyCalled = true;
                this.serviceKey += serviceKey;
            }
            public void setTypeName(string typeName)
            {
                this.IsSetTypeNameCalled = true;
                this.typeName += typeName;
            }
            public void setBBox(string bbox) 
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
            public void setSrsName(string srsName) 
            {
                this.IsSetSrsNameCalled = true;
                this.srsName += srsName; 
            }

            public void setEndPointUrl(string methodName, string serviceKey, string bbox, string pnu, string typeName = "F171", string maxFeature = "10", string resultType = "results", string srsName = "EPSG:5174")
            {
                if (methodName != "")
                    this.setGisMethodName(methodName);
                if (serviceKey != "")
                    this.setServiceKey(serviceKey);
                if (bbox != "")
                    this.setBBox(bbox);
                if (pnu != "")
                    this.setPnu(pnu);
                if (typeName != "")
                    this.setTypeName(typeName);
                if (maxFeature != "")
                    this.setMaxFeature(maxFeature);
                if (resultType != "")
                    this.setResultType(resultType);
                if (srsName != "")
                    this.setSrsName(srsName);

                this.url = this.apiEndPoint + this.gisMethodName + this.serviceKey + this.typeName + this.bbox + this.maxFeature + this.resultType + this.srsName;
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
                this.url = this.apiEndPoint + this.gisMethodName + this.serviceKey + this.typeName + this.bbox + this.maxFeature + this.resultType + this.srsName;
                if (this.IsSetServiceKeyCalled) this.url += this.serviceKey;
                if (this.IsSetTypeNameCalled) this.url += this.typeName;
                if (this.IsSetBBoxCalled) this.url += this.bbox;
                if (this.IsSetPnuCalled) this.url += this.pnu;
                if (this.IsSetMaxFeatureCalled) this.url += this.maxFeature;
                if (this.IsSetResultTypeCalled) this.url += this.resultType;
                if (this.IsSetSrsNameCalled) this.url += this.srsName;

                // debug;
                Console.WriteLine(url);
            }

            /* --------------------------------------
             * load data as xml
            -------------------------------------- */
            public void loadArchDataFromApiAsXml()
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
                Console.WriteLine(results);

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

            public void readArchHs()
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
                Console.WriteLine("Raw Lower Corner");
                rt.printString();

                return rt;
            }

            public Point getMapUpperCorner()
            {
                this.readUpperCorner();
                string upperCorner = gml_upperCorner[0].InnerText;
                string[] upperCornerCoordinate = upperCorner.Split(' ');
                Point rt = new Point(Convert.ToDouble(upperCornerCoordinate[0]), Convert.ToDouble(upperCornerCoordinate[1]), 0d);

                //Debug
                Console.WriteLine("Raw Lower Corner");
                rt.printString();

                return rt;
            }

            public int getFeatureMembersCnt()
            {
                return this.gml_featureMembers.Count;
            }

            public double getArchH(int idx)
            {
                return Convert.ToDouble(NSDI_BULD_HGs[idx].InnerText);
            }

            public Point[] getPosList(int idx)
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
            public void testGisBuildingService(string methodName, string serviceKey, string bbox, string pnu, string typeName = "F171", string maxFeature = "10", string resultType = "results", string srsName = "EPSG:5174")
            {
                this.setEndPointUrl(methodName, serviceKey, bbox, pnu, typeName, maxFeature, resultType, srsName);

                this.loadArchDataFromApiAsXml();

                Point lowerCorner = this.getMapLowerCorner();

                Point upperCorner = this.getMapUpperCorner();

                Point transformedLowerCorner = TransformCoordinate(lowerCorner, 5174, 4326);
                Console.WriteLine("Transformed Lower Corner");
                transformedLowerCorner.printString();

                Point transformedUpperCorner = TransformCoordinate(upperCorner, 5174, 4326);
                Console.WriteLine("Transformed Upper Corner");
                transformedUpperCorner.printString();

                Console.WriteLine("가로 길이: {0}", getDistanceBetweenPointsOfepsg4326(transformedLowerCorner.getX(), transformedLowerCorner.getY(), transformedUpperCorner.getX(), transformedLowerCorner.getY()));
                Console.WriteLine("세로 길이: {0}", getDistanceBetweenPointsOfepsg4326(transformedLowerCorner.getX(), transformedLowerCorner.getY(), transformedLowerCorner.getX(), transformedUpperCorner.getY()));

                this.readFeatureMembers();
                this.readPosLists();
                this.readArchHs();

                List<Point[]> pls = new List<Point[]>();
                List<double> hs = new List<double>();

                for (int i = 0; i<this.getFeatureMembersCnt(); i++)
                {
                    double h = Convert.ToDouble(NSDI_BULD_HGs[i].InnerText);
                    if (h > 0)
                    {
                        hs.Add(h);

                        Point[] pl = this.getPosList(i);
                        Point[] transformedPl = TransformCoordinate(pl, 5174, 4326);

                        // 프로그램상의 좌표계로 변환
                        // 지도 범위의 왼쪽 위를 기준으로 한다.
                        Point[] plOnSystem = calcIndexOnProg(transformedPl, transformedLowerCorner.getX(), transformedUpperCorner.getY());

                        pls.Add(plOnSystem);
                    }
                }

                archs = new Architecture[pls.Count];
                for(int i = 0; i<pls.Count; i++)
                {
                    archs[i] = new Architecture();
                    archs[i].define_Architecture(pls[i], hs[i]);
                }
            }

            public void testGisBuildingServiceByGui()
            {
                Point lowerCorner = this.getMapLowerCorner();

                Point upperCorner = this.getMapUpperCorner();

                Point transformedLowerCorner = TransformCoordinate(lowerCorner, 5174, 4326);
                Console.WriteLine("Transformed Lower Corner");
                transformedLowerCorner.printString();

                Point transformedUpperCorner = TransformCoordinate(upperCorner, 5174, 4326);
                Console.WriteLine("Transformed Upper Corner");
                transformedUpperCorner.printString();

                Console.WriteLine("가로 길이: {0}", getDistanceBetweenPointsOfepsg4326(transformedLowerCorner.getX(), transformedLowerCorner.getY(), transformedUpperCorner.getX(), transformedLowerCorner.getY()));
                Console.WriteLine("세로 길이: {0}", getDistanceBetweenPointsOfepsg4326(transformedLowerCorner.getX(), transformedLowerCorner.getY(), transformedLowerCorner.getX(), transformedUpperCorner.getY()));

                this.readFeatureMembers();
                this.readPosLists();
                this.readArchHs();

                List<Point[]> pls = new List<Point[]>();
                List<double> hs = new List<double>();

                for (int i = 0; i < this.getFeatureMembersCnt(); i++)
                {
                    double h = Convert.ToDouble(NSDI_BULD_HGs[i].InnerText);
                    if (h > 0)
                    {
                        hs.Add(h);

                        Point[] pl = this.getPosList(i);
                        Point[] transformedPl = TransformCoordinate(pl, 5174, 4326);

                        // 프로그램상의 좌표계로 변환
                        // 지도 범위의 왼쪽 위를 기준으로 한다.
                        Point[] plOnSystem = calcIndexOnProg(transformedPl, transformedLowerCorner.getX(), transformedUpperCorner.getY());

                        pls.Add(plOnSystem);
                    }
                }
            }
        }
    }
}
