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
        // lVector를 이용한 내적
        public static double InnerProduct(lVector a, lVector b)
        {
            return a.getComponetX() * b.getComponetX() + a.getComponetY() * b.getComponetY();
        }

        public static double Norm(double[] a)
        {
            return Math.Sqrt(a[0] * a[0] + a[1] * a[1]);
        }
        // lVector를 이용한 벡터 Norm
        public static double Norm(lVector a)
        {
            return Math.Sqrt(a.getComponetX() * a.getComponetX() + a.getComponetY() * a.getComponetY());
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

        // 선분의 중점
        public static Point calcMidpointOfLine(Line l)
        {
            Point rt = new Point();

            rt.setX((l.getP1().getX() + l.getP2().getX()) / 2);
            rt.setY((l.getP1().getY() + l.getP2().getY()) / 2);

            return rt;
        }
    }

}
