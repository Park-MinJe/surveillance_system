using DotSpatial.Projections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public class lVector
        {
            public double componet_x { get; private set; }
            public double componet_y { get; private set; }

            public lVector() { }

            // 230504 pmj
            // initalizer used to clone
            public lVector(lVector lv)
            {
                this.componet_x = lv.componet_x;
                this.componet_y = lv.componet_y;
            }

            public lVector(double x, double y)
            {
                this.componet_x = x;
                this.componet_y = y;
            }

            // 벡터 p1p2
            public void setComponetX(Point p1, Point p2)
            {
                this.componet_x = p2.x - p1.x;
            }
            public void setComponetY(Point p1, Point p2)
            {
                this.componet_y = p2.y - p1.y;
            }
        }

        public class Point
        {
            public double x { get; private set; }
            public double y { get; private set; }
            public double z { get; private set; }

            public Point() { }

            // 230504 pmj
            // initalizer used to clone
            public Point(Point p)
            {
                this.x = p.x;
                this.y = p.y;
                this.z = p.z;
            }

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

            public void printString()
            {
                Console.WriteLine("x: {0}\ty: {1}\tz: {2}", this.x, this.y, this.z);
            }
        }

        public class Segment
        {
            public Point p1 { get; private set; }
            public Point p2 { get; private set; }

            public Segment() { }

            // 230504 pmj
            // initalizer used to clone
            public Segment(Segment s)
            {
                this.p1 = new Point(s.p1);
                this.p2 = new Point(s.p2);
            }

            public Segment(Point p1, Point p2)
            {
                this.p1 = new Point(p1);
                this.p2 = new Point(p2);
            }
            public Segment(double p1X, double p1Y, double p1Z, double p2X, double p2Y, double p2Z)
            {
                this.p1 = new Point(p1X, p1Y, p1Z);
                this.p2 = new Point(p2X, p2Y, p2Z);
            }

            public void setP1(Point p1) { this.p1 = p1; }
            public void setP2(Point p2) { this.p2 = p2; }
        }

        public class Polygon
        {
            public Segment[] segments { get; private set; }

            public Polygon() { }

            // 230504 pmj
            // initalizer used to clone
            public Polygon(Polygon p)
            {
                this.segments = new Segment[p.segments.Length];
                for(int i = 0; i < p.segments.Length; i++)
                {
                    this.segments[i] = new Segment(p.segments[i]);
                }
            }
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
        }

        public static int getPositionOfPointRelativeToSegment(Segment AB, Point anotherP)
        {
            double dxAB, dxAP, dyAB, dyAP;
            int dir= 0;

            dxAB = AB.p2.x - AB.p1.x;
            dyAB = AB.p2.y - AB.p1.y;

            dxAP = anotherP.x - AB.p1.x;
            dyAP = anotherP.y - AB.p1.y;

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
            if (((getPositionOfPointRelativeToSegment(AB, CD.p1) * getPositionOfPointRelativeToSegment(AB, CD.p2)) <= 0) &&
                ((getPositionOfPointRelativeToSegment(CD, AB.p1) * getPositionOfPointRelativeToSegment(CD, AB.p2)) <= 0))
            {
                SegmentCrossing = true;
            }
            else SegmentCrossing = false;
            return SegmentCrossing;
        }
    }
}
