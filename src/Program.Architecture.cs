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
            public double X;
            public double Y;

            public double Direction = 0;

            public double W;
            public double H;
            public double D1;
            public double D2;
            public double W2;

            public double[] midPoint = new double[2];
            public double[] Pos_H1 = new double[2];
            public double[] Pos_H2 = new double[2];
            public double[] Pos_V1 = new double[2];
            public double[] Pos_V2 = new double[2];

            // Polygon
            public Point[] pointsOfBottom;

            public double[] Directions;

            public Point[] midPoints;

            public Line[] H_Lines;

            public Line[] V_Lines;

            public void define_Architecture(
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
            }

            // 실제 data를 이용해 건물 객체 생성
            public void define_Architecture(double[] xy, double h)
            {
                int dotCnt = xy.Length / 2;
                this.pointsOfBottom = new Point[dotCnt];
                for (int i = 0; i < dotCnt; i++)
                {
                    this.pointsOfBottom[i] = new Point(xy[i * 2], xy[i * 2 + 1]);
                }

                this.H = h;

                Directions = new double[dotCnt - 1];
                midPoints = new Point[dotCnt - 1];
                H_Lines = new Line[dotCnt - 1];
                V_Lines = new Line[dotCnt - 1];

                /*---------------------- 다각형과 H가 이루는 다면체의 각 면마다 연산 필요 ----------------------*/
                for(int i = 0; i< dotCnt - 1; i++)
                {
                    this.H_Lines[i] = new Line(this.pointsOfBottom[i], this.pointsOfBottom[i + 1]);

                    this.V_Lines[i] = new Line(this.pointsOfBottom[i].getX(), this.H, this.pointsOfBottom[i].getX(), 0);

                    this.midPoints[i] = calcMidpointOfLine(this.H_Lines[i]);

                    this.Directions[i] = calcDirection(this.H_Lines[i]);
                }
            }

            // 벡터 H1H2의 직교 벡터를 이용해 Direction 연산
            public double calcDirection(Line l)
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
                Console.WriteLine("다각형 좌표 :");
                foreach (Point p in pointsOfBottom)
                {
                    Console.WriteLine("\t({0},{1})", p.getX(), p.getY());
                }

                foreach (double d in Directions)
                {
                    Console.WriteLine("\n방향 각도(라디안) : {0}", d);
                }

                for(int i = 0; i<this.H_Lines.Length; i++)
                {
                    Console.WriteLine("\nPos_H1 : ({0},{1})   Pos_H2 : ({2},{3})",
                        this.H_Lines[i].getP1().getX(), this.H_Lines[i].getP1().getY(), this.H_Lines[i].getP2().getX(), this.H_Lines[i].getP2().getY());
                    Console.WriteLine("Pos_V1 : ({0},{1})   Pos_V2 : ({2},{3})",
                        this.V_Lines[i].getP1().getX(), this.V_Lines[i].getP1().getY(), this.V_Lines[i].getP2().getX(), this.V_Lines[i].getP2().getY());
                }
            }
        }
    }
}
