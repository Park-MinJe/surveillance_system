﻿using DotSpatial.Projections;
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
            public double componet_x { get; private set; }
            public double componet_y { get; private set; }

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
        }

        public class Point
        {
            public double x { get; private set; }
            public double y { get; private set; }
            public double z { get; private set; }

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

            public void setPoint(Point p)
            {
                this.x = p.x;
                this.y = p.y;
                this.z = p.z;
            }

            public void setX(double x) { this.x = x; }
            public void setY(double y) { this.y = y; }
            public void setZ(double z) { this.z = z; }

            public void addX(double x) { this.x += x; }
            public void addY(double y) { this.y += y; }
            public void addZ(double z) { this.z += z; }

            public void subX(double x) { this.x -= x; }
            public void subY(double y) { this.y -= y; }
            public void subZ(double z) { this.z -= z; }

            public void printString()
            {
                Console.WriteLine("x: {0}\ty: {1}\tz: {2}", this.x, this.y, this.z);
            }
        }

        public class Segment
        {
            public Point p1 { get; private set; }
            public Point p2 { get; private set; }

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
        }

        public class Polygon
        {
            public Segment[] segments { get; private set; }

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

            public Segment getSegmentByIdx(int idx) { return segments[idx]; }

            public int getSegmentCnt() { return segments.Length; }
        }
    }
}
