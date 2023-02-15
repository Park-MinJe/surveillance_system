using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        // 도로 데이터를 받아올 api의 출력 양식에 따라 변경 필요
        // 우선 리펙토링 절차의 결과 확인을 위해 기존의 방식대로 진행하나, 수정 필요
        public interface initRoad
        {
            Point[] initDst(int lane_num, double X_interval, double Y_interval, int wd);

            Point[,] initIntersectionArea(int lane_num, Point[] dst, int wd);
        }

        public class initRoadByUsrInput : initRoad
        {
            public Point[] initDst(int lane_num, double X_interval, double Y_interval, int wd)
            {
                Point[] rt = new Point[lane_num * lane_num];

                for(int idx = 0, i = 0; i < lane_num; i++)
                {
                    for(int j = 0; j < lane_num; idx++, j++)
                    {
                        double dstX = (X_interval + wd) * i + (wd / 2);
                        double dstY = (Y_interval + wd) * j + (wd / 2);

                        rt[idx] = new Point(dstX, dstY, 0d);
                    }
                }

                return rt;
            }

            public Point[,] initIntersectionArea(int lane_num, Point[] dst, int wd)
            {
                Point[,] rt = new Point[lane_num * lane_num, 2];

                for (int idx = 0, i = 0; i < lane_num; i++)
                {
                    for (int j = 0; j < lane_num; idx++, j++)
                    {
                        double intersectionArea_xMin = dst[idx].x - (wd / 2);
                        double intersectionArea_xMax = dst[idx].x + (wd / 2);
                        double intersectionArea_yMin = dst[idx].y - (wd / 2);
                        double intersectionArea_yMax = dst[idx].y + (wd / 2);

                        rt[idx, 0] = new Point(intersectionArea_xMin, intersectionArea_yMin, 0d);
                        rt[idx, 1] = new Point(intersectionArea_xMax, intersectionArea_yMax, 0d);
                    }
                }

                return rt;
            }
        }
    }
}
