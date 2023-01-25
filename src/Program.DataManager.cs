using System;
using System.Collections.Generic;
using System.Text;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        public class TargetDataManager
        {
            private int N_Ped;
            private int N_Car;
            private int N_Target;

            private int trace_idx;          // csv 파일 출력 index
            private double[,] traffic_x;     // csv 파일 출력 위한 보행자별 x좌표
            private double[,] traffic_y;     // csv 파일 출력 위한 보행자별 y좌표
            private int[,] detection;     // csv 파일 출력 위한 추적여부
            private double[] header;

            /* --------------------------------------
             * 생성자
            -------------------------------------- */
            public TargetDataManager() { }

            public TargetDataManager(int N_Ped, int N_Car, int trace_idx)
            {
                this.N_Ped = N_Ped;
                this.N_Car = N_Car;
                this.N_Target = N_Ped + N_Car;
                
                this.trace_idx = trace_idx;
                this.traffic_x = new double[N_Target, trace_idx];
                this.traffic_y = new double[N_Target, trace_idx]; // csv 파일 출력 위한 보행자별 y좌표
                this.detection = new int[N_Target, trace_idx]; // csv 파일 출력 위한 추적여부
                this.header = new double[trace_idx];
            }

            /* --------------------------------------
             * Set Data
            -------------------------------------- */
            public void addTraffic_x(int targetIdx, int timeIdx, double v)
            {
                this.traffic_x[targetIdx, timeIdx] = v;
            }
            
            public void addTraffic_y(int targetIdx, int timeIdx, double v)
            {
                this.traffic_y[targetIdx, timeIdx] = v;
            }

            public void addDetection(int targetIdx, int timeIdx, int v)
            {
                this.detection[targetIdx, timeIdx] = v;
            }

            public void addHeader(int timeIdx, int v)
            {
                this.header[timeIdx] = v;
            }

            /* --------------------------------------
             * Print CSV
            -------------------------------------- */

            public void initialPedsToCSV(int simIdx, int N_Ped)
            {
                for (int i = 0; i < peds.Length + cars.Length; i++)
                {
                    string fileName = "object\\Sim" + simIdx + ".Peds.csv";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                    {
                        file.WriteLine("idx,X,Y,DST_X,DST_Y,Direction,Velocity,Unit_Travel_Dist,MAX_Dist_X,MAX_Dist_Y,ground,W,H,D1,D2,W2,N_Surv,TTL");
                        for (int j = 0; j < N_Ped; j++)
                        {
                            file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, peds[j].X, peds[j].Y, peds[j].DST_X, peds[j].DST_Y, peds[j].Direction, peds[j].Velocity, peds[j].Unit_Travel_Dist, peds[j].MAX_Dist_X, peds[j].MAX_Dist_Y, peds[j].ground, peds[j].W, peds[j].H, peds[j].D1, peds[j].D2, peds[j].W2, peds[j].N_Surv, peds[j].TTL);
                        }
                    }
                }
            }

            public void initialCarsToCSV(int simIdx, int N_Car)
            {
                for (int i = 0; i < peds.Length + cars.Length; i++)
                {
                    string fileName = "object\\Sim" + simIdx + ".Cars.csv";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                    {
                        file.WriteLine("idx,X,Y,DST_X,DST_Y,Direction,Velocity,Unit_Travel_Dist,MAX_Dist_X,MAX_Dist_Y,ground,W,H,D1,D2,W2,N_Surv,TTL");
                        for (int j = 0; j < N_Car; j++)
                        {
                            file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, cars[j].X, cars[j].Y, cars[j].DST_X, cars[j].DST_Y, cars[j].Direction, cars[j].Velocity, cars[j].Unit_Travel_Dist, cars[j].MAX_Dist_X, cars[j].MAX_Dist_Y, cars[j].ground, cars[j].W, cars[j].H, cars[j].D1, cars[j].D2, cars[j].W2, cars[j].N_Surv, cars[j].TTL);
                        }
                    }
                }
            }

            public void TraceLogToCSV(int cctvSetIdx, int simIdx, int trace_idx, double[] header, double[,] traffic_x, double[,] traffic_y, int[,] detection)
            {
                for (int i = 0; i < peds.Length + cars.Length; i++)
                {
                    string fileName = "target\\CctvSet" + cctvSetIdx + ".Sim" + simIdx + ".target" + i + ".csv";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                    {
                        file.WriteLine("header,traffic_x,traffic_y,detection");
                        for (int j = 0; j < trace_idx; j++)
                        {
                            file.WriteLine("{0},{1},{2},{3}", header[j], traffic_x[i, j], traffic_y[i, j], detection[i, j]);
                        }
                    }
                }
            }

            /* --------------------------------------
             * Read CSV
            -------------------------------------- */
        }

        public class CctvDataManager
        {

        }
    }
}
