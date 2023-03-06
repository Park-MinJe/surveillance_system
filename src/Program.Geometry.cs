using DotSpatial.Projections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public class lVector
        {
            private double componet_x;
            private double componet_y;

            public lVector() { }

            public lVector(double x, double y)
            {
                this.componet_x = x;
                this.componet_y = y;
            }

            // 벡터 p1p2
            public void setComponetX(Point p1, Point p2)
            {
                this.componet_x = p2.getX() - p1.getX();
            }
            public void setComponetY(Point p1, Point p2)
            {
                this.componet_y = p2.getY() - p1.getY();
            }

            public double getComponetX() { return this.componet_x; }
            public double getComponetY() { return this.componet_y; }
        }

        public class Point
        {
            private double x, y, z;

            public Point() { }

            public Point(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public void setPoint(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public void setX(double x) { this.x = x; }
            public void setY(double y) { this.y = y; }
            public void setZ(double z) { this.z = z; }

            public double getX() { return this.x; }
            public double getY() { return this.y; }
            public double getZ() { return this.z; }

            public void printString()
            {
                Console.WriteLine("x: {0}\ty: {1}\tz: {2}", this.x, this.y, this.z);
            }
        }

        public class Segment
        {
            private Point p1;
            private Point p2;

            public Segment(Point p1, Point p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
            public Segment(double p1X, double p1Y, double p1Z, double p2X, double p2Y, double p2Z)
            {
                this.p1 = new Point(p1X, p1Y, p1Z);
                this.p2 = new Point(p2X, p2Y, p2Z);
            }

            public void setP1(Point p1) { this.p1 = p1; }
            public void setP2(Point p2) { this.p2 = p2; }

            public Point getP1() { return this.p1; }
            public Point getP2() { return this.p2; }
        }

        public class Polygon
        {
            private Segment[] segments;

            public Polygon(Segment[] segments) { this.segments = segments; }
            public Polygon(Point[] vertexes)
            {
                int vertexNum = vertexes.Length;
                segments = new Segment[vertexNum - 1];
                for(int i = 0; i < vertexNum - 1; i++)
                {
                    segments[i] = new Segment(vertexes[i], vertexes[i + 1]);
                }
            }

            public void setPolygon(Point[] vertexes)
            {
                int vertexNum = vertexes.Length;
                segments = new Segment[vertexNum - 1];
                for (int i = 0; i < vertexNum - 1; i++)
                {
                    segments[i] = new Segment(vertexes[i], vertexes[i + 1]);
                }
            }

            public Segment[] getSegments() { return segments; }

            public Segment getSegmentByIdx(int idx) { return segments[idx]; }

            public int getSegmentCnt() { return segments.Length; }
        }

        public static int getPositionOfPointRelativeToSegment(Segment AB, Point anotherP)
        {
            double dxAB, dxAP, dyAB, dyAP;
            int dir= 0;

            dxAB = AB.getP2().getX() - AB.getP1().getX();
            dyAB = AB.getP2().getY() - AB.getP1().getY();

            dxAP = anotherP.getX() - AB.getP1().getX();
            dyAP = anotherP.getY() - AB.getP1().getY();

            if (dxAB * dyAP < dyAB * dxAP) dir = 1;     // AB 기울기 > AP 기울기
            if (dxAB * dyAP > dyAB * dxAP) dir = -1;    // AB 기울기 < AP 기울기
            if(dxAB*dyAP == dyAB * dxAP)                // AB 기울기 = AP 기울기
            {
                if (dxAB == 0 && dyAB == 0) dir = 0;
                if ((dxAB * dxAP < 0) || (dyAB * dyAP < 0)) dir = -1;
                else if ((dxAB * dxAB + dyAB * dyAB) >= (dxAP * dxAP + dyAP * dyAP)) dir = 0;
                else dir = 1;
            }

            return dir;
        }

        public static bool segmentIntersection(Segment AB, Segment CD)
        {
            bool SegmentCrossing = false;
            if (((getPositionOfPointRelativeToSegment(AB, CD.getP1()) * getPositionOfPointRelativeToSegment(AB, CD.getP2())) <= 0) &&
                ((getPositionOfPointRelativeToSegment(CD, AB.getP1()) * getPositionOfPointRelativeToSegment(CD, AB.getP2())) <= 0))
            {
                SegmentCrossing = true;
            }
            else SegmentCrossing = false;
            return SegmentCrossing;
        }
    }
}
