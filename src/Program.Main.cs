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
        // Main에서 사용할 Object To Xml 객체
        public static DataManger ObjToXml = new DataManger();

        static void Main(string[] args)
        {
            int nCctv = 0, nPed = 0, nCar = 0, 
                cctvMode = 0, 
                numberOfCCTVSet = 1, 
                simulationTimesForCCTVSet = 10;
            string inputNCctvOption = "N",
                inputNPedOption = "N",
                inputNCarOption = "N";

            List<double> successRates = new List<double>();
            List<CCTV[]> cctvAtSim = new List<CCTV[]>();
            // List<Pedestrian[]> pedestrianAtSim = new List<Pedestrian[]>();
            // List<Car[]> carAtSim = new List<Car[]>();

            List<Road> roadAtSim = new List<Road>();

            Console.Write("input Number of CCTV set: ");
            numberOfCCTVSet = Convert.ToInt32(Console.ReadLine());

            Console.Write("input Simulation times for Each CCTV set: ");
            simulationTimesForCCTVSet = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\ninput CCTV collocating mode: ");
            while (true)
            {
                Console.Write("(0: pos cctv as grid    1: pos cctv as random)? ");
                cctvMode = Convert.ToInt32(Console.ReadLine());

                if (cctvMode == 0 || cctvMode == 1 || cctvMode == 2) { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("\nDo you want to enter CCTV Numbers(Y/N)? ");
                inputNCctvOption = Console.ReadLine();

                if (inputNCctvOption == "N" || inputNCctvOption == "n") { break; }
                else if (inputNCctvOption == "Y" || inputNCctvOption == "y") { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("Do you want to enter Pedestrian Numbers(Y/N)? ");
                inputNPedOption = Console.ReadLine();

                if (inputNPedOption == "N" || inputNPedOption == "n") { break; }
                else if (inputNPedOption == "Y" || inputNPedOption == "y") { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("Do you want to enter Car Numbers(Y/N)? ");
                inputNCarOption = Console.ReadLine();

                if (inputNCarOption == "N" || inputNCarOption == "n") { break; }
                else if (inputNCarOption == "Y" || inputNCarOption == "y") { break; }
                else { continue; }
            }

            Console.WriteLine("");

            if (inputNCctvOption == "Y" || inputNCctvOption == "y")
            {
                Console.Write("input number of CCTV: ");
                nCctv = Convert.ToInt32(Console.ReadLine());
            }

            if (inputNPedOption == "Y" || inputNPedOption == "y")
            {
                Console.Write("input number of Pedestrian: ");
                nPed = Convert.ToInt32(Console.ReadLine());
            }

            if (inputNCarOption == "Y" || inputNCarOption == "y")
            {
                Console.Write("input number of Car: ");
                nCar = Convert.ToInt32(Console.ReadLine());
            }


            Simulator[] sims = new Simulator[simulationTimesForCCTVSet];
            // s.simulateAll(cctvMode);
            for(int i = 0; i < simulationTimesForCCTVSet; i++)
            {
                sims[i] = new Simulator();
                sims[i].setIdx(i);

                sims[i].setgetCCTVNumFromUser(inputNCctvOption);
                sims[i].setgetPedNumFromUser(inputNPedOption);
                sims[i].setgetCarNumFromUser(inputNCarOption);

                sims[i].initNCctv(nCctv);
                sims[i].initNPed(nPed);
                sims[i].initNCar(nCar);

                sims[i].initVariables();

                sims[i].initTimer();

                // sims[i].startTimer();
                sims[i].idx = i;
                sims[i].initMap(cctvMode);
                roadAtSim.Add(road);
                // 다차원 배열로 인해 serialize 어려움
                // writeInitialRoadToXML(i);

                // pedestrianAtSim.Add(peds);
                writeInitialPedsToXML(i);
                // writePedsToXML(i);

                // carAtSim.Add(cars);
                writeInitialCarsToXML(i);
                // writeCarsToXML(i);

                // sims[i].stopTimer();
            }

            for (int i = 0; i < numberOfCCTVSet; i++)
            {
                double successRateForCCTVSet = 0.0;
                for (int j = 0; j < simulationTimesForCCTVSet; j++)
                {
                    // 다차원 배열로 인해 serialize 어려움
                    road = roadAtSim[j];
                    // road = readInitialRoadFromXML(j);

                    peds = readInitialPedsFromXML(j);
                    cars = readInitialCarsFromXML(j);

                    if (j == 0)
                    {
                        if (i == 0 && numberOfCCTVSet != 1)
                        {
                            road.setCCTV(sims[j].getNCCTV(), road.width, road.lane_num);
                        }
                        else
                        {
                            switch (cctvMode)
                            {
                                case 0:
                                    road.setCCTV(sims[j].getNCCTV(), road.width, road.lane_num);
                                    break;
                                case 1:
                                    road.setCCTVForBrute(sims[j].getNCCTV());
                                    break;
                            }
                        }

                        cctvAtSim.Add(cctvs);
                        // writeInitialCctvsToXML(i);
                        // writeCctvsToXML(i);
                    }
                    else
                    {
                        // 다차원 배열로 인해 serialize 어려움
                        cctvs = cctvAtSim[i];
                    }

                    sims[j].startTimer();
                    Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Simulatioin Start " + i + " - " + j);
                    sims[j].operateSim();
                    sims[j].stopTimer();
                    sims[j].printResultAsCSV();
                    double successRate = sims[j].printResultRate();
                    successRateForCCTVSet += successRate;
                    // s.printDetectedResults();

                    sims[j].resetTimer();
                }
                successRates.Add(successRateForCCTVSet / simulationTimesForCCTVSet);
            }

            Console.WriteLine("\n\n====== Simulation Results ======");
            Console.WriteLine("print index of CCTV set and Target detected Rate\n");
            for (int i = 0; i< successRates.Count ; i++)
            {
                Console.WriteLine("CCTV set {0}\t{1:F2}%", i, successRates[i]);
                Console.WriteLine();
            }

            Console.WriteLine("\n\n====== Best CCTV set ======");
            int bestCCTVIdx = successRates.IndexOf(successRates.Max());

            for (int i = 0; i < 52; i++)
            {
                Console.Write("{0, 2}", i);
            }
            Console.WriteLine();
            for (int i = 0; i < 52; i++)
            {
                Console.Write("{0, 2}", i);

                for (int j = 0; j < 52; j++)
                {
                    if (road.cctvPos[i, j] <= 0)
                        // Console.Write("{0, 2}", " ", cctvPos[i,j]);
                        Console.Write("{0, 2}", " ");
                    else
                        // Console.Write("{0, 2}", "*", cctvPos[i,j]);
                        Console.Write("{0, 2}", roadAtSim[bestCCTVIdx].cctvPos[i, j]);

                }
                Console.WriteLine();
            }
        }



        /* --------------------------------------
         * XML 함수 - Write
        -------------------------------------- */
        public static void writeInitialRoadToXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\InitialPeds" + this.idx + ".xml";
            string path = @"data\InitialRoad" + idx + ".xml";
            ObjToXml.writeRoadToXml(path);
        }

        public static void writeInitialPedsToXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\InitialPeds" + this.idx + ".xml";
            string path = @"data\InitialPeds" + idx + ".xml";
            ObjToXml.writePedsToXml(path);
        }
        public static void writePedsToXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\Peds" + this.idx + ".xml";
            string path = @"data\Peds" + idx + ".xml";
            ObjToXml.writePedsToXml(path);
        }

        public static void writeInitialCarsToXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\InitialCars" + this.idx + ".xml";
            string path = @"data\InitialCars" + idx + ".xml";
            ObjToXml.writeCarsToXml(path);
        }
        public static void writeCarsToXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\Cars" + this.idx + ".xml";
            string path = @"data\Cars" + idx + ".xml";
            ObjToXml.writeCarsToXml(path);
        }

        public static void writeInitialCctvsToXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\Cctvs" + this.idx + ".xml";
            string path = @"data\InitialCctvs" + idx + ".xml";
            ObjToXml.writeCctvsToXml(path);
        }
        public static void writeCctvsToXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\Cctvs" + this.idx + ".xml";
            string path = @"data\Cctvs" + idx + ".xml";
            ObjToXml.writeCctvsToXml(path);
        }

        /* --------------------------------------
         * XML 함수 - Read
        -------------------------------------- */
        public static Road readInitialRoadFromXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\InitialPeds" + this.idx + ".xml";
            string path = @"data\InitialRoad" + idx + ".xml";
            return ObjToXml.readRoadFromXml(path);
        }

        public static Pedestrian[] readInitialPedsFromXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\InitialPeds" + this.idx + ".xml";
            string path = @"data\InitialPeds" + idx + ".xml";
            return ObjToXml.readPedsFromXml(path);
        }

        public static Car[] readInitialCarsFromXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\InitialCars" + this.idx + ".xml";
            string path = @"data\InitialCars" + idx + ".xml";
            return ObjToXml.readCarsFromXml(path);
        }

        public static CCTV[] readInitialCctvsFromXML(int idx)
        {
            // string path = @"C:\Users\win11\학교\22-계절\개별연구\2021-2_SurveillanceSystem-main\surveillance_system\data\Cctvs" + this.idx + ".xml";
            string path = @"data\InitialCctvs" + idx + ".xml";
            return ObjToXml.readCctvsFromXml(path);
        }
    }
}
