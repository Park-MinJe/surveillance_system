using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        public interface initSurvTrg
        {
            Point initXy();
            double initW();
            double initH();
            double initD1();
            double initD2();
            double initW2();
            Point initPos_H1();
            Point initPos_H2();
            Point initPos_V1();
            Point initPos_V2();
            Point initDST();
            double initDirection();
            double initVelocity();
            double initUnit_Travel_Dist();
            int initN_Surv();
            int initTtl();
        }

        public class initSurvTrgByRandom : initSurvTrg
        {
            public TargetType targetType { get; private set; }
            public double w { get; private set; }
            public double h { get; private set; }
            public double v { get; private set; }
            public double minDist { get; private set; }
            public Road r { get; private set; }
            public Point xy { get; private set; }
            public double D1 { get; private set; }
            public double D2 { get; private set; }
            public double W2 { get; private set; }

            /* ----------------------------------------------------------------------------------------------------------------------
             * Random 배치
            ---------------------------------------------------------------------------------------------------------------------- */
            public initSurvTrgByRandom(TargetType targetType, double w, double h, double v, double minDist, Road road)
            {
                this.targetType = targetType;
                this.w = w;
                this.h = h;
                this.v = v;
                this.minDist = minDist;
                this.r = road;
            }

            public Point initXy()
            {
                xy = new Point();

                Random rand = new Random();
                int intersectidx = rand.Next(this.r.lane_num * this.r.lane_num);

                if (this.targetType == TargetType.PED)
                {
                    double[,] newPos = r.getPointOfAdjacentRoad(intersectidx);
                    xy.setPoint(Math.Round(newPos[0, 0]), Math.Round(newPos[0, 1]), 0d);
                }
                else if(this.targetType == TargetType.CAR)
                {
                    xy.setPoint(r.DST[intersectidx].x, r.DST[intersectidx].y, 0d);

                    int carintersectidx = rand.Next(4); // 0, 1, 2, 3
                    if (carintersectidx == 0)
                    {// down left
                        xy.subX(r.width / 4);
                        xy.addY(r.width / 4);
                    }
                    else if (carintersectidx == 1)
                    {// up left
                        xy.addX(r.width / 4);
                        xy.addY(r.width / 4);
                    }
                    else if (carintersectidx == 2)
                    {// up right
                        xy.addX(r.width / 4);
                        xy.subY(r.width / 4);
                    }
                    else if (carintersectidx == 3)
                    {// down right
                        xy.subX(r.width / 4);
                        xy.subY(r.width / 4);
                    }
                }

                return xy;
            }
            public double initW()
            {
                return this.w;
            }
            public double initH()
            {
                return this.h;
            }
            public double initD1()
            {
                this.D1 = 90 * Math.PI / 180;
                return this.D1;   // modified by 0BoO, deg -> rad
            }
            public double initD2()
            {
                Random rand = new Random();
                this.D2 = (180 + 90 * rand.NextDouble()) * Math.PI / 180;
                return this.D2; // modified by 0BoO, deg -> rad
            }
            public double initW2()
            {
                return this.w / 2;
            }
            public Point initPos_H1()
            {
                double h1_x = Math.Round(this.W2 * Math.Cos(D1) + this.xy.x, 2);
                double h1_y = Math.Round(this.W2 * Math.Sin(D1) + this.xy.y, 2);

                return new Point(h1_x, h1_y, 0d);
            }
            public Point initPos_H2()
            {
                double h2_x = Math.Round(this.W2 * Math.Cos(D2) + this.xy.x, 2);
                double h2_y = Math.Round(this.W2 * Math.Sin(D2) + this.xy.y, 2);

                return new Point(h2_x, h2_y, 0d);
            }
            public Point initPos_V1()
            {
                return new Point(this.xy.x, 0d, this.h);
            }
            public Point initPos_V2()
            {
                return new Point(this.xy.x, 0d, 0d);
            }
            public Point initDST()
            {
                Point dst = new Point();

                if(this.targetType == TargetType.PED)
                {
                    double[,] newPos = r.getPointOfAdjacentRoad(r.getIdxOfIntersection(this.xy.x, this.xy.y));
                    double dst_x = Math.Round(newPos[0, 0]);
                    double dst_y = Math.Round(newPos[0, 1]);

                    dst.setPoint(dst_x, dst_y, 0d);
                }
                else if(this.targetType == TargetType.CAR)
                {
                    double[,] newPos = r.getPointOfAdjacentIntersection(r.getIdxOfIntersection(this.xy.x, this.xy.y), this.xy.x, this.xy.y);
                    double dst_x = Math.Round(newPos[0, 0]);
                    double dst_y = Math.Round(newPos[0, 1]);

                    dst.setPoint(dst_x, dst_y, 0d);
                }

                return dst;
            }
            public double initVelocity()
            {
                return this.v;
            }
            public double initDirection()
            {
                return 0d;
            }
            public double initUnit_Travel_Dist()
            {
                return this.v * aUnitTime;
            }
            public int initN_Surv()
            {
                return 0;
            }
            public int initTtl()
            {
                return (int)Math.Ceiling((this.minDist / this.v) / aUnitTime);
            }
        }

        /* ----------------------------------------------------------------------------------------------------------------------
        * Random 배치
        ---------------------------------------------------------------------------------------------------------------------- */
        public class initSurvTrgByCsv : initSurvTrg
        {
            StreamReader sr_Trg, sr_Pos;
            int simIdx, trgIdx;
            string[] values, values_Pos;

            public initSurvTrgByCsv(TargetType targetType, int simIdx, int trgIdx)
            {
                this.simIdx = simIdx;
                this.trgIdx = trgIdx;

                TargetCSVReader tcr = new TargetCSVReader();
                string fileName;
                if (targetType == TargetType.PED)
                {
                    fileName = "object\\target\\Sim" + simIdx + ".Peds.csv";
                    sr_Trg = tcr.getStreamReader(fileName);

                    fileName = "object\\target\\Sim" + simIdx + ".Peds.Pos.csv";
                    sr_Pos = tcr.getStreamReader(fileName);
                }
                else if (targetType == TargetType.CAR)
                {
                    fileName = "object\\target\\Sim" + simIdx + ".Cars.csv";
                    sr_Trg = tcr.getStreamReader(fileName);

                    fileName = "object\\target\\Sim" + simIdx + ".Cars.Pos.csv";
                    sr_Pos = tcr.getStreamReader(fileName);
                }

                string lines = null;
                this.values = null;
                while((lines = sr_Trg.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(lines)) return;

                    if (lines.Substring(0, 1).Equals("#"))  // 첫줄에 #이 있을 경우 Key로 처리
                    {
                        continue;
                    }

                    this.values = lines.Split(',');  // 콤마로 분리v

                    int idx = Convert.ToInt32(this.values[0]);
                    if(idx == trgIdx)
                    {
                        break;
                    }
                }

                lines = null;
                this.values_Pos = null;
                while ((lines = sr_Pos.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(lines)) return;

                    if (lines.Substring(0, 1).Equals("#"))  // 첫줄에 #이 있을 경우 Key로 처리
                    {
                        continue;
                    }

                    this.values_Pos = lines.Split(',');  // 콤마로 분리v

                    int idx = Convert.ToInt32(this.values_Pos[0]);
                    if (idx == trgIdx)
                    {
                        break;
                    }
                }
            }

            public Point initXy()
            {
                double x = Convert.ToDouble(values[1]);
                double y = Convert.ToDouble(values[2]);
                return new Point(x, y, 0d);
            }
            public double initW()
            {
                return Convert.ToDouble(values[11]);
            }
            public double initH()
            {
                return Convert.ToDouble(values[12]);
            }
            public double initD1()
            {
                return Convert.ToDouble(values[13]);
            }
            public double initD2()
            {
                return Convert.ToDouble(values[14]);
            }
            public double initW2()
            {
                return Convert.ToDouble(values[15]);
            }
            public Point initPos_H1()
            {
                double x = Convert.ToDouble(values_Pos[1]);
                double y = Convert.ToDouble(values_Pos[2]);

                return new Point(x, y, 0d);
            }
            public Point initPos_H2()
            {
                double x = Convert.ToDouble(values_Pos[3]);
                double y = Convert.ToDouble(values_Pos[4]);

                return new Point(x, y, 0d);
            }
            public Point initPos_V1()
            {
                double x = Convert.ToInt32(values_Pos[5]);
                double z = Convert.ToDouble(values_Pos[6]);

                return new Point(x, 0d, z);
            }
            public Point initPos_V2()
            {
                double x = Convert.ToDouble(values_Pos[7]);
                double z = Convert.ToDouble(values_Pos[8]);

                return new Point(x, 0d, z);
            }
            public Point initDST()
            {
                double x = Convert.ToDouble(values[3]);
                double y = Convert.ToDouble(values[4]);

                return new Point(x, y, 0d);
            }
            public double initDirection()
            {
                return Convert.ToDouble(values[5]);
            }
            public double initVelocity()
            {
                return Convert.ToDouble(values[6]);
            }
            public double initUnit_Travel_Dist()
            {
                return Convert.ToInt32(values[16]);
            }
            public int initN_Surv()
            {
                return Convert.ToInt32(values[16]);
            }
            public int initTtl()
            {
                return Convert.ToInt32(values[17]);
            }
        }
    }
}
