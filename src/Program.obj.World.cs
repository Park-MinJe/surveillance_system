using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        // 시뮬레이션 모델로 구현된 디지털 트윈
        public class World
        {
            // World 제원
            // Map의 실제 범위
            public Point lowerCorner { get; private set; }
            public Point upperCorner { get; private set; }

            // Map의 가로/세로 길이
            public double X_mapSize { get; private set; }
            public double Y_mapSize { get; private set; }

            //Map grid
            public int X_grid_num;
            public int Y_grid_num;

            // Map grid 상 obj 위치
            // 건물 위치
            public int[,] archPos;
            // CCTV 위치
            public int[,] cctvPos;
            // Ped 위치
            public int[,] pedPos;
            // Car 위치
            public int[,] carPos;


            // World를 구성하는 구조물
            // 도로
            public Road road { get; private set; }

            // 건물
            public int nArch { get; private set; }
            public List<Architecture> archs { get; private set; }


            // World 내의 카메라
            // CCTV
            public int nCctv { get; private set; }
            public CCTV[] cctvs { get; private set; }


            // World를 구성하는 구성원
            // 감시 대상
            public int nPed { get; private set; }
            public int nCar { get; private set; }
            public int nTrg { get; private set; }
            public SurveillanceTarget[] surveillanceTargets { get; private set; }


            /* ----------------------------------------------------------------------------------------------------------------------
             * World 변수 초기화
            ---------------------------------------------------------------------------------------------------------------------- */
            // 기본적인 변수를 초기화하고, 객체 및 배열을 동적할당 한다.
            // 이때 객체의 할당은 Factory method를 사용한다.
            public void define_World(int nCctv, int nPed, int nCar, initWorld initWorldBy)
            {
                // World 제원 초기화

                // EPSG:4326 좌표계 기반 탐색 범위
                this.lowerCorner = initWorldBy.initLowerCorner();
                this.upperCorner = initWorldBy.initUpperCorner();

                // 실제 탐색 범위 좌표를 이용해 탐색 범위의 가로/세로 제원 획득
                this.X_mapSize = initWorldBy.X_mapSize(this.lowerCorner, this.upperCorner);
                //Console.WriteLine("x map size: {0}", this.X_mapSize);
                this.Y_mapSize = initWorldBy.Y_mapSize(this.lowerCorner, this.upperCorner);
                //Console.WriteLine("y map size: {0}", this.Y_mapSize);

                // Map grid 세팅
                this.X_grid_num = (int)Math.Truncate(this.X_mapSize) / 10000 + 1;
                //Console.WriteLine("x grid num: {0}", this.X_grid_num);
                this.Y_grid_num = (int)Math.Truncate(this.Y_mapSize) / 10000 + 1;
                //Console.WriteLine("y grid num: {0}", this.Y_grid_num);

                this.archPos = new int[this.Y_grid_num, this.X_grid_num];
                this.cctvPos = new int[this.Y_grid_num, this.X_grid_num];
                this.pedPos = new int[this.Y_grid_num, this.X_grid_num];
                this.carPos = new int[this.Y_grid_num, this.X_grid_num];



                // World 구성 요소 갯수 입력

                // 임의의 값을 세팅할 때 필요  ex) 현재 보행자, 차량, cctv
                this.nCctv = nCctv;
                this.nPed = nPed;
                this.nCar = nCar;
                this.nTrg = this.nPed + this.nCar;
            }


            /* ----------------------------------------------------------------------------------------------------------------------
             * World 구성 객체 초기화
            ---------------------------------------------------------------------------------------------------------------------- */
            
            // 도로
            public void initRoad(int wd, int n_interval)
            {
                // 할당
                this.road = new RoadFactory().createRoad();

                // 초기화
                this.road.define_Road(this.X_mapSize, this.Y_mapSize, wd, n_interval, new initRoadByUsrInput());
            }

            // 건물
            public void initArch(initArchType initType)
            {
                this.archs = new ArchFactory().createArchList();
                initArchitecture initArchBy;

                if (initType == initArchType.GIS)
                {
                    initArchBy = new initArchByGis(lowerCorner, upperCorner);

                    for (; initArchBy.nextArch();)
                    {
                        Architecture tmpArch = new ArchFactory().createArch();
                        tmpArch.define_Architecture(initArchBy);
                    }
                }
            }

            // CCTV
            public void initCctv()
            {
                this.cctvs = new CctvFactory().createCctvArr(this.nCctv);
                for (int i = 0; i < this.nCctv; i++)
                {
                    
                }


            }

            // 감시 대상
            public void initSurvTrg(
                int simIdx, 
                double Width,
                double Height,
                double Velocity, 
                double minDist, 
                initSurvType initType
            )
            {
                // 할당
                this.surveillanceTargets = new SurveillanceTargetFactory().createSurvArr(this.nTrg);
                int j = 0;
                for (j = 0; j < this.nPed; j++)
                {
                    this.surveillanceTargets[j] = new SurveillanceTargetFactory().createPed();
                }
                for (; j < this.nTrg; j++)
                {
                    this.surveillanceTargets[j] = new SurveillanceTargetFactory().createCar();
                }

                // 초기화
                int trgIdx = 0;
                foreach (SurveillanceTarget st in surveillanceTargets)
                {
                    if(st is Pedestrian)
                    {
                        if(initType == initSurvType.RAND)
                        {
                            st.define_TARGET(new initSurvTrgByRandom(TargetType.PED, Width, Height, Velocity, minDist, this.road));
                        }
                        else if(initType == initSurvType.CSV)
                        {
                            st.define_TARGET(new initSurvTrgByCsv(TargetType.PED, simIdx, trgIdx));
                        }
                    }
                    else if(st is Car)
                    {
                        if (initType == initSurvType.RAND)
                        {
                            st.define_TARGET(new initSurvTrgByRandom(TargetType.CAR, Width, Height, Velocity, minDist, this.road));
                        }
                        else if (initType == initSurvType.CSV)
                        {
                            st.define_TARGET(new initSurvTrgByCsv(TargetType.CAR, simIdx, trgIdx - this.nPed));
                        }
                    }

                    st.setDirection();
                    trgIdx++;
                }
            }
        }
    }
}
