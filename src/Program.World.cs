using System;
using System.Collections.Generic;
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


            // World를 구성하는 구조물
            // 도로
            public Road road { get; private set; }

            // 건물
            public int nArch { get; private set; }
            public Architecture[] archs { get; private set; }


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

            public World(int nCctv, int nPed, int nCar, initWorld initWorldBy)
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

                

                // World 구성 요소 갯수 입력
                
                // 임의의 값을 세팅할 때 필요  ex) 현재 보행자, 차량, cctv
                this.nCctv = nCctv;
                this.nPed = nPed;
                this.nCar = nCar;
                this.nTrg = this.nPed + this.nCar;



                // World를 구성하는 구조물 할당

                // 도로
                this.road = new RoadFactory().createRoad();

                // 건물




                // World 내의 카메라 할당

                // CCTV
                



                // World를 구성원 할당

                // 감시 대상
                
            }
        }
    }
}
