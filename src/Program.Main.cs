// using Internal;
using static surveillance_system.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using Tutorial;

namespace surveillance_system
{
    public partial class Program
    {
        // Data Handler
        public static BuildingCSVWriter bw = new BuildingCSVWriter();
        public static BuildingCSVReader br = new BuildingCSVReader();

        public static TargetCSVWriter tw = new TargetCSVWriter();
        public static TargetCSVReader tr = new TargetCSVReader();

        public static CctvCSVWriter cw = new CctvCSVWriter();
        public static CctvCSVReader cr = new CctvCSVReader();

        public static TargetLogCSVWriter tlog = new TargetLogCSVWriter();
        public static CctvLogCSVWriter clog = new CctvLogCSVWriter();

        // api로부터 입력 받는 interface로 통합
        // 230228 1018 pmj
        //public static OsmReader or = new OsmReader();
        //public static GisBuildingService gbs = new GisBuildingService();
        //public static VworldService vs = new VworldService();

        public static InputFromApi buildingfromApi = new VworldService();

        public static GuiManager gm = new GuiManager();

        //public static GraphicManager_slik graphic_Silk = new GraphicManager_slik();
        //public static GraphicManager_OpneTK graphic = new GraphicManager_OpneTK();

        static void Main(string[] args)
        {
            int operatingMode;          // 0: 디지털 모델링   1: 시뮬레이션

            Console.WriteLine("\ninput Operating Mode: ");
            while (true)
            {
                Console.Write("(0: 디지털 모델링   1: 시뮬레이션)? ");
                operatingMode = Convert.ToInt32(Console.ReadLine());

                if (operatingMode == 0 || operatingMode == 1) { break; }
                else { continue; }
            }

            if(operatingMode == 0)
            {
                operateMapping();
            }

            else if(operatingMode == 1)
            {
                //operateSimulation();
            }
        }

        static void vworldTest()
        {
            // 230227 1649 pmj
            // vworld api
            if (buildingfromApi is VworldService)
            {
                buildingfromApi.setRequest("GetFeature");
                buildingfromApi.setServiceKey("9A183823-9348-31F4-9B73-C38E3C014311");
                buildingfromApi.setTypename("lt_c_bldginfo");
                buildingfromApi.setBbox("14135114.246047439,4517725.3072321005,14135687.864251547,4518285.658803624");
                buildingfromApi.setSrsname("EPSG:3857");
                buildingfromApi.setEndPointUrl();

                buildingfromApi.loadBuildingDataFromApiAsXml();
                buildingfromApi.readFeatureMembers();
                buildingfromApi.readPosLists();
                buildingfromApi.readBuildingHs();

                Point lower = TransformCoordinate(buildingfromApi.getMapLowerCorner(), 3857, 4326);
                lower.printString();

                Point upper = TransformCoordinate(buildingfromApi.getMapUpperCorner(), 3857, 4326);
                upper.printString();

                int building_cnt = buildingfromApi.getFeatureMembersCnt();
                Console.WriteLine("building_cnt: {0}", building_cnt);
                for (int i = 0; i < building_cnt; i++)
                {
                    if (buildingfromApi.getBuildingHByIdx(i) == 53)
                    {
                        Console.WriteLine("H: {0}", buildingfromApi.getBuildingHByIdx(i));

                        Point[] tmp = TransformCoordinate(buildingfromApi.getPosListByIdx(i), 3857, 4326);
                        foreach (Point p in tmp)
                        {
                            p.printString();
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        // 230317 박민제
        // 실제 데이터를 이용한 디지털 매핑
        static void operateMapping()
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
            foreach(Building building in buildings)
            {
                Console.WriteLine();
                building.printBuildingInfo();
            }*/

            if (buildingfromApi is GisBuildingService)
                (buildingfromApi as GisBuildingService).setEndPointUrl(methodName, serviceKey, bbox, pnu, typeName, maxFeature, resultType, srsName);
            else if (buildingfromApi is VworldService)
            {
                buildingfromApi.setRequest(methodName);
                buildingfromApi.setServiceKey(serviceKey);
                buildingfromApi.setTypename(typeName);
                buildingfromApi.setBbox(bbox);
                buildingfromApi.setSrsname(srsName);

                buildingfromApi.setEndPointUrl();
            }
            buildingfromApi.loadBuildingDataFromApiAsXml();

            int cctvMode = 3;

            DigitalMappingModule mappingModule = new DigitalMappingModule();
            mappingModule.initVariables();
            mappingModule.initMap(cctvMode, buildingfromApi.getMapUpperCorner(), buildingfromApi.getMapLowerCorner());
            bw.BuildingsToCSV("DigitalMappingResult.Buildings");

            tw.PedsToCSV("DigitalMappingResult.Peds");
            tw.CarsToCSV("DigitalMappingResult.Cars");

            road.setCCTVbyRealWorldData(mappingModule.N_CCTV);
            cw.CctvsToCSV("DigitalMappingResult.CctvSet");

            //debug
            road.printAllPos();
        }



        // 230317 박민제
        // 디지털 매핑된 것을 기반으로 시뮬레이션 구현

        // 기존 시뮬레이션 부분
        //static void operateSimulation()
        //{
        //    // 230221 1135 pmj6823
        //    // graphic tutorial
        //    //graphic_Silk.graphicTutorial();
        //    //SilkProgram sp = new SilkProgram();
        //    //sp.tutorial_2_2();

        //    /*using (GraphicManager_OpneTK graphic = new GraphicManager_OpneTK(800, 600, "LearnOpenTK"))
        //    {
        //        graphic.Run();
        //    }*/

        //    // 도로 정보

        //    // 230207 pmj
        //    // Open Api를 이용해 건물 정보 받아오기 test
        //    // MSTest v2 test project로 분기 예정
        //    Console.Write("사용 함수: ");
        //    string methodName = Console.ReadLine();
        //    Console.Write("서비스 키: ");
        //    string serviceKey = Console.ReadLine();
        //    Console.Write("Type Name: ");
        //    string typeName = Console.ReadLine();
        //    Console.Write("bbox: ");
        //    string bbox = Console.ReadLine();
        //    Console.Write("Pnu: ");
        //    string pnu = Console.ReadLine();
        //    Console.Write("검색 범위: ");
        //    string maxFeature = Console.ReadLine();
        //    Console.Write("Result Type: ");
        //    string resultType = Console.ReadLine();
        //    Console.Write("Srs Name: ");
        //    string srsName = Console.ReadLine();

        //    /*gbs.testGisBuildingService(methodName, serviceKey, bbox, pnu, typeName, maxFeature, resultType, srsName);
        //    foreach(Building building in buildings)
        //    {
        //        Console.WriteLine();
        //        building.printBuildingInfo();
        //    }*/

        //    if (buildingfromApi is GisBuildingService)
        //        (buildingfromApi as GisBuildingService).setEndPointUrl(methodName, serviceKey, bbox, pnu, typeName, maxFeature, resultType, srsName);
        //    else if (buildingfromApi is VworldService)
        //    {
        //        buildingfromApi.setRequest(methodName);
        //        buildingfromApi.setServiceKey(serviceKey);
        //        buildingfromApi.setTypename(typeName);
        //        buildingfromApi.setBbox(bbox);
        //        buildingfromApi.setSrsname(srsName);

        //        buildingfromApi.setEndPointUrl();
        //    }
        //    buildingfromApi.loadBuildingDataFromApiAsXml();

        //    int nBuilding = 0, nCctv = 0, nPed = 0, nCar = 0,
        //        cctvMode = 0,
        //        numberOfCCTVSet = 1,
        //        simulationTimesForCCTVSet = 100;
        //    string inputNBuildingOption = "N",
        //        inputNCctvOption = "N",
        //        inputCctvRotate = "N",
        //        inputNPedOption = "N",
        //        inputNCarOption = "N",
        //        InputcreateCSV = "N";

        //    List<double> successRates = new List<double>();
        //    //List<CCTV[]> cctvAtSim = new List<CCTV[]>();
        //    //List<Pedestrian[]> pedestrianAtSim = new List<Pedestrian[]>();
        //    //List<Car[]> carAtSim = new List<Car[]>();

        //    //List<int[,]> cctvPosAtSim = new List<int[,]>();

        //    while (true)
        //    {
        //        Console.Write("\ninput Number of CCTV set( 1~ ): ");
        //        numberOfCCTVSet = Convert.ToInt32(Console.ReadLine());

        //        if (numberOfCCTVSet < 1) { continue; }
        //        else { break; }
        //    }

        //    while (true)
        //    {
        //        Console.Write("input Simulation times for Each CCTV set( 1~ ): ");
        //        simulationTimesForCCTVSet = Convert.ToInt32(Console.ReadLine());

        //        if (simulationTimesForCCTVSet < 1) { continue; }
        //        else { break; }
        //    }

        //    Console.WriteLine("\ninput CCTV collocating mode: ");
        //    while (true)
        //    {
        //        Console.Write("(0: pos cctv as grid    1: pos cctv as random at DST    2: pos cctv as random in int)? ");
        //        cctvMode = Convert.ToInt32(Console.ReadLine());

        //        // 230314 박민제
        //        // cctvMode 3은 realWorldCctvFromCSV() 함수 테스팅을 위함 임시 모드로, 디지털 매핑 과정으로 분기 예정
        //        if (cctvMode == 0 || cctvMode == 1 || cctvMode == 2 || cctvMode == 3) { break; }
        //        else { continue; }
        //    }

        //    while (true)
        //    {
        //        Console.Write("\nDo you want to rotate cctv(Y/N)? ");
        //        inputCctvRotate = Console.ReadLine();

        //        if (inputCctvRotate == "N" || inputCctvRotate == "n") { break; }
        //        else if (inputCctvRotate == "Y" || inputCctvRotate == "y") { break; }
        //        else { continue; }
        //    }

        //    while (true)
        //    {
        //        Console.Write("\nDo you want to enter Architecture Numbers(Y/N)? ");
        //        inputNBuildingOption = Console.ReadLine();

        //        if (inputNBuildingOption == "N" || inputNBuildingOption == "n") { break; }
        //        else if (inputNBuildingOption == "Y" || inputNBuildingOption == "y") { break; }
        //        else { continue; }
        //    }

        //    while (true)
        //    {
        //        Console.Write("Do you want to enter CCTV Numbers(Y/N)? ");
        //        inputNCctvOption = Console.ReadLine();

        //        if (inputNCctvOption == "N" || inputNCctvOption == "n") { break; }
        //        else if (inputNCctvOption == "Y" || inputNCctvOption == "y") { break; }
        //        else { continue; }
        //    }

        //    while (true)
        //    {
        //        Console.Write("Do you want to enter Pedestrian Numbers(Y/N)? ");
        //        inputNPedOption = Console.ReadLine();

        //        if (inputNPedOption == "N" || inputNPedOption == "n") { break; }
        //        else if (inputNPedOption == "Y" || inputNPedOption == "y") { break; }
        //        else { continue; }
        //    }

        //    while (true)
        //    {
        //        Console.Write("Do you want to enter Car Numbers(Y/N)? ");
        //        inputNCarOption = Console.ReadLine();

        //        if (inputNCarOption == "N" || inputNCarOption == "n") { break; }
        //        else if (inputNCarOption == "Y" || inputNCarOption == "y") { break; }
        //        else { continue; }
        //    }

        //    while (true)
        //    {
        //        Console.Write("Do you wand results as csv file(Y/N)? ");
        //        InputcreateCSV = Console.ReadLine();

        //        if (InputcreateCSV == "N" || InputcreateCSV == "n") { break; }
        //        else if (InputcreateCSV == "Y" || InputcreateCSV == "y") { break; }
        //        else { continue; }
        //    }

        //    Console.WriteLine("");

        //    if (inputNBuildingOption == "Y" || inputNBuildingOption == "y")
        //    {
        //        Console.Write("input number of Building: ");
        //        nBuilding = Convert.ToInt32(Console.ReadLine());
        //    }

        //    if (inputNCctvOption == "Y" || inputNCctvOption == "y")
        //    {
        //        Console.Write("input number of CCTV: ");
        //        nCctv = Convert.ToInt32(Console.ReadLine());
        //    }

        //    if (inputNPedOption == "Y" || inputNPedOption == "y")
        //    {
        //        Console.Write("input number of Pedestrian: ");
        //        nPed = Convert.ToInt32(Console.ReadLine());
        //    }

        //    if (inputNCarOption == "Y" || inputNCarOption == "y")
        //    {
        //        Console.Write("input number of Car: ");
        //        nCar = Convert.ToInt32(Console.ReadLine());
        //    }


        //    SimulationModel[] sims = new SimulationModel[simulationTimesForCCTVSet];
        //    // s.simulateAll(cctvMode);
        //    for (int i = 0; i < simulationTimesForCCTVSet; i++)
        //    {
        //        sims[i] = new SimulationModel();
        //        sims[i].setcreateCSV(InputcreateCSV);
        //        sims[i].setCctvFixMode(inputCctvRotate);
        //        sims[i].setgetBuildingNumFromUser(inputNBuildingOption);
        //        sims[i].setgetCCTVNumFromUser(inputNCctvOption);
        //        sims[i].setgetPedNumFromUser(inputNPedOption);
        //        sims[i].setgetCarNumFromUser(inputNCarOption);

        //        sims[i].initNBuilding(nBuilding);
        //        sims[i].initNCctv(nCctv);
        //        sims[i].initNPed(nPed);
        //        sims[i].initNCar(nCar);

        //        sims[i].initVariables();

        //        sims[i].initTimer();

        //        // sims[i].startTimer();
        //        sims[i].initMap(cctvMode, buildingfromApi.getMapUpperCorner(), buildingfromApi.getMapLowerCorner());
        //        sims[i].initialBuildingsToCSV(i);
        //        sims[i].initialPedsToCSV(i);
        //        sims[i].initialCarsToCSV(i);
        //        //pedestrianAtSim.Add(peds);
        //        //carAtSim.Add(cars);
        //        //sims[i].stopTimer();
        //    }

        //    for (int i = 0; i < numberOfCCTVSet; i++)
        //    {
        //        double successRateForCCTVSet = 0.0;
        //        for (int j = 0; j < simulationTimesForCCTVSet; j++)
        //        {
        //            sims[j].initPedsWithCSV(j);
        //            sims[j].initCarsWithCSV(j);
        //            sims[j].startTimer();
        //            if (j == 0)
        //            {
        //                if (i == 0 && numberOfCCTVSet > 1)
        //                {
        //                    road.setCCTV(sims[j].getNCCTV(), road.width, road.lane_num);
        //                }
        //                else
        //                {
        //                    switch (cctvMode)
        //                    {
        //                        case 0:
        //                            road.setCCTV(sims[j].getNCCTV(), road.width, road.lane_num);
        //                            break;
        //                        case 1:
        //                            road.setCCTVbyRandomInDST(sims[j].getNCCTV());
        //                            break;
        //                        case 2:
        //                            road.setCCTVbyRandomInInt(sims[j].getNCCTV());
        //                            break;
        //                        case 3:
        //                            road.setCCTVbyRealWorldData(sims[j].getNCCTV());
        //                            break;
        //                    }
        //                }
        //                //cctvAtSim.Add(cctvs);

        //                // 230317 박민제
        //                // Digital Mapping Module에서 처리
        //                /*cw.setCctvCSVWriter(sims[j].getNCCTV());
        //                cw.initialCctvsToCSV(i);*/


        //                //cctvPosAtSim.Add(road.cctvPos);
        //            }
        //            road.printAllPos();

        //            Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Simulatioin Start " + i + " - " + j);
        //            sims[j].operateSim();
        //            sims[j].stopTimer();
        //            sims[j].TraceLogToCSV(i, j);
        //            double successRate = sims[j].printResultRate();
        //            successRateForCCTVSet += successRate;
        //            //sims[j].printDetectedResults();
        //            sims[j].DetectedResultsToCSV(i, j);
        //            sims[j].ShadowedLogToCSV(i, j);

        //            sims[j].resetTimer();
        //        }
        //        successRates.Add(successRateForCCTVSet / simulationTimesForCCTVSet);
        //    }

        //    Console.WriteLine("\n\n====== Simulation Results ======");
        //    Console.WriteLine("print index of CCTV set and Target detected Rate\n");
        //    for (int i = 0; i < successRates.Count; i++)
        //    {
        //        Console.WriteLine("CCTV set {0}\t{1:F2}%", i, successRates[i]);
        //        Console.WriteLine();
        //    }

        //    Console.WriteLine("\n\n====== Best CCTV set ======");
        //    int bestCCTVIdx = successRates.IndexOf(successRates.Max());

        //    Console.WriteLine("====== CCTV set {0} ======", bestCCTVIdx);
        //    road.setCctvswithCSV(bestCCTVIdx);
        //    road.printPos(road.cctvPos);
        //}
    }
}
