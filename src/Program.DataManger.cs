// using Internal;
using static surveillance_system.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace surveillance_system
{
    public partial class Program
    {
        public class DataManger
        {
            public void writeRoadToXml(string path)
            {
                // write on Peds.xml
                using (StreamWriter wr = new StreamWriter(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Road));
                    xs.Serialize(wr, road);
                }
            }
            public Road readRoadFromXml(string path)
            {
                using (var reader = new StreamReader(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Road));
                    Road roadFromXml = (Road)xs.Deserialize(reader);

                    // Console.WriteLine("{0}, {1}", emp.Id, emp.Name);

                    return roadFromXml;
                }
            }

            public void writePedsToXml(string path)
            {
                // write on Peds.xml
                using (StreamWriter wr = new StreamWriter(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Pedestrian[]));
                    xs.Serialize(wr, peds);
                }
            }
            public Pedestrian[] readPedsFromXml(string path)
            {
                using (var reader = new StreamReader(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Pedestrian[]));
                    Pedestrian[] pedsFromXml = (Pedestrian[])xs.Deserialize(reader);

                    // Console.WriteLine("{0}, {1}", emp.Id, emp.Name);

                    return pedsFromXml;
                }
            }

            public void writeCarsToXml(string path)
            {
                // write on Cars.xml
                using (StreamWriter wr = new StreamWriter(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Car[]));
                    xs.Serialize(wr, cars);
                }
            }
            public Car[] readCarsFromXml(string path)
            {
                using (var reader = new StreamReader(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Car[]));
                    Car[] carsFromXml = (Car[])xs.Deserialize(reader);

                    // Console.WriteLine("{0}, {1}", emp.Id, emp.Name);

                    return carsFromXml;
                }
            }

            public void writeCctvsToXml(string path)
            {
                // write on Cctvs.xml
                using (StreamWriter wr = new StreamWriter(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(CCTV[]));
                    xs.Serialize(wr, cctvs);
                }
            }
            public CCTV[] readCctvsFromXml(string path)
            {
                using (var reader = new StreamReader(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(CCTV[]));
                    CCTV[] cctvsFromXml = (CCTV[])xs.Deserialize(reader);

                    // Console.WriteLine("{0}, {1}", emp.Id, emp.Name);

                    return cctvsFromXml;
                }
            }
        }
    }
}
