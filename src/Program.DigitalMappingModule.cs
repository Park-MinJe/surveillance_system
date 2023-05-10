using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        

        public class DigitalMappingModule
        {
            /* -------------------------Digital Mapping 대상 객체-------------------------*/
            public Building[] buildings { get;  private set; }
            public CCTV[] cctvs { get; private set; }
            public Pedestrian[] peds { get; private set; }
            public Car[] cars { get; private set; }

            public Road road { get; private set; } = new Road();

            /* ---------------------------시뮬레이션 조건----------------------------*/
            public int N_Building { get; private set; }         // 실제 데이터에서 받아와 initBuilding method에서 초기화
            public int N_CCTV { get; private set; } = 100;      // default 100
            public int N_Ped { get; private set; } = 5;         // default 5
            public int N_Car { get; private set; } = 5;         // default 5
            public int N_Target { get; private set; }
            
            private Random rand;

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

            private bool fixMode = false;
            private double rotateTerm = 30.0; // sec

            // calculate vertical/horizontal AOV
            private double H_AOV = 2 * Math.Atan(WD / (2 * Lens_FocalLength));//RadToDeg(2 * Math.Atan(WD / (2 * Lens_FocalLength))); // Horizontal AOV
            private double V_AOV = 2 * Math.Atan(HE / (2 * Lens_FocalLength));//RadToDeg(2 * Math.Atan(HE / (2 * Lens_FocalLength))); // Vertical AOV

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


            /* --------------------------------------
             * 초기화 함수
            -------------------------------------- */
            /* ---------------------------시뮬레이션 환경----------------------------*/
            public void initDigitalMappingVariables(int randomSeed)
            {
                rand = new Random(randomSeed);

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

                // 230314 박민제
                // 실제 데이터 활용
                /*cctvs = new CCTV[N_CCTV];
                for (int i = 0; i < N_CCTV; i++)
                {
                    cctvs[i] = new CCTV();
                }*/

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
            }

            public void initMap(int cctvMode, Point upperCorner, Point lowerCorner)
            {
                // mode 0: pos cctv as grid    1: pos cctv as random
                try
                {
                    if (On_Road_Builder)
                    {
                        // 도로 정보 생성, 보행자 정보 생성
                        road.roadBuilder(Road_Width, Road_Interval, Road_N_Interval, upperCorner, lowerCorner);
                        //road.setBuilding(N_Building);
                        road.setPed(this.peds);
                        road.setCar(this.cars);

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
                        this.initPeds();

                        // car init
                        this.initCars();

                        // cctv init
                        if (cctvMode == 3)
                        {
                            this.initCCTVByRealWorldData();
                        }
                        else
                        {
                            this.initCCTVs();
                        }
                    }
                }
                catch (Exception ex)
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
                    buildingfromApi.readFeatureMembers();
                    buildingfromApi.readPosLists();
                    buildingfromApi.readBuildingHs();

                    List<Point[]> pls = new List<Point[]>();
                    List<double> hs = new List<double>();

                    //debug
                    //int hs_cnt = 0;
                    //Console.WriteLine("Feature Member cnt = {0}", buildingfromApi.getFeatureMembersCnt());

                    for (int i = 0; i < buildingfromApi.getFeatureMembersCnt(); i++)
                    {
                        double h = buildingfromApi.getBuildingHByIdx(i);
                        if (h > 0)
                        {
                            hs.Add(h);

                            Point[] pl = buildingfromApi.getPosListByIdx(i);
                            Point[] transformedPl = TransformCoordinate(pl, 3857, 4326);

                            // 프로그램상의 좌표계로 변환
                            // 지도 범위의 왼쪽 위를 기준으로 한다.
                            Point[] plOnSystem = calcIndexOnProg(transformedPl, road.lowerCorner, road.upperCorner);

                            //debug
                            //hs_cnt++;
                            //for (int j = 0; j < plOnSystem.Length; j++)
                            //{
                            //    plOnSystem[j].printString();
                            //}
                            //Console.WriteLine();

                            pls.Add(plOnSystem);
                        }
                    }

                    this.N_Building = pls.Count;
                    //debug
                    //Console.WriteLine("N_Building = {0}", this.N_Building);
                    //Console.WriteLine("pls count = {0}", pls.Count);
                    //Console.WriteLine("hs count = {0}", hs.Count);
                    //Console.WriteLine("hs_cnt = {0}", hs_cnt);

                    buildings = new Building[this.N_Building];
                    bw.setBuildingCSVWriter(this.N_Building);
                    for (int i = 0; i < this.N_Building; i++)
                    {
                        buildings[i] = new Building();
                        buildings[i].define_Building(pls[i], hs[i]);

                        // Debug
                        //buildings[i].printBuildingInfo();
                    }
                    //Debug
                    //Console.WriteLine("Err after defining building");

                    road.setBuilding(this.N_Building, this.buildings);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Err while initializing Building\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Building Setting Completed\n");
            }

            public void initPeds()
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
                        
                        //debug
                        //ped.printTargetInfo();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Err while initializing Pedestrians\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("PED Setting Completed\n");
            }

            public void initCars()
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
                        
                        //debug
                        //car.printTargetInfo();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Err while initializing Cars\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("CAR Setting Completed\n");
            }

            public void initCCTVs()
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

                        cctvs[i].calcBlindToPed();          // (23-02-01) added by 0BoO
                        cctvs[i].calcEffDistToPed(3000);     // (23-02-01) added by 0BoO, input value is 3000mm(3meter)

                        //debug
                        // cctvs[i].printCCTVInfo();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Err while initializing CCTVs\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("\nCCTV Setting Completed\n");
            }

            // 230314 박민제
            // 실제 데이터를 활용한 cctv 객체 초기화
            // 우선 위치 데이터와 카메라 댓수 활용
            public void initCCTVByRealWorldData()
            {
                try
                {
                    cr.realWorldCctvFromCSV(this.road);
                    this.N_CCTV = cr.realWorldCctvNum;
                    cctvs = new CCTV[N_CCTV];
                    for (int i = 0; i < N_CCTV; i++)
                    {
                        cctvs[i] = new CCTV();
                    }

                    for (int i = 0; i < N_CCTV; i++)
                    {
                        cctvs[i].setZ((int)Math.Ceiling(rand.NextDouble() * (Height.Max() - 3000)) + 3000);
                        cctvs[i].WD = WD;
                        cctvs[i].HE = HE;
                        cctvs[i].imW = (int)imW;
                        cctvs[i].imH = (int)imH;
                        cctvs[i].Focal_Length = Lens_FocalLength;

                        cctvs[i].setViewAngleH(rand.NextDouble() * 360 * Math.PI / 180);  // (23-02-02) modified by 0BoO, deg -> rad
                        cctvs[i].setViewAngleV(-45.0 * Math.PI / 180);   // (23-02-02) modified by 0BoO, deg -> rad


                        cctvs[i].setFixMode(fixMode); // default (rotate)

                        cctvs[i].H_AOV = 2 * Math.Atan(WD / (2 * Lens_FocalLength));
                        cctvs[i].V_AOV = 2 * Math.Atan(HE / (2 * Lens_FocalLength));

                        // 기기 성능상의 최대 감시거리 (임시값)
                        cctvs[i].Max_Dist = 50 * 100 * 10; // 50m (milimeter)
                                                           // cctvs[i].Max_Dist = 500 * 100 * 100; // 500m (milimeter)

                        cctvs[i]
                            .get_PixelDensity(Dist,
                            cctvs[i].WD,
                            cctvs[i].HE,
                            cctvs[i].Focal_Length,
                            cctvs[i].imW,
                            cctvs[i].imH);

                        cctvs[i].get_H_FOV(Dist, cctvs[i].WD, cctvs[i].Focal_Length, cctvs[i].ViewAngleH, cctvs[i].X, cctvs[i].Y);
                        cctvs[i].get_V_FOV(Dist, cctvs[i].HE, cctvs[i].Focal_Length, cctvs[i].ViewAngleV, cctvs[i].X, cctvs[i].Z);

                        cctvs[i].calcBlindToPed();          // (23-02-01) added by 0BoO
                        cctvs[i].calcEffDistToPed(3000);     // (23-02-01) added by 0BoO, input value is 3000mm(3meter)

                        //debug
                        //cctvs[i].printCCTVInfo();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Err while initializing CCTVs\n");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("\nCCTV Setting Completed\n");
            }

            /* --------------------------------------
             * Generate JUST ONE obj. Work as Object Factory
            -------------------------------------- */

            public Pedestrian pedFactory()
            {
                Pedestrian ped = new Pedestrian();
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

                //debug
                //ped.printTargetInfo();

                return ped;
            }

            public Car carFactory()
            {
                Car car = new Car();
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

                //debug
                //car.printTargetInfo();
                
                return car;
            }

            public CCTV cctvFactory()
            {
                CCTV cctv = new CCTV();

                cctv.setZ((int)Math.Ceiling(rand.NextDouble() * (Height.Max() - 3000)) + 3000);
                cctv.WD = WD;
                cctv.HE = HE;
                cctv.imW = (int)imW;
                cctv.imH = (int)imH;
                cctv.Focal_Length = Lens_FocalLength;
                // 220104 초기 각도 설정
                // cctvs[i].ViewAngleH = rand.NextDouble() * 360;
                // cctvs[i].ViewAngleV = -35 - 20 * rand.NextDouble();

                cctv.setViewAngleH(rand.NextDouble() * 360 * Math.PI / 180);  // (23-02-02) modified by 0BoO, deg -> rad
                // cctvs[i].setViewAngleH(rand.Next(4) * 90);
                // cctvs[i].setViewAngleV(-35 - 20 * rand.NextDouble());
                cctv.setViewAngleV(-45.0 * Math.PI / 180);   // (23-02-02) modified by 0BoO, deg -> rad


                cctv.setFixMode(fixMode); // default (rotate)

                cctv.H_AOV = 2 * Math.Atan(WD / (2 * Lens_FocalLength));
                cctv.V_AOV = 2 * Math.Atan(HE / (2 * Lens_FocalLength));

                // 기기 성능상의 최대 감시거리 (임시값)
                cctv.Max_Dist = 50 * 100 * 10; // 50m (milimeter)
                                                    // cctvs[i].Max_Dist = 500 * 100 * 100; // 500m (milimeter)

                // Line 118~146
                /*  여기부턴 Road_Builder 관련 정보가 없으면 의미가 없을거같아서 주석처리했어용..
                    그리고 get_Sectoral_Coverage 이런함수도 지금은 구현해야할지 애매해서..?
                */

                cctv
                    .get_PixelDensity(Dist,
                    cctv.WD,
                    cctv.HE,
                    cctv.Focal_Length,
                    cctv.imW,
                    cctv.imH);

                cctv.get_H_FOV(Dist, cctv.WD, cctv.Focal_Length, cctv.ViewAngleH, cctv.X, cctv.Y);
                cctv.get_V_FOV(Dist, cctv.HE, cctv.Focal_Length, cctv.ViewAngleV, cctv.X, cctv.Z);

                cctv.calcBlindToPed();          // (23-02-01) added by 0BoO
                cctv.calcEffDistToPed(3000);     // (23-02-01) added by 0BoO, input value is 3000mm(3meter)

                //debug
                // cctvs[i].printCCTVInfo();

                return cctv;
            }

            /* --------------------------------------
             * init obj with csv
            -------------------------------------- */
            //public void initPedsWithCSV(int simIdx)
            //{
            //    road.setPedswithCSV(simIdx);
            //}

            //public void initCarsWithCSV(int simIdx)
            //{
            //    road.setCarswithCSV(simIdx);
            //}

            //public void initCctvsWithCSV(int cctvSetIdx)
            //{
            //    road.setCctvswithCSV(cctvSetIdx);
            //    foreach (CCTV cctv in cctvs)
            //    {
            //        clog.clearCctvLog();

            //        cctv.get_PixelDensity(Dist,
            //                cctv.WD,
            //                cctv.HE,
            //                cctv.Focal_Length,
            //                cctv.imW,
            //                cctv.imH);

            //        cctv.get_H_FOV(Dist, cctv.WD, cctv.Focal_Length, cctv.ViewAngleH, cctv.X, cctv.Y);
            //        cctv.get_V_FOV(Dist, cctv.HE, cctv.Focal_Length, cctv.ViewAngleV, cctv.X, cctv.Z);
            //    }
            //}

            /* --------------------------------------
             * save initial objs as csv
            -------------------------------------- */
            //public void initialBuildingsToCSV(int simIdx)
            //{
            //    aw.initialBuildingsToCSV(simIdx);
            //}

            //public void initialPedsToCSV(int simIdx)
            //{
            //    tw.initialPedsToCSV(simIdx);
            //}

            //public void initialCarsToCSV(int simIdx)
            //{
            //    tw.initialCarsToCSV(simIdx);
            //}

            //public void initialCctvsToCSV(int cctvIdx)
            //{
            //    cw.setCctvCSVWriter(this.N_CCTV);
            //    cw.initialCctvsToCSV(cctvIdx);
            //}
        }
    }
}
