// using Internal;
using System.Runtime.CompilerServices;
using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using System.Runtime.Intrinsics.X86;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        public static Building[] buildings;
        public static CCTV[] cctvs;
        public static Pedestrian[] peds;
        public static Car[] cars;

        // Configuration: simulation time
        const double aUnitTime = 100 * 0.001; // (sec)
        public static Road road = new Road();

        public class SimulationModel
        {
            /* ---------------------------시뮬레이션 조건----------------------------*/
            private bool getBuildingNumFromUser = false;
            private bool getCCTVNumFromUser = false;
            private bool getPedNumFromUser = false;
            private bool getCarNumFromUser = false;

            private int N_Building;         // 실제 데이터에서 받아와 initBuilding method에서 초기화
            private int N_CCTV = 100;
            private int N_Ped = 5;
            private int N_Car = 5;
            private int N_Target;

            public int getNCCTV() { return N_CCTV; }

            // ped csv file 출력 여부
            private bool createCSV = true;

            private Random rand;

            private double Sim_Time = 600;
            private double Now = 0;

            private Stopwatch stopwatch;

            /* ------------------------------CCTV 제원------------------------------*/
            private const double Lens_FocalLength = 2.8; // mm, [2.8 3.6 6 8 12 16 25]
            private const double WD = 3.6; // (mm) width, horizontal size of camera sensor
            private const double HE = 2.7; // (mm) height, vertical size of camera sensor

            // const double Diag = Math.Sqrt(WD*WD + HE*HE), diagonal size
            private const double imW = 1920; // (pixels) image width
            private const double imH = 1080; // (pixels) image height

            private const double cctv_rotate_degree = -1; //90; --> 30초에 한바퀴?, -1: angle이 회전하는 옵션 disable (note 23-01-16)
                                                  // Installation [line_23]
            private const double Angle_H = 0; // pi/2, (deg), Viewing Angle (Horizontal Aspects)
            private const double Angle_V = 0; // pi/2, (deg), Viewing Angle (Vertical Aspects)

            private bool fixMode = true;
            private double rotateTerm = 30.0; // sec

            // calculate vertical/horizontal AOV
            double H_AOV = 2 * Math.Atan(WD / (2 * Lens_FocalLength));//RadToDeg(2 * Math.Atan(WD / (2 * Lens_FocalLength))); // Horizontal AOV
            double V_AOV = 2 * Math.Atan(HE / (2 * Lens_FocalLength));//RadToDeg(2 * Math.Atan(HE / (2 * Lens_FocalLength))); // Vertical AOV

            private double[] Dist = new double[25000];
            private int dist_len = 100000;
            private double[] Height = new double[25000];

            /* ------------------------------MAP 제원------------------------------*/
            // configuration: road
            // const int Road_WD = 5000; // 이거 안쓰는 변수? Road_Width 존재
            private bool On_Road_Builder = true; // 0:No road, 1:Grid

            private int Road_Width = 0;
            private int Road_Interval = 0;
            private int Road_N_Interval = 0;

            private int road_min = 0;
            private int road_max;

            /* ------------------------------건물 설정------------------------------*/
            private const int Building_Width = 9000;
            private const int Building_Height = 17000;

            /* ------------------------------PED 설정------------------------------*/
            private bool Opt_Observation = false;
            private bool Opt_Demo = false;
            private int[] log_PED_position = null;

            // Configuration: Pedestrian (Target Object)
            // When Pedestrian
            private const int Ped_Width = 900; // (mm)
            private const int Ped_Height = 1700; // (mm)
            private const int Ped_Velocity = 1500; // (mm/s)

            /* ------------------------------CAR 설정------------------------------*/
            private int[] log_CAR_position = null;

            // Configuration: Pedestrian (Target Object)
            // When Car
            private const int Car_Width = 1600; // (mm)
            private const int Car_Height = 2000; // (mm)
                                            // const int Car_Length = 3600; // (mm)
            private const int Car_Velocity = 14000; // (mm/s)

            /* ---------------------------시뮬레이션 결과----------------------------*/
            // Console.WriteLine(">>> Simulating . . . \n");
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
            public double simulateAll(int cctvMode)
            {
                /*------------------------------------------------------------------------
                  % note 1) To avoid confusing, all input parameters for a distance has a unit as a milimeter
                -------------------------------------------------------------------------*/
                // Configuration: surveillance cameras
                // constant


                /*------------------------------------------------------------------------
                  % Xml Document
                -------------------------------------------------------------------------*/
                // XmlDocument xdoc = new XmlDocument();
                // xdoc.Load(@"XMLFile1.xml");

                /* ---------------------------변수 초기화------------------------------*/
                this.setgetCCTVNumFromUser();
                this.setgetPedNumFromUser();
                this.setgetCarNumFromUser();

                this.initVariables();


                /* ---------------------------실행 시간 측정---------------------------*/
                this.initTimer();
                this.startTimer();


                /* -------------------------------------------
                *  도로 정보 생성 + 보행자/CCTV 초기화 시작
                *  타이머 작동
                ------------------------------------------- */
                this.initMap(cctvMode);
                road.printRoadInfo();

                /* -------------------------------------------
                *  도로 정보 생성 + 보행자/CCTV 초기화 끝
                ------------------------------------------- */

                // Console.WriteLine(">>> Simulating . . . \n");

                /* -------------------------------------------
                *  시뮬레이션 진행
                ------------------------------------------- */
                this.operateSim();
                this.stopTimer();
                /* -------------------------------------------
                *  시뮬레이션 종료
                *  타이머 stop
                ------------------------------------------- */

                // create .csv file
                this.TraceLogToCSV(0, 0);

                // 결과(탐지율)
                double successRate = this.printResultRate();

                // 결과(탐지 결과)
                // 시뮬레이션 결과, 탐지된 기록을 출력한다.
                this.printDetectedResults();

                // 결과(시간)
                // Console.WriteLine("Execution time : {0}", stopwatch.ElapsedMilliseconds + "ms");
                // accTime += stopwatch.ElapsedMilliseconds;

                // Console.WriteLine("\n============ RESULT ============");
                // Console.WriteLine("CCTV: {0}, Ped: {1}", N_CCTV, N_Ped);
                // Console.WriteLine("Execution time : {0}\n", (accTime / 1000.0 ) + " sec");

                return successRate;
            }

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
                if (getBuildingNumFromUser)
                {
                    N_Building = nBuilding;
                }
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
                if (getCCTVNumFromUser)
                {
                    N_CCTV = nCctv;
                }
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
                if (getPedNumFromUser)
                {
                    N_Ped = nPed;
                }
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
                if (getCarNumFromUser)
                {
                    N_Car = nCar;
                }
            }

            /* ---------------------------시뮬레이션 환경----------------------------*/

            public void initVariables()
            {
                N_Target = N_Ped + N_Car;

                rand = new Random();

                /* ------------------------------CCTV 제원------------------------------*/
                /* double D_AOV = RadToDeg(2 * Math.Atan(Diag / (2 * Lens_FocalLength)));
                (mm)distance
                 double[] Dist = new double[10000];
                int dist_len = 100000;
                double[] Height = new double[10000];
                for (int i = 0; i < 10000; i++)
                {
                    Dist[i] = i;
                    Height[i] = i;
                } */
                for (int i = 0; i < 25000; i++)
                {
                    Dist[i] = i;
                    Height[i] = i;
                }

                /* ------------------------------MAP 제원------------------------------*/
                if (On_Road_Builder)
                {
                    Road_Width = 10000; // mm, 10 meter
                    Road_Interval = 88000; // mm, 88 meter
                    Road_N_Interval = 5;
                }

                /* ------------------------------건물 제원------------------------------*/


                /* ------------------------------PED 설정------------------------------*/
                if (Opt_Demo)
                {
                    log_PED_position = new int[5];
                }

                /* ------------------------------CAR 설정------------------------------*/
                if (Opt_Demo)
                {
                    log_CAR_position = new int[5];
                }

                /* ---------------------------전역 변수 할당---------------------------*/
                /*buildings = new Building[N_Building];
                for (int i = 0; i < N_Building; i++)
                {
                    buildings[i] = new Building();
                }*/
                cctvs = new CCTV[N_CCTV];
                for (int i = 0; i < N_CCTV; i++)
                {
                    cctvs[i] = new CCTV();
                }
                peds = new Pedestrian[N_Ped];
                for (int i = 0; i < N_Ped; i++)
                {
                    peds[i] = new Pedestrian();
                }
                cars = new Car[N_Car];
                for (int i = 0; i < N_Car; i++)
                {
                    cars[i] = new Car();
                }

                //aw.setBuildingCSVWriter(N_Building);

                tw.setTargetCSVWriter(N_Ped, N_Car);

                cw.setCctvCSVWriter(N_CCTV);

                tlog.setTargetLogCSVWriter(N_Ped, N_Car, (int)(Sim_Time / aUnitTime));

                clog.setDetectedLogCSVWriter();
                clog.setShadowedLogCSVWriter();
            }

            public void initMap(int cctvMode)
            {
                // mode 0: pos cctv as grid    1: pos cctv as random
                try
                {
                    if (On_Road_Builder)
                    {
                        // 도로 정보 생성, 보행자 정보 생성
                        road.roadBuilder(Road_Width, Road_Interval, Road_N_Interval);
                        //road.setBuilding(N_Building);
                        road.setPed(N_Ped);
                        road.setCar(N_Car);

                        /*
                        // debug 220428
                        for(int i = 0 ; i < N_CCTV; i++) {
                            Console.Write(cctvs[i].X);
                          Console.Write(", ");
                          Console.WriteLine(cctvs[i].Y);

                        }
                        */
                        // road.printRoadInfo();

                        /*

                        //*  보행자, cctv 초기 설정
                        for (int i = 0; i < N_Ped; i++)
                        {
                            Console.WriteLine("{0}번째 보행자 = ({1}, {2}) ", i + 1, peds[i].X, peds[i].Y);
                        }
                        Console.WriteLine("\n============================================================\n");
                        for (int i = 0; i < N_CCTV; i++)
                        {
                            Console.WriteLine("{0}번째 cctv = ({1}, {2}) ", i + 1, cctvs[i].X, cctvs[i].Y);
                        }
                        */


                        // 건물 init
                        this.initBuilding();
                        
                        // ped init
                        this.initPed();

                        // car init
                        this.initCar();

                        // cctv init
                        this.initCCTV();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Err while initializing Simulator\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Road Setting Completed");
            }

            public void initBuilding()
            {
                try
                {
                    gbs.readFeatureMembers();
                    gbs.readPosLists();
                    gbs.readBuildingHs();

                    List<Point[]> pls = new List<Point[]>();
                    List<double> hs = new List<double>();

                    for (int i = 0; i < gbs.getFeatureMembersCnt(); i++)
                    {
                        double h = gbs.getBuildingH(i);
                        if (h > 0)
                        {
                            hs.Add(h);

                            Point[] pl = gbs.getPosList(i);
                            Point[] transformedPl = TransformCoordinate(pl, 5174, 4326);

                            // 프로그램상의 좌표계로 변환
                            // 지도 범위의 왼쪽 위를 기준으로 한다.
                            Point[] plOnSystem = calcIndexOnProg(transformedPl, road.lowerCorner.getX(), road.upperCorner.getY());

                            pls.Add(plOnSystem);
                        }
                    }

                    this.N_Building = pls.Count;
                    buildings = new Building[this.N_Building];
                    aw.setBuildingCSVWriter(this.N_Building);
                    for (int i = 0; i < this.N_Building; i++)
                    {
                        buildings[i] = new Building();
                        buildings[i].define_Building(pls[i], hs[i]);
                        //Debug
                        buildings[i].printBuildingInfo();
                    }
                    road.setBuilding(this.N_Building);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Err while initializing Building\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Building Setting Completed\n");
            }

            public void initPed()
            {
                try
                {
                    foreach (Pedestrian ped in peds)
                    {
                        double minDist = 0.0;
                        //int idx_minDist = 0;
                        //double[] Dist_Map = new double[road.DST.GetLength(0)];

                        // 맨처음 위치에서 가장 가까운 도착지를 설정 (보행자 맨처음 위치는 setPed()로 설정)
                        double[,] newPos = road.getPointOfAdjacentRoad(road.getIdxOfIntersection(ped.X, ped.Y));
                        double dst_x = Math.Round(newPos[0, 0]);
                        double dst_y = Math.Round(newPos[0, 1]);

                        // Car object일경우 가까운 도착지 설정
                        // double[,] newPos = road.getPointOfAdjacentIntersection(road.getIdxOfIntersection(ped.X, ped.Y), ped.X, ped.Y);
                        // double dst_x = Math.Round(newPos[0, 0]);
                        // double dst_y = Math.Round(newPos[0, 1]);

                        //Calc_Dist_and_get_MinDist(road.DST, ped.X, ped.Y, ref Dist_Map, ref minDist, ref idx_minDist);

                        //double dst_x = road.DST[idx_minDist, 0];
                        //double dst_y = road.DST[idx_minDist, 1];

                        // 보행자~목적지 벡터
                        /*
                        double[] A = new double[2];
                        A[0] = dst_x - ped.X;
                        A[1] = dst_y - ped.Y;        

                        double[] B = { 0.001, 0 };
                        double direction = Math.Round(Math.Acos(InnerProduct(A, B) / (Norm(A) * Norm(B))),8);
                        if(ped.Y > dst_y)
                        {
                            direction = Math.Round(2 * Math.PI - direction, 8); 
                        }
                        */
                        ped.define_TARGET(Ped_Width, Ped_Height, dst_x, dst_y, Ped_Velocity);
                        ped.setDirection();
                        ped.TTL = (int)Math.Ceiling((minDist / ped.Velocity) / aUnitTime);
                        // ped.printTargetInfo();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Err while initializing Pedestrians\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("PED Setting Completed\n");
            }

            public void initCar()
            {
                try
                {
                    foreach (Car car in cars)
                    {
                        double minDist = 0.0;

                        // Car object일경우 가까운 도착지 설정
                        double[,] newPos = road.getPointOfAdjacentIntersection(road.getIdxOfIntersection(car.X, car.Y), car.X, car.Y);
                        // debug
                        // Console.WriteLine("get destination Completed\n");
                        double dst_x = Math.Round(newPos[0, 0]);
                        double dst_y = Math.Round(newPos[0, 1]);

                        // debug
                        // Calc_Dist_and_get_MinDist(road.DST, ped.X, ped.Y, ref Dist_Map, ref minDist, ref idx_minDist);

                        car.define_TARGET(Car_Width, Car_Height, dst_x, dst_y, Car_Velocity);
                        // debug
                        // Console.WriteLine("define_TARGET Completed\n");
                        car.setDirection();
                        // debug
                        // Console.WriteLine("setDirection Completed\n");
                        car.TTL = (int)Math.Ceiling((minDist / car.Velocity) / aUnitTime);
                        // car.printTargetInfo();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Err while initializing Cars\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("CAR Setting Completed\n");
            }

            public void initCCTV()
            {
                try
                {
                    for (int i = 0; i < N_CCTV; i++)
                    {
                        // 220317
                        // Height.Max() 는 고정값 (=대충 10000)..
                        // 상수로 바꿔도 될듯??
                        // default Z는 3000
                        // 3000 ~ 10000 사이 값, 즉 7000이 변하는 값
                        // default(min) : 3000, variant : 7000 
                        // maxZ = min + variant 이런식으로?..

                        // cctvs[i].Z =
                        //     (int)Math.Ceiling(rand.NextDouble() * (Height.Max() - 3000)) + 3000; // milimeter
                        cctvs[i].setZ((int)Math.Ceiling(rand.NextDouble() * (Height.Max() - 3000)) + 3000);
                        cctvs[i].WD = WD;
                        cctvs[i].HE = HE;
                        cctvs[i].imW = (int)imW;
                        cctvs[i].imH = (int)imH;
                        cctvs[i].Focal_Length = Lens_FocalLength;
                        // 220104 초기 각도 설정
                        // cctvs[i].ViewAngleH = rand.NextDouble() * 360;
                        // cctvs[i].ViewAngleV = -35 - 20 * rand.NextDouble();

                        cctvs[i].setViewAngleH(rand.NextDouble() * 360 * Math.PI / 180);  // (23-02-02) modified by 0BoO, deg -> rad
                        // cctvs[i].setViewAngleH(rand.Next(4) * 90);
                        // cctvs[i].setViewAngleV(-35 - 20 * rand.NextDouble());
                        cctvs[i].setViewAngleV(-45.0 * Math.PI / 180);   // (23-02-02) modified by 0BoO, deg -> rad


                        cctvs[i].setFixMode(fixMode); // default (rotate)

                        cctvs[i].H_AOV = 2 * Math.Atan(WD / (2 * Lens_FocalLength));
                        cctvs[i].V_AOV = 2 * Math.Atan(HE / (2 * Lens_FocalLength));

                        // 기기 성능상의 최대 감시거리 (임시값)
                        cctvs[i].Max_Dist = 50 * 100 * 10; // 50m (milimeter)
                                                           // cctvs[i].Max_Dist = 500 * 100 * 100; // 500m (milimeter)

                        // Line 118~146
                        /*  여기부턴 Road_Builder 관련 정보가 없으면 의미가 없을거같아서 주석처리했어용..
                            그리고 get_Sectoral_Coverage 이런함수도 지금은 구현해야할지 애매해서..?
                        */

                        cctvs[i]
                            .get_PixelDensity(Dist,
                            cctvs[i].WD,
                            cctvs[i].HE,
                            cctvs[i].Focal_Length,
                            cctvs[i].imW,
                            cctvs[i].imH);

                        cctvs[i].get_H_FOV(Dist, cctvs[i].WD, cctvs[i].Focal_Length, cctvs[i].ViewAngleH, cctvs[i].X, cctvs[i].Y);
                        cctvs[i].get_V_FOV(Dist, cctvs[i].HE, cctvs[i].Focal_Length, cctvs[i].ViewAngleV, cctvs[i].X, cctvs[i].Z);
                        // cctvs[i].printCCTVInfo();

                        cctvs[i].calcBlindToPed();          // (23-02-01) added by 0BoO
                        cctvs[i].calcEffDistToPed(3000);     // (23-02-01) added by 0BoO, input value is 3000mm(3meter)
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Err while initializing CCTVs\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("\nCCTV Setting Completed\n");
            }

            /* --------------------------------------
             * init obj with csv
            -------------------------------------- */
            public void initPedsWithCSV(int simIdx)
            {
                road.setPedswithCSV(simIdx);
            }

            public void initCarsWithCSV(int simIdx)
            {
                road.setCarswithCSV(simIdx);
            }

            public void initCctvsWithCSV(int cctvSetIdx)
            {
                road.setCctvswithCSV(cctvSetIdx);
                foreach(CCTV cctv in cctvs)
                {
                    clog.clearCctvLog();

                    cctv.get_PixelDensity(Dist,
                            cctv.WD,
                            cctv.HE,
                            cctv.Focal_Length,
                            cctv.imW,
                            cctv.imH);

                    cctv.get_H_FOV(Dist, cctv.WD, cctv.Focal_Length, cctv.ViewAngleH, cctv.X, cctv.Y);
                    cctv.get_V_FOV(Dist, cctv.HE, cctv.Focal_Length, cctv.ViewAngleV, cctv.X, cctv.Z);
                }
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
            public void operateSim()
            {
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

                //road_min = 0;
                //road_max = road.mapSize;

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
                    int[] res = this.checkDetection(Now, N_Building, N_CCTV, N_Ped, N_Car);
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


                    if (createCSV) {
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
            public int[] checkDetection(double nowTime, int N_Building, int N_CCTV, int N_Ped, int N_Car)
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

                for (int i = 0; i < N_CCTV; i++)
                {
                    // 건물과 cctv 간 거리 검사
                    for (int j = 0; j < N_Building; j++)
                    {
                        building_in_range_h[i, j] = -1;
                        building_in_range_v[i, j] = -1;

                        foreach(Polygon poly in buildings[j].facesOfBuilding)
                        {
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

                            foreach(Point pointOfBottom in buildings[j].pointsOfBottom)
                            {
                                B[0] = pointOfBottom.getX() - cctvs[i].X;
                                B[1] = pointOfBottom.getY() - cctvs[i].Y;

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
                            {
                                B[0] = pointOfBottom.getX() - cctvs[i].X;
                                B[1] = pointOfBottom.getZ() - cctvs[i].Z;

                                double tmp_cosine = InnerProduct(A, B) / (Norm(A) * Norm(B));
                                cosine_Building_v1[i, j] = (cosine_Building_v1[i, j] > tmp_cosine) ? tmp_cosine : cosine_Building_v1[i, j];
                                cosine_Building_v2[i, j] = (cosine_Building_v2[i, j] > tmp_cosine) ? cosine_Building_v2[i, j] : tmp_cosine;
                            }
                            // 윗면의 점들
                            foreach (Point pointOfTop in buildings[j].pointsOfTop)
                            {
                                B[0] = pointOfTop.getX() - cctvs[i].X;
                                B[1] = pointOfTop.getZ() - cctvs[i].Z;

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
                    {
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
                {
                    double cosine_H_AOV = Math.Cos(cctvs[i].H_AOV / 2);
                    double cosine_V_AOV = Math.Cos(cctvs[i].V_AOV / 2);

                    for (int j = 0; j < N_Target; j++)
                    {
                        // 거리상 미탐지면 넘어감 
                        if (candidate_detected_target_h[i, j] != 1 || candidate_detected_target_v[i, j] != 1)
                        {
                            continue;
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
                            {
                                if (building_in_range_h[i, k] == 1)
                                {
                                    if (cosine_TARGET_h1 >= cosine_Building_h1[i, k] && cosine_TARGET_h2 >= cosine_Building_h2[i, k])
                                    {
                                        if (target_dist[i, j] >= building_dist[i, k])
                                        {
                                            h_detected = -2;
                                            if (j < N_Ped)
                                            {
                                                clog.addShadowedLog(i, k, 'h', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                            }
                                            else
                                            {
                                                clog.addShadowedLog(i, k, 'h', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
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
                            {
                                if (building_in_range_v[i, k] == 1)
                                {
                                    if (cosine_TARGET_v1 >= cosine_Building_v1[i, k] && cosine_TARGET_v2 >= cosine_Building_v2[i, k])
                                    {
                                        if (target_dist[i, j] >= building_dist[i, k])
                                        {
                                            v_detected = -2;
                                            if (j < N_Ped)
                                            {
                                                clog.addShadowedLog(i, k, 'v', "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));
                                            }
                                            else
                                            {
                                                clog.addShadowedLog(i, k, 'v', "Car", j - N_Ped, Math.Round(cars[j - N_Ped].X, 2), Math.Round(cars[j - N_Ped].Y, 2), Math.Round(cars[j - N_Ped].Velocity, 2), Math.Round(nowTime, 2));
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
                                clog.addDetectedLog(i, "Pedestrian", j, Math.Round(peds[j].X, 2), Math.Round(peds[j].Y, 2), Math.Round(peds[j].Velocity, 2), Math.Round(nowTime, 2));

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
                                clog.addDetectedLog(i, "Car", carIdx, Math.Round(cars[carIdx].X, 2), Math.Round(cars[carIdx].Y, 2), Math.Round(cars[carIdx].Velocity, 2), Math.Round(nowTime, 2));

                                // Increase Velocity
                                cars[j - N_Ped].upVelocity();
                            }
                        }
                        // 건물에 가림
                        else if(h_detected == -2 || v_detected == -2)
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
                    for (int j = 0; j < N_Target; j++)
                    {
                        cctv_missing_count_h[i] += missed_map_h[i, j];
                        cctv_missing_count_v[i] += missed_map_v[i, j];
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
                            if (peds[i].X < road_min || peds[i].X > road_max)
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

                            if (peds[i].Y < road_min || peds[i].Y > road_max)
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

                        peds[i].move();
                    }
                    else
                    {
                        if (createCSV)
                        {
                            if (cars[i - pedLen].X < road_min || cars[i - pedLen].X > road_max)
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

                            if (cars[i - pedLen].Y < road_min || cars[i - pedLen].Y > road_max)
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
                        cars[i - pedLen].move();
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
            public void initialBuildingsToCSV(int simIdx)
            {
                aw.initialBuildingsToCSV(simIdx);
            }

            public void initialPedsToCSV(int simIdx)
            {
                tw.initialPedsToCSV(simIdx);
            }

            public void initialCarsToCSV(int simIdx)
            {
                tw.initialCarsToCSV(simIdx);
            }

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
                    clog.DetectedLogToCSV(cctvSetIdx, simIdx);
                }
            }

            public void ShadowedLogToCSV(int cctvSetIdx, int simIdx)
            {
                if (createCSV)
                {
                    clog.ShadowedByBuildingLogToCSV(cctvSetIdx, simIdx);
                }
            }

            public double printResultRate()
            {
                double totalSimCount = Sim_Time / aUnitTime * N_Target;
                double outOfRangeRate = 100 * outOfRange.Sum() / totalSimCount;
                double directionErrorRate = 100 * directionError.Sum() / totalSimCount;
                double shadowedRate = 100 * shadowedByBuilding.Sum() / totalSimCount;

                double successRate = 100 * R_Surv_Time.Sum() / totalSimCount;

                // 결과(탐지율)
                Console.WriteLine("====== Surveillance Time Result ======");
                Console.WriteLine("N_CCTV: {0}, N_Ped: {1}, N_Car: {2}", N_CCTV, N_Ped, N_Car);
                Console.WriteLine("[Result]");
                Console.WriteLine("  - Execution time : {0}", stopwatch.ElapsedMilliseconds + "ms");
                Console.WriteLine("[Fail]");
                Console.WriteLine("  - Out of Range: {0:F2}% ({1}/{2})", outOfRangeRate, outOfRange.Sum(), totalSimCount);
                Console.WriteLine("  - Direction Error: {0:F2}% ({1}/{2})", directionErrorRate, directionError.Sum(), totalSimCount);
                Console.WriteLine("  - Shadowed by Building: {0:F2}% ({1}/{2})", shadowedRate, shadowedByBuilding.Sum(), totalSimCount);
                Console.WriteLine("[Success]");
                Console.WriteLine("  - Surveillance Time: {0:F2}% ({1}/{2})\n", successRate, R_Surv_Time.Sum(), totalSimCount);

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
                        for (int i = 0; i < clog.getLogSize(); i++)
                        {
                            Console.WriteLine("{0, 8}\t{1, 4}\t{2, 11}\t{3, 5}\t{4, 18}\t{5, 18}\t{6, 18}\t{7}", i, clog.getCctvIdx(i), clog.getTargetType(i), clog.getTargetIdx(i), clog.getX(i), clog.getY(i), clog.getV(i), clog.getT(i));
                        }
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
