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
            return a.componet_x * b.componet_x + a.componet_y * b.componet_y;
        }

        public static double Norm(double[] a)
        {
            return Math.Sqrt(a[0] * a[0] + a[1] * a[1]);
        }
        // lVector를 이용한 벡터 Norm
        public static double Norm(lVector a)
        {
            return Math.Sqrt(a.componet_x * a.componet_x + a.componet_y * a.componet_y);
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
        public static Point calcMidpointOfLine(Segment l)
        {
            Point rt = new Point();

            rt.setX((l.p1.x + l.p2.x) / 2);
            rt.setY((l.p1.y + l.p2.y) / 2);

            return rt;
        }

        // 꼭짓점 평균 위치
        public static Point calcMidpointOfPolygon(Polygon p)
        {
            Point rt = new Point();
            double xSum = 0, 
                ySum = 0, 
                zSum = 0;

            foreach(Segment s in p.segments)
            {
                xSum += s.p1.x;
                ySum += s.p1.y;
                zSum += s.p1.z;
            }
            int segNum = p.segments.Length;
            rt.setX(xSum / segNum);
            rt.setY(ySum / segNum);
            rt.setZ(zSum / segNum);

            return rt;
        }
    }

}
