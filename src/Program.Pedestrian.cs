using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public class SurveillanceTarget {
            public double X;
            public double Y;

            public double DST_X;
            public double DST_Y;

            public double Direction;
            public double Velocity;
            public double Unit_Travel_Dist;

            public double MAX_Dist_X;
            public double MAX_Dist_Y;

            public double ground; // Z ?
            /* ==================================
            /   추가
            /   line 67~ 87 변수 
            /   line 91~127 define_PED 함수 (깃허브 define_PED.m)
            /           라인 500~507에서 ped 위치 변수 사용하는데 이걸  define_PED에서 처리하는거 같아서 구현해놨습니다
            / ===================================*/
            public double W;
            public double H;
            public double D1;
            public double D2;
            public double W2;

            public double[] Pos_H1 = new double[2];
            public double[] Pos_H2 = new double[2];
            public double[] Pos_V1 = new double[2];
            public double[] Pos_V2 = new double[2];

            public int N_Surv;  //number of surveillance camera viewing this target.

            public int TTL;     // Time To Live

            public SurveillanceTarget() { }

            // 230504 pmj
            // initalizer used to clone
            public SurveillanceTarget(SurveillanceTarget t)
            {
                this.X = t.X;
                this.Y = t.Y;
                
                this.DST_X= t.DST_X;
                this.DST_Y= t.DST_Y;

                this.Direction = t.Direction;
                this.Velocity = t.Velocity;
                this.Unit_Travel_Dist= t.Unit_Travel_Dist;
                
                this.MAX_Dist_X= t.MAX_Dist_X;
                this.MAX_Dist_Y = t.MAX_Dist_Y;

                this.ground= t.ground;

                this.W = t.W;
                this.H = t.H;
                this.D1= t.D1;
                this.D2 = t.D2;
                this.W2 = t.W2;

                for(int i = 0; i < 2; i++)
                {
                    this.Pos_H1[i] = t.Pos_H1[i];
                    this.Pos_H2[i] = t.Pos_H2[i];
                    this.Pos_V1[i] = t.Pos_V1[i];
                    this.Pos_V2[i] = t.Pos_V2[i];
                }

                this.N_Surv = t.N_Surv;
                this.TTL = t.TTL;
            }

            public void define_TARGET(
                double Width,
                double Height,
                double DST_X,
                double DST_Y,
                double Velocity
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
                // [220331] may be height of ground, instead of 0
                this.Pos_V2[1] = 0; 

                this.DST_X = DST_X;
                this.DST_Y = DST_Y;
                this.Velocity = Velocity;

                this.Unit_Travel_Dist = Velocity * aUnitTime;

                // % for performace measure
                this.N_Surv = 0;
            }

            public void setDirection()
            {
                double[] A = new double[2];
                A[0] = DST_X - X;
                A[1] = DST_Y - Y;

                double[] B = { 0.001, 0 };
                Direction = Math.Round(Math.Acos(InnerProduct(A, B) / (Norm(A) * Norm(B))), 8);
                if (Y > DST_Y)
                {
                    Direction = Math.Round(2 * Math.PI - Direction, 8);
                }
            }

            public Boolean isArrived()
            {
                double[] dist = { X - DST_X, Y - DST_Y };
                if (Norm(dist) < 1000) return true;
                else return false;
            }

            public void printTargetInfo()
            {
                Console.WriteLine("======================Info======================");
                Console.WriteLine("출발지 : ({0},{1}) \n", this.X, this.Y);
                Console.WriteLine("목적지 : ({0},{1}) \n", this.DST_X, this.DST_Y);
                Console.WriteLine("방향 각도(라디안) : {0} \n", this.Direction);
                Console.WriteLine("속도 : {0} \n", this.Velocity);
                Console.WriteLine("단위이동거리 : {0} \n", this.Unit_Travel_Dist);
                Console.WriteLine("Pos_H1 : ({0},{1})   Pos_H2 : ({2},{3})  \n",
                    this.Pos_H1[0], this.Pos_H1[1], this.Pos_H2[0], this.Pos_H2[1]);
                Console.WriteLine("Pos_V1 : ({0},{1})   Pos_V2 : ({2},{3}) \n",
                    this.Pos_V1[0], this.Pos_V1[1], this.Pos_V2[0], this.Pos_V2[1]);
                Console.WriteLine("TTL : {0} \n", this.TTL);
            }

            public Boolean outOfRange(double X_mapSize, double Y_mapSize)
            {
                if (X < 0 || X > X_mapSize || Y < 0 || Y > Y_mapSize)
                {
                    return true;
                }
                return false;
            }
        }

        interface Movable
        {
            void move(Map map); // 초당 이동
            void updateDestination(Map map);
            void upVelocity(); // 속도 증가
            void downVelocity(); // 속도 감소
        }

        interface Person: Movable {}

        interface Vehicle: Movable {}

        public class Pedestrian : SurveillanceTarget, Person
        {
            public Pedestrian() { }
            public Pedestrian(Pedestrian ped)
            {
                this.X = ped.X;
                this.Y = ped.Y;

                this.DST_X = ped.DST_X;
                this.DST_Y = ped.DST_Y;

                this.Direction = ped.Direction;
                this.Velocity = ped.Velocity;
                this.Unit_Travel_Dist = ped.Unit_Travel_Dist;

                this.MAX_Dist_X = ped.MAX_Dist_X;
                this.MAX_Dist_Y = ped.MAX_Dist_Y;

                this.ground = ped.ground;

                this.W = ped.W;
                this.H = ped.H;
                this.D1 = ped.D1;
                this.D2 = ped.D2;
                this.W2 = ped.W2;

                for (int i = 0; i < 2; i++)
                {
                    this.Pos_H1[i] = ped.Pos_H1[i];
                    this.Pos_H2[i] = ped.Pos_H2[i];
                    this.Pos_V1[i] = ped.Pos_V1[i];
                    this.Pos_V2[i] = ped.Pos_V2[i];
                }

                this.N_Surv = ped.N_Surv;
                this.TTL = ped.TTL;
            }

            public void move(Map map)
            {
                // 이동
                X += Unit_Travel_Dist * Math.Cos(Direction);
                Y += Unit_Travel_Dist * Math.Sin(Direction);
                // Console.WriteLine("move to {0} {1} ", X, Y);

                Pos_H1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H1[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H2[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H2[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_V1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_V2[0] += Unit_Travel_Dist * Math.Cos(Direction);

                // 목적지 도착 검사
                if (isArrived() || outOfRange(map.X_mapSize, map.Y_mapSize))
                {
                    // Index out of range
                    updateDestination(map); 
                    setDirection();
                }
            }
            public void updateDestination(Map map)
            {
                double[,] newPos = map.getPointOfAdjacentRoad(map.getIdxOfIntersection(X, Y));
                DST_X = Math.Round(newPos[0, 0]);
                DST_Y = Math.Round(newPos[0, 1]);
            }
            public void upVelocity()
            {
                if (this.Velocity > 0.01f && this.Velocity < 4000)
                    this.Velocity += 0.01f;
                else
                    Console.WriteLine("Out of Velocity range");
            }
            public void downVelocity()
            {
                if (this.Velocity > 0.01f && this.Velocity < 4000)
                    this.Velocity -= 0.01f;
                else
                    Console.WriteLine("Out of Velocity range");
            }

        }

        public class Car: SurveillanceTarget, Vehicle
        {
            public Car() { }
            public Car(Car car)
            {
                this.X = car.X;
                this.Y = car.Y;

                this.DST_X = car.DST_X;
                this.DST_Y = car.DST_Y;

                this.Direction = car.Direction;
                this.Velocity = car.Velocity;
                this.Unit_Travel_Dist = car.Unit_Travel_Dist;

                this.MAX_Dist_X = car.MAX_Dist_X;
                this.MAX_Dist_Y = car.MAX_Dist_Y;

                this.ground = car.ground;

                this.W = car.W;
                this.H = car.H;
                this.D1 = car.D1;
                this.D2 = car.D2;
                this.W2 = car.W2;

                for (int i = 0; i < 2; i++)
                {
                    this.Pos_H1[i] = car.Pos_H1[i];
                    this.Pos_H2[i] = car.Pos_H2[i];
                    this.Pos_V1[i] = car.Pos_V1[i];
                    this.Pos_V2[i] = car.Pos_V2[i];
                }

                this.N_Surv = car.N_Surv;
                this.TTL = car.TTL;
            }

            public void move(Map map)
            {
                // 이동
                X += Unit_Travel_Dist * Math.Cos(Direction);
                Y += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H1[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H2[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H2[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_V1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_V2[0] += Unit_Travel_Dist * Math.Cos(Direction);

                // 목적지 도착 검사
                if (isArrived() || outOfRange(map.X_mapSize, map.Y_mapSize))
                {
                    updateDestination(map);
                    setDirection();
                }
            }

            public void updateDestination(Map map)
            {
                // Console.WriteLine("update destination1");
                //double[,] newPos = map.getPointOfAdjacentIntersection(map.getIdxOfIntersection(X, Y), X, Y);
                double[,] newPos = map.getPointOfAdjacentIntersection(map.getIdxOfIntersection(X, Y), X, Y);
                // Console.WriteLine("update destination2");
                DST_X = Math.Round(newPos[0, 0]);
                DST_Y = Math.Round(newPos[0, 1]);
            }
            
            public void downVelocity()
            {
                if (this.Velocity > 0.1f && this.Velocity < 33333)
                    this.Velocity -= 0.1f;
                else
                    Console.WriteLine("Out of Velocity range");
            }
            public void upVelocity()
            {
                if (this.Velocity > 0.1f && this.Velocity < 33333)
                    this.Velocity += 0.1f;
                else
                    Console.WriteLine("Out of Velocity range");
            }
        }

        public class ElectricScooter : SurveillanceTarget, Vehicle
        {
            public void move(Map map)
            {
                // 이동
                X += Unit_Travel_Dist * Math.Cos(Direction);
                Y += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H1[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_H2[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_H2[1] += Unit_Travel_Dist * Math.Sin(Direction);

                Pos_V1[0] += Unit_Travel_Dist * Math.Cos(Direction);
                Pos_V2[0] += Unit_Travel_Dist * Math.Cos(Direction);

                // 목적지 도착 검사
                if (isArrived())
                {
                    updateDestination(map);
                    setDirection();
                }
            }
            public void updateDestination(Map map)
            {

            }
            public void downVelocity()
            {

            }
            public void upVelocity()
            {

            }
        }
    }
}
