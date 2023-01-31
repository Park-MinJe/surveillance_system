using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace surveillance_system
{
    public partial class Program
    {
        public static double InnerProduct(double[] a, double[] b)
        {
            if (a.Length != b.Length) return 0;

            double acc = 0;
            for (int i = 0; i < a.Length; i++)
            {
                acc += a[i] * b[i];
            }
            return acc;
        }

        public static double Norm(double[] a)
        {
            return Math.Sqrt(a[0] * a[0] + a[1] * a[1]);
        }

        public static double RadToDeg(double angle)
        {
            return (180 / Math.PI) * angle;
        }

        public static double DegToRad(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        static public double getDistanceBetweenPoints(double minlat, double minlon, double maxlat, double maxlon)
        {
            double theta = maxlon - minlon;
            // kilometers
            double distance = Math.Round(
                60 * 1.1515 * (180 / Math.PI) * Math.Acos(
                    Math.Sin(minlat * (Math.PI / 180)) * Math.Sin(maxlat * (Math.PI / 180)) +
                    Math.Cos(minlat * (Math.PI / 180)) * Math.Cos(maxlat * (Math.PI / 180)) * Math.Cos(theta * (Math.PI / 180))) * 1.609344
                , 2);
            return distance;
        }
    }

}
