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
            private Point lowerCorner;
            private Point upperCorner;

                // Map의 가로/세로 길이
            private double X_mapSize;
            private double Y_mapSize;


            // World를 구성하는 구조물
                // 도로
            private Road road;

                // 건물
            private Architecture[] archs;


            // World 내의 카메라
                // CCTV
            private CCTV[] cctvs;


            // World를 구성하는 구성원
                // 감시 대상
            private SurveillanceTarget[] surveillanceTargets;

            public World()
            {
                // World 제원 초기화
                    // GIS open api로 부터 받은 탐색 범위
                lowerCorner = gbs.getMapLowerCorner();
                upperCorner = gbs.getMapUpperCorner();

                    // 실제 탐색 범위 좌표를 프로그램에서 사용할 좌표계로 변환
                lowerCorner = TransformCoordinate(lowerCorner, 5174, 4326);
                upperCorner = TransformCoordinate(upperCorner, 5174, 4326);

                    // 실제 탐색 범위 좌표를 이용해 탐색 범위의 가로/세로 제원 획득
                this.X_mapSize = getDistanceBetweenPointsOfepsg4326(lowerCorner.getX(), lowerCorner.getY(), upperCorner.getX(), lowerCorner.getY());
                //Console.WriteLine("x map size: {0}", this.X_mapSize);
                this.Y_mapSize = getDistanceBetweenPointsOfepsg4326(lowerCorner.getX(), lowerCorner.getY(), lowerCorner.getX(), upperCorner.getY());
                //Console.WriteLine("y map size: {0}", this.Y_mapSize);


                // World를 구성하는 구조물 초기화
                    // 도로
                road = 
            }
        }
    }
}
