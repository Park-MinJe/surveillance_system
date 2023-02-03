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
        public class Point
        {
            private double x, y;

            public Point() { }

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public void setPoint(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public void setX(double x) { this.x = x; }
            public void setY(double y) { this.y = y; }

            public double getX() { return this.x; }
            public double getY() { return this.y; }
        }

        public class Line
        {
            private Point p1;
            private Point p2;

            public Line(Point p1, Point p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
            public Line(double p1X, double p1Y, double p2X, double p2Y)
            {
                this.p1 = new Point(p1X, p1Y);
                this.p2 = new Point(p2X, p2Y);
            }

            public Point getP1() { return this.p1; }
            public Point getP2() { return this.p2; }
        }

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

            public double getComponetX() { return this.componet_x; }
            public double getComponetY() { return this.componet_y; }
        }

        public class Polygon
        {
            
        }
    }
}
