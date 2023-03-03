using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        // 좌표계 proj4 string
        // 구글맵
        // 거리 계산 안됨
        public static string proj4_epsg3857 = "+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +no_defs";
        // LOCALDATA CCTV정보
        public static string proj4_epsg4326 = "+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs";
        
        // 국토교통부_건물융합정보
        public static string proj4_epsg5174 = "+proj=tmerc +lat_0=38 +lon_0=127.0028902777778 +k=1 +x_0=200000 +y_0=500000 +ellps=bessel +units=m +no_defs +towgs84=-115.80,474.99,674.11,1.16,-2.31,-1.63,6.43";

        // DotSpatial을 사용한 좌표계 변환
        public static void demo_DotSpatial()
        {
            Console.WriteLine("C#: Test coordinate conversion from EPSG:5174 to EPSG:4326");

            /*double[] x = { 201135.301 };
            double[] y = { 453182.0030000005 };*/
            double[] xy = { 198076.9349999996, 451133.9399999995, 198076.1840000004, 451123.4399999995, 198059.9500000002, 451125.0649999995, 198060.7779999999, 451135.4399999995, 198076.9349999996, 451133.9399999995 };
            double[] z = new double[xy.Length / 2];

            double[] realxy = { 126.978995, 37.562515, 126.978991, 37.562466, 126.978830, 37.562481, 126.978831, 37.562525, 126.978995, 37.562515 };
            double[] distance = new double[xy.Length / 2];

            for (int i = 0; i <= z.Length - 1; i++)
            {
                Console.WriteLine("input geographic EPSG:5174 p{0} = {1} {2}", i + 1, xy[i * 2], xy[i * 2 + 1]);
                z[i] = 0;
            }

            DotSpatial.Projections.ProjectionInfo src =
                DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg5174);
            DotSpatial.Projections.ProjectionInfo trg =
                DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg4326);
            
            DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, src, trg, 0, z.Length);

            for (int i = 0; i <= z.Length - 1; i++)
            {
                Console.WriteLine("output EPSG:4326 p{0} = {1} {2}", i + 1, xy[i * 2], xy[i * 2 + 1]);
                distance[i] = getDistanceBetweenPointsOfepsg4326(realxy[i * 2], realxy[i * 2 + 1], xy[i * 2], xy[i * 2 + 1]);
                Console.WriteLine("distance between real Coordinate = {0}", distance[i]);
            }

            Console.WriteLine();
            double mean_distance = 0;
            for(int i = 0; i<distance.Length; i++)
            {
                mean_distance += distance[i];
            }

            Console.WriteLine("{0}", mean_distance / distance.Length);
        }

        // 좌표계 변환 함수
        // 1개의 점
        public static Point TransformCoordinate(Point p, int src_epsgN, int trg_epsgN)
        {
            Point rt = new Point();

            double[] xy = new double[2];
            xy[0] = p.getX();
            xy[1] = p.getY();

            double[] z = new double[xy.Length / 2];
            z[0] = p.getZ();

            // 원본 좌표계
            DotSpatial.Projections.ProjectionInfo src = new DotSpatial.Projections.ProjectionInfo();
            switch (src_epsgN)
            {
                case 3857: 
                    src = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg3857);
                    break;
                case 4326:
                    src = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg4326);
                    break;
                case 5174:
                    src = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg5174);
                    break;
            }

            // 변환할 좌표계
            DotSpatial.Projections.ProjectionInfo trg = new DotSpatial.Projections.ProjectionInfo();
            switch (trg_epsgN)
            {
                case 3857:
                    trg = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg3857);
                    break;
                case 4326:
                    trg = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg4326);
                    break;
                case 5174:
                    trg = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg5174);
                    break;
            }

            // 좌표계 변환
            DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, src, trg, 0, z.Length);

            rt = new Point(xy[0], xy[1], 0d);

            return rt;
        }
        // 여러개의 점
        public static Point[] TransformCoordinate(Point[] ps, int src_epsgN, int trg_epsgN)
        {
            Point[] rt = new Point[ps.Length];

            double[] xy = new double[2 * ps.Length];
            double[] z = new double[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                xy[i * 2] = ps[i].getX();
                xy[i * 2 + 1] = ps[i].getY();
                z[i] = ps[i].getZ();
            }

            // 원본 좌표계
            DotSpatial.Projections.ProjectionInfo src = new DotSpatial.Projections.ProjectionInfo();
            switch (src_epsgN)
            {
                case 3857:
                    src = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg3857);
                    break;
                case 4326:
                    src = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg4326);
                    break;
                case 5174:
                    src = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg5174);
                    break;
            }

            // 변환할 좌표계
            DotSpatial.Projections.ProjectionInfo trg = new DotSpatial.Projections.ProjectionInfo();
            switch (trg_epsgN)
            {
                case 3857:
                    trg = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg3857);
                    break;
                case 4326:
                    trg = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg4326);
                    break;
                case 5174:
                    trg = DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg5174);
                    break;
            }

            // 좌표계 변환
            DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, src, trg, 0, z.Length);

            for (int i = 0; i < ps.Length; i++)
            {
                rt[i] = new Point(xy[i * 2], xy[i * 2 + 1], z[i]);
            }

            return rt;
        }
        // proj4_epsg4326 to system 좌표계
        public static Point[] calcIndexOnProg(Point[] ps, double lowerCornerX, double upperCornerY)
        {
            Point[] rt = new Point[ps.Length];
            for(int i = 0; i<ps.Length; i++)
            {
                double x = ps[i].getX();
                double y = ps[i].getY();

                rt[i] = new Point(getDistanceBetweenPointsOfepsg4326(lowerCornerX, y, x, y),
                                    getDistanceBetweenPointsOfepsg4326(x, upperCornerY, x, y),
                                    0d);
            }

            return rt;
        }

        public static double getDistanceBetweenPointsOfepsg4326(double lat0, double lon0, double lat1, double lon1)
        {
            double minlat = Math.Min(lat0, lat1);
            double maxlat = Math.Max(lat0, lat1);
            double minlon = Math.Min(lon0, lon1);
            double maxlon = Math.Max(lon0, lon1);

            double theta = maxlon - minlon;
            // kilometers
            double distance = 
                60 * 1.1515 * (180 / Math.PI) * Math.Acos(
                    Math.Sin(minlat * (Math.PI / 180)) * Math.Sin(maxlat * (Math.PI / 180)) +
                    Math.Cos(minlat * (Math.PI / 180)) * Math.Cos(maxlat * (Math.PI / 180)) * Math.Cos(theta * (Math.PI / 180))) * 1.609344 * 1000 * 1000;
            return Math.Round(distance, 10);
        }
    }
}
