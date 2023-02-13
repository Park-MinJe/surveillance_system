using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public interface initWorld
        {
            Point initLowerCorner();
            Point initUpperCorner();

            double X_mapSize(Point lowerCorner, Point upperCorner);
            double Y_mapSize(Point lowerCorner, Point upperCorner);
        }

        public class initWorldByGis : initWorld
        {
            public Point initLowerCorner()
            {
                // GIS open api로 부터 받은 탐색 범위
                Point rt = gbs.getMapLowerCorner();

                // 실제 탐색 범위 좌표를 프로그램에서 사용할 좌표계로 변환
                rt = TransformCoordinate(rt, 5174, 4326);

                return rt;
            }

            public Point initUpperCorner()
            {
                // GIS open api로 부터 받은 탐색 범위
                Point rt = gbs.getMapUpperCorner();

                // 실제 탐색 범위 좌표를 프로그램에서 사용할 좌표계로 변환
                rt = TransformCoordinate(rt, 5174, 4326);

                return rt;
            }

            public double X_mapSize(Point lowerCorner, Point upperCorner)
            {
                return getDistanceBetweenPointsOfepsg4326(lowerCorner.x, lowerCorner.y, upperCorner.x, lowerCorner.y);
            }

            public double Y_mapSize(Point lowerCorner, Point upperCorner)
            {
                return getDistanceBetweenPointsOfepsg4326(lowerCorner.x, lowerCorner.y, lowerCorner.x, upperCorner.y);
            }
        }
    }
}
