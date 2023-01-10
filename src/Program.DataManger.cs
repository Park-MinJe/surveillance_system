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
            public void writePedsToXml(string path)
            {
                // write on Peds.xml
                using (StreamWriter wr = new StreamWriter(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Pedestrian[]));
                    xs.Serialize(wr, peds);
                }
            }
            public void readPedsFromXml() { }

            public void writeCarsToXml(string path)
            {
                // write on Cars.xml
                using (StreamWriter wr = new StreamWriter(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Car[]));
                    xs.Serialize(wr, cars);
                }
            }
            public void readCarsFromXml() { }

            public void writeCctvsToXml(string path)
            {
                // write on Cctvs.xml
                using (StreamWriter wr = new StreamWriter(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(CCTV[]));
                    xs.Serialize(wr, cctvs);
                }
            }
            public void readCctvsFromXml() { }
        }
    }
}
