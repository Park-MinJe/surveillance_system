using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        /* --------------------------------------
         * 건물 객체 CSV
        -------------------------------------- */
        public class BuildingCSVWriter
        {
            private int N_Building;

            /* --------------------------------------
             * Set Data
            -------------------------------------- */
            public void setBuildingCSVWriter(int N_Building)
            {
                this.N_Building = N_Building;
            }

            /* --------------------------------------
             * Print CSV
            -------------------------------------- */

            // 임의 좌표 사용시
            /*public void initialBuildingsToCSV(int simIdx)
            {
                string fn = "object\\building\\Sim" + simIdx + ".Buildings.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,X,Y,Direction,W,H,D1,D2,W2");
                    for (int j = 0; j < N_Building; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}", j, buildings[j].X, buildings[j].Y, buildings[j].Direction, buildings[j].W, buildings[j].H, buildings[j].D1, buildings[j].D2, buildings[j].W2);
                    }
                }

                fn = "object\\building\\Sim" + simIdx + ".Buildings.Pos.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,Pos_H1_X,Pos_H1_Y,Pos_H2_X,Pos_H2_Y,Pos_V1_X,Pos_V1_Y,Pos_V2_X,Pos_V2_Y");
                    for (int j = 0; j < N_Building; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}", j, buildings[j].Pos_H1[0], buildings[j].Pos_H1[1], buildings[j].Pos_H2[0], buildings[j].Pos_H2[0], buildings[j].Pos_V1[0], buildings[j].Pos_V1[1], buildings[j].Pos_V2[0], buildings[j].Pos_V2[1]);
                    }
                }
            }*/

            public void BuildingsToCSV(string filename)
            {
                // 디지털 매핑 시 파일명: "DigitalMappingResult.Buildings"
                // 시뮬레이션 초기데이터의 파일명: "Sim" + simIdx + ".Buildings"
                string fn = "object\\building\\" + filename + ".csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    //Console.WriteLine("{0}", buildings.Length);
                    file.WriteLine("#idx,X,Y,Z,H");
                    for (int j = 0; j < N_Building; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4}", j, buildings[j].X, buildings[j].Y, buildings[j].Z, buildings[j].H);
                    }
                }
            }
        }
        public class BuildingCSVReader
        {

        }

        /* --------------------------------------
         * 보행자/차량 객체 CSV
        -------------------------------------- */
        public class TargetCSVWriter
        {
            private int N_Ped;
            private int N_Car;

            /* --------------------------------------
             * Set Data
            -------------------------------------- */
            public void setTargetCSVWriter(int N_Ped, int N_Car)
            {
                this.N_Ped = N_Ped;
                this.N_Car = N_Car;
            }

            /* --------------------------------------
             * Print CSV
            -------------------------------------- */
            public void PedsToCSV(string filename)
            {
                // 디지털 매핑 시 파일명: "DigitalMappingResult.Peds"
                // 시뮬레이션 초기데이터의 파일명: "Sim" + simIdx + ".Peds"
                string fn = "object\\target\\" + filename + ".csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,X,Y,DST_X,DST_Y,Direction,Velocity,Unit_Travel_Dist,MAX_Dist_X,MAX_Dist_Y,ground,W,H,D1,D2,W2,N_Surv,TTL");
                    for (int j = 0; j < N_Ped; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, peds[j].X, peds[j].Y, peds[j].DST_X, peds[j].DST_Y, peds[j].Direction, peds[j].Velocity, peds[j].Unit_Travel_Dist, peds[j].MAX_Dist_X, peds[j].MAX_Dist_Y, peds[j].ground, peds[j].W, peds[j].H, peds[j].D1, peds[j].D2, peds[j].W2, peds[j].N_Surv, peds[j].TTL);
                    }
                }

                fn = "object\\target\\" + filename + ".Pos.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,Pos_H1_X,Pos_H1_Y,Pos_H2_X,Pos_H2_Y,Pos_V1_X,Pos_V1_Y,Pos_V2_X,Pos_V2_Y");
                    for (int j = 0; j < N_Ped; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}", j, peds[j].Pos_H1[0], peds[j].Pos_H1[1], peds[j].Pos_H2[0], peds[j].Pos_H2[0], peds[j].Pos_V1[0], peds[j].Pos_V1[1], peds[j].Pos_V2[0], peds[j].Pos_V2[1]);
                    }
                }
            }

            public void CarsToCSV(string filename)
            {
                // 디지털 매핑 시 파일명: "DigitalMappingResult.Cars"
                // 시뮬레이션 초기데이터의 파일명: "Sim" + simIdx + ".Cars"
                string fn = "object\\target\\" + filename + ".csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,X,Y,DST_X,DST_Y,Direction,Velocity,Unit_Travel_Dist,MAX_Dist_X,MAX_Dist_Y,ground,W,H,D1,D2,W2,N_Surv,TTL");
                    for (int j = 0; j < N_Car; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, cars[j].X, cars[j].Y, cars[j].DST_X, cars[j].DST_Y, cars[j].Direction, cars[j].Velocity, cars[j].Unit_Travel_Dist, cars[j].MAX_Dist_X, cars[j].MAX_Dist_Y, cars[j].ground, cars[j].W, cars[j].H, cars[j].D1, cars[j].D2, cars[j].W2, cars[j].N_Surv, cars[j].TTL);
                    }
                }

                fn = "object\\target\\" + filename + ".Pos.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,Pos_H1_X,Pos_H1_Y,Pos_H2_X,Pos_H2_Y,Pos_V1_X,Pos_V1_Y,Pos_V2_X,Pos_V2_Y");
                    for (int j = 0; j < N_Car; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}", j, cars[j].Pos_H1[0], cars[j].Pos_H1[1], cars[j].Pos_H2[0], cars[j].Pos_H2[0], cars[j].Pos_V1[0], cars[j].Pos_V1[1], cars[j].Pos_V2[0], cars[j].Pos_V2[1]);
                    }
                }
            }
        }

        public class TargetCSVReader
        {
            public void PedsFromCSV(string filename)
            {
                // 디지털 매핑 시 파일명: "DigitalMappingResult.Peds"
                // 시뮬레이션 초기데이터의 파일명: "Sim" + simIdx + ".Peds"
                try
                {
                    string fn = "object\\target\\" + filename + ".csv";
                    using (FileStream fs = new FileStream(@fn, FileMode.Open))
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

                    fn = "object\\target\\" + filename + ".Pos.csv";
                    using (FileStream fs = new FileStream(@fn, FileMode.Open))
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

            public void CarsFromCSV(string filename)
            {
                // 디지털 매핑 시 파일명: "DigitalMappingResult.Cars"
                // 시뮬레이션 초기데이터의 파일명: "Sim" + simIdx + ".Cars"
                try
                {
                    string fn = "object\\target\\" + filename + ".csv";
                    using (FileStream fs = new FileStream(@fn, FileMode.Open))
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

                    fn = "object\\target\\" + filename + ".Pos.csv";
                    using (FileStream fs = new FileStream(@fn, FileMode.Open))
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

                                cars[idx].Pos_V1[0] = Convert.ToDouble(values[5]);
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

        /* --------------------------------------
         * CCTV 객체 CSV
        -------------------------------------- */
        public class CctvCSVWriter
        {
            private int N_Cctv;

            public void setCctvCSVWriter(int N_Cctv)
            {
                this.N_Cctv= N_Cctv;

                //debug
                //Console.WriteLine(this.N_Cctv);
            }

            public void CctvsToCSV(string filename)
            {
                // 디지털 매핑 시 파일명: "DigitalMappingResult.CctvSet"
                // 시뮬레이션 초기데이터의 파일명: "CctvSet" + cctvSetIdx
                string fn = "object\\cctv\\" + filename + ".csv";
                
                //debug
                //Console.WriteLine(fn);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,X,Y,Z,WD,HE,H_AOV,V_AOV,imW,imH,Focal_Length,ViewAngleH,ViewAngleV,Eff_Dist_From,Eff_Dist_To,Direction,isFixed,Max_Dist");
                    //debug
                    //Console.WriteLine("#idx,X,Y,Z,WD,HE,H_AOV,V_AOV,imW,imH,Focal_Length,ViewAngleH,ViewAngleV,Eff_Dist_From,Eff_Dist_To,Direction,isFixed,Max_Dist");
                    for (int j = 0; j < N_Cctv; j++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, cctvs[j].X, cctvs[j].Y, cctvs[j].Z, cctvs[j].WD, cctvs[j].HE, cctvs[j].H_AOV, cctvs[j].V_AOV, cctvs[j].imW, cctvs[j].imH, cctvs[j].Focal_Length, cctvs[j].ViewAngleH, cctvs[j].ViewAngleV, cctvs[j].Eff_Dist_From, cctvs[j].Eff_Dist_To, cctvs[j].Direction, cctvs[j].isFixed, cctvs[j].Max_Dist);
                        //debug
                        //Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", j, cctvs[j].X, cctvs[j].Y, cctvs[j].Z, cctvs[j].WD, cctvs[j].HE, cctvs[j].H_AOV, cctvs[j].V_AOV, cctvs[j].imW, cctvs[j].imH, cctvs[j].Focal_Length, cctvs[j].ViewAngleH, cctvs[j].ViewAngleV, cctvs[j].Eff_Dist_From, cctvs[j].Eff_Dist_To, cctvs[j].Direction, cctvs[j].isFixed, cctvs[j].Max_Dist);
                    }
                }
            }
        }

        public class CctvCSVReader
        {
            // csv 파일을 저장하기 위한 리스트
            public List<string[]> realWorldCctvData { get; private set; }
            public int realWorldCctvNum { get; private set; }

            // csv 파일료 저장된 초기 cctv 객체 데이터를 읽어와 cctv 위치 데이터를 초기화해준다.
            public void CctvsFromCSV(string filename)
            {
                // 디지털 매핑 시 파일명: "DigitalMappingResult.CctvSet"
                // 시뮬레이션 초기데이터의 파일명: "CctvSet" + cctvSetIdx
                try
                {
                    string fn = "object\\cctv\\" + filename + ".csv";
                    using (FileStream fs = new FileStream(@fn, FileMode.Open))
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

            public void realWorldCctvFromCSV(string filename = "C:\\Users\\rprpr\\OneDrive - dgu.ac.kr\\Lab\\지능 융합 보안 서비스 개발을 위한 오픈소스 시뮬레이터\\cctv\\12_04_08_E_CCTV정보.csv")
            {
                realWorldCctvData = new List<string[]>();
                realWorldCctvNum = 0;

                try
                {
                    // 파일 위치 예시
                    using (FileStream fs = new FileStream(@filename, FileMode.Open))
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

                                values = lines.Split(",");

                                //debug
                                //Console.WriteLine(values[7] + "," + values[8]);
                                double y = Convert.ToDouble(values[7]);
                                double x = Convert.ToDouble(values[8]);
                                if ((y > road.lowerCorner.getY() && y < road.upperCorner.getY())
                                    && (x > road.lowerCorner.getX() && x < road.upperCorner.getX()))
                                {
                                    realWorldCctvNum += Convert.ToInt32(values[2]);
                                    realWorldCctvData.Add(values);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                //debug
                //Console.WriteLine("Real World CCTV Num = {0}", this.realWorldCctvNum);
                /*for(int i = 0; i < realWorldCctvData.Count(); i++)
                {
                    for(int j = 0; j < realWorldCctvData[i].Length; j++)
                    {
                        Console.Write(realWorldCctvData[i][j] + ",");
                    }
                    Console.WriteLine();
                }*/
            }
        }

        /* --------------------------------------
         * Log CSV
        -------------------------------------- */
        public class TargetLogCSVWriter
        {
            private int N_Ped;
            private int N_Car;
            private int N_Target;

            private int trace_idx;          // csv 파일 출력 index
            private double[,] traffic_x;    // csv 파일 출력 위한 보행자별 x좌표
            private double[,] traffic_y;    // csv 파일 출력 위한 보행자별 y좌표
            private int[,] detection;       // csv 파일 출력 위한 추적여부
            private double[] header;

            /* --------------------------------------
             * Set Data
            -------------------------------------- */
            public void setTargetLogCSVWriter(int N_Ped, int N_Car, int trace_idx)
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

            public void TraceLogToCSV(int cctvSetIdx, int simIdx)
            {
                for (int i = 0; i < N_Target; i++)
                {
                    string fn = "trace\\CctvSet" + cctvSetIdx + ".Sim" + simIdx + ".target" + i + ".csv";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
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

        public class CctvLogCSVWriter
        {
            // Detected Log
            private int detectedLogSize;

            private List<int> detectedCctvIdx;
            private List<string> detectedTargetType;
            private List<int> detectedTargetIdx;
            private List<double> detectedX;
            private List<double> detectedY;
            private List<double> detectedV;
            private List<double> detectedT;

            public void setDetectedLogCSVWriter()
            {
                this.detectedLogSize = 0;

                this.detectedCctvIdx = new List<int>();
                this.detectedTargetType = new List<string>();
                this.detectedTargetIdx = new List<int>();
                this.detectedX = new List<double>();
                this.detectedY = new List<double>();
                this.detectedV = new List<double>();
                this.detectedT = new List<double>();
            }

            public void addDetectedLog(int cctvIdx, string targetType, int targetIdx, double x, double y, double v, double t)
            {
                this.detectedCctvIdx.Add(cctvIdx);
                this.detectedTargetType.Add(targetType);
                this.detectedTargetIdx.Add(targetIdx);
                this.detectedX.Add(x);
                this.detectedY.Add(y);
                this.detectedV.Add(v);
                this.detectedT.Add(t);

                this.detectedLogSize++;
            }

            public int getLogSize() { return this.detectedLogSize; }
            public int getCctvIdx(int logIdx) { return this.detectedCctvIdx[logIdx];}
            public string getTargetType(int logIdx) { return this.detectedTargetType[logIdx];}
            public int getTargetIdx(int logIdx) { return this.detectedTargetIdx[logIdx];}
            public double getX(int logIdx) { return this.detectedX[logIdx];}
            public double getY(int logIdx) { return this.detectedY[logIdx];}
            public double getV(int logIdx) { return this.detectedV[logIdx];}
            public double getT(int logIdx) { return this.detectedT[logIdx];}

            // Shadowed by Building Log
            private int shadowedLogSize;

            private List<int> shadowedCctvIdx;
            private List<int> shadowedBuildingIdx;
            private List<char> shadowedType;
            private List<string> shadowedTargetType;
            private List<int> shadowedTargetIdx;
            private List<double> shadowedX;
            private List<double> shadowedY;
            private List<double> shadowedV;
            private List<double> shadowedT;

            public void setShadowedLogCSVWriter()
            {
                this.shadowedLogSize = 0;

                this.shadowedCctvIdx = new List<int>();
                this.shadowedBuildingIdx = new List<int>();
                this.shadowedType = new List<char>();
                this.shadowedTargetType = new List<string>();
                this.shadowedTargetIdx = new List<int>();
                this.shadowedX = new List<double>();
                this.shadowedY = new List<double>();
                this.shadowedV = new List<double>();
                this.shadowedT = new List<double>();
            }

            public void addShadowedLog(int cctvIdx, int buildingIdx, char shadowType, string targetType, int targetIdx, double x, double y, double v, double t)
            {
                this.shadowedCctvIdx.Add(cctvIdx);
                this.shadowedBuildingIdx.Add(buildingIdx);
                this.shadowedType.Add(shadowType);
                this.shadowedTargetType.Add(targetType);
                this.shadowedTargetIdx.Add(targetIdx);
                this.shadowedX.Add(x);
                this.shadowedY.Add(y);
                this.shadowedV.Add(v);
                this.shadowedT.Add(t);

                this.shadowedLogSize++;
            }

            public void clearCctvLog()
            {
                this.detectedLogSize = 0;

                this.detectedCctvIdx.Clear();
                this.detectedTargetType.Clear();
                this.detectedTargetIdx.Clear();
                this.detectedX.Clear();
                this.detectedY.Clear();
                this.detectedV.Clear();
                this.detectedT.Clear();

                this.shadowedLogSize = 0;

                this.shadowedCctvIdx.Clear();
                this.shadowedBuildingIdx.Clear();
                this.shadowedType.Clear();
                this.shadowedTargetType.Clear();
                this.shadowedTargetIdx.Clear();
                this.shadowedX.Clear();
                this.shadowedY.Clear();
                this.shadowedV.Clear();
                this.shadowedT.Clear();
            }

            public void DetectedLogToCSV(int cctvSetIdx, int simIdx)
            {
                string fn = "trace\\CctvSet" + cctvSetIdx + ".Sim" + simIdx + ".DetectedLog.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,CCTV,TYPE,TARGET,X,Y,V,Time");
                    for (int i = 0; i < this.detectedLogSize; i++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}", i, this.detectedCctvIdx[i], this.detectedTargetType[i], this.detectedTargetIdx[i], this.detectedX[i], this.detectedY[i], this.detectedV[i], this.detectedT[i]);
                    }
                }
            }

            public void ShadowedByBuildingLogToCSV(int cctvSetIdx, int simIdx)
            {
                string fn = "trace\\CctvSet" + cctvSetIdx + ".Sim" + simIdx + ".ShadowedByBuildingLog.csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fn))
                {
                    file.WriteLine("#idx,CCTV,BUILDING,SHADOW_TYPE,TARGET_TYPE,TARGET,X,Y,V,Time");
                    for (int i = 0; i < this.shadowedLogSize; i++)
                    {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", i, this.shadowedCctvIdx[i], this.shadowedBuildingIdx[i], this.shadowedType[i], this.shadowedTargetType[i], this.shadowedTargetIdx[i], this.shadowedX[i], this.shadowedY[i], this.shadowedV[i], this.shadowedT[i]);
                    }
                }
            }
        }
    }
}
