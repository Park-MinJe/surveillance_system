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

            public OsmReader(string fileName)
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
            private string apiEndPoint = "http://apis.data.go.kr/1611000/nsdi/GisBuildingService";
            private string serviceKey = "?ServiceKey=";     // 필수
            private string typeName = "&typename=";
            private string bbox = "&bbox=";
            private string pnu = "&pnu=";
            private string maxFeature = "&maxFeatures=";
            private string resultType = "&resultType=";
            private string srsName = "&srsName=";

            private bool IsSetServiceKeyCalled = false;
            private bool IsSetTypeNameCalled = false;
            private bool IsSetBBoxCalled = false;
            private bool IsSetPnuCalled = false;
            private bool IsSetMaxFeatureCalled = false;
            private bool IsSetResultTypeCalled = false;
            private bool IsSetSrsNameCalled = false;

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

            public void testGisBuildingService(string methodName, string serviceKey, string bbox, string typeName = "F171", string maxFeature = "10", string resultType = "results", string srsName = "EPSG:5174")
            {
                this.setServiceKey(serviceKey);
                this.setBBox(bbox);
                //this.setPnu(pnu);
                this.setTypeName(typeName);
                this.setMaxFeature(maxFeature);
                this.setResultType(resultType);
                this.setSrsName(srsName);

                string url = this.apiEndPoint + methodName + this.serviceKey + this.typeName + this.bbox + this.maxFeature + this.resultType + this.srsName;
                // debug;
                Console.WriteLine(url);

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                string results = string.Empty;
                HttpWebResponse response;
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    results = reader.ReadToEnd();
                }

                Console.WriteLine(results);

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(results);
                XmlNodeList gml_lowerCorner = xml.GetElementsByTagName("gml:lowerCorner");
                double[,] lowerCorner = new double[gml_lowerCorner.Count, 2];
                XmlNodeList gml_upperCorner = xml.GetElementsByTagName("gml:upperCorner");
                double[,] upperCorner = new double[gml_upperCorner.Count, 2];

                for (int i = 0; i < gml_lowerCorner.Count; i++)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Raw Lower Conrer");
                    string phrase = gml_lowerCorner[i].InnerText;
                    Console.WriteLine("{0}", phrase);
                    string[] words = phrase.Split(' ');
                    lowerCorner[i, 0] = Convert.ToDouble(words[0]);
                    lowerCorner[i, 1] = Convert.ToDouble(words[1]);

                    Console.WriteLine("Raw Upper Conrer");
                    phrase = gml_upperCorner[i].InnerText;
                    Console.WriteLine("{0}", phrase);
                    words = phrase.Split(" ");
                    upperCorner[i, 0] = Convert.ToDouble(words[0]);
                    upperCorner[i, 1] = Convert.ToDouble(words[1]);

                    double[,] transformedCorners = TransformCoordinate(lowerCorner[i, 0], lowerCorner[i, 1], upperCorner[i, 0], upperCorner[i, 1]);
                    Console.WriteLine("Transformed Lower Corner");
                    Console.WriteLine("x: {0}\ty:{1}", transformedCorners[0, 0], transformedCorners[0, 1]);
                    Console.WriteLine("Transformed Upper Corner");
                    Console.WriteLine("x: {0}\ty:{1}", transformedCorners[1, 0], transformedCorners[1, 1]);
                }

                XmlNodeList gml_featureMembers = xml.GetElementsByTagName("gml:featureMember");
                XmlNodeList gml_posLists = xml.GetElementsByTagName("gml:posList");
                XmlNodeList NSDI_BULD_HGs = xml.GetElementsByTagName("NSDI:BULD_HG");

                for(int i = 0; i<gml_featureMembers.Count; i++)
                {
                    if (Convert.ToDouble(NSDI_BULD_HGs[i].InnerText) > 0)
                    {
                        Console.WriteLine("\n");
                        Console.WriteLine("posList:");
                        string phrase = gml_posLists[i].InnerText;
                        Console.WriteLine("{0}\n", phrase);
                        string[] words = phrase.Split(" ");
                        double[] pl = new double[words.Length];
                        for(int j = 0;j < words.Length; j++)
                        {
                            pl[j] = Convert.ToDouble(words[j]);
                            Console.Write("{0} ", pl[j]);
                        }
                        Console.WriteLine();
                        TransformCoordinate(pl);
                        for(int j = 0; j < pl.Length / 2; j++)
                        {
                            Console.WriteLine("x: {0}, y: {1}", pl[j * 2], pl[j * 2 + 1]);
                        }

                        Console.WriteLine("\nBuilding height: ");
                        Console.WriteLine("{0}", NSDI_BULD_HGs[i].InnerText);
                    }
                }
            }
        }
    }
}
