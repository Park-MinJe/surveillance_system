using System;
using System.Collections.Generic;
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
    }
}
