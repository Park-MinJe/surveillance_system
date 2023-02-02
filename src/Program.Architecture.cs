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

            // Polygon
            public double[] pX;
            public double[] pY;

            public double Direction = 0;

            public double W;
            public double H;
            public double D1;
            public double D2;
            public double W2;

            public double[] Pos_H1 = new double[2];
            public double[] Pos_H2 = new double[2];
            public double[] Pos_V1 = new double[2];
            public double[] Pos_V2 = new double[2];

            // Polygon
            public List<double[]> Pos_H1s = new List<double[]>();
            public List<double[]> Pos_H2s = new List<double[]>();
            public List<double[]> Pos_V1s = new List<double[]>();
            public List<double[]> Pos_V2s = new List<double[]>();

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
                Random rand = new Random();

                this.pX = new double[xy.Length / 2];
                this.pY = new double[xy.Length / 2];
                for(int i = 0; i< xy.Length / 2; i++)
                {
                    this.pX[i] = xy[i * 2];
                    this.pY[i] = xy[i * 2 + 1];
                }

                this.H = h;

                /*---------------------- 다각형과 H가 이루는 다면체의 각 면마다 연산 필요 ----------------------*/
                this.D1 = 90 * Math.PI / 180;
                this.D2 = (180 + 90 * rand.NextDouble()) * Math.PI / 180; // modified by 0BoO, deg -> rad

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

            public void printArchInfo()
            {
                Console.WriteLine("======================Info======================");
                Console.WriteLine("다각형 좌표 :");
                for(int i = 0; i<this.pX.Length; i++)
                {
                    Console.WriteLine("\t({0},{1})", this.pX[i], this.pY[i]);
                }

                Console.WriteLine("\n방향 각도(라디안) : {0} \n", this.Direction);
                Console.WriteLine("Pos_H1 : ({0},{1})   Pos_H2 : ({2},{3})  \n",
                    this.Pos_H1[0], this.Pos_H1[1], this.Pos_H2[0], this.Pos_H2[1]);
                Console.WriteLine("Pos_V1 : ({0},{1})   Pos_V2 : ({2},{3}) \n",
                    this.Pos_V1[0], this.Pos_V1[1], this.Pos_V2[0], this.Pos_V2[1]);
            }
        }
    }
}
