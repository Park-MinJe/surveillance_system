using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace surveillance_system
{
    public partial class Program
    {
        public class Architecture
        {
            // 실제 데이터 미사용 시에만 사용
            /*public double W;
            public double D1;
            public double D2;
            public double W2;

            public double[] Pos_H1 = new double[2];
            public double[] Pos_H2 = new double[2];
            public double[] Pos_V1 = new double[2];
            public double[] Pos_V2 = new double[2];*/
            /*************************************/

            public double X;
            public double Y;
            public double Z;

            public double Direction = 0;

            public double H;

            // Polygon
            public Point[] pointsOfBottom;

            public Point[] pointsOfTop;

            public double[] Directions;

            public Point[] midPoints;

            public Segment[] H_Segment;

            public Segment[] V_Segment;

            public Polygon[] facesOfArch;       // 첫번째 면은 바닥면과 두번째 면은 옥상면

            /*public void define_Architecture(
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

            // 실제 data를 이용해 건물 객체 생성
            public void define_Architecture(Point[] p, double h)
            {
                int dotCnt = p.Length;
                this.H = h;

                this.pointsOfBottom = new Point[dotCnt];
                this.pointsOfTop = new Point[dotCnt];
                for (int i = 0; i < dotCnt; i++)
                {
                    this.pointsOfBottom[i] = new Point(p[i].getX(), p[i].getY(), 0);
                    this.pointsOfTop[i] = new Point(p[i].getX(), p[i].getY(), this.H);
                }

                this.facesOfArch = new Polygon[dotCnt + 1];
                this.facesOfArch[0] = new Polygon(this.pointsOfBottom);
                this.facesOfArch[1] = new Polygon(this.pointsOfTop);

                this.Directions = new double[dotCnt - 1];
                this.midPoints = new Point[dotCnt - 1];
                this.H_Segment = new Segment[dotCnt - 1];
                this.V_Segment = new Segment[dotCnt - 1];

                /*---------------------- 다각형과 H가 이루는 다면체의 각 면마다 연산 필요 ----------------------*/
                for(int i = 0; i< dotCnt - 1; i++)
                {
                    this.H_Segment[i] = facesOfArch[0].getSegmentByIdx(i);

                    this.V_Segment[i] = new Segment(this.pointsOfBottom[i], this.pointsOfTop[i]);

                    this.midPoints[i] = calcMidpointOfLine(this.H_Segment[i]);

                    this.Directions[i] = calcDirection(this.H_Segment[i]);
                }

                for (int i = 0, faceIdx = 2; i < dotCnt - 1; i++, faceIdx++)
                {
                    Segment[] segments = new Segment[4];
                    segments[0] = this.H_Segment[i];
                    segments[1] = this.V_Segment[(i + 1) % (dotCnt - 1)];
                    segments[2] = facesOfArch[1].getSegmentByIdx(i);
                    segments[3] = this.V_Segment[i];

                    this.facesOfArch[faceIdx] = new Polygon(segments);
                }

                Point midpointOfPolygon = calcMidpointOfPolygon(facesOfArch[0]);
                this.X = midpointOfPolygon.getX();
                this.Y = midpointOfPolygon.getY();
                this.Z = midpointOfPolygon.getZ();
            }

            // 벡터 H1H2의 직교 벡터를 이용해 Direction 연산
            public double calcDirection(Segment l)
            {
                double rt;

                lVector H1H2Vector = new lVector(l.getP2().getX() - l.getP1().getX(), 
                                                    l.getP2().getY() - l.getP1().getY());

                lVector verticalVector = new lVector(-H1H2Vector.getComponetY(), H1H2Vector.getComponetX());

                lVector XunitVector = new lVector(0.001, 0);
                rt = Math.Round(Math.Acos(InnerProduct(verticalVector, XunitVector) / (Norm(verticalVector) * Norm(XunitVector))), 8);
                if (verticalVector.getComponetY() < 0)
                {
                    rt = Math.Round(2 * Math.PI - Direction, 8);
                }

                return rt;
            }

            public void printArchInfo()
            {
                Console.WriteLine("======================Info======================");
                Console.WriteLine("면 좌표 :");
                for (int i = 0; i < facesOfArch.Length; i++)
                {
                    Console.Write("#면 {0}", i);
                    int segmentNum = facesOfArch[i].getSegmentCnt();
                    for(int j = 0; j<segmentNum; j++)
                    {
                        Console.WriteLine("\t-선분 {0}", j);
                        Segment tmp = facesOfArch[i].getSegmentByIdx(j);
                        Console.WriteLine("\t  P1 : ({0},{1},{2})   P2 : ({3},{4},{5})",
                            tmp.getP1().getX(), tmp.getP1().getY(), tmp.getP1().getZ(),
                            tmp.getP2().getX(), tmp.getP2().getY(), tmp.getP2().getZ());
                    }
                }

                foreach (double d in Directions)
                {
                    Console.WriteLine("\n방향 각도(라디안) : {0}", d);
                }

                for(int i = 0; i<this.H_Segment.Length; i++)
                {
                    Console.WriteLine("\nPos_H1 : ({0},{1},{2})   Pos_H2 : ({3},{4},{5})",
                        this.H_Segment[i].getP1().getX(), this.H_Segment[i].getP1().getY(), this.H_Segment[i].getP1().getZ(), 
                        this.H_Segment[i].getP2().getX(), this.H_Segment[i].getP2().getY(), this.H_Segment[i].getP1().getZ());
                    Console.WriteLine("Pos_V1 : ({0},{1},{2})   Pos_V2 : ({3},{4},{5})",
                        this.V_Segment[i].getP1().getX(), this.V_Segment[i].getP1().getY(), this.V_Segment[i].getP1().getZ(), 
                        this.V_Segment[i].getP2().getX(), this.V_Segment[i].getP2().getY(), this.V_Segment[i].getP2().getZ());
                }
            }
        }
    }
}
