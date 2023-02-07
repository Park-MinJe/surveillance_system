// using Internal;
using static surveillance_system.Program;
using System;
using System.Collections.Generic;
using System.Linq;

namespace surveillance_system
{
    public partial class Program
    {
        // Data Handler
        public static ArchCSVWriter aw = new ArchCSVWriter();
        public static ArchCSVReader ar = new ArchCSVReader();

        public static TargetCSVWriter tw = new TargetCSVWriter();
        public static TargetCSVReader tr = new TargetCSVReader();

        public static CctvCSVWriter cw = new CctvCSVWriter();
        public static CctvCSVReader cr = new CctvCSVReader();

        public static TargetLogCSVWriter tlog = new TargetLogCSVWriter();
        public static CctvLogCSVWriter clog = new CctvLogCSVWriter();

        public static OsmReader or = new OsmReader();
        public static GisBuildingService gbs = new GisBuildingService();

        static void Main(string[] args)
        {
            // 도로 정보

            // 230207 pmj
            // Open Api를 이용해 건물 정보 받아오기 test
            // MSTest v2 test project로 분기 예정
            Console.Write("사용 함수: ");
            string methodName = Console.ReadLine();
            Console.Write("서비스 키: ");
            string serviceKey = Console.ReadLine();
            Console.Write("Type Name: ");
            string typeName = Console.ReadLine();
            Console.Write("bbox: ");
            string bbox = Console.ReadLine();
            Console.Write("Pnu: ");
            string pnu = Console.ReadLine();
            Console.Write("검색 범위: ");
            string maxFeature = Console.ReadLine();
            Console.Write("Result Type: ");
            string resultType = Console.ReadLine();
            Console.Write("Srs Name: ");
            string srsName = Console.ReadLine();

            /*gbs.testGisBuildingService(methodName, serviceKey, bbox, pnu, typeName, maxFeature, resultType, srsName);
            foreach(Architecture arch in archs)
            {
                Console.WriteLine();
                arch.printArchInfo();
            }*/

            gbs.setEndPointUrl(methodName, serviceKey, bbox, pnu, typeName, maxFeature, resultType, srsName);
            gbs.loadArchDataFromApiAsXml();

            int nArch = 0, nCctv = 0, nPed = 0, nCar = 0, 
                cctvMode = 0, 
                numberOfCCTVSet = 1, 
                simulationTimesForCCTVSet = 100;
            string inputNArchOption = "N", 
                inputNCctvOption = "N", 
                inputCctvRotate = "N", 
                inputNPedOption = "N",
                inputNCarOption = "N",
                InputcreateCSV = "N";

            List<double> successRates = new List<double>();
            //List<CCTV[]> cctvAtSim = new List<CCTV[]>();
            //List<Pedestrian[]> pedestrianAtSim = new List<Pedestrian[]>();
            //List<Car[]> carAtSim = new List<Car[]>();

            //List<int[,]> cctvPosAtSim = new List<int[,]>();

            while (true)
            {
                Console.Write("\ninput Number of CCTV set( 1~ ): ");
                numberOfCCTVSet = Convert.ToInt32(Console.ReadLine());

                if (numberOfCCTVSet < 1) { continue; }
                else { break; }
            }

            while (true)
            {
                Console.Write("input Simulation times for Each CCTV set( 1~ ): ");
                simulationTimesForCCTVSet = Convert.ToInt32(Console.ReadLine());

                if(simulationTimesForCCTVSet < 1) { continue; }
                else { break; }
            }

            Console.WriteLine("\ninput CCTV collocating mode: ");
            while (true)
            {
                Console.Write("(0: pos cctv as grid    1: pos cctv as random at DST    2: pos cctv as random in int)? ");
                cctvMode = Convert.ToInt32(Console.ReadLine());

                if (cctvMode == 0 || cctvMode == 1 || cctvMode == 2) { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("\nDo you want to rotate cctv(Y/N)? ");
                inputCctvRotate = Console.ReadLine();

                if (inputCctvRotate == "N" || inputCctvRotate == "n") { break; }
                else if (inputCctvRotate == "Y" || inputCctvRotate == "y") { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("\nDo you want to enter Architecture Numbers(Y/N)? ");
                inputNArchOption = Console.ReadLine();

                if (inputNArchOption == "N" || inputNArchOption == "n") { break; }
                else if (inputNArchOption == "Y" || inputNArchOption == "y") { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("Do you want to enter CCTV Numbers(Y/N)? ");
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

            while (true)
            {
                Console.Write("Do you wand results as csv file(Y/N)? ");
                InputcreateCSV = Console.ReadLine();

                if (InputcreateCSV == "N" || InputcreateCSV == "n") { break; }
                else if (InputcreateCSV == "Y" || InputcreateCSV == "y") { break; }
                else { continue; }
            }

            Console.WriteLine("");

            if (inputNArchOption == "Y" || inputNArchOption == "y")
            {
                Console.Write("input number of Architecture: ");
                nArch = Convert.ToInt32(Console.ReadLine());
            }

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
                sims[i].setcreateCSV(InputcreateCSV);
                sims[i].setCctvFixMode(inputCctvRotate);
                sims[i].setgetArchNumFromUser(inputNArchOption);
                sims[i].setgetCCTVNumFromUser(inputNCctvOption);
                sims[i].setgetPedNumFromUser(inputNPedOption);
                sims[i].setgetCarNumFromUser(inputNCarOption);

                sims[i].initNArch(nArch);
                sims[i].initNCctv(nCctv);
                sims[i].initNPed(nPed);
                sims[i].initNCar(nCar);

                sims[i].initVariables();

                sims[i].initTimer();

                // sims[i].startTimer();
                sims[i].initMap(cctvMode);
                sims[i].initialArchsToCSV(i);
                sims[i].initialPedsToCSV(i);
                sims[i].initialCarsToCSV(i);
                //pedestrianAtSim.Add(peds);
                //carAtSim.Add(cars);
                // sims[i].stopTimer();
            }

            for (int i = 0; i < numberOfCCTVSet; i++)
            {
                double successRateForCCTVSet = 0.0;
                for (int j = 0; j < simulationTimesForCCTVSet; j++)
                {
                    sims[j].initPedsWithCSV(j);
                    sims[j].initCarsWithCSV(j);
                    sims[j].startTimer();
                    if (j == 0)
                    {
                        if (i == 0 && numberOfCCTVSet > 1)
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
                                    road.setCCTVbyRandomInDST(sims[j].getNCCTV());
                                    break;
                                case 2:
                                    road.setCCTVbyRandomInInt(sims[j].getNCCTV());
                                    break;
                            }
                        }
                        //cctvAtSim.Add(cctvs);
                        cw.initialCctvsToCSV(i);
                        //cctvPosAtSim.Add(road.cctvPos);
                    }
                    road.printAllPos();

                    Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Simulatioin Start " + i + " - " + j);
                    sims[j].operateSim();
                    sims[j].stopTimer();
                    sims[j].TraceLogToCSV(i, j);
                    double successRate = sims[j].printResultRate();
                    successRateForCCTVSet += successRate;
                    //sims[j].printDetectedResults();
                    sims[j].DetectedResultsToCSV(i, j);
                    sims[j].ShadowedLogToCSV(i, j);

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

            Console.WriteLine("====== CCTV set {0} ======", bestCCTVIdx);
            road.setCctvswithCSV(bestCCTVIdx);
            road.printPos(road.cctvPos);
        }
    }
}
