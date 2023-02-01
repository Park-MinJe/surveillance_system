﻿using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        // 좌표계 proj4 string
        public static string proj4_epsg5174 = "+proj=tmerc +lat_0=38 +lon_0=127.0028902777778 +k=1 +x_0=200000 +y_0=500000 +ellps=bessel +units=m +no_defs +towgs84=-115.80,474.99,674.11,1.16,-2.31,-1.63,6.43";
        public static string proj4_epsg4326 = "+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs";

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

            //rewrite xy array for input into Proj4
            //double[] xy = new double[2 * x.Length];
            /*int ixy = 0;
            for (int i = 0; i <= z.Length - 1; i++)
            {
                xy[ixy] = x[i];
                xy[ixy + 1] = y[i];
                z[i] = 0;
                ixy += 2;
            }*/

            DotSpatial.Projections.ProjectionInfo src =
                DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg5174);
            DotSpatial.Projections.ProjectionInfo trg =
                DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg4326);

            DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, src, trg, 0, z.Length);

            for (int i = 0; i <= z.Length - 1; i++)
            {
                Console.WriteLine("output EPSG:4326 p{0} = {1} {2}", i + 1, xy[i * 2], xy[i * 2 + 1]);
                distance[i] = getDistanceBetweenPoints(realxy[i * 2], realxy[i * 2 + 1], xy[i * 2], xy[i * 2 + 1]);
                Console.WriteLine("distance between real Coordinate = {0}", distance[i]);
            }

            Console.WriteLine();
            double mean_distance = 0;
            for(int i = 0; i<distance.Length; i++)
            {
                mean_distance += distance[i];
            }

            Console.WriteLine("{0}", mean_distance / distance.Length);

            //Console.Write("Press any key to continue . . . ");
            //Console.ReadKey(true);
        }

        // 좌표계 변환 함수
        // 추후 확장 가능하도록 매개변수로 기존 좌표계, 변환할 좌표계 받을 계획
        public static double[,] TransformCoordinate(double x1, double y1, double x2, double y2)
        {
            double[,] rt = new double[2, 2];

            double[] xy = new double[4];
            xy[0] = x1;
            xy[1] = y1;
            xy[2] = x2;
            xy[3] = y2;

            double[] z = new double[xy.Length / 2];

            DotSpatial.Projections.ProjectionInfo src =
                DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg5174);
            DotSpatial.Projections.ProjectionInfo trg =
                DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg4326);

            DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, src, trg, 0, z.Length);

            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j<2;j++)
                {
                    rt[i, j] = xy[i * 2 + j];
                }
            }

            return rt;
        }

        public static void TransformCoordinate(double[] xy)
        {
            double[] z = new double[xy.Length / 2];

            DotSpatial.Projections.ProjectionInfo src =
                DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg5174);
            DotSpatial.Projections.ProjectionInfo trg =
                DotSpatial.Projections.ProjectionInfo.FromProj4String(proj4_epsg4326);

            DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, src, trg, 0, z.Length);
        }

        public static double getDistanceBetweenPoints(double minlat, double minlon, double maxlat, double maxlon)
        {
            double theta = maxlon - minlon;
            // kilometers
            double distance = 
                60 * 1.1515 * (180 / Math.PI) * Math.Acos(
                    Math.Sin(minlat * (Math.PI / 180)) * Math.Sin(maxlat * (Math.PI / 180)) +
                    Math.Cos(minlat * (Math.PI / 180)) * Math.Cos(maxlat * (Math.PI / 180)) * Math.Cos(theta * (Math.PI / 180))) * 1.609344 * 1000;
            return distance;
        }
    }
}