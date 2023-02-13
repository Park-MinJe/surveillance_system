using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public class SurveillanceTarget {
            //public double X;
            //public double Y;
            public Point xy { get; protected set; }

            //public double DST_X;
            //public double DST_Y;
            public Point DST_xy { get; protected set; }

            public double Direction { get; protected set; }
            public double Velocity { get; protected set; }
            public double Unit_Travel_Dist { get; protected set; }

            //public double MAX_Dist_X;
            //public double MAX_Dist_Y;
            public Point MAX_DIST_xy { get; protected set; }

            public double ground { get; protected set; } // Z ?
            /* ==================================
            /   추가
            /   line 67~ 87 변수 
            /   line 91~127 define_PED 함수 (깃허브 define_PED.m)
            /           라인 500~507에서 ped 위치 변수 사용하는데 이걸  define_PED에서 처리하는거 같아서 구현해놨습니다
            / ===================================*/
            public double W { get; protected set; }
            public double H { get; protected set; }
            public double D1 { get; protected set; }
            public double D2 { get; protected set; }
            public double W2 { get; protected set; }

            public Point Pos_H1 { get; protected set; }
            public Point Pos_H2 { get; protected set; }
            public Point Pos_V1 { get; protected set; }
            public Point Pos_V2 { get; protected set; }

            public int N_Surv { get; protected set; }  //number of surveillance camera viewing this target.

            public int TTL { get; protected set; }     // Time To Live

            public SurveillanceTarget()
            {
                xy = new Point();
                DST_xy = new Point();
                MAX_DIST_xy = new Point();
                Pos_H1 = new Point();
                Pos_H2 = new Point();
                Pos_V1 = new Point();
                Pos_V2 = new Point();
            }

            public void define_TARGET(
                /*double Width,
                double Height,
                double DST_X,
                double DST_Y,
                double Velocity, */
                initSurvTrg initSurvTrgBy
            )
            {
                // interface initSurvTrg 사용
                if(initSurvTrgBy is initSurvTrgByRandom)
                {
                    this.xy.setPoint(initSurvTrgBy.initXy());
                    this.W = initSurvTrgBy.initW();
                    this.H = initSurvTrgBy.initH();
                    this.D1 = initSurvTrgBy.initD1();
                    this.D2 = initSurvTrgBy.initD2();
                    this.W2 = initSurvTrgBy.initW2();

                    this.Pos_H1.setPoint(initSurvTrgBy.initPos_H1());
                    this.Pos_H2.setPoint(initSurvTrgBy.initPos_H2());
                    this.Pos_V1.setPoint(initSurvTrgBy.initPos_V1());
                    this.Pos_V2.setPoint(initSurvTrgBy.initPos_V2());

                    this.DST_xy.setPoint(initSurvTrgBy.initDST());

                    this.Direction = initSurvTrgBy.initDirection();

                    this.Velocity = initSurvTrgBy.initVelocity();

                    this.Unit_Travel_Dist = initSurvTrgBy.initUnit_Travel_Dist();

                    this.N_Surv = initSurvTrgBy.initN_Surv();
                }


                // 기존 구현
                /*Random rand = new Random();

                this.W = Width;
                this.H = Height;
                this.D1 = 90 * Math.PI / 180;   // modified by 0BoO, deg -> rad
                this.D2 = (180 + 90 * rand.NextDouble()) * Math.PI / 180; // modified by 0BoO, deg -> rad
                this.W2 = this.W / 2;

                double h1_x = Math.Round(this.W2 * Math.Cos(D1 + this.Direction) + this.xy.x, 2);
                double h1_y = Math.Round(this.W2 * Math.Sin(D1 + this.Direction) + this.xy.y, 2);
                double h2_x = Math.Round(this.W2 * Math.Cos(D2 + this.Direction) + this.xy.x, 2);
                double h2_y = Math.Round(this.W2 * Math.Sin(D2 + this.Direction) + this.xy.y, 2);

                this.Pos_H1 = new Point(h1_x, h1_y, 0d);
                this.Pos_H2 = new Point(h2_x, h2_y, 0d);

                //this.Pos_V1[0] = this.X;
                //this.Pos_V1[1] = this.H;
                this.Pos_V1 = new Point(this.xy.x, 0d, this.H);

                //this.Pos_V2[0] = this.X;
                //// [220331] may be height of ground, instead of 0
                //this.Pos_V2[1] = 0; 
                this.Pos_V2 = new Point(this.xy.x, 0d, 0d);

                //this.DST_X = DST_X;
                //this.DST_Y = DST_Y;
                this.DST_xy = new Point(DST_X, DST_Y, 0d);

                this.Velocity = Velocity;

                this.Unit_Travel_Dist = Velocity * aUnitTime;

                // % for performace measure
                this.N_Surv = 0;*/
            }

            public void setDirection()
            {
                double[] A = new double[2];
                A[0] = this.DST_xy.x - this.xy.x;
                A[1] = this.DST_xy.y - this.xy.y;

                double[] B = { 0.001, 0 };
                Direction = Math.Round(Math.Acos(InnerProduct(A, B) / (Norm(A) * Norm(B))), 8);
                if (this.xy.y > this.DST_xy.y)
                {
                    Direction = Math.Round(2 * Math.PI - Direction, 8);
                }
            }

            public Boolean isArrived()
            {
                double[] dist = { this.xy.x - this.DST_xy.x, this.xy.y - this.DST_xy.y };
                if (Norm(dist) < 1000) return true;
                else return false;
            }

            public void printTargetInfo()
            {
                Console.WriteLine("======================Info======================");
                Console.WriteLine("출발지 : ({0},{1}) \n", this.xy.x, this.xy.y);
                Console.WriteLine("목적지 : ({0},{1}) \n", this.DST_xy.x, this.DST_xy.y);
                Console.WriteLine("방향 각도(라디안) : {0} \n", this.Direction);
                Console.WriteLine("속도 : {0} \n", this.Velocity);
                Console.WriteLine("단위이동거리 : {0} \n", this.Unit_Travel_Dist);
                Console.WriteLine("Pos_H1 : ({0},{1})   Pos_H2 : ({2},{3})  \n",
                    this.Pos_H1.x, this.Pos_H1.y, this.Pos_H2.x, this.Pos_H2.y);
                Console.WriteLine("Pos_V1 : ({0},{1})   Pos_V2 : ({2},{3}) \n",
                    this.Pos_V1.x, this.Pos_V1.y, this.Pos_V2.x, this.Pos_V2.y);
                Console.WriteLine("TTL : {0} \n", this.TTL);
            }

            public Boolean outOfRange()
            {
                if (this.xy.x < 0 || this.xy.x > road.X_mapSize || this.xy.y < 0 || this.xy.y > road.Y_mapSize)
                {
                    return true;
                }
                return false;
            }
        }

        interface Movable
        {
            void move(); // 초당 이동
            void updateDestination();
            void upVelocity(); // 속도 증가
            void downVelocity(); // 속도 감소
        }

        interface Person: Movable {}

        interface Vehicle: Movable {}

        public class Pedestrian : SurveillanceTarget, Person
        {
            public void move()
            {
                // 이동
                this.xy.addX(Unit_Travel_Dist * Math.Cos(Direction));
                this.xy.addY(Unit_Travel_Dist * Math.Sin(Direction));
                // Console.WriteLine("move to {0} {1} ", X, Y);

                Pos_H1.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_H1.addY(Unit_Travel_Dist * Math.Sin(Direction));

                Pos_H2.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_H2.addY(Unit_Travel_Dist * Math.Sin(Direction));

                Pos_V1.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_V2.addX(Unit_Travel_Dist * Math.Cos(Direction));

                // 목적지 도착 검사
                if (isArrived() || outOfRange())
                {
                    // Index out of range
                    updateDestination(); 
                    setDirection();
                }
            }
            public void downVelocity()
            {
                if (this.Velocity > 0.01f && this.Velocity < 4000)
                    this.Velocity -= 0.01f;
                else
                    Console.WriteLine("Out of Velocity range");
            }
            public void upVelocity()
            {
                if (this.Velocity > 0.01f && this.Velocity < 4000)
                    this.Velocity += 0.01f;
                else
                    Console.WriteLine("Out of Velocity range");
            }
            public  void  updateDestination()
            {      
                double[,] newPos = road.getPointOfAdjacentRoad(road.getIdxOfIntersection(xy.x, xy.y));
                DST_xy.setX(Math.Round(newPos[0, 0]));
                DST_xy.setY(Math.Round(newPos[0, 1]));
            }

        }

        public class Car: SurveillanceTarget, Vehicle
        {
            public void move()
            {
                // 이동
                xy.addX(Unit_Travel_Dist * Math.Cos(Direction));
                xy.addY(Unit_Travel_Dist * Math.Sin(Direction));

                Pos_H1.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_H1.addY(Unit_Travel_Dist * Math.Sin(Direction));

                Pos_H2.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_H2.addY(Unit_Travel_Dist * Math.Sin(Direction));

                Pos_V1.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_V2.addX(Unit_Travel_Dist * Math.Cos(Direction));

                // 목적지 도착 검사
                if (isArrived() || outOfRange())
                {
                    updateDestination();
                    setDirection();
                }
            }

            public void updateDestination()
            {
                // Console.WriteLine("update destination1");
                double[,] newPos = road.getPointOfAdjacentIntersection(road.getIdxOfIntersection(xy.x, xy.y), xy.x, xy.y);
                // Console.WriteLine("update destination2");
                DST_xy.setX(Math.Round(newPos[0, 0]));
                DST_xy.setY(Math.Round(newPos[0, 1]));
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
            public void move()
            {
                // 이동
                xy.addX(Unit_Travel_Dist * Math.Cos(Direction));
                xy.addY(Unit_Travel_Dist * Math.Sin(Direction));

                Pos_H1.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_H1.addY(Unit_Travel_Dist * Math.Sin(Direction));

                Pos_H2.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_H2.addY(Unit_Travel_Dist * Math.Sin(Direction));

                Pos_V1.addX(Unit_Travel_Dist * Math.Cos(Direction));
                Pos_V2.addX(Unit_Travel_Dist * Math.Cos(Direction));

                // 목적지 도착 검사
                if (isArrived())
                {
                    updateDestination();
                    setDirection();
                }
            }
            public void updateDestination()
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
