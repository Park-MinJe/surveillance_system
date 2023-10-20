using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        // Configuration: simulation time
        const double aUnitTime = 100 * 0.001; // (sec)

        public static double[] Dist = new double[25000];

        public class SimulatorCore
        {
            /* -------------------------시뮬레이션 대상 객체-------------------------*/
            public Building[] buildings { get; private set; }
            public CCTV[] cctvs { get; private set; }
            public Pedestrian[] peds { get; private set; }
            public Car[] cars { get; private set; }

            public Map map { get; private set; } = new Map();

            /* ---------------------------시뮬레이션 조건----------------------------*/
            public int N_Building { get; private set; }         // 실제 데이터에서 받아와 initBuilding method에서 초기화
            public int N_CCTV { get; private set; } = 100;      // default 100
            public int N_Ped { get; private set; } = 5;         // default 5
            public int N_Car { get; private set; } = 5;         // default 5
            public int N_Target { get; private set; }

            private bool getBuildingNumFromUser = false;
            private bool getCCTVNumFromUser = false;
            private bool getPedNumFromUser = false;
            private bool getCarNumFromUser = false;

            // ped csv file 출력 여부
            private bool createCSV = true;

            private Random rand;

            private double Sim_Time = 600;
            private double Now = 0;

            private Stopwatch stopwatch;

            /* ------------------------------CCTV 제원------------------------------*/
            private const double cctv_rotate_degree = -1; //90; --> 30초에 한바퀴?, -1: angle이 회전하는 옵션 disable (note 23-01-16)

            private bool fixMode = false;
            private double rotateTerm = 30.0; // sec

            /* ------------------------------MAP 제원------------------------------*/
            private double road_min_x = 0;
            private double road_max_x;
            private double road_min_y = 0;
            private double road_max_y;

            /* ---------------------------시뮬레이션 결과----------------------------*/
            // Console.WriteLine(">>> Simulating . . . \n");
            int cctvSetIdx = 0,
                simIdx = 0;

            private int[] R_Surv_Time;      // 탐지 
            private int[] directionError;   // 방향 미스
            private int[] outOfRange;       // 거리 범위 밖
            private int[] shadowedByBuilding;   // 건물에 가림

            //private int trace_idx;          // csv 파일 출력 index
            //private double[,] traffic_x;     // csv 파일 출력 위한 보행자별 x좌표
            //private double[,] traffic_y;     // csv 파일 출력 위한 보행자별 y좌표
            //private int[,] detection;     // csv 파일 출력 위한 추적여부
            //private double[] header;
            int stCnt;

            /* --------------------------------------
             * 전체 시뮬레이션 함수
            -------------------------------------- */
            //public double simulateAll(int cctvMode)
            //{
            //    /*------------------------------------------------------------------------
            //      % note 1) To avoid confusing, all input parameters for a distance has a unit as a milimeter
            //    -------------------------------------------------------------------------*/
            //    // Configuration: surveillance cameras
            //    // constant


            //    /*------------------------------------------------------------------------
            //      % Xml Document
            //    -------------------------------------------------------------------------*/
            //    // XmlDocument xdoc = new XmlDocument();
            //    // xdoc.Load(@"XMLFile1.xml");

            //    /* ---------------------------변수 초기화------------------------------*/
            //    this.setgetCCTVNumFromUser();
            //    this.setgetPedNumFromUser();
            //    this.setgetCarNumFromUser();

            //    this.initVariables();


            //    /* ---------------------------실행 시간 측정---------------------------*/
            //    this.initTimer();
            //    this.startTimer();


            //    /* -------------------------------------------
            //    *  도로 정보 생성 + 보행자/CCTV 초기화 시작
            //    *  타이머 작동
            //    ------------------------------------------- */
            //    this.initMap(cctvMode);
            //    road.printRoadInfo();

            //    /* -------------------------------------------
            //    *  도로 정보 생성 + 보행자/CCTV 초기화 끝
            //    ------------------------------------------- */

            //    // Console.WriteLine(">>> Simulating . . . \n");

            //    /* -------------------------------------------
            //    *  시뮬레이션 진행
            //    ------------------------------------------- */
            //    this.operateSim();
            //    this.stopTimer();
            //    /* -------------------------------------------
            //    *  시뮬레이션 종료
            //    *  타이머 stop
            //    ------------------------------------------- */

            //    // create .csv file
            //    this.TraceLogToCSV(0, 0);

            //    // 결과(탐지율)
            //    double successRate = this.printResultRate();

            //    // 결과(탐지 결과)
            //    // 시뮬레이션 결과, 탐지된 기록을 출력한다.
            //    this.printDetectedResults();

            //    // 결과(시간)
            //    // Console.WriteLine("Execution time : {0}", stopwatch.ElapsedMilliseconds + "ms");
            //    // accTime += stopwatch.ElapsedMilliseconds;

            //    // Console.WriteLine("\n============ RESULT ============");
            //    // Console.WriteLine("CCTV: {0}, Ped: {1}", N_CCTV, N_Ped);
            //    // Console.WriteLine("Execution time : {0}\n", (accTime / 1000.0 ) + " sec");

            //    return successRate;
            //}

            /* --------------------------------------
             * 입력 활성화 함수
            -------------------------------------- */
            public void setCctvFixMode()
            {
                while (true)
                {
                    Console.Write("Do you want to rotate cctv(Y/N)? ");
                    String input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        fixMode = false;
                        break;
                    }
                    else if (input == "N" || input == "n")
                    {
                        fixMode = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            public void setCctvFixMode(string input)
            {
                if (input == "Y" || input == "y")
                {
                    fixMode = false;
                }
                else if (input == "N" || input == "n")
                {
                    fixMode = true;
                }
            }

            public void setgetBuildingNumFromUser()
            {
                while (true)
                {
                    Console.Write("Do you want to enter Building Numbers(Y/N)? ");
                    String input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        getBuildingNumFromUser = true;
                        break;
                    }
                    else if (input == "N" || input == "n")
                    {
                        getBuildingNumFromUser = false;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            public void setgetBuildingNumFromUser(string input)
            {
                if (input == "Y" || input == "y")
                {
                    getBuildingNumFromUser = true;
                }
                else if (input == "N" || input == "n")
                {
                    getBuildingNumFromUser = false;
                }
            }

            public void setgetCCTVNumFromUser()
            {
                while (true)
                {
                    Console.Write("Do you want to enter CCTV Numbers(Y/N)? ");
                    String input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        getCCTVNumFromUser = true;
                        break;
                    }
                    else if (input == "N" || input == "n")
                    {
                        getCCTVNumFromUser = false;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            public void setgetCCTVNumFromUser(string input)
            {
                if (input == "Y" || input == "y")
                {
                    getCCTVNumFromUser = true;
                }
                else if (input == "N" || input == "n")
                {
                    getCCTVNumFromUser = false;
                }
            }

            public void setgetPedNumFromUser()
            {
                while (true)
                {
                    Console.Write("Do you want to enter Pedestrian Numbers(Y/N)? ");
                    String input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        getPedNumFromUser = true;
                        break;
                    }
                    else if (input == "N" || input == "n")
                    {
                        getPedNumFromUser = false;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            public void setgetPedNumFromUser(string input)
            {
                if (input == "Y" || input == "y")
                {
                    getPedNumFromUser = true;
                }
                else if (input == "N" || input == "n")
                {
                    getPedNumFromUser = false;
                }
            }

            public void setgetCarNumFromUser()
            {
                while (true)
                {
                    Console.Write("Do you want to enter Car Numbers(Y/N)? ");
                    String input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        getCarNumFromUser = true;
                        break;
                    }
                    else if (input == "N" || input == "n")
                    {
                        getCarNumFromUser = false;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            public void setgetCarNumFromUser(string input)
            {
                if (input == "Y" || input == "y")
                {
                    getCarNumFromUser = true;
                }
                else if (input == "N" || input == "n")
                {
                    getCarNumFromUser = false;
                }
            }

            /* --------------------------------------
             * 출력 활성화 함수
            -------------------------------------- */
            public void setcreateCSV()
            {
                while (true)
                {
                    Console.Write("Do you wand results as csv file(Y/N)? ");
                    String input = Console.ReadLine();

                    if (input == "Y" || input == "y")
                    {
                        createCSV = true;
                        break;
                    }
                    else if (input == "N" || input == "n")
                    {
                        createCSV = false;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            public void setcreateCSV(string input)
            {
                if (input == "Y" || input == "y")
                {
                    createCSV = true;
                }
                else if (input == "N" || input == "n")
                {
                    createCSV = false;
                }
            }

            /* --------------------------------------
             * 초기화 함수
            -------------------------------------- */

            /* ---------------------------시뮬레이션 조건----------------------------*/
            public void initNBuilding()
            {
                if (getBuildingNumFromUser)
                {
                    Console.Write("input number of Building: ");
                    N_CCTV = Convert.ToInt32(Console.ReadLine());
                }
            }
            public void initNBuilding(int nBuilding)
            {
                N_Building = nBuilding;
            }

            public void initNCctv()
            {
                if (getCCTVNumFromUser)
                {
                    Console.Write("input number of CCTV: ");
                    N_CCTV = Convert.ToInt32(Console.ReadLine());
                }
            }
            public void initNCctv(int nCctv)
            {
                N_CCTV = nCctv;
            }

            public void initNPed()
            {
                if (getPedNumFromUser)
                {
                    Console.Write("input number of Pedestrian: ");
                    N_Ped = Convert.ToInt32(Console.ReadLine());
                }
            }
            public void initNPed(int nPed)
            {
                N_Ped = nPed;
            }

            public void initNCar()
            {
                if (getCarNumFromUser)
                {
                    Console.Write("input number of Car: ");
                    N_Car = Convert.ToInt32(Console.ReadLine());
                }
            }
            public void initNCar(int nCar)
            {
                N_Car = nCar;
            }

            /* ---------------------------시뮬레이션 환경----------------------------*/

            // 230627 박민제
            // 건물 초기화
            public void initSimBuildings()
            {
                this.buildings = new Building[this.N_Building];
                for (int i = 0; i < this.N_Building; i++)
                {
                    if (i < mappingModule.buildings.Length)
                    {
                        this.buildings[i] = new Building(mappingModule.buildings[i]);
                    }
                    else
                    {
                        //this.buildings[i] = new Building();
                        Console.WriteLine("***You can't access here yet. Program need update to user generated buildings.***");
                        Environment.Exit(0);
                    }
                }
            }

            // 230627 박민제
            // cctv 초기화
            public void initSimCctvs(int cctvRandomSeed)
            {
                this.cctvs = new CCTV[this.N_CCTV];
                Random randForCctvsPos = new Random(cctvRandomSeed);

                for (int i = 0; i < this.N_CCTV; i++)
                {
                    if (i < mappingModule.cctvs.Length)
                    {
                        this.cctvs[i] = new CCTV(mappingModule.cctvs[i]);
                    }
                    else
                    {
                        this.cctvs[i] = mappingModule.cctvFactory(this.map, i, 
                            randForCctvsPos.Next(Convert.ToInt32(Math.Truncate(this.map.X_mapSize))),
                            randForCctvsPos.Next(Convert.ToInt32(Math.Truncate(this.map.Y_mapSize))));
                        //Console.WriteLine("***You can't access here yet. Program need update to user generated CCTVs.***");
                        //Environment.Exit(0);
                    }
                }
            }

            // 230504 pmj
            public void initSimTargetObjs(int pedRandomSeed, int carRandomSeed)
            {
                // just clone digital mapped objects.
                this.peds = new Pedestrian[this.N_Ped];
                this.cars = new Car[this.N_Car];
                this.map = new Map(mappingModule.map);

                this.N_Target = this.N_Ped + this.N_Car;

                Random randForPedsPos = new Random(pedRandomSeed);
                Random randForCarsPos = new Random(carRandomSeed);

                for (int i = 0; i < this.N_Ped; i++)
                {
                    // 230627 박민제
                    // 디지털 매핑 시, 보행자, 차량 생성 안함
                    //if (i < mappingModule.peds.Length)
                    //{
                    //    this.peds[i] = new Pedestrian(mappingModule.peds[i]);
                    //}
                    //else
                    //{
                    //    this.peds[i] = mappingModule.pedFactory(this.road);
                    //    //Console.WriteLine("***You can't access here yet. Program need update to user generated Peds.***");
                    //    //Environment.Exit(0);
                    //}
                    this.peds[i] = mappingModule.pedFactory(this.map, randForPedsPos.Next(this.map.roadsRefs.Count));
                }
                for(int i = 0; i < this.N_Car; i++)
                {
                    // 230627 박민제
                    // 디지털 매핑 시, 보행자, 차량 생성 안함
                    //if(i < mappingModule.cars.Length)
                    //{
                    //    this.cars[i] = new Car(mappingModule.cars[i]);
                    //}
                    //else
                    //{
                    //    this.cars[i] = mappingModule.carFactory(this.road);
                    //    //Console.WriteLine("***You can't access here yet. Program need update to user generated Cars.***");
                    //    //Environment.Exit(0);
                    //}
                    this.cars[i] = mappingModule.carFactory(this.map, randForCarsPos.Next(this.map.roadsRefs.Count),
                        randForCarsPos.Next(4));
                }
            }

            public void initSimulatorCoreVariables(int randomSeed)
            {
                rand = new Random(randomSeed);

                /* ------------------------------CCTV 제원------------------------------*/
                for (int i = 0; i < 25000; i++)
                {
                    Dist[i] = i;
                }

                /* ---------------------------전역 변수 할당---------------------------*/
                bw.setBuildingCSVWriter(N_Building);

                tw.setTargetCSVWriter(N_Ped, N_Car);

                cw.setCctvCSVWriter(N_CCTV);

                tlog.setTargetLogCSVWriter(N_Ped, N_Car, (int)(Sim_Time / aUnitTime));

                //clog.setDetectedLogCSVWriter(N_CCTV * N_Target * ((int)(Sim_Time / aUnitTime) +1));

                //Console.WriteLine("%d", N_CCTV * N_Building * N_Target * ((int)(Sim_Time / aUnitTime) + 1));
                //clog.setShadowedLogCSVWriter(N_CCTV * N_Building * N_Target * ((int)(Sim_Time / aUnitTime) + 1));
            }

            /* --------------------------------------
             * 타이머 함수
            -------------------------------------- */
            public void initTimer()
            {
                stopwatch = new Stopwatch();
            }

            public void startTimer()
            {
                /* ---------------------------실행 시간 측정---------------------------*/
                // time check start
                // double accTime = 0.0;

                stopwatch.Start();
            }

            public void stopTimer()
            {
                stopwatch.Stop();
            }

            public void resetTimer()
            {
                stopwatch.Reset();
            }

            /* --------------------------------------
             * 시뮬레이션 수행 함수
            -------------------------------------- */
            public void operateSim(int cctvSetIdx, int simIdx)
            {
                this.cctvSetIdx = cctvSetIdx;
                this.simIdx = simIdx;

                R_Surv_Time = new int[N_Target]; // 탐지 
                directionError = new int[N_Target]; // 방향 미스
                outOfRange = new int[N_Target]; // 거리 범위 밖
                shadowedByBuilding = new int[N_Target]; // 건물에 가림

                //trace_idx = (int)(Sim_Time / aUnitTime);
                //traffic_x = new double[N_Target, trace_idx]; // csv 파일 출력 위한 보행자별 x좌표
                //traffic_y = new double[N_Target, trace_idx]; // csv 파일 출력 위한 보행자별 y좌표
                //detection = new int[N_Target, trace_idx]; // csv 파일 출력 위한 추적여부
                //header = new double[trace_idx];

                stCnt = 0;

                road_min_x = 0;
                road_max_x = map.X_mapSize;

                road_min_y = 0;
                road_max_y = map.Y_mapSize;

                Now = 0;

                /* Console.WriteLine("=== 성공 ====");
                Console.WriteLine("print index of CCTV and detected Target\n");
                Console.WriteLine("{0, 4}\t{1, 5}\t{2, 18}\t{3, 18}\t{4}", "CCTV", "TARGET", "X", "Y", "V");*/
                // Console.WriteLine("Now: {0}, Sim_Time: {1}, routine times: {2}\n", Now, Sim_Time, (Sim_Time - Now) / aUnitTime);

                // simulation
                // Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Simulatioin Start");
                while (Now < Sim_Time)
                {
                    Console.Write(".");
                    // 추적 검사
                    int[] res = this.checkDetectionWithParallel(Now, N_Building, N_CCTV, N_Ped, N_Car);     // 병렬처리 사용
                    //int[] res = this.checkDetectionWithoutParallel(Now, N_Building, N_CCTV, N_Ped, N_Car);     // 병렬처리 미사용
                    // debug
                    // Console.WriteLine("Checking Detection Completed\n");
                    // threading.. error
                    // int[] res = new int[N_Ped];

                    // Thread ThreadForWork = new Thread( () => { res = checkDetection(N_CCTV, N_Ped); });     
                    // ThreadForWork.Start();

                    for (int i = 0; i < res.Length; i++)
                    {
                        if (createCSV)
                        {
                            //detection[i, stCnt] = res[i];
                            tlog.addDetection(i, stCnt, res[i]);
                        }

                        if (res[i] == 0) outOfRange[i]++;
                        else if (res[i] == -1) directionError[i]++;
                        else if (res[i] == 1) R_Surv_Time[i]++;
                        else if (res[i] == -2) shadowedByBuilding[i]++;
                    }
                    // debug
                    // Console.WriteLine("while simulation 1");

                    /* 220407 
                     * 보행자 방향 따라 CCTV 회전 제어
                     * 각 보행자가 탐지/미탐지 여부를 넘어서
                     * 특정 CCTV가 지금 탐지한 보행자의 정보를 알아야함
                     * 그래야 보행자의 범위 내 위치, 방향을 읽어서
                     * 보행자의 이동 방향으로 CCTV 회전 여부, 회전 시 방향 및 각도 설정 가능
                    */

                    // 이동
                    this.moveTarget();

                    this.rotateCCTVs();


                    if (createCSV)
                    {
                        //header[stCnt] = Math.Round(Now, 1);
                        tlog.addHeader(stCnt, Math.Round(Now, 1));
                        stCnt++;
                    }
                    Now += aUnitTime;

                    // debug
                    // Console.WriteLine("while simulation 3");
                }

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Simulation Completed");
            }

            /* --------------------------------------
             * 추적 여부 검사 함수
            -------------------------------------- */
            // Parallelism 적용 대상
            public int[] checkDetectionWithoutParallel(double nowTime, int N_Building, int N_CCTV, int N_Ped, int N_Car)
            {

                int N_Target = N_Ped + N_Car;
                int[] returnArr = new int[N_Target]; // 반환할 탐지 결과 (1: 탐지  0: 거리상 미탐지  -1: 방향 미스  -2: 건물에 가림)

                // 건물 대상 거리 검사 결과
                // 건물과 cctv 간 거리
                double[,] building_dist = new double[N_CCTV, N_Building];

                // cctv 탐지 거리 안의 건물
                int[,] candidate_detected_building_h = new int[N_CCTV, N_Building];
                int[,] candidate_detected_building_v = new int[N_CCTV, N_Building];

                // cctv에 잡힌 건물의 cos 좌표
                double[,] cosine_Building_h1 = new double[N_CCTV, N_Building];
                double[,] cosine_Building_h2 = new double[N_CCTV, N_Building];
                double[,] cosine_Building_v1 = new double[N_CCTV, N_Building];
                double[,] cosine_Building_v2 = new double[N_CCTV, N_Building];

                // cctv 감시 범위 안의 건물
                double[,] building_in_range_h = new double[N_CCTV, N_Building];
                double[,] building_in_range_v = new double[N_CCTV, N_Building];

                // 감시 대상 거리 검사 결과
                // 감시 대상과 cctv 간 거리
                double[,] target_dist = new double[N_CCTV, N_Target];

                // cctv 탐지 거리 안의 감시 대상
                int[,] candidate_detected_target_h = new int[N_CCTV, N_Target];
                int[,] candidate_detected_target_v = new int[N_CCTV, N_Target];

                try
                {
                    for (int i = 0; i < N_CCTV; i++)
                    //Parallel.For(0, N_CCTV, (i) =>
                    {
                        //debug
                        //Console.WriteLine("ThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                        // 건물과 cctv 간 거리 검사
                        for (int j = 0; j < N_Building; j++)
                        //Parallel.For(0, N_Building, (j) =>
                        {
                            //debug
                            //Console.WriteLine("\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                            building_in_range_h[i, j] = -1;
                            building_in_range_v[i, j] = -1;

                            foreach (Polygon poly in buildings[j].facesOfBuilding)
                            //Parallel.ForEach(buildings[j].facesOfBuilding, poly =>
                            {
                                //debug
                                //Console.WriteLine("\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                building_dist[i, j] = cctvs[i].calcDistToBuildingFace(poly);

                                if (building_dist[i, j] < cctvs[i].Max_Dist)
                                {
                                    candidate_detected_building_h[i, j] = 1;
                                    candidate_detected_building_v[i, j] = 1;
                                }
                            }

                            /*building_dist_h1[i, j] = Math
                                    .Sqrt(Math.Pow(cctvs[i].X - buildings[j].Pos_H1[0], 2) +
                                    Math.Pow(cctvs[i].Y - buildings[j].Pos_H1[1], 2));
                            building_dist_h2[i, j] = Math
                                    .Sqrt(Math.Pow(cctvs[i].X - buildings[j].Pos_H2[0], 2) +
                                    Math.Pow(cctvs[i].Y - buildings[j].Pos_H2[1], 2));
                            building_dist_v1[i, j] = Math
                                    .Sqrt(Math.Pow(cctvs[i].X - buildings[j].Pos_V1[0], 2) +
                                    Math.Pow(cctvs[i].Z - buildings[j].Pos_V1[1], 2));
                            building_dist_v2[i, j] = Math
                                    .Sqrt(Math.Pow(cctvs[i].X - buildings[j].Pos_V2[0], 2) +
                                    Math.Pow(cctvs[i].Z - buildings[j].Pos_V2[1], 2));

                            foreach (double survdist_h in cctvs[i].SurvDist_H)
                            {
                                if (building_dist_h1[i, j] <= survdist_h * 100 * 10 && building_dist_h2[i, j] <= survdist_h * 100 * 10)
                                {
                                    candidate_detected_building_h[i, j] = 1;
                                }
                            }
                            foreach (double survdist_v in cctvs[i].SurvDist_V)
                            {
                                if (building_dist_v1[i, j] <= survdist_v * 100 * 10 && building_dist_v2[i, j] <= survdist_v * 100 * 10)
                                {
                                    candidate_detected_building_v[i, j] = 1;
                                }
                            }*/

                            // 건물이 차지하는 각도 검사
                            double cosine_H_AOV = Math.Cos(cctvs[i].H_AOV / 2);
                            double cosine_V_AOV = Math.Cos(cctvs[i].V_AOV / 2);

                            // 거리상 미탐지면 넘어감 
                            if (candidate_detected_building_h[i, j] != 1 || candidate_detected_building_v[i, j] != 1)
                            {
                                continue;

                                // Parallel에서
                                //return;
                                // debug
                                //Console.ReadLine();
                            }

                            // 거리가 범위 내이면
                            if (candidate_detected_building_h[i, j] == 1)
                            {
                                // len equals Dist
                                int len = cctvs[i].H_FOV.X0.GetLength(0);
                                double[] A = { cctvs[i].H_FOV.X0[len - 1] - cctvs[i].X, cctvs[i].H_FOV.Y0[len - 1] - cctvs[i].Y };
                                double[] B = new double[2];

                                // 밑면의 점 중 H1과 H2를 구한다.
                                // x 단위 벡터와의 각도를 기준으로 가장 작은 점이 H1, 가장 큰 점이 H2가 된다.
                                // 각도는 라디안
                                cosine_Building_h1[i, j] = 2;       // 가장 작은 값을 얻기 위해 가장 큰 값으로 초기화
                                cosine_Building_h2[i, j] = -2;      // 가장 큰 값을 얻기 위해 가장 작은 값으로 초기화

                                foreach (Point pointOfBottom in buildings[j].pointsOfBottom)
                                //Parallel.ForEach(buildings[j].pointsOfBottom, pointOfBottom =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    B[0] = pointOfBottom.x - cctvs[i].X;
                                    B[1] = pointOfBottom.y - cctvs[i].Y;

                                    double tmp_cosine = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                    cosine_Building_h1[i, j] = (cosine_Building_h1[i, j] > tmp_cosine) ? tmp_cosine : cosine_Building_h1[i, j];
                                    cosine_Building_h2[i, j] = (cosine_Building_h2[i, j] > tmp_cosine) ? cosine_Building_h2[i, j] : tmp_cosine;
                                }

                                // horizontal 각도 검사 
                                if (cosine_Building_h1[i, j] >= cosine_H_AOV || cosine_Building_h2[i, j] >= cosine_H_AOV)
                                {
                                    //감지 됨
                                    building_in_range_h[i, j] = 1;
                                }
                                else
                                {
                                    building_in_range_h[i, j] = 0;
                                }
                            }

                            // vertical  각도 검사 
                            if (candidate_detected_building_v[i, j] == 1)
                            {
                                int len = cctvs[i].V_FOV.X0.GetLength(0);
                                double[] A = { cctvs[i].V_FOV.X0[len - 1] - cctvs[i].X, cctvs[i].V_FOV.Z0[len - 1] - cctvs[i].Z };
                                double[] B = new double[2];

                                // 수평 각도 검사와 동일
                                cosine_Building_v1[i, j] = 2;       // 가장 작은 값을 얻기 위해 가장 큰 값으로 초기화
                                cosine_Building_v2[i, j] = -2;      // 가장 큰 값을 얻기 위해 가장 작은 값으로 초기화

                                // 230207 카메라와 건물의 상대적 위치에 따라 연산 후보군에 해당하는 점이 바뀌므로 추가적 업데이트 필요
                                // 밑면의 점들
                                foreach (Point pointOfBottom in buildings[j].pointsOfBottom)
                                //Parallel.ForEach(buildings[j].pointsOfBottom, pointOfBottom =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    B[0] = pointOfBottom.x - cctvs[i].X;
                                    B[1] = pointOfBottom.z - cctvs[i].Z;

                                    double tmp_cosine = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                    cosine_Building_v1[i, j] = (cosine_Building_v1[i, j] > tmp_cosine) ? tmp_cosine : cosine_Building_v1[i, j];
                                    cosine_Building_v2[i, j] = (cosine_Building_v2[i, j] > tmp_cosine) ? cosine_Building_v2[i, j] : tmp_cosine;
                                }
                                // 윗면의 점들
                                foreach (Point pointOfTop in buildings[j].pointsOfTop)
                                //Parallel.ForEach(buildings[j].pointsOfTop, pointOfTop =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    B[0] = pointOfTop.x - cctvs[i].X;
                                    B[1] = pointOfTop.z - cctvs[i].Z;

                                    double tmp_cosine = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                    cosine_Building_v1[i, j] = (cosine_Building_v1[i, j] > tmp_cosine) ? tmp_cosine : cosine_Building_v1[i, j];
                                    cosine_Building_v2[i, j] = (cosine_Building_v2[i, j] > tmp_cosine) ? cosine_Building_v2[i, j] : tmp_cosine;
                                }

                                if (cosine_Building_v1[i, j] >= cosine_V_AOV || cosine_Building_v2[i, j] >= cosine_V_AOV)
                                {
                                    //감지 됨
                                    building_in_range_v[i, j] = 1;
                                }
                                else
                                {
                                    building_in_range_v[i, j] = 0;
                                }
                            }
                        }

                        // 감시 대상과 cctv 간 거리 검사
                        for (int j = 0; j < N_Target; j++)
                        //Parallel.For(0, N_Target, (j) =>
                        {
                            // debug
                            //Console.WriteLine("\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                            if (j < N_Ped)
                            {
                                target_dist[i, j] = cctvs[i].calcDistToTarget(peds[j]);
                            }
                            else
                            {
                                target_dist[i, j] = cctvs[i].calcDistToTarget(cars[j - N_Ped]);
                                /*target_dist_h1[i, j] = Math
                                        .Sqrt(Math.Pow(cctvs[i].X - cars[j - N_Ped].Pos_H1[0], 2) +
                                        Math.Pow(cctvs[i].Y - cars[j - N_Ped].Pos_H1[1], 2));
                                target_dist_h2[i, j] = Math
                                        .Sqrt(Math.Pow(cctvs[i].X - cars[j - N_Ped].Pos_H2[0], 2) +
                                        Math.Pow(cctvs[i].Y - cars[j - N_Ped].Pos_H2[1], 2));
                                target_dist_v1[i, j] = Math
                                        .Sqrt(Math.Pow(cctvs[i].X - cars[j - N_Ped].Pos_V1[0], 2) +
                                        Math.Pow(cctvs[i].Z - cars[j - N_Ped].Pos_V1[1], 2));
                                target_dist_v2[i, j] = Math
                                        .Sqrt(Math.Pow(cctvs[i].X - cars[j - N_Ped].Pos_V2[0], 2) +
                                        Math.Pow(cctvs[i].Z - cars[j - N_Ped].Pos_V2[1], 2));*/
                            }

                            if (target_dist[i, j] >= cctvs[i].Eff_Dist_From &&
                                target_dist[i, j] <= cctvs[i].Eff_Dist_To)
                            {
                                candidate_detected_target_h[i, j] = 1;
                                candidate_detected_target_v[i, j] = 1;
                            }

                            /*foreach (double survdist_h in cctvs[i].SurvDist_H)
                            {
                                if (target_dist_h1[i, j] <= survdist_h * 100 * 10 && target_dist_h2[i, j] <= survdist_h * 100 * 10)
                                {
                                    candidate_detected_target_h[i, j] = 1;
                                }
                            }
                            foreach (double survdist_v in cctvs[i].SurvDist_V)
                            {
                                if (target_dist_v1[i, j] <= survdist_v * 100 * 10 && target_dist_v2[i, j] <= survdist_v * 100 * 10)
                                {
                                    candidate_detected_target_v[i, j] = 1;
                                }
                            }*/

                            // if (cctvs[i].isPedInEffDist(peds[j])) {
                            //   candidate_detected_ped_h[i, j] = 1;
                            //   candidate_detected_ped_v[i, j] = 1;
                            // }

                            // candidate_detected_ped_h[i, j] = 1;
                            // candidate_detected_ped_v[i, j] = 1;
                        }
                    }

                    // 각 CCTV의 감시대상 탐지횟수 계산
                    int[] cctv_detecting_cnt = new int[N_CCTV];
                    int[] cctv_missing_cnt = new int[N_CCTV];

                    int[,] missed_map_h = new int[N_CCTV, N_Target];
                    int[,] missed_map_v = new int[N_CCTV, N_Target];

                    int[,] detected_map = new int[N_CCTV, N_Target];

                    // 각도 검사 
                    for (int i = 0; i < N_CCTV; i++)
                    //Parallel.For(0, N_CCTV, (i) =>
                    {
                        // debug
                        //Console.WriteLine("ThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                        double cosine_H_AOV = Math.Cos(cctvs[i].H_AOV / 2);
                        double cosine_V_AOV = Math.Cos(cctvs[i].V_AOV / 2);

                        for (int j = 0; j < N_Target; j++)
                        //Parallel.For(0, N_Target, (j) =>
                        {
                            // debug
                            //Console.WriteLine("\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                            // 거리상 미탐지면 넘어감 
                            if (candidate_detected_target_h[i, j] != 1 || candidate_detected_target_v[i, j] != 1)
                            {
                                continue;

                                // Parallel에서
                                //return;
                                // debug
                                //Console.ReadLine();
                            }

                            int h_detected = -1;
                            int v_detected = -1;

                            // 거리가 범위 내이면
                            if (candidate_detected_target_h[i, j] == 1)
                            {
                                // len equals Dist
                                int len = cctvs[i].H_FOV.X0.GetLength(0);
                                double[] A = { cctvs[i].H_FOV.X0[len - 1] - cctvs[i].X, cctvs[i].H_FOV.Y0[len - 1] - cctvs[i].Y };
                                double[] B = new double[2];
                                if (j < N_Ped)
                                {
                                    B[0] = peds[j].Pos_H1[0] - cctvs[i].X;
                                    B[1] = peds[j].Pos_H1[1] - cctvs[i].Y;
                                }
                                else
                                {
                                    int carIdx = j - N_Ped;
                                    B[0] = cars[carIdx].Pos_H1[0] - cctvs[i].X;
                                    B[1] = cars[carIdx].Pos_H1[1] - cctvs[i].Y;
                                }
                                double cosine_TARGET_h1 = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                if (j < N_Ped)
                                {
                                    B[0] = peds[j].Pos_H2[0] - cctvs[i].X;
                                    B[1] = peds[j].Pos_H2[1] - cctvs[i].Y;
                                }
                                else
                                {
                                    int carIdx = j - N_Ped;
                                    B[0] = cars[carIdx].Pos_H2[0] - cctvs[i].X;
                                    B[1] = cars[carIdx].Pos_H2[1] - cctvs[i].Y;
                                }
                                double cosine_TARGET_h2 = InnerProduct(A, B) / (Norm(A) * Norm(B));

                                // horizontal 각도 검사 
                                // 건물이 차지하는 영역 대비
                                for (int k = 0; k < N_Building; k++)
                                //Parallel.For(0, N_Building, (k) =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    if (building_in_range_h[i, k] == 1)
                                    {
                                        if (cosine_TARGET_h1 >= cosine_Building_h1[i, k] && cosine_TARGET_h2 >= cosine_Building_h2[i, k])
                                        {
                                            if (target_dist[i, j] >= building_dist[i, k])
                                            {
                                                h_detected = -2;
                                                if (j < N_Ped)
                                                {
                                                    //clog.addShadowedLog(i, k, 'h', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                                    //clog.ShadowedByBuildingLogToCSV(this.cctvSetIdx, this.simIdx, i, k, 'h', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                                }
                                                else
                                                {
                                                    //clog.addShadowedLog(i, k, 'h', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
                                                    //clog.ShadowedByBuildingLogToCSV(this.cctvSetIdx, this.simIdx, i, k, 'h', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
                                                }
                                            }
                                        }
                                    }
                                }

                                // cctv aov 대비
                                if (h_detected != -2)
                                {
                                    if (cosine_TARGET_h1 >= cosine_H_AOV && cosine_TARGET_h2 >= cosine_H_AOV)
                                    {
                                        //감지 됨
                                        h_detected = 1;
                                    }
                                    else
                                    {
                                        h_detected = 0;
                                    }
                                }
                            }

                            // vertical  각도 검사 
                            if (candidate_detected_target_v[i, j] == 1)
                            {
                                // Surv_SYS_v210202.m [line 260]
                                /*         
                                  if ismember(j, Candidates_Detected_PED_V1)
                                  A = [CCTV(i).V_FOV_X0(1,:); CCTV(i).V_FOV_Z0(1,:)] - [CCTV(i).X; CCTV(i).Z];
                                  B = [PED(j).Pos_V1(1); PED(j).Pos_V1(2)] - [CCTV(i).X; CCTV(i).Z]; 
                                */
                                int len = cctvs[i].V_FOV.X0.GetLength(0);
                                double[] A = { cctvs[i].V_FOV.X0[len - 1] - cctvs[i].X, cctvs[i].V_FOV.Z0[len - 1] - cctvs[i].Z };
                                double[] B = new double[2];
                                if (j < N_Ped)
                                {
                                    B[0] = peds[j].Pos_V1[0] - cctvs[i].X;
                                    B[1] = peds[j].Pos_V1[1] - cctvs[i].Z;
                                }
                                else
                                {
                                    int carIdx = j - N_Ped;
                                    B[0] = cars[carIdx].Pos_V1[0] - cctvs[i].X;
                                    B[1] = cars[carIdx].Pos_V1[1] - cctvs[i].Z;
                                }
                                double cosine_TARGET_v1 = InnerProduct(A, B) / (Norm(A) * Norm(B));

                                if (j < N_Ped)
                                {
                                    B[0] = peds[j].Pos_V2[0] - cctvs[i].X;
                                    B[1] = peds[j].Pos_V2[1] - cctvs[i].Z;
                                }
                                else
                                {
                                    int carIdx = j - N_Ped;
                                    B[0] = cars[carIdx].Pos_V2[0] - cctvs[i].X;
                                    B[1] = cars[carIdx].Pos_V2[1] - cctvs[i].Z;
                                }
                                double cosine_TARGET_v2 = InnerProduct(A, B) / (Norm(A) * Norm(B));

                                // 건물이 차지하는 영역 대비
                                for (int k = 0; k < N_Building; k++)
                                //Parallel.For(0, N_Building, (k) =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    if (building_in_range_v[i, k] == 1)
                                    {
                                        if (cosine_TARGET_v1 >= cosine_Building_v1[i, k] && cosine_TARGET_v2 >= cosine_Building_v2[i, k])
                                        {
                                            if (target_dist[i, j] >= building_dist[i, k])
                                            {
                                                v_detected = -2;
                                                if (j < N_Ped)
                                                {
                                                    //clog.addShadowedLog(i, k, 'v', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                                    //clog.ShadowedByBuildingLogToCSV(this.cctvSetIdx, this.simIdx, i, k, 'v', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                                }
                                                else
                                                {
                                                    //clog.addShadowedLog(i, k, 'v', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
                                                    //clog.ShadowedByBuildingLogToCSV(this.cctvSetIdx, this.simIdx, i, k, 'v', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
                                                }
                                            }
                                        }
                                    }
                                }

                                // cctv aov 대비
                                if (v_detected != -2)
                                {

                                    if (cosine_TARGET_v1 >= cosine_V_AOV && cosine_TARGET_v2 >= cosine_V_AOV)
                                    {
                                        //감지 됨
                                        v_detected = 1;
                                    }
                                    else
                                    {
                                        v_detected = 0;
                                    }
                                }
                            }


                            if (h_detected == 1 && v_detected == 1)
                            {
                                detected_map[i, j] = 1;
                                // 각 CCTV[i]의 보행자 탐지 횟수 증가
                                cctv_detecting_cnt[i]++;

                                returnArr[j] = 1;

                                // record detected Target & increase velocity when detected
                                //CCTV.detectedTarget detectedTargetInfo = new CCTV.detectedTarget();
                                //detectedTargetInfo.setIdx(j);
                                //detectedTargetInfo.setT(nowTime);

                                if (j < N_Ped)
                                {
                                    // Record Detected Target
                                    //detectedTargetInfo.setX(peds[j].X);
                                    //detectedTargetInfo.setY(peds[j].Y);
                                    //detectedTargetInfo.setV(peds[j].Velocity);
                                    //cctvs[i].detectedTargets.Add(detectedTargetInfo);

                                    //clog.addDetectedLog(i, "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                    //clog.DetectedLogToCSV(this.cctvSetIdx, this.simIdx, i, "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));

                                    // Increase Velocity
                                    peds[j].upVelocity();
                                }
                                else
                                {
                                    // Record Detected Target
                                    //detectedTargetInfo.setX(cars[j - N_Ped].X);
                                    //detectedTargetInfo.setY(cars[j - N_Ped].Y);
                                    //detectedTargetInfo.setV(cars[j - N_Ped].Velocity);
                                    //cctvs[i].detectedTargets.Add(detectedTargetInfo);
                                    int carIdx = j - N_Ped;

                                    //clog.addDetectedLog(i, "Car", carIdx, Math.Round(cars[carIdx].X, 2), Math.Round(cars[carIdx].Y, 2), Math.Round(cars[carIdx].Velocity, 2), Math.Round(nowTime, 2));
                                    //clog.DetectedLogToCSV(this.cctvSetIdx, this.simIdx, i, "Car", carIdx, Math.Round(cars[carIdx].X, 2), Math.Round(cars[carIdx].Y, 2), Math.Round(cars[carIdx].Velocity, 2), Math.Round(nowTime, 2));

                                    // Increase Velocity
                                    cars[j - N_Ped].upVelocity();
                                }
                            }
                            // 건물에 가림
                            else if (h_detected == -2 || v_detected == -2)
                            {
                                //Console.WriteLine("가림");
                                returnArr[j] = -2;
                            }
                            // 방향 미스 (h or v 중 하나라도 방향이 맞지 않는 경우)
                            else // cctv[i]가 보행자[j]를 h or v 탐지 실패 여부 추가
                            {
                                cctv_missing_cnt[i]++;

                                if (h_detected == 0) missed_map_h[i, j] = 1;

                                if (v_detected == 0) missed_map_v[i, j] = 1;

                                returnArr[j] = (returnArr[j] == 1 ? 1 : -1);

                                /*
                                if(h_detected != 1)
                                {
                                    Console.WriteLine("[{0}] horizontal 감지 못함", h_detected);
                                }
                                else if(v_detected != 1)
                                {
                                    Console.WriteLine("[{0}] vertical 감지 못함 ", v_detected);
                                }
                                */
                            }


                        } // 탐지 여부 계산 완료
                    }



                    // 여기부턴 h or v 각각 분석
                    // 각 cctv는 h, v 축에서 얼마나 많이 놓쳤나?
                    int[] cctv_missing_count_h = new int[N_CCTV];
                    int[] cctv_missing_count_v = new int[N_CCTV];

                    for (int i = 0; i < N_CCTV; i++)
                    {
                        for (int j = 0; j < N_Target; j++)
                        {
                            cctv_missing_count_h[i] += missed_map_h[i, j];
                            cctv_missing_count_v[i] += missed_map_v[i, j];
                        }
                    }

                    // target을 탐지한 cctv 수
                    int[] detecting_cctv_cnt = new int[N_Target];
                    // target을 탐지하지 못한 cctv 수
                    int[] missing_cctv_cnt = new int[N_Target];

                    // detection 결과 출력 
                    for (int i = 0; i < N_CCTV; i++)
                    {
                        for (int j = 0; j < N_Target; j++)
                        {
                            if (detected_map[i, j] == 1)
                            {
                                detecting_cctv_cnt[j]++;
                            }
                            else
                            {
                                missing_cctv_cnt[j]++;
                            }
                        }
                    }

                    // for time check
                    // Console.WriteLine("---------------------------------");
                    // Console.WriteLine("   성공  ||   실패  ");
                    // for (int i = 0; i < N_Ped; i++)
                    // {
                    //     if (detecting_cctv_cnt[i] == 0)
                    //     {
                    //         Console.WriteLine("         ||   ped{0} ", i + 1);
                    //     }
                    //     else
                    //     {
                    //         Console.WriteLine("ped{0} ", i + 1);
                    //     }
                    // }
                    // Console.WriteLine("---------------------------------");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Err while checkDetection func: " + e.Message);
                    Console.WriteLine(e.StackTrace);
                }


                return returnArr;
            }

            public int[] checkDetectionWithParallel(double nowTime, int N_Building, int N_CCTV, int N_Ped, int N_Car)
            {

                int N_Target = N_Ped + N_Car;
                int[] returnArr = new int[N_Target]; // 반환할 탐지 결과 (1: 탐지  0: 거리상 미탐지  -1: 방향 미스  -2: 건물에 가림)

                // 건물 대상 거리 검사 결과
                // 건물과 cctv 간 거리
                double[,] building_dist = new double[N_CCTV, N_Building];

                // cctv 탐지 거리 안의 건물
                int[,] candidate_detected_building_h = new int[N_CCTV, N_Building];
                int[,] candidate_detected_building_v = new int[N_CCTV, N_Building];

                // cctv에 잡힌 건물의 cos 좌표
                double[,] cosine_Building_h1 = new double[N_CCTV, N_Building];
                double[,] cosine_Building_h2 = new double[N_CCTV, N_Building];
                double[,] cosine_Building_v1 = new double[N_CCTV, N_Building];
                double[,] cosine_Building_v2 = new double[N_CCTV, N_Building];

                // cctv 감시 범위 안의 건물
                double[,] building_in_range_h = new double[N_CCTV, N_Building];
                double[,] building_in_range_v = new double[N_CCTV, N_Building];

                // 감시 대상 거리 검사 결과
                // 감시 대상과 cctv 간 거리
                double[,] target_dist = new double[N_CCTV, N_Target];

                // cctv 탐지 거리 안의 감시 대상
                int[,] candidate_detected_target_h = new int[N_CCTV, N_Target];
                int[,] candidate_detected_target_v = new int[N_CCTV, N_Target];

                try
                {
                    //for (int i = 0; i < N_CCTV; i++)
                    Parallel.For(0, N_CCTV, (i) =>
                    {
                        //debug
                        //Console.WriteLine("ThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                        // 건물과 cctv 간 거리 검사
                        for (int j = 0; j < N_Building; j++)
                        //Parallel.For(0, N_Building, (j) =>
                        {
                            //debug
                            //Console.WriteLine("\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                            building_in_range_h[i, j] = -1;
                            building_in_range_v[i, j] = -1;

                            foreach (Polygon poly in buildings[j].facesOfBuilding)
                            //Parallel.ForEach(buildings[j].facesOfBuilding, poly =>
                            {
                                //debug
                                //Console.WriteLine("\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                building_dist[i, j] = cctvs[i].calcDistToBuildingFace(poly);

                                if (building_dist[i, j] < cctvs[i].Max_Dist)
                                {
                                    candidate_detected_building_h[i, j] = 1;
                                    candidate_detected_building_v[i, j] = 1;
                                }
                            }

                            /*building_dist_h1[i, j] = Math
                                    .Sqrt(Math.Pow(cctvs[i].X - buildings[j].Pos_H1[0], 2) +
                                    Math.Pow(cctvs[i].Y - buildings[j].Pos_H1[1], 2));
                            building_dist_h2[i, j] = Math
                                    .Sqrt(Math.Pow(cctvs[i].X - buildings[j].Pos_H2[0], 2) +
                                    Math.Pow(cctvs[i].Y - buildings[j].Pos_H2[1], 2));
                            building_dist_v1[i, j] = Math
                                    .Sqrt(Math.Pow(cctvs[i].X - buildings[j].Pos_V1[0], 2) +
                                    Math.Pow(cctvs[i].Z - buildings[j].Pos_V1[1], 2));
                            building_dist_v2[i, j] = Math
                                    .Sqrt(Math.Pow(cctvs[i].X - buildings[j].Pos_V2[0], 2) +
                                    Math.Pow(cctvs[i].Z - buildings[j].Pos_V2[1], 2));

                            foreach (double survdist_h in cctvs[i].SurvDist_H)
                            {
                                if (building_dist_h1[i, j] <= survdist_h * 100 * 10 && building_dist_h2[i, j] <= survdist_h * 100 * 10)
                                {
                                    candidate_detected_building_h[i, j] = 1;
                                }
                            }
                            foreach (double survdist_v in cctvs[i].SurvDist_V)
                            {
                                if (building_dist_v1[i, j] <= survdist_v * 100 * 10 && building_dist_v2[i, j] <= survdist_v * 100 * 10)
                                {
                                    candidate_detected_building_v[i, j] = 1;
                                }
                            }*/

                            // 건물이 차지하는 각도 검사
                            double cosine_H_AOV = Math.Cos(cctvs[i].H_AOV / 2);
                            double cosine_V_AOV = Math.Cos(cctvs[i].V_AOV / 2);

                            // 거리상 미탐지면 넘어감 
                            if (candidate_detected_building_h[i, j] != 1 || candidate_detected_building_v[i, j] != 1)
                            {
                                continue;

                                // Parallel에서
                                //return;
                                // debug
                                //Console.ReadLine();
                            }

                            // 거리가 범위 내이면
                            if (candidate_detected_building_h[i, j] == 1)
                            {
                                // len equals Dist
                                int len = cctvs[i].H_FOV.X0.GetLength(0);
                                double[] X = { 1, 0 };      // x 단위 벡터
                                double[] A = { cctvs[i].H_FOV.X0[len - 1] - cctvs[i].X, cctvs[i].H_FOV.Y0[len - 1] - cctvs[i].Y };
                                double[] B = new double[2];

                                // 밑면의 점 중 H1과 H2를 구한다.
                                // x 단위 벡터와의 각도를 기준으로 가장 작은 점이 H1, 가장 큰 점이 H2가 된다.
                                // 각도는 라디안
                                double cosine_Building_h1_from_X = -2;       // 가장 큰 값을 얻기 위해 가장 작은 값으로 초기화
                                double cosine_Building_h2_from_X = 2;      // 가장 작은 값을 얻기 위해 가장 큰 값으로 초기화

                                foreach (Point pointOfBottom in buildings[j].pointsOfBottom)
                                //Parallel.ForEach(buildings[j].pointsOfBottom, pointOfBottom =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    B[0] = pointOfBottom.x - cctvs[i].X;
                                    B[1] = pointOfBottom.y - cctvs[i].Y;

                                    double tmp_cosine_from_X = InnerProduct(X, B) / (Norm(X) * Norm(B));
                                    double tmp_cosine_halfAOV = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                    if (cosine_Building_h1_from_X < tmp_cosine_from_X)
                                    {
                                        cosine_Building_h1_from_X = tmp_cosine_from_X;
                                        cosine_Building_h1[i, j] = tmp_cosine_halfAOV;
                                    }
                                    if (cosine_Building_h2_from_X > tmp_cosine_from_X)
                                    {
                                        cosine_Building_h2_from_X = tmp_cosine_from_X;
                                        cosine_Building_h2[i, j] = tmp_cosine_halfAOV;
                                    }
                                }

                                // horizontal 각도 검사 
                                if (cosine_Building_h1[i, j] >= cosine_H_AOV || cosine_Building_h2[i, j] >= cosine_H_AOV)
                                {
                                    //감지 됨
                                    building_in_range_h[i, j] = 1;
                                }
                                else
                                {
                                    building_in_range_h[i, j] = 0;
                                }
                            }

                            // vertical  각도 검사 
                            if (candidate_detected_building_v[i, j] == 1)
                            {
                                int len = cctvs[i].V_FOV.X0.GetLength(0);
                                double[] X = { 1, 0 };
                                double[] A = { cctvs[i].V_FOV.X0[len - 1] - cctvs[i].X, cctvs[i].V_FOV.Z0[len - 1] - cctvs[i].Z };
                                double[] B = new double[2];

                                // 수평 각도 검사와 동일
                                double cosine_Building_v1_from_X = -2;       // 가장 큰 값을 얻기 위해 가장 작은 값으로 초기화
                                double cosine_Building_v2_from_X = 2;      // 가장 작은 값을 얻기 위해 가장 큰 값으로 초기화

                                cosine_Building_v1[i, j] = 2;       // 가장 작은 값을 얻기 위해 가장 큰 값으로 초기화
                                cosine_Building_v2[i, j] = -2;      // 가장 큰 값을 얻기 위해 가장 작은 값으로 초기화

                                // 230207 카메라와 건물의 상대적 위치에 따라 연산 후보군에 해당하는 점이 바뀌므로 추가적 업데이트 필요
                                // 밑면의 점들
                                foreach (Point pointOfBottom in buildings[j].pointsOfBottom)
                                //Parallel.ForEach(buildings[j].pointsOfBottom, pointOfBottom =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    B[0] = pointOfBottom.x - cctvs[i].X;
                                    B[1] = pointOfBottom.z - cctvs[i].Z;

                                    double tmp_cosine_from_X = InnerProduct(X, B) / (Norm(X) * Norm(B));
                                    double tmp_cosine_halfAOV = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                    if (cosine_Building_v1_from_X < tmp_cosine_from_X)
                                    {
                                        cosine_Building_v1_from_X = tmp_cosine_from_X;
                                        cosine_Building_v1[i, j] = tmp_cosine_halfAOV;
                                    }
                                    if (cosine_Building_v2_from_X > tmp_cosine_from_X)
                                    {
                                        cosine_Building_v2_from_X = tmp_cosine_from_X;
                                        cosine_Building_v2[i, j] = tmp_cosine_halfAOV;
                                    }
                                }
                                // 윗면의 점들
                                foreach (Point pointOfTop in buildings[j].pointsOfTop)
                                //Parallel.ForEach(buildings[j].pointsOfTop, pointOfTop =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    B[0] = pointOfTop.x - cctvs[i].X;
                                    B[1] = pointOfTop.z - cctvs[i].Z;

                                    double tmp_cosine_from_X = InnerProduct(X, B) / (Norm(X) * Norm(B));
                                    double tmp_cosine_halfAOV = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                    if (cosine_Building_v1_from_X < tmp_cosine_from_X)
                                    {
                                        cosine_Building_v1_from_X = tmp_cosine_from_X;
                                        cosine_Building_v1[i, j] = tmp_cosine_halfAOV;
                                    }
                                    if (cosine_Building_v2_from_X > tmp_cosine_from_X)
                                    {
                                        cosine_Building_v2_from_X = tmp_cosine_from_X;
                                        cosine_Building_v2[i, j] = tmp_cosine_halfAOV;
                                    }
                                }

                                if (cosine_Building_v1[i, j] >= cosine_V_AOV || cosine_Building_v2[i, j] >= cosine_V_AOV)
                                {
                                    //감지 됨
                                    building_in_range_v[i, j] = 1;
                                }
                                else
                                {
                                    building_in_range_v[i, j] = 0;
                                }
                            }
                        }

                        // 감시 대상과 cctv 간 거리 검사
                        for (int j = 0; j < N_Target; j++)
                        //Parallel.For(0, N_Target, (j) =>
                        {
                            // debug
                            //Console.WriteLine("\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                            if (j < N_Ped)
                            {
                                target_dist[i, j] = cctvs[i].calcDistToTarget(peds[j]);
                            }
                            else
                            {
                                target_dist[i, j] = cctvs[i].calcDistToTarget(cars[j - N_Ped]);
                                /*target_dist_h1[i, j] = Math
                                        .Sqrt(Math.Pow(cctvs[i].X - cars[j - N_Ped].Pos_H1[0], 2) +
                                        Math.Pow(cctvs[i].Y - cars[j - N_Ped].Pos_H1[1], 2));
                                target_dist_h2[i, j] = Math
                                        .Sqrt(Math.Pow(cctvs[i].X - cars[j - N_Ped].Pos_H2[0], 2) +
                                        Math.Pow(cctvs[i].Y - cars[j - N_Ped].Pos_H2[1], 2));
                                target_dist_v1[i, j] = Math
                                        .Sqrt(Math.Pow(cctvs[i].X - cars[j - N_Ped].Pos_V1[0], 2) +
                                        Math.Pow(cctvs[i].Z - cars[j - N_Ped].Pos_V1[1], 2));
                                target_dist_v2[i, j] = Math
                                        .Sqrt(Math.Pow(cctvs[i].X - cars[j - N_Ped].Pos_V2[0], 2) +
                                        Math.Pow(cctvs[i].Z - cars[j - N_Ped].Pos_V2[1], 2));*/
                            }

                            if (target_dist[i, j] >= cctvs[i].Eff_Dist_From &&
                                target_dist[i, j] <= cctvs[i].Eff_Dist_To)
                            {
                                candidate_detected_target_h[i, j] = 1;
                                candidate_detected_target_v[i, j] = 1;
                            }

                            /*foreach (double survdist_h in cctvs[i].SurvDist_H)
                            {
                                if (target_dist_h1[i, j] <= survdist_h * 100 * 10 && target_dist_h2[i, j] <= survdist_h * 100 * 10)
                                {
                                    candidate_detected_target_h[i, j] = 1;
                                }
                            }
                            foreach (double survdist_v in cctvs[i].SurvDist_V)
                            {
                                if (target_dist_v1[i, j] <= survdist_v * 100 * 10 && target_dist_v2[i, j] <= survdist_v * 100 * 10)
                                {
                                    candidate_detected_target_v[i, j] = 1;
                                }
                            }*/

                            // if (cctvs[i].isPedInEffDist(peds[j])) {
                            //   candidate_detected_ped_h[i, j] = 1;
                            //   candidate_detected_ped_v[i, j] = 1;
                            // }

                            // candidate_detected_ped_h[i, j] = 1;
                            // candidate_detected_ped_v[i, j] = 1;
                        }
                    });

                    // 각 CCTV의 감시대상 탐지횟수 계산
                    int[] cctv_detecting_cnt = new int[N_CCTV];
                    int[] cctv_missing_cnt = new int[N_CCTV];

                    int[,] missed_map_h = new int[N_CCTV, N_Target];
                    int[,] missed_map_v = new int[N_CCTV, N_Target];

                    int[,] detected_map = new int[N_CCTV, N_Target];

                    // 각도 검사 
                    //for (int i = 0; i < N_CCTV; i++)
                    Parallel.For(0, N_CCTV, (i) =>
                    {
                        // debug
                        //Console.WriteLine("ThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                        double cosine_H_AOV = Math.Cos(cctvs[i].H_AOV / 2);
                        double cosine_V_AOV = Math.Cos(cctvs[i].V_AOV / 2);

                        for (int j = 0; j < N_Target; j++)
                        //Parallel.For(0, N_Target, (j) =>
                        {
                            // debug
                            //Console.WriteLine("\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                            // 거리상 미탐지면 넘어감 
                            if (candidate_detected_target_h[i, j] != 1 || candidate_detected_target_v[i, j] != 1)
                            {
                                continue;

                                // Parallel에서
                                //return;
                                // debug
                                //Console.ReadLine();
                            }

                            int h_detected = -1;
                            int v_detected = -1;

                            // 거리가 범위 내이면
                            if (candidate_detected_target_h[i, j] == 1)
                            {
                                // len equals Dist
                                int len = cctvs[i].H_FOV.X0.GetLength(0);
                                double[] A = { cctvs[i].H_FOV.X0[len - 1] - cctvs[i].X, cctvs[i].H_FOV.Y0[len - 1] - cctvs[i].Y };
                                double[] B = new double[2];
                                if (j < N_Ped)
                                {
                                    B[0] = peds[j].Pos_H1[0] - cctvs[i].X;
                                    B[1] = peds[j].Pos_H1[1] - cctvs[i].Y;
                                }
                                else
                                {
                                    int carIdx = j - N_Ped;
                                    B[0] = cars[carIdx].Pos_H1[0] - cctvs[i].X;
                                    B[1] = cars[carIdx].Pos_H1[1] - cctvs[i].Y;
                                }
                                double cosine_TARGET_h1 = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                if (j < N_Ped)
                                {
                                    B[0] = peds[j].Pos_H2[0] - cctvs[i].X;
                                    B[1] = peds[j].Pos_H2[1] - cctvs[i].Y;
                                }
                                else
                                {
                                    int carIdx = j - N_Ped;
                                    B[0] = cars[carIdx].Pos_H2[0] - cctvs[i].X;
                                    B[1] = cars[carIdx].Pos_H2[1] - cctvs[i].Y;
                                }
                                double cosine_TARGET_h2 = InnerProduct(A, B) / (Norm(A) * Norm(B));

                                // horizontal 각도 검사 
                                // 건물이 차지하는 영역 대비
                                for (int k = 0; k < N_Building; k++)
                                //Parallel.For(0, N_Building, (k) =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    if (building_in_range_h[i, k] == 1)
                                    {
                                        if (cosine_TARGET_h1 >= cosine_Building_h1[i, k] && cosine_TARGET_h2 >= cosine_Building_h2[i, k])
                                        {
                                            if (target_dist[i, j] >= building_dist[i, k])
                                            {
                                                h_detected = -2;
                                                if (j < N_Ped)
                                                {
                                                    //clog.addShadowedLog(i, k, 'h', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                                    //clog.ShadowedByBuildingLogToCSV(this.cctvSetIdx, this.simIdx, i, k, 'h', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                                }
                                                else
                                                {
                                                    //clog.addShadowedLog(i, k, 'h', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
                                                    //clog.ShadowedByBuildingLogToCSV(this.cctvSetIdx, this.simIdx, i, k, 'h', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
                                                }
                                            }
                                        }
                                    }
                                }

                                // cctv aov 대비
                                if (h_detected != -2)
                                {
                                    if (cosine_TARGET_h1 >= cosine_H_AOV && cosine_TARGET_h2 >= cosine_H_AOV)
                                    {
                                        //감지 됨
                                        h_detected = 1;
                                    }
                                    else
                                    {
                                        h_detected = 0;
                                    }
                                }
                            }

                            // vertical  각도 검사 
                            if (candidate_detected_target_v[i, j] == 1)
                            {
                                // Surv_SYS_v210202.m [line 260]
                                /*         
                                  if ismember(j, Candidates_Detected_PED_V1)
                                  A = [CCTV(i).V_FOV_X0(1,:); CCTV(i).V_FOV_Z0(1,:)] - [CCTV(i).X; CCTV(i).Z];
                                  B = [PED(j).Pos_V1(1); PED(j).Pos_V1(2)] - [CCTV(i).X; CCTV(i).Z]; 
                                */
                                int len = cctvs[i].V_FOV.X0.GetLength(0);
                                double[] A = { cctvs[i].V_FOV.X0[len - 1] - cctvs[i].X, cctvs[i].V_FOV.Z0[len - 1] - cctvs[i].Z };
                                double[] B = new double[2];
                                if (j < N_Ped)
                                {
                                    B[0] = peds[j].Pos_V1[0] - cctvs[i].X;
                                    B[1] = peds[j].Pos_V1[1] - cctvs[i].Z;
                                }
                                else
                                {
                                    int carIdx = j - N_Ped;
                                    B[0] = cars[carIdx].Pos_V1[0] - cctvs[i].X;
                                    B[1] = cars[carIdx].Pos_V1[1] - cctvs[i].Z;
                                }
                                double cosine_TARGET_v1 = InnerProduct(A, B) / (Norm(A) * Norm(B));

                                if (j < N_Ped)
                                {
                                    B[0] = peds[j].Pos_V2[0] - cctvs[i].X;
                                    B[1] = peds[j].Pos_V2[1] - cctvs[i].Z;
                                }
                                else
                                {
                                    int carIdx = j - N_Ped;
                                    B[0] = cars[carIdx].Pos_V2[0] - cctvs[i].X;
                                    B[1] = cars[carIdx].Pos_V2[1] - cctvs[i].Z;
                                }
                                double cosine_TARGET_v2 = InnerProduct(A, B) / (Norm(A) * Norm(B));

                                // 건물이 차지하는 영역 대비
                                for (int k = 0; k < N_Building; k++)
                                //Parallel.For(0, N_Building, (k) =>
                                {
                                    // debug
                                    //Console.WriteLine("\t\tThreadId = {0}", Thread.CurrentThread.ManagedThreadId);

                                    if (building_in_range_v[i, k] == 1)
                                    {
                                        if (cosine_TARGET_v1 >= cosine_Building_v1[i, k] && cosine_TARGET_v2 >= cosine_Building_v2[i, k])
                                        {
                                            if (target_dist[i, j] >= building_dist[i, k])
                                            {
                                                v_detected = -2;
                                                if (j < N_Ped)
                                                {
                                                    //clog.addShadowedLog(i, k, 'v', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                                    //clog.ShadowedByBuildingLogToCSV(this.cctvSetIdx, this.simIdx, i, k, 'v', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                                }
                                                else
                                                {
                                                    //clog.addShadowedLog(i, k, 'v', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
                                                    //clog.ShadowedByBuildingLogToCSV(this.cctvSetIdx, this.simIdx, i, k, 'v', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
                                                }
                                            }
                                        }
                                    }
                                }

                                // cctv aov 대비
                                if (v_detected != -2)
                                {

                                    if (cosine_TARGET_v1 >= cosine_V_AOV && cosine_TARGET_v2 >= cosine_V_AOV)
                                    {
                                        //감지 됨
                                        v_detected = 1;
                                    }
                                    else
                                    {
                                        v_detected = 0;
                                    }
                                }
                            }


                            if (h_detected == 1 && v_detected == 1)
                            {
                                detected_map[i, j] = 1;
                                // 각 CCTV[i]의 보행자 탐지 횟수 증가
                                cctv_detecting_cnt[i]++;

                                returnArr[j] = 1;

                                // record detected Target & increase velocity when detected
                                //CCTV.detectedTarget detectedTargetInfo = new CCTV.detectedTarget();
                                //detectedTargetInfo.setIdx(j);
                                //detectedTargetInfo.setT(nowTime);

                                if (j < N_Ped)
                                {
                                    // Record Detected Target
                                    //detectedTargetInfo.setX(peds[j].X);
                                    //detectedTargetInfo.setY(peds[j].Y);
                                    //detectedTargetInfo.setV(peds[j].Velocity);
                                    //cctvs[i].detectedTargets.Add(detectedTargetInfo);

                                    //clog.addDetectedLog(i, "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                    //clog.DetectedLogToCSV(this.cctvSetIdx, this.simIdx, i, "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));

                                    // Increase Velocity
                                    peds[j].upVelocity();
                                }
                                else
                                {
                                    // Record Detected Target
                                    //detectedTargetInfo.setX(cars[j - N_Ped].X);
                                    //detectedTargetInfo.setY(cars[j - N_Ped].Y);
                                    //detectedTargetInfo.setV(cars[j - N_Ped].Velocity);
                                    //cctvs[i].detectedTargets.Add(detectedTargetInfo);
                                    int carIdx = j - N_Ped;

                                    //clog.addDetectedLog(i, "Car", carIdx, Math.Round(cars[carIdx].X, 2), Math.Round(cars[carIdx].Y, 2), Math.Round(cars[carIdx].Velocity, 2), Math.Round(nowTime, 2));
                                    //clog.DetectedLogToCSV(this.cctvSetIdx, this.simIdx, i, "Car", carIdx, Math.Round(cars[carIdx].X, 2), Math.Round(cars[carIdx].Y, 2), Math.Round(cars[carIdx].Velocity, 2), Math.Round(nowTime, 2));

                                    // Increase Velocity
                                    cars[j - N_Ped].upVelocity();
                                }
                            }
                            // 건물에 가림
                            else if (h_detected == -2 || v_detected == -2)
                            {
                                //Console.WriteLine("가림");
                                returnArr[j] = -2;
                            }
                            // 방향 미스 (h or v 중 하나라도 방향이 맞지 않는 경우)
                            else // cctv[i]가 보행자[j]를 h or v 탐지 실패 여부 추가
                            {
                                cctv_missing_cnt[i]++;

                                if (h_detected == 0) missed_map_h[i, j] = 1;

                                if (v_detected == 0) missed_map_v[i, j] = 1;

                                returnArr[j] = (returnArr[j] == 1 ? 1 : -1);

                                /*
                                if(h_detected != 1)
                                {
                                    Console.WriteLine("[{0}] horizontal 감지 못함", h_detected);
                                }
                                else if(v_detected != 1)
                                {
                                    Console.WriteLine("[{0}] vertical 감지 못함 ", v_detected);
                                }
                                */
                            }


                        } // 탐지 여부 계산 완료
                    });



                    // 여기부턴 h or v 각각 분석
                    // 각 cctv는 h, v 축에서 얼마나 많이 놓쳤나?
                    int[] cctv_missing_count_h = new int[N_CCTV];
                    int[] cctv_missing_count_v = new int[N_CCTV];

                    for (int i = 0; i < N_CCTV; i++)
                    {
                        for (int j = 0; j < N_Target; j++)
                        {
                            cctv_missing_count_h[i] += missed_map_h[i, j];
                            cctv_missing_count_v[i] += missed_map_v[i, j];
                        }
                    }

                    // target을 탐지한 cctv 수
                    int[] detecting_cctv_cnt = new int[N_Target];
                    // target을 탐지하지 못한 cctv 수
                    int[] missing_cctv_cnt = new int[N_Target];

                    // detection 결과 출력 
                    for (int i = 0; i < N_CCTV; i++)
                    {
                        for (int j = 0; j < N_Target; j++)
                        {
                            if (detected_map[i, j] == 1)
                            {
                                detecting_cctv_cnt[j]++;
                            }
                            else
                            {
                                missing_cctv_cnt[j]++;
                            }
                        }
                    }

                    // for time check
                    // Console.WriteLine("---------------------------------");
                    // Console.WriteLine("   성공  ||   실패  ");
                    // for (int i = 0; i < N_Ped; i++)
                    // {
                    //     if (detecting_cctv_cnt[i] == 0)
                    //     {
                    //         Console.WriteLine("         ||   ped{0} ", i + 1);
                    //     }
                    //     else
                    //     {
                    //         Console.WriteLine("ped{0} ", i + 1);
                    //     }
                    // }
                    // Console.WriteLine("---------------------------------");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Err while checkDetection func: " + e.Message);
                    Console.WriteLine(e.StackTrace);
                }


                return returnArr;
            }

            /* --------------------------------------
             * 시뮬레이션 모듈 함수
            -------------------------------------- */
            public void moveTarget()
            {
                int pedLen = peds.Length;
                int carLen = cars.Length;
                for (int i = 0; i < pedLen + carLen; i++)
                {
                    if (i < pedLen)
                    {
                        // debug
                        // Console.WriteLine("ped[{0}]", i);
                        if (createCSV)
                        {
                            if (peds[i].X < road_min_x || peds[i].X > road_max_x)
                            {
                                //traffic_x[i] += "Out of range,";
                                //traffic_x[i, stCnt] = -1;
                                tlog.addTraffic_x(i, stCnt, -1);
                            }
                            else
                            {
                                //traffic_x[i, stCnt] = Math.Round(peds[i].X, 2);
                                tlog.addTraffic_x(i, stCnt, Math.Round(peds[i].X, 2));
                            }

                            if (peds[i].Y < road_min_y || peds[i].Y > road_max_y)
                            {
                                //traffic_y[i] += "Out of range,";
                                //traffic_y[i, stCnt] = -1;
                                tlog.addTraffic_y(i, stCnt, -1);
                            }
                            else
                            {
                                //traffic_y[i, stCnt] = Math.Round(peds[i].Y, 2);
                                tlog.addTraffic_y(i, stCnt, Math.Round(peds[i].Y, 2));
                            }
                        }

                        peds[i].move(this.map);
                    }
                    else
                    {
                        if (createCSV)
                        {
                            if (cars[i - pedLen].X < road_min_x || cars[i - pedLen].X > road_max_x)
                            {
                                //traffic_x[i] += "Out of range,";
                                //traffic_x[i, stCnt] = -1;
                                tlog.addTraffic_x(i, stCnt, -1);
                            }
                            else
                            {
                                //traffic_x[i, stCnt] = Math.Round(cars[i - pedLen].X, 2);
                                tlog.addTraffic_x(i, stCnt, Math.Round(cars[i - pedLen].X, 2));
                            }

                            if (cars[i - pedLen].Y < road_min_y || cars[i - pedLen].Y > road_max_y)
                            {
                                //traffic_y[i] += "Out of range,";
                                //traffic_y[i, stCnt] = -1;
                                tlog.addTraffic_y(i, stCnt, -1);
                            }
                            else
                            {
                                //traffic_y[i, stCnt] = Math.Round(cars[i - pedLen].Y, 2);
                                tlog.addTraffic_y(i, stCnt, Math.Round(cars[i - pedLen].Y, 2));
                            }
                        }

                        // debug
                        // Console.WriteLine("car[{0}]", i - pedLen);
                        cars[i - pedLen].move(this.map);
                        // debug
                        // Console.WriteLine("car[{0}]", i - pedLen);
                    }
                }
                // debug
                // Console.WriteLine("while simulation 2");
            }

            public void rotateCCTVs()
            {
                // 220317 cctv rotation
                if (cctv_rotate_degree > 0)
                {
                    for (int i = 0; i < N_CCTV; i++)
                    {
                        // 220331 rotate 후 fov 재계산
                        // 30초마다 한바퀴 돌도록 -> 7.5초마다 90도
                        // Now는 현재 simulation 수행 경과 시간
                        // 360/cctv_rotate_degree = 4
                        // 30/4 = 7.5
                        if (Math.Round(Now, 2) % Math.Round(rotateTerm / (360.0 / cctv_rotate_degree), 2) == 0)
                        {
                            // cctv.setFixMode(false)로 설정해줘야함!
                            // Console.WriteLine("[Rotate] Now: {0}, Degree: {1}", Math.Round(Now, 2), cctvs[i].ViewAngleH);
                            cctvs[i].rotateHorizon(cctv_rotate_degree); // 90
                                                                        // 회전후 수평 FOV update (지금은 전부 Update -> 시간 오래걸림 -> 일부만(일부FOV구성좌표만)해야할듯)
                            if (!cctvs[i].isFixed)
                                cctvs[i].get_H_FOV(Dist, cctvs[i].WD, cctvs[i].Focal_Length, cctvs[i].ViewAngleH, cctvs[i].X, cctvs[i].Y);
                        }
                    }
                }
            }

            /* --------------------------------------
             * 결과 출력 함수
            -------------------------------------- */
            public void TraceLogToCSV(int cctvSetIdx, int simIdx)
            {
                if (createCSV)
                {
                    tlog.TraceLogToCSV(cctvSetIdx, simIdx);
                }
            }

            public void DetectedResultsToCSV(int cctvSetIdx, int simIdx)
            {
                if (createCSV)
                {
                    //clog.DetectedLogToCSV(cctvSetIdx, simIdx);
                }
            }

            public void ShadowedLogToCSV(int cctvSetIdx, int simIdx)
            {
                if (createCSV)
                {
                    //clog.ShadowedByBuildingLogToCSV(cctvSetIdx, simIdx);
                }
            }

            public double printResultRate(StreamWriter sw, List<long> opTime, int cctvSetIdx, int simIdx)
            {
                double totalSimCount = Sim_Time / aUnitTime * N_Target;
                double outOfRangeRate = 100 * outOfRange.Sum() / totalSimCount;
                double directionErrorRate = 100 * directionError.Sum() / totalSimCount;
                double shadowedRate = 100 * shadowedByBuilding.Sum() / totalSimCount;

                double successRate = 100 * R_Surv_Time.Sum() / totalSimCount;

                // 결과(탐지율)
                Console.WriteLine("====== Surveillance{0}-{1} Time Result ======", cctvSetIdx, simIdx);
                sw.WriteLine("====== Surveillance{0}-{1} Time Result ======", cctvSetIdx, simIdx);

                Console.WriteLine("N_CCTV: {0}, N_Ped: {1}, N_Car: {2}, N_Building: {3}", N_CCTV, N_Ped, N_Car, N_Building);
                sw.WriteLine("N_CCTV: {0}, N_Ped: {1}, N_Car: {2}, N_Building: {3}", N_CCTV, N_Ped, N_Car, N_Building);

                Console.WriteLine("[Result]");
                sw.WriteLine("[Result]");

                opTime.Add(stopwatch.ElapsedMilliseconds);
                Console.WriteLine("  - Execution time : {0}", stopwatch.ElapsedMilliseconds + "ms");
                sw.WriteLine("  - Execution time : {0}", stopwatch.ElapsedMilliseconds + "ms");

                Console.WriteLine("[Fail]");
                sw.WriteLine("[Fail]");

                Console.WriteLine("  - Out of Range: {0:F2}% ({1}/{2})", outOfRangeRate, outOfRange.Sum(), totalSimCount);
                sw.WriteLine("  - Out of Range: {0:F2}% ({1}/{2})", outOfRangeRate, outOfRange.Sum(), totalSimCount);

                Console.WriteLine("  - Direction Error: {0:F2}% ({1}/{2})", directionErrorRate, directionError.Sum(), totalSimCount);
                sw.WriteLine("  - Direction Error: {0:F2}% ({1}/{2})", directionErrorRate, directionError.Sum(), totalSimCount);

                Console.WriteLine("  - Shadowed by Building: {0:F2}% ({1}/{2})", shadowedRate, shadowedByBuilding.Sum(), totalSimCount);
                sw.WriteLine("  - Shadowed by Building: {0:F2}% ({1}/{2})", shadowedRate, shadowedByBuilding.Sum(), totalSimCount);

                Console.WriteLine("[Success]");
                sw.WriteLine("[Success]");

                Console.WriteLine("  - Surveillance Time: {0:F2}% ({1}/{2})\n", successRate, R_Surv_Time.Sum(), totalSimCount);
                sw.WriteLine("  - Surveillance Time: {0:F2}% ({1}/{2})\n", successRate, R_Surv_Time.Sum(), totalSimCount);

                return successRate;
            }

            public void printDetectedResults()
            {
                while (true)
                {
                    Console.Write("Do you want Target Detection List(Y/N)? ");
                    String printDetectionList = Console.ReadLine();

                    if (printDetectionList == "Y" || printDetectionList == "y")
                    {
                        Console.WriteLine("\n====== Surveillance Target Result ======");
                        Console.WriteLine("print index of CCTV and detected Target\n");
                        Console.WriteLine("{0, 8}\t{1, 4}\t{2, 11}\t{3, 5}\t{4, 18}\t{5, 18}\t{6, 18}\t{7}", "CNT", "CCTV", "TYPE", "TARGET", "X", "Y", "V", "Time");
                        int cnt = 1;
                        //for (int i = 0; i < clog.getLogSize(); i++)
                        //{
                        //    Console.WriteLine("{0, 8}\t{1, 4}\t{2, 11}\t{3, 5}\t{4, 18}\t{5, 18}\t{6, 18}\t{7}", i, clog.getCctvIdx(i), clog.getTargetType(i), clog.getTargetIdx(i), clog.getX(i), clog.getY(i), clog.getV(i), clog.getT(i));
                        //}
                        break;
                    }
                    else if (printDetectionList == "N" || printDetectionList == "n")
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }
}
