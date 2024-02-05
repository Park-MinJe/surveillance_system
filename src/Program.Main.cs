// using Internal;
using static surveillance_system.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace surveillance_system
{
    public partial class Program
    {
        public static Logger _mainLoger = new Logger("Main");
        public static Stopwatch _globalStopwatch = new Stopwatch();

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

        // osm data
        public static OsmReader osmReader = new OsmReader();
        public static OsmWriter osmWriter = new OsmWriter();

        public static GuiManager gm = new GuiManager();

        //public static GraphicManager_slik graphic_Silk = new GraphicManager_slik();
        //public static GraphicManager_OpneTK graphic = new GraphicManager_OpneTK();

        public static DigitalMappingModule mappingModule = new DigitalMappingModule();

        static void Main(string[] args)
        {
            int operatingMode;          // 0: 디지털 모델링   1: 시뮬레이션

            Console.WriteLine("\ninput Operating Mode: ");
            while (true)
            {
                Console.Write("(0: 디지털 모델링    1: 시뮬레이션    2: osm test    3: vworld test    -1: 종료)? ");
                operatingMode = Convert.ToInt32(Console.ReadLine());

                if (operatingMode == -1) { break; }
                switch (operatingMode)
                {
                    case 0:
                        operateMapping();
                        break;
                    case 1:
                        operateSimulation();
                        break;
                    case 2:
                        osmTest();
                        break;
                    case 3:
                        vworldTest();
                        break;
                    default:
                        continue;
                }
            }
        }

        static void osmTest()
        {
            Console.Write("Input OSM file location -> ");
            string osmLoc = Console.ReadLine();

            osmReader.setOsmReader(osmLoc);

            Point lower = osmReader.getMapLowerCorner();
            lower.printString();

            Point upper = osmReader.getMapUpperCorner();
            upper.printString();

            calcIndexOnProg(lower, lower, upper).printString();
            calcIndexOnProg(upper, lower, upper).printString();

            Console.WriteLine("Press 'Enter' to continue...");
            Console.ReadLine();

            for (int i = 0; i < osmReader.NodeMap.Count; i++)
            {
                if (osmReader.NodeMap.ElementAt(i).Value.SurvInfoExist)
                {
                    Console.WriteLine("\nNodeId -> " + Convert.ToString(osmReader.NodeMap.ElementAt(i).Key));
                    FOSMNodeInfo tmpNodeInfo = osmReader.NodeMap.ElementAt(i).Value;
                    Point tmpNodeOnProg = new Point(tmpNodeInfo.Longitude, tmpNodeInfo.Latitude, 0);
                    tmpNodeOnProg = calcIndexOnProg(tmpNodeOnProg, lower, upper);
                    tmpNodeOnProg.printString();

                    Console.WriteLine(tmpNodeInfo.ToString());
                }
            }

            Console.WriteLine("Press 'Enter' to continue...");
            Console.ReadLine();

            for (int i = 0; i < osmReader.Ways.Count; i++)
            {
                /*if (osmReader.Ways[i].WayType != OsmReader.EOSMWayType.Building
                    && osmReader.Ways[i].WayType != OsmReader.EOSMWayType.Other)*/
                if (osmReader.Ways[i].WayType == EOSMWayType.Primary)
                {
                    Console.WriteLine(EOSMWayTypeName[Convert.ToInt32(osmReader.Ways[i].WayType)]);
                    Console.WriteLine(osmReader.Ways[i].ToString());
                    Console.WriteLine("");
                }
            }
        }

        static void vworldTest()
        {
            // 230227 1649 pmj
            // vworld api
            if (buildingfromApi is VworldService)
            {
                buildingfromApi.setRequest("GetFeature");
                buildingfromApi.setServiceKey("29F9EBAE-41BB-3873-B83E-7E059A0CB564");
                buildingfromApi.setTypename("lt_c_bldginfo");
                buildingfromApi.setBbox("14145605.918811569,4525871.524209193,14146051.196774742,4526433.703997406");
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

                calcIndexOnProg(lower, lower, upper).printString();
                calcIndexOnProg(upper, lower, upper).printString();

                int building_cnt = buildingfromApi.getFeatureMembersCnt();
                Console.WriteLine("building_cnt: {0}", building_cnt);
                for (int i = 0; i < building_cnt; i++)
                {
                    if (buildingfromApi.getBuildingHByIdx(i) == 10)
                    {
                        Point[] tmp = TransformCoordinate(buildingfromApi.getPosListByIdx(i), 3857, 4326);
                        Point[] tmpOnProg = calcIndexOnProg(tmp, lower, upper);
                        for(int j = 0; j < tmp.Length; j++)
                        {
                            if (tmp[j].x < lower.x || tmp[j].x > upper.x || tmp[j].y < lower.y || tmp[j].y > upper.y)
                            {
                                Console.WriteLine("H: {0}", buildingfromApi.getBuildingHByIdx(i));
                                tmp[j].printString();
                                tmpOnProg[j].printString();
                                Console.WriteLine();
                            }
                        }
                    }
                }
            }
        }

        // 230317 박민제
        // 실제 데이터를 이용한 디지털 매핑
        static void operateMapping()
        {
            Random rand = new Random();

            // 도로 정보
            osmTest();

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

            //mappingModule = new DigitalMappingModule();
            mappingModule.initDigitalMappingVariables(rand.Next());
            /** 230731 박민제
             * TODO: get upper/lower corner from osm data
             */
            mappingModule.initMap(cctvMode, buildingfromApi.getMapUpperCorner(), buildingfromApi.getMapLowerCorner());
            mappingModule.map.setCCTVsArrPosbyRealWorldData(mappingModule.cctvs);

            //*  보행자, 차량, cctv 초기 설정
            /*Console.WriteLine("\n============================================================\n");
            for (int i = 0; i < mappingModule.N_Ped; i++)
            {
                Console.WriteLine("{0}번째 보행자 = ({1}, {2}) ", i + 1, mappingModule.peds[i].X, mappingModule.peds[i].Y);
            }
            Console.WriteLine("\n============================================================\n");
            for (int i = 0; i < mappingModule.N_Car; i++)
            {
                Console.WriteLine("{0}번째 차량 = ({1}, {2}) ", i + 1, mappingModule.cars[i].X, mappingModule.cars[i].Y);
            }
            Console.WriteLine("\n============================================================\n");
            for (int i = 0; i < mappingModule.cctvs.Length; i++)
            {
                Console.WriteLine("{0}번째 cctv = ({1}, {2}) ", i + 1, mappingModule.cctvs[i].X, mappingModule.cctvs[i].Y);
            }
            Console.WriteLine("\n============================================================\n");*/

            bw.setBuildingCSVWriter(mappingModule.N_Building);
            // 230627 박민제
            // 디지털 매핑 시, 보행자, 차량 생성 안함
            //tw.setTargetCSVWriter(mappingModule.N_Ped, mappingModule.N_Car);
            cw.setCctvCSVWriter(mappingModule.N_CCTV);

            bw.BuildingsToCSV("DigitalMappingResult.Buildings", mappingModule.buildings);
            // 230627 박민제
            // 디지털 매핑 시, 보행자, 차량 생성 안함
            //tw.PedsToCSV("DigitalMappingResult.Peds", mappingModule.peds);
            //tw.CarsToCSV("DigitalMappingResult.Cars", mappingModule.cars);
            cw.CctvsToCSV("DigitalMappingResult.CctvSet", mappingModule.cctvs);

            //debug
            mappingModule.map.printBuildingPos();
            mappingModule.map.printCctvPos();
        }



        // 230317 박민제
        // 디지털 매핑된 것을 기반으로 시뮬레이션 구현
        static void operateSimulation()
        {
            Random randomForSeed = new Random();

            int nBuilding = 0, nCctv = 0, nPed = 0, nCar = 0,
                cctvMode = 0,
                numberOfCCTVSet = 1,
                simulationTimesForCCTVSet = 100;
            string inputNBuildingOption = "N",
                inputNCctvOption = "N",
                inputCctvRotate = "N",
                inputNPedOption = "N",
                inputNCarOption = "N",
                InputcreateCSV = "N",
                InputcreateOSM = "N";
            bool printResultAsOSM = false;

            List<double> successRates = new List<double>();
            List<long> opTime = new List<long>();

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

                if (simulationTimesForCCTVSet < 1) { continue; }
                else { break; }
            }

            Console.WriteLine("\ninput CCTV collocating mode: ");
            while (true)
            {
                Console.Write("(0: pos cctv as grid    1: pos cctv as random at DST    2: pos cctv as random in int)? ");
                cctvMode = Convert.ToInt32(Console.ReadLine());

                // 230314 박민제
                // cctvMode 3은 realWorldCctvFromCSV() 함수 테스팅을 위함 임시 모드로, 디지털 매핑 과정으로 분기 예정
                if (cctvMode == 0 || cctvMode == 1 || cctvMode == 2 || cctvMode == 3) { break; }
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
                inputNBuildingOption = Console.ReadLine();

                if (inputNBuildingOption == "N" || inputNBuildingOption == "n") { break; }
                else if (inputNBuildingOption == "Y" || inputNBuildingOption == "y") { break; }
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
                Console.Write("Do you want results as csv file(Y/N)? ");
                InputcreateCSV = Console.ReadLine();

                if (InputcreateCSV == "N" || InputcreateCSV == "n") { break; }
                else if (InputcreateCSV == "Y" || InputcreateCSV == "y") { break; }
                else { continue; }
            }

            Console.WriteLine("");

            while (true)
            {
                Console.Write("Do you want results as osm file(Y/N)? ");
                InputcreateOSM = Console.ReadLine();

                if (InputcreateOSM == "N" || InputcreateOSM == "n") { printResultAsOSM = false; break; }
                else if (InputcreateOSM == "Y" || InputcreateOSM == "y") { printResultAsOSM = true; break; }
                else { continue; }
            }

            Console.WriteLine("");

            if (inputNBuildingOption == "Y" || inputNBuildingOption == "y")
            {
                Console.Write("input number of Building: ");
                nBuilding = Convert.ToInt32(Console.ReadLine());
            }
            else
            {
                nBuilding = mappingModule.buildings.Length;
            }

            if (inputNCctvOption == "Y" || inputNCctvOption == "y")
            {
                Console.Write("input number of CCTV: ");
                nCctv = Convert.ToInt32(Console.ReadLine());
            }
            else
            {
                nCctv = mappingModule.cctvs.Length;
            }

            if (inputNPedOption == "Y" || inputNPedOption == "y")
            {
                Console.Write("input number of Pedestrian: ");
                nPed = Convert.ToInt32(Console.ReadLine());
            }
            else
            {
                nPed = 5;   // default 5
            }

            if (inputNCarOption == "Y" || inputNCarOption == "y")
            {
                Console.Write("input number of Car: ");
                nCar = Convert.ToInt32(Console.ReadLine());
            }
            else
            {
                nCar = 5;   // default 5
            }


            SimulatorCore[] sims = new SimulatorCore[simulationTimesForCCTVSet];

            int[] cctvRandomSeeds = new int[numberOfCCTVSet];
            int[] pedRandomSeeds = new int[simulationTimesForCCTVSet];
            int[] carRandomSeeds = new int[simulationTimesForCCTVSet];

            string[] buildingFileName = new string[simulationTimesForCCTVSet];
            string[] pedsFileName = new string[simulationTimesForCCTVSet];
            string[] carsFileName = new string[simulationTimesForCCTVSet];

            string genTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            for (int i = 0; i < numberOfCCTVSet; i++)
            {
                cctvRandomSeeds[i] = randomForSeed.Next();
            }
            for (int i = 0; i < simulationTimesForCCTVSet; i++)
            {
                pedRandomSeeds[i] = randomForSeed.Next();
                carRandomSeeds[i] = randomForSeed.Next();

                sims[i] = new SimulatorCore();
                sims[i].setcreateCSV(InputcreateCSV);
                sims[i].setCctvFixMode(inputCctvRotate);
                sims[i].setgetBuildingNumFromUser(inputNBuildingOption);
                sims[i].setgetCCTVNumFromUser(inputNCctvOption);
                sims[i].setgetPedNumFromUser(inputNPedOption);
                sims[i].setgetCarNumFromUser(inputNCarOption);

                sims[i].initNBuilding(nBuilding);
                sims[i].initNPed(nPed);
                sims[i].initNCar(nCar);

                sims[i].initSimulatorCoreVariables(randomForSeed.Next());
                sims[i].initSimBuildings();
                sims[i].initSimTargetObjs(pedRandomSeeds[i], carRandomSeeds[i]);

                sims[i].initTimer();

                // sims[i].startTimer();
                //sims[i].initMap(cctvMode, buildingfromApi.getMapUpperCorner(), buildingfromApi.getMapLowerCorner());

                buildingFileName[i] = "Sim" + i + ".Buildings_" + genTime;
                pedsFileName[i] = "Sim" + i + ".Peds_" + genTime;
                carsFileName[i] = "Sim" + i + ".Cars_" + genTime;

                bw.BuildingsToCSV(buildingFileName[i], sims[i].buildings);
                tw.PedsToCSV(pedsFileName[i], sims[i].peds);
                tw.CarsToCSV(carsFileName[i], sims[i].cars);
                //pedestrianAtSim.Add(peds);
                //carAtSim.Add(cars);
                //sims[i].stopTimer();
            }

            String logFileName = "log\\Simulation-ResultLog_" + genTime + ".csv";
            StreamWriter sw = new StreamWriter(logFileName);      // 병렬처리 사용 실험 결과 로그
            sw.WriteLine("#cctv_set_idx,sim_idx,size_map_x,size_map_y," +
                "n_building,n_cctv,n_ped,n_car,t_success,t_out_of_range,t_direction_error,t_shadowd_by_building,t_execution," +
                "total_building_area,percent_of_building_area," +
                "total_building_volume,percent_of_building_volume");
            //StreamWriter sw = new StreamWriter("log\\Simulation-ResultLog.txt");      // 병렬처리 사용 실험 결과 로그
            //StreamWriter sw = new StreamWriter("log\\Simulation-ResultLog-withoutParallel.txt");      // 일반 for문 사용 실험 결과 로그

            double[] successRateForCCTVSetList = new double[simulationTimesForCCTVSet];
            for (int i = 0; i < numberOfCCTVSet; i++)
            {
                //double successRateForCCTVSet = 0.0;

                for (int j = 0; j < simulationTimesForCCTVSet; j++)
                {
                    sims[j].map.setPedswithCSV(pedsFileName[j], sims[j].peds);
                    sims[j].map.setCarswithCSV(carsFileName[j], sims[j].cars);

                    successRateForCCTVSetList[j] = 0;
                }

                int leftSimsToInitialize = simulationTimesForCCTVSet;
                int leftSimsInOperating = simulationTimesForCCTVSet;
                int leftSimsToFinish = simulationTimesForCCTVSet;

                _globalStopwatch.Start();
                //for (int j = 0; j < simulationTimesForCCTVSet; j++)
                Parallel.For(0, simulationTimesForCCTVSet, (j) =>
                {
                    //sims[j].road.setPedsArrPos(sims[j].peds, pedRandomSeeds[j]);
                    //sims[j].road.setCarsArrPos(sims[j].cars, carRandomSeeds[j]);

                    sims[j].startTimer();
                    _mainLoger.debug(string.Format("Simulation {0} for cctv set {1} is initializing. {2} sims are left to init.", j, i, leftSimsToInitialize--));
                    // 첫번째 시뮬레이터는 디지털 매핑 결과를 이용
                    if (i > 1 && numberOfCCTVSet > 2)
                    {
                        sims[j].initNCctv(nCctv);
                        sims[j].initSimCctvs(cctvRandomSeeds[i]);

                        switch (cctvMode)
                        {
                            case 0:
                                sims[j].map.setCCTVsArrPos(sims[j].cctvs);
                                break;
                            case 1:
                                sims[j].map.setCCTVsArrPosbyRandomInDST(sims[j].cctvs);
                                break;
                            case 2:
                                sims[j].map.setCCTVsArrPosbyRandomInInt(sims[j].cctvs);
                                break;
                            case 3:
                                //road.setCCTVbyRealWorldData(sims[j].N_CCTV);

                                break;
                        }
                        // 231117 temporary blocked for journal writing
                        //cw.setCctvCSVWriter(sims[j].N_CCTV);
                        //cw.CctvsToCSV("CctvSet" + i + "_" + genTime, sims[j].cctvs);
                    }
                    else if (i == 1 && numberOfCCTVSet > 1)
                    {
                        sims[j].initNCctv(nCctv);
                        sims[j].initSimCctvs(cctvRandomSeeds[i]);

                        // 231117 temporary blocked for journal writing
                        //cw.setCctvCSVWriter(sims[j].N_CCTV);
                        //cw.CctvsToCSV("CctvSet" + i + "_" + genTime, sims[j].cctvs);
                    }
                    else
                    {
                        sims[j].initNCctv(mappingModule.N_CCTV);
                        sims[j].initSimCctvs(cctvRandomSeeds[i]);

                        // 231117 temporary blocked for journal writing
                        //cw.setCctvCSVWriter(sims[j].N_CCTV);
                        //cw.CctvsToCSV("DigitalMappingResult.CctvSet_" + genTime, sims[j].cctvs);
                    }
                    //cctvAtSim.Add(cctvs);

                    // 230317 박민제
                    //cw.setCctvCSVWriter(sims[j].N_CCTV);
                    //cw.CctvsToCSV("CctvSet" + i);


                    //cctvPosAtSim.Add(road.cctvPos);
                    //sims[j].map.printAllPos();

                    //Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Simulatioin Start " + i + " - " + j);
                    //Console.WriteLine("x map size: {0} / y map size: {1}", sims[j].map.X_mapSize, sims[j].map.Y_mapSize);
                    //Console.WriteLine("N_CCTV: {0}, N_Ped: {1}, N_Car: {2}, N_Building: {3}", sims[j].N_CCTV, sims[j].N_Ped, sims[j].N_Car, sims[j].N_Building);

                    _mainLoger.debug(string.Format("Simulation {0} for cctv set {1} starts operating detectness calculation. {2} sims are left to start operation.", j, i, leftSimsInOperating--));
                    sims[j].operateSim(i, j);
                    sims[j].stopTimer();
                    _mainLoger.debug(string.Format("Simulation {0} for cctv set {1} is finished. {2} sims are left to finish.", j, i, leftSimsToFinish--));
                    //sims[j].TraceLogToCSV(i, j, genTime);
                    double successRate = sims[j].printResultRate(sw, opTime, i, j);
                    _mainLoger.debug(string.Format("Simulation {0} for cctv set {1} success rate is {2}. ", j, i, successRate));
                    successRateForCCTVSetList[j] = successRate;
                    //sims[j].printDetectedResults();
                    //sims[j].DetectedResultsToCSV(i, j);
                    //sims[j].ShadowedLogToCSV(i, j);

                    sims[j].resetTimer();

                    if (printResultAsOSM) osmWriter.printAsOsm(sims[j], i, j, genTime);
                });
                _globalStopwatch.Stop();
                _mainLoger.info(string.Format("{0} sims are finished in {1} ms.", simulationTimesForCCTVSet, _globalStopwatch.ElapsedMilliseconds));
                _globalStopwatch.Reset();

                double sumOfSuccessRate = 0;
                for (int j = 0; j < simulationTimesForCCTVSet; j++)
                {
                    sumOfSuccessRate += successRateForCCTVSetList[j];
                }
                double meanOfSuccessRate = sumOfSuccessRate / simulationTimesForCCTVSet;
                _mainLoger.info(string.Format("The mean of success rates for cctv set {0} is {1}", i, meanOfSuccessRate));

                successRates.Add(meanOfSuccessRate);
            }

            long totalOpTime = opTime.Sum();
            int totalSimTimes = numberOfCCTVSet * simulationTimesForCCTVSet;

            _mainLoger.info(string.Format("- Average Simulation Time: {0}/{1} = {2} ms per simulation", totalOpTime, totalSimTimes, totalOpTime / totalSimTimes));
            //sw.WriteLine("\n- Average Simulation Time: {0}/{1} = {2} ms per simulation", totalOpTime, totalSimTimes, totalOpTime/totalSimTimes);

            sw.Close();

            Console.WriteLine("\n\n====== Simulation Results ======");
            Console.WriteLine("print index of CCTV set and Target detected Rate\n");
            for (int i = 0; i < successRates.Count; i++)
            {
                Console.WriteLine("CCTV set {0}\t{1:F2}%", i, successRates[i]);
                Console.WriteLine();
            }

            Console.WriteLine("\n\n====== Best CCTV set ======");
            int bestCCTVIdx = successRates.IndexOf(successRates.Max());
            Console.WriteLine(bestCCTVIdx);

            //Road realRoadClone = new Road(mappingModule.road);
            Map realRoadClone;
            /*CCTV[] bestCctvSet = new CCTV[mappingModule.cctvs.Length];
            for(int i = 0; i < mappingModule.cctvs.Length; i++)
            {
                bestCctvSet[i] = new CCTV(mappingModule.cctvs[i]);
            }*/

            // 231023 건물에 의한 영항 파악 실험으로 인해 비활성화
            //if (bestCCTVIdx == 0)
            //{
            //    Console.WriteLine("====== Real World CCTV set ======");
            //    realRoadClone = new Map(mappingModule.map);
            //    //realRoadClone.setCctvswithCSV("DigitalMappingResult.CctvSet", bestCctvSet);
            //    realRoadClone.getPosOfCctvs(cr.CctvsFromCsvAsArray("DigitalMappingResult.CctvSet"));
            //}
            //else
            //{
            //    Console.WriteLine("====== CCTV set {0} ======", bestCCTVIdx);
            //    realRoadClone = new Map(sims[bestCCTVIdx].map);
            //    //realRoadClone.setCctvswithCSV("CctvSet" + bestCCTVIdx, bestCctvSet);
            //    realRoadClone.getPosOfCctvs(cr.CctvsFromCsvAsArray("CctvSet" + bestCCTVIdx));
            //}
            //realRoadClone.printPos(realRoadClone.cctvPos);
        }
    }
}
