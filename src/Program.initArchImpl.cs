using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        public interface initArchitecture
        {
            double initX();
            double initY();
            double initZ();
            double initH();
            Point[] initPointsOfBottom();
            double[] initDirections();
            Point[] initMidPoints();
            Segment[] initH_Segment();
            Segment[] initV_Segment();
            Polygon[] initFacesOfArch();
        }

        public class initArchByGis : initArchitecture
        {
            List<Point[]> pls;
            List<double> hs;

            public initArchByGis(Point lowerCorner, Point upperCorner)
            {
                gbs.readFeatureMembers();
                gbs.readPosLists();
                gbs.readArchHs();

                List<Point[]> pls = new List<Point[]>();
                List<double> hs = new List<double>();

                for (int i = 0; i < gbs.getFeatureMembersCnt(); i++)
                {
                    double h = gbs.getArchH(i);
                    if (h > 0)
                    {
                        hs.Add(h);

                        Point[] pl = gbs.getPosList(i);
                        Point[] transformedPl = TransformCoordinate(pl, 5174, 4326);

                        // 프로그램상의 좌표계로 변환
                        // 지도 범위의 왼쪽 위를 기준으로 한다.
                        Point[] plOnSystem = calcIndexOnProg(transformedPl, lowerCorner.x, upperCorner.y);

                        pls.Add(plOnSystem);
                    }
                }
            }

            public int getNArchs()
            {
                return pls.Count;
            }
            public double initX()
            {
                
            }
            public double initY()
            {

            }
            public double initZ()
            {

            }
            public double initH()
            {

            }
            public Point[] initPointsOfBottom()
            {

            }
            public double[] initDirections()
            {

            }
            public Point[] initMidPoints()
            {

            }
            public Segment[] initH_Segment()
            {

            }
            public Segment[] initV_Segment()
            {

            }
            public Polygon[] initFacesOfArch()
            {

            }
        }
    }
}
