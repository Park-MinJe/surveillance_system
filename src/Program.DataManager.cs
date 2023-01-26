using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        public class TargetCSVWriter
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
            public TargetCSVWriter() { }


            /* --------------------------------------
             * Set Data
            -------------------------------------- */
            public void setTargetCSVWriter(int N_Ped, int N_Car, int trace_idx)
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

            public void addHeader(int timeIdx, double v)
            {
                this.header[timeIdx] = v;
            }

            /* --------------------------------------
             * Print CSV
            -------------------------------------- */

            public void initialPedsToCSV(int simIdx)
            {
                string fileName = "object\\target\\Sim" + simIdx + ".Peds.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                {
                    file.WriteLine("#idx,X,Y,DST_X,DST_Y,Direction,Velocity,Unit_Travel_Dist,MAX_Dist_X,MAX_Dist_Y,ground,W,H,D1,D2,W2,N_Surv,TTL");
                    for (int j = 0; j < N_Ped; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, peds[j].X, peds[j].Y, peds[j].DST_X, peds[j].DST_Y, peds[j].Direction, peds[j].Velocity, peds[j].Unit_Travel_Dist, peds[j].MAX_Dist_X, peds[j].MAX_Dist_Y, peds[j].ground, peds[j].W, peds[j].H, peds[j].D1, peds[j].D2, peds[j].W2, peds[j].N_Surv, peds[j].TTL);
                    }
                }

                fileName = "object\\target\\Sim" + simIdx + ".Peds.Pos.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                {
                    file.WriteLine("#idx,Pos_H1_X,Pos_H1_Y,Pos_H2_X,Pos_H2_Y,Pos_V1_X,Pos_V1_Y,Pos_V2_X,Pos_V2_Y");
                    for (int j = 0; j < N_Ped; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}", j, peds[j].Pos_H1[0], peds[j].Pos_H1[1], peds[j].Pos_H2[0], peds[j].Pos_H2[0], peds[j].Pos_V1[0], peds[j].Pos_V1[1], peds[j].Pos_V2[0], peds[j].Pos_V2[1]);
                    }
                }
            }

            public void initialCarsToCSV(int simIdx)
            {
                string fileName = "object\\target\\Sim" + simIdx + ".Cars.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                {
                    file.WriteLine("#idx,X,Y,DST_X,DST_Y,Direction,Velocity,Unit_Travel_Dist,MAX_Dist_X,MAX_Dist_Y,ground,W,H,D1,D2,W2,N_Surv,TTL");
                    for (int j = 0; j < N_Car; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, cars[j].X, cars[j].Y, cars[j].DST_X, cars[j].DST_Y, cars[j].Direction, cars[j].Velocity, cars[j].Unit_Travel_Dist, cars[j].MAX_Dist_X, cars[j].MAX_Dist_Y, cars[j].ground, cars[j].W, cars[j].H, cars[j].D1, cars[j].D2, cars[j].W2, cars[j].N_Surv, cars[j].TTL);
                    }
                }

                fileName = "object\\target\\Sim" + simIdx + ".Cars.Pos.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                {
                    file.WriteLine("#idx,Pos_H1_X,Pos_H1_Y,Pos_H2_X,Pos_H2_Y,Pos_V1_X,Pos_V1_Y,Pos_V2_X,Pos_V2_Y");
                    for (int j = 0; j < N_Ped; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}", j, cars[j].Pos_H1[0], cars[j].Pos_H1[1], cars[j].Pos_H2[0], cars[j].Pos_H2[0], cars[j].Pos_V1[0], cars[j].Pos_V1[1], cars[j].Pos_V2[0], cars[j].Pos_V2[1]);
                    }
                }
            }

            public void TraceLogToCSV(int cctvSetIdx, int simIdx)
            {
                for (int i = 0; i < N_Target; i++)
                {
                    string fileName = "trace\\CctvSet" + cctvSetIdx + ".Sim" + simIdx + ".target" + i + ".csv";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                    {
                        file.WriteLine("#header,traffic_x,traffic_y,detection");
                        for (int j = 0; j < trace_idx; j++)
                        {
                            file.WriteLine("{0},{1},{2},{3}", header[j], traffic_x[i, j], traffic_y[i, j], detection[i, j]);
                        }
                    }
                }
            }
        }

        public class TargetCSVReader
        {
            public void initialPedsFromCSV(int simIdx)
            {
                try
                {
                    string fileName = "object\\target\\Sim" + simIdx + ".Peds.csv";
                    using (FileStream fs = new FileStream(@fileName, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
                        {
                            string lines = null;
                            //string[] keys = null;
                            string[] values = null;

                            while ((lines = sr.ReadLine()) != null)
                            {
                                if (string.IsNullOrEmpty(lines)) return;

                                if (lines.Substring(0, 1).Equals("#"))  // 첫줄에 #이 있을 경우 Key로 처리
                                {
                                    continue;
                                }

                                values = lines.Split(',');  // 콤마로 분리

                                int idx = Convert.ToInt32(values[0]);
                                peds[idx].X = Convert.ToDouble(values[1]);
                                peds[idx].Y = Convert.ToDouble(values[2]);

                                peds[idx].DST_X = Convert.ToDouble(values[3]);
                                peds[idx].DST_Y = Convert.ToDouble(values[4]);

                                peds[idx].Direction = Convert.ToDouble(values[5]);
                                peds[idx].Velocity = Convert.ToDouble(values[6]);
                                peds[idx].Unit_Travel_Dist = Convert.ToDouble(values[7]);

                                peds[idx].MAX_Dist_X = Convert.ToDouble(values[8]);
                                peds[idx].MAX_Dist_Y = Convert.ToDouble(values[9]);

                                peds[idx].ground = Convert.ToDouble(values[10]);

                                peds[idx].W = Convert.ToDouble(values[11]);
                                peds[idx].H = Convert.ToDouble(values[12]);
                                peds[idx].D1 = Convert.ToDouble(values[13]);
                                peds[idx].D2 = Convert.ToDouble(values[14]);
                                peds[idx].W2 = Convert.ToDouble(values[15]);
                                
                                peds[idx].N_Surv = Convert.ToInt32(values[16]);

                                peds[idx].TTL = Convert.ToInt32(values[17]);
                            }
                        }
                    }

                    fileName = "object\\target\\Sim" + simIdx + ".Peds.Pos.csv";
                    using (FileStream fs = new FileStream(@fileName, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
                        {
                            string lines = null;
                            //string[] keys = null;
                            string[] values = null;

                            while ((lines = sr.ReadLine()) != null)
                            {
                                if (string.IsNullOrEmpty(lines)) return;

                                if (lines.Substring(0, 1).Equals("#"))  // 첫줄에 #이 있을 경우 Key로 처리
                                {
                                    continue;
                                }

                                values = lines.Split(',');  // 콤마로 분리

                                int idx = Convert.ToInt32(values[0]);
                                peds[idx].Pos_H1[0] = Convert.ToDouble(values[1]);
                                peds[idx].Pos_H1[1] = Convert.ToDouble(values[2]);

                                peds[idx].Pos_H2[0] = Convert.ToDouble(values[3]);
                                peds[idx].Pos_H2[1] = Convert.ToDouble(values[4]);

                                peds[idx].Pos_V1[0] = Convert.ToInt32(values[5]);
                                peds[idx].Pos_V1[1] = Convert.ToDouble(values[6]);

                                peds[idx].Pos_V2[0] = Convert.ToDouble(values[7]);
                                peds[idx].Pos_V2[1] = Convert.ToDouble(values[8]);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            public void initialCarsFromCSV(int simIdx)
            {
                try
                {
                    string fileName = "object\\target\\Sim" + simIdx + ".Cars.csv";
                    using (FileStream fs = new FileStream(@fileName, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
                        {
                            string lines = null;
                            //string[] keys = null;
                            string[] values = null;

                            while ((lines = sr.ReadLine()) != null)
                            {
                                if (string.IsNullOrEmpty(lines)) return;

                                if (lines.Substring(0, 1).Equals("#"))  // 첫줄에 #이 있을 경우 Key로 처리
                                {
                                    continue;
                                }

                                values = lines.Split(',');  // 콤마로 분리

                                int idx = Convert.ToInt32(values[0]);
                                cars[idx].X = Convert.ToDouble(values[1]);
                                cars[idx].Y = Convert.ToDouble(values[2]);

                                cars[idx].DST_X = Convert.ToDouble(values[3]);
                                cars[idx].DST_Y = Convert.ToDouble(values[4]);

                                cars[idx].Direction = Convert.ToDouble(values[5]);
                                cars[idx].Velocity = Convert.ToDouble(values[6]);
                                cars[idx].Unit_Travel_Dist = Convert.ToDouble(values[7]);

                                cars[idx].MAX_Dist_X = Convert.ToDouble(values[8]);
                                cars[idx].MAX_Dist_Y = Convert.ToDouble(values[9]);

                                cars[idx].ground = Convert.ToDouble(values[10]);

                                cars[idx].W = Convert.ToDouble(values[11]);
                                cars[idx].H = Convert.ToDouble(values[12]);
                                cars[idx].D1 = Convert.ToDouble(values[13]);
                                cars[idx].D2 = Convert.ToDouble(values[14]);
                                cars[idx].W2 = Convert.ToDouble(values[15]);

                                cars[idx].N_Surv = Convert.ToInt32(values[16]);

                                cars[idx].TTL = Convert.ToInt32(values[17]);
                            }
                        }
                    }

                    fileName = "object\\target\\Sim" + simIdx + ".Cars.Pos.csv";
                    using (FileStream fs = new FileStream(@fileName, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
                        {
                            string lines = null;
                            //string[] keys = null;
                            string[] values = null;

                            while ((lines = sr.ReadLine()) != null)
                            {
                                if (string.IsNullOrEmpty(lines)) return;

                                if (lines.Substring(0, 1).Equals("#"))  // 첫줄에 #이 있을 경우 Key로 처리
                                {
                                    continue;
                                }

                                values = lines.Split(',');  // 콤마로 분리

                                int idx = Convert.ToInt32(values[0]);
                                cars[idx].Pos_H1[0] = Convert.ToDouble(values[1]);
                                cars[idx].Pos_H1[1] = Convert.ToDouble(values[2]);

                                cars[idx].Pos_H2[0] = Convert.ToDouble(values[3]);
                                cars[idx].Pos_H2[1] = Convert.ToDouble(values[4]);

                                cars[idx].Pos_V1[0] = Convert.ToInt32(values[5]);
                                cars[idx].Pos_V1[1] = Convert.ToDouble(values[6]);

                                cars[idx].Pos_V2[0] = Convert.ToDouble(values[7]);
                                cars[idx].Pos_V2[1] = Convert.ToDouble(values[8]);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public class CctvCSVWriter
        {
            private int N_Cctv;

            public void setCctvCSVWriter(int N_Cctv)
            {
                this.N_Cctv= N_Cctv;
            }

            public void initialCctvsToCSV(int cctvSetIdx)
            {
                string fileName = "object\\cctv\\CctvSet" + cctvSetIdx + ".csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fileName))
                {
                    file.WriteLine("#idx,X,Y,Z,WD,HE,H_AOV,V_AOV,imW,imH,Focal_Length,ViewAngleH,ViewAngleV,Eff_Dist_From,Eff_Dist_To,Direction,isFixed,Max_Dist");
                    for (int j = 0; j < N_Cctv; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, cctvs[j].X, cctvs[j].Y, cctvs[j].Z, cctvs[j].WD, cctvs[j].HE, cctvs[j].H_AOV, cctvs[j].V_AOV, cctvs[j].imW, cctvs[j].imH, cctvs[j].Focal_Length, cctvs[j].ViewAngleH, cctvs[j].ViewAngleV, cctvs[j].Eff_Dist_From, cctvs[j].Eff_Dist_To, cctvs[j].Direction, cctvs[j].isFixed, cctvs[j].Max_Dist);
                    }
                }
            }
        }

        public class CctvCSVReader
        {
            public void initialCctvsFromCSV(int cctvSetIdx)
            {
                try
                {
                    string fileName = "object\\cctv\\CctvSet" + cctvSetIdx + ".csv";
                    using (FileStream fs = new FileStream(@fileName, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
                        {
                            string lines = null;
                            //string[] keys = null;
                            string[] values = null;

                            while ((lines = sr.ReadLine()) != null)
                            {
                                if (string.IsNullOrEmpty(lines)) return;

                                if (lines.Substring(0, 1).Equals("#"))  // 첫줄에 #이 있을 경우 Key로 처리
                                {
                                    continue;
                                }

                                values = lines.Split(',');  // 콤마로 분리

                                int idx = Convert.ToInt32(values[0]);
                                cctvs[idx].X = Convert.ToInt32(values[1]);
                                cctvs[idx].Y = Convert.ToInt32(values[2]);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
