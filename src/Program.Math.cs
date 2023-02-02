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
        
        // Polygon의 무게 중심
        public static double[] calcPolyCenterOfGravity(double[] xs, double[] ys)
        {
            double[] rt = new double[2];   // [0]: x, [1]: y
            for (int i = 0; i < rt.Length; i++) rt[i] = 0;

            double factor = 0;
            double area = 0;
            for(int i = 0; i< xs.Length; i++)
            {
                int next_i = (i + 1) % xs.Length;

                factor = (xs[i] * ys[next_i]) - (xs[next_i] * ys[i]);
                area += factor;
                rt[0] += (xs[i] + xs[next_i]) * factor;
                rt[1] += (ys[i] + ys[next_i]) * factor;
            }

            area /= 2.0d;

            factor = 1.0d / (area * 6.0d);

            rt[0] *= factor;
            rt[1] *= factor;

            return rt;
        }
    }

}
