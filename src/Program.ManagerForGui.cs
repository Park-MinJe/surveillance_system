using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static surveillance_system.Program;

namespace surveillance_system
{
    public partial class Program
    {
        public class GuiManager
        {
            // Gis api 세팅
            public void setEndPointUrlByGui(string methodName, string serviceKey, string typeName, string bbox, string pnu, string maxFeature, string resultType, string srsName)
            {
                Console.WriteLine(methodName);
                Console.WriteLine(serviceKey);
                Console.WriteLine(typeName);
                Console.WriteLine(bbox);
                Console.WriteLine(pnu);
                Console.WriteLine(maxFeature);
                Console.WriteLine(resultType);
                Console.WriteLine(srsName);

                buildingfromApi.setRequest(methodName);
                buildingfromApi.setServiceKey(serviceKey);
                buildingfromApi.setTypename(typeName);
                buildingfromApi.setBbox(bbox);
                buildingfromApi.setSrsname(srsName);

                if(buildingfromApi is GisBuildingService)
                {
                    (buildingfromApi as GisBuildingService).setPnu(pnu);
                    (buildingfromApi as GisBuildingService).setMaxFeature(maxFeature);
                    (buildingfromApi as GisBuildingService).setResultType(resultType);
                }

                buildingfromApi.setEndPointUrl();
            }

            public void loadBuildingDataFromApiAsXmlByGui()
            {
                buildingfromApi.loadBuildingDataFromApiAsXml();
            }

            // 시뮬레이션 기본 세팅
            private int numberOfCCTVSetByInt,
                    simulationTimesForCCTVSetByInt,
                    cctvArrangementModeByInt,
                    N_CctvByInt,
                    N_PedByInt,
                    N_CarByInt;

            public void setSimulationSettingByGui(string numberOfCCTVSet, string simulationTimesForCCTVSet, string cctvArrangementMode, string N_Cctv, string N_Ped, string N_Car)
            {
                /*int numberOfCCTVSetByInt, 
                    simulationTimesForCCTVSetByInt, 
                    cctvArrangementModeByInt, 
                    N_CctvByInt, 
                    N_PedByInt, 
                    N_CarByInt;*/

                numberOfCCTVSetByInt = Convert.ToInt32(numberOfCCTVSet);
                
                simulationTimesForCCTVSetByInt = Convert.ToInt32(simulationTimesForCCTVSet);

                if (simulationTimesForCCTVSet == "CCTV as Grid")
                    simulationTimesForCCTVSetByInt = 0;
                else if (simulationTimesForCCTVSet == "CCTV at DST")
                    simulationTimesForCCTVSetByInt = 1;
                else if (simulationTimesForCCTVSet == "CCTV as Random")
                    simulationTimesForCCTVSetByInt = 2;

                N_CctvByInt = Convert.ToInt32(N_Cctv);

                N_PedByInt = Convert.ToInt32(N_Ped);

                N_CarByInt = Convert.ToInt32(N_Car);

                // debug
                Console.WriteLine("CCTV 배치 세트 개수:\t{0}", numberOfCCTVSetByInt);
                Console.WriteLine("CCTV 당 시뮬레이션 횟수:\t{0}", simulationTimesForCCTVSetByInt);
                if (simulationTimesForCCTVSetByInt == 0)
                    Console.WriteLine("CCTV 배치 유형:\tCCTV as Grid");
                else if (simulationTimesForCCTVSetByInt == 1)
                    Console.WriteLine("CCTV 배치 유형:\tCCTV at DST");
                else if (simulationTimesForCCTVSetByInt == 2)
                    Console.WriteLine("CCTV 배치 유형:\tCCTV as Random");
                Console.WriteLine("CCTV 댓수:\t{0}", N_CctvByInt);
                Console.WriteLine("보행자 수:\t{0}", N_PedByInt);
                Console.WriteLine("차량 수:\t{0}", N_CarByInt);
            }

            // 시뮬레이션 시작
            public void startSimulationByGui()
            {
                List<double> successRates = new List<double>();

                SimulationModel[] sims = new SimulationModel[simulationTimesForCCTVSetByInt];
                // s.simulateAll(cctvMode);
                for (int i = 0; i < simulationTimesForCCTVSetByInt; i++)
                {
                    sims[i] = new SimulationModel();

                    sims[i].initNCctv(N_CctvByInt);
                    sims[i].initNPed(N_PedByInt);
                    sims[i].initNCar(N_CarByInt);

                    sims[i].initVariables();

                    sims[i].initTimer();

                    // sims[i].startTimer();
                    sims[i].initMap(cctvArrangementModeByInt, buildingfromApi.getMapUpperCorner(), buildingfromApi.getMapLowerCorner());
                    sims[i].initialBuildingsToCSV(i);
                    sims[i].initialPedsToCSV(i);
                    sims[i].initialCarsToCSV(i);
                    //pedestrianAtSim.Add(peds);
                    //carAtSim.Add(cars);
                    // sims[i].stopTimer();
                }

                for (int i = 0; i < numberOfCCTVSetByInt; i++)
                {
                    double successRateForCCTVSet = 0.0;
                    for (int j = 0; j < simulationTimesForCCTVSetByInt; j++)
                    {
                        sims[j].initPedsWithCSV(j);
                        sims[j].initCarsWithCSV(j);
                        sims[j].startTimer();
                        if (j == 0)
                        {
                            if (i == 0 && numberOfCCTVSetByInt > 1)
                            {
                                road.setCCTV(sims[j].getNCCTV(), road.width, road.lane_num);
                            }
                            else
                            {
                                switch (cctvArrangementModeByInt)
                                {
                                    case 0:
                                        road.setCCTV(sims[j].getNCCTV(), road.width, road.lane_num);
                                        break;
                                    case 1:
                                        road.setCCTVbyRandomInDST(sims[j].getNCCTV());
                                        break;
                                    case 2:
                                        road.setCCTVbyRandomInInt(sims[j].getNCCTV());
                                        break;
                                }
                            }
                            //cctvAtSim.Add(cctvs);
                            cw.initialCctvsToCSV(i);
                            //cctvPosAtSim.Add(road.cctvPos);
                        }
                        road.printAllPos();

                        Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Simulatioin Start " + i + " - " + j);
                        sims[j].operateSim();
                        sims[j].stopTimer();
                        sims[j].TraceLogToCSV(i, j);
                        double successRate = sims[j].printResultRate();
                        successRateForCCTVSet += successRate;
                        //sims[j].printDetectedResults();
                        sims[j].DetectedResultsToCSV(i, j);
                        sims[j].ShadowedLogToCSV(i, j);

                        sims[j].resetTimer();
                    }
                    successRates.Add(successRateForCCTVSet / simulationTimesForCCTVSetByInt);
                }

                Console.WriteLine("\n\n====== Simulation Results ======");
                Console.WriteLine("print index of CCTV set and Target detected Rate\n");
                for (int i = 0; i < successRates.Count; i++)
                {
                    Console.WriteLine("CCTV set {0}\t{1:F2}%", i, successRates[i]);
                    Console.WriteLine();
                }

                Console.WriteLine("\n\n====== Best CCTV set ======");
                int bestCCTVIdx = successRates.IndexOf(successRates.Max());

                Console.WriteLine("====== CCTV set {0} ======", bestCCTVIdx);
                road.setCctvswithCSV(bestCCTVIdx);
                road.printPos(road.cctvPos);
            }
        }
    }
}
