﻿using Silk.NET.Vulkan.Video;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace surveillance_system
{
    public partial class Program
    {
        public class Building
        {
            // 실제 데이터 미사용 시에만 사용
            /*public double W;
            public double D1;
            public double D2;
            public double W2;

            public double[] Pos_H1 = new double[2];
            public double[] Pos_H2 = new double[2];
            public double[] Pos_V1 = new double[2];
            public double[] Pos_V2 = new double[2];

            public double Direction = 0;*/
            /*************************************/

            public double X;
            public double Y;
            public double Z;

            public double H;

            public double areaOfBottom;

            public Point upperCorner;
            public Point lowerCorner;

            // Polygon
            public Point[] pointsOfBottom;

            public Point[] pointsOfTop;

            public double[] Directions;

            public Point[] midPoints;

            public Segment[] H_Segment;

            public Segment[] V_Segment;

            public Polygon[] facesOfBuilding;       // 첫번째 면은 바닥면과 두번째 면은 옥상면

            /*public void define_Building(
                double Width, 
                double Height
            )
            {
                Random rand = new Random();

                this.W = Width;
                this.H = Height;
                this.D1 = 90 * Math.PI / 180;   // modified by 0BoO, deg -> rad
                this.D2 = (180 + 90 * rand.NextDouble()) * Math.PI / 180; // modified by 0BoO, deg -> rad
                this.W2 = this.W / 2;

                this.Pos_H1[0] =
                    Math.Round(this.W2 * Math.Cos(D1 + this.Direction) + this.X, 2);
                this.Pos_H1[1] =
                    Math.Round(this.W2 * Math.Sin(D1 + this.Direction) + this.Y, 2);
                this.Pos_H2[0] =
                    Math.Round(this.W2 * Math.Cos(D2 + this.Direction) + this.X, 2);
                this.Pos_H2[1] =
                    Math.Round(this.W2 * Math.Sin(D2 + this.Direction) + this.Y, 2);

                this.Pos_V1[0] = this.X;
                this.Pos_V1[1] = this.H;

                this.Pos_V2[0] = this.X;
                this.Pos_V2[1] = 0;
            }*/

            // 230504 pmj
            // initalizer used to clone
            public Building() { }

            public Building(Building b)
            {
                this.X = b.X;
                this.Y = b.Y;
                this.Z = b.Z;
                this.H = b.H;

                this.areaOfBottom = b.areaOfBottom;

                this.pointsOfBottom = new Point[b.pointsOfBottom.Length];
                for(int i = 0; i < b.pointsOfBottom.Length;i++)
                {
                    this.pointsOfBottom[i] = new Point(b.pointsOfBottom[i]);
                }

                this.pointsOfTop = new Point[b.pointsOfTop.Length];
                for (int i = 0; i < b.pointsOfTop.Length; i++)
                {
                    this.pointsOfTop[i] = new Point(b.pointsOfTop[i]);
                }

                this.Directions = new double[b.Directions.Length];
                for (int i = 0; i < b.Directions.Length; i++)
                {
                    this.Directions[i] = b.Directions[i];
                }

                this.midPoints = new Point[b.midPoints.Length];
                for (int i = 0; i < b.midPoints.Length; i++)
                {
                    this.midPoints[i] = new Point(b.midPoints[i]);
                }

                this.H_Segment = new Segment[b.H_Segment.Length];
                for (int i = 0; i < b.H_Segment.Length; i++)
                {
                    this.H_Segment[i] = new Segment(b.H_Segment[i]);
                }

                this.V_Segment = new Segment[b.V_Segment.Length];
                for (int i = 0; i < b.V_Segment.Length; i++)
                {
                    this.V_Segment[i] = new Segment(b.V_Segment[i]);
                }

                this.facesOfBuilding = new Polygon[b.facesOfBuilding.Length];
                for (int i = 0; i < b.facesOfBuilding.Length; i++)
                {
                    this.facesOfBuilding[i] = new Polygon(b.facesOfBuilding[i]);
                }
            }

            // 실제 data를 이용해 건물 객체 생성
            public void define_Building(Point[] p, double h)
            {
                int dotCnt = p.Length;
                this.H = h;

                this.pointsOfBottom = new Point[dotCnt];
                this.pointsOfTop = new Point[dotCnt];
                this.lowerCorner = new Point(p[0].x, p[0].y, 0);
                this.upperCorner = new Point(p[0].x, p[0].y, 0);
                for (int i = 0; i < dotCnt; i++)
                {
                    this.pointsOfBottom[i] = new Point(p[i].x, p[i].y, 0);
                    this.pointsOfTop[i] = new Point(p[i].x, p[i].y, this.H);

                    if (this.lowerCorner.x > p[i].x)
                        this.lowerCorner.setX(p[i].x);
                    if (this.lowerCorner.y > p[i].y)
                        this.lowerCorner.setY(p[i].y);

                    if (this.upperCorner.x < p[i].x)
                        this.upperCorner.setX(p[i].x);
                    if (this.upperCorner.x < p[i].y)
                        this.upperCorner.setY(p[i].y);
                }

                this.facesOfBuilding = new Polygon[dotCnt + 1];
                this.facesOfBuilding[0] = new Polygon(this.pointsOfBottom);
                this.facesOfBuilding[1] = new Polygon(this.pointsOfTop);

                // 240205 pmj 건물 밑면의 넓이
                this.areaOfBottom = this.facesOfBuilding[0].getPolygonArea();

                this.Directions = new double[dotCnt - 1];
                this.midPoints = new Point[dotCnt - 1];
                this.H_Segment = new Segment[dotCnt - 1];
                this.V_Segment = new Segment[dotCnt - 1];

                /*---------------------- 다각형과 H가 이루는 다면체의 각 면마다 연산 필요 ----------------------*/
                for(int i = 0; i< dotCnt - 1; i++)
                {
                    this.H_Segment[i] = facesOfBuilding[0].segments[i];

                    this.V_Segment[i] = new Segment(this.pointsOfBottom[i], this.pointsOfTop[i]);

                    this.midPoints[i] = calcMidpointOfLine(this.H_Segment[i]);

                    this.Directions[i] = calcDirection(this.H_Segment[i]);
                }

                for (int i = 0, faceIdx = 2; i < dotCnt - 1; i++, faceIdx++)
                {
                    Segment[] segments = new Segment[4];
                    segments[0] = this.H_Segment[i];
                    segments[1] = this.V_Segment[(i + 1) % (dotCnt - 1)];
                    segments[2] = facesOfBuilding[1].segments[i];
                    segments[3] = this.V_Segment[i];

                    this.facesOfBuilding[faceIdx] = new Polygon(segments);
                }

                Point midpointOfPolygon = calcMidpointOfPolygon(facesOfBuilding[0]);
                this.X = midpointOfPolygon.x;
                this.Y = midpointOfPolygon.y;
                this.Z = midpointOfPolygon.z;
            }

            // 벡터 H1H2의 직교 벡터를 이용해 Direction 연산
            public double calcDirection(Segment l)
            {
                double rt;

                lVector H1H2Vector = new lVector(l.p2.x - l.p1.x, 
                                                    l.p2.y - l.p1.y);

                lVector verticalVector = new lVector(-H1H2Vector.componet_y, H1H2Vector.componet_x);

                lVector XunitVector = new lVector(0.001, 0);
                rt = Math.Round(Math.Acos(InnerProduct(verticalVector, XunitVector) / (Norm(verticalVector) * Norm(XunitVector))), 8);
                if (verticalVector.componet_y < 0)
                {
                    rt = Math.Round(2 * Math.PI - rt, 8);
                }

                return rt;
            }

            public void printBuildingInfo()
            {
                Console.WriteLine("======================Info======================");
                Console.WriteLine("면 좌표 :");
                for (int i = 0; i < facesOfBuilding.Length; i++)
                {
                    Console.Write("#면 {0}", i);
                    int segmentNum = facesOfBuilding[i].segments.Length;
                    for(int j = 0; j<segmentNum; j++)
                    {
                        Console.WriteLine("\t-선분 {0}", j);
                        Segment tmp = facesOfBuilding[i].segments[j];
                        Console.WriteLine("\t  P1 : ({0},{1},{2})   P2 : ({3},{4},{5})",
                            tmp.p1.x, tmp.p1.y, tmp.p1.z,
                            tmp.p2.x, tmp.p2.y, tmp.p2.z);
                    }
                }

                foreach (double d in Directions)
                {
                    Console.WriteLine("\n방향 각도(라디안) : {0}", d);
                }

                for(int i = 0; i<this.H_Segment.Length; i++)
                {
                    Console.WriteLine("\nPos_H1 : ({0},{1},{2})   Pos_H2 : ({3},{4},{5})",
                        this.H_Segment[i].p1.x, this.H_Segment[i].p1.y, this.H_Segment[i].p1.z, 
                        this.H_Segment[i].p2.x, this.H_Segment[i].p2.y, this.H_Segment[i].p2.z);
                    Console.WriteLine("Pos_V1 : ({0},{1},{2})   Pos_V2 : ({3},{4},{5})",
                        this.V_Segment[i].p1.x, this.V_Segment[i].p1.y, this.V_Segment[i].p1.z, 
                        this.V_Segment[i].p2.x, this.V_Segment[i].p2.y, this.V_Segment[i].p2.z);
                }
            }
        }
    }
}
