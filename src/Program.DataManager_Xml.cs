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
