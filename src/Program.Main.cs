// using Internal;
using static surveillance_system.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace surveillance_system
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            int nCctv = 0, nPed = 0, nCar = 0, 
                cctvMode = 0, 
                numberOfCCTVSet = 1, 
                simulationTimesForCCTVSet = 100;
            string inputNCctvOption = "N",
                inputNPedOption = "N",
                inputNCarOption = "N";

            List<double> successRates = new List<double>();
            List<CCTV[]> cctvAtSim = new List<CCTV[]>();
            List<Pedestrian[]> pedestrianAtSim = new List<Pedestrian[]>();
            List<Car[]> carAtSim = new List<Car[]>();

            List<Road> roadAtSim = new List<Road>();

            Console.Write("input Number of CCTV set: ");
            numberOfCCTVSet = Convert.ToInt32(Console.ReadLine());

            Console.Write("input Simulation times for Each CCTV set: ");
            simulationTimesForCCTVSet = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\ninput CCTV collocating mode: ");
            while (true)
            {
                Console.Write("(0: pos cctv as grid    1: pos cctv as random)? ");
                cctvMode = Convert.ToInt32(Console.ReadLine());

                if (cctvMode == 0 || cctvMode == 1 || cctvMode == 2) { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("\nDo you want to enter CCTV Numbers(Y/N)? ");
                inputNCctvOption = Console.ReadLine();

                if (inputNCctvOption == "N" || inputNCctvOption == "n") { break; }
                else if (inputNCctvOption == "Y" || inputNCctvOption == "y") { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("Do you want to enter Pedestrian Numbers(Y/N)? ");
                inputNPedOption = Console.ReadLine();

                if (inputNPedOption == "N" || inputNPedOption == "n") { break; }
                else if (inputNPedOption == "Y" || inputNPedOption == "y") { break; }
                else { continue; }
            }

            while (true)
            {
                Console.Write("Do you want to enter Car Numbers(Y/N)? ");
                inputNCarOption = Console.ReadLine();

                if (inputNCarOption == "N" || inputNCarOption == "n") { break; }
                else if (inputNCarOption == "Y" || inputNCarOption == "y") { break; }
                else { continue; }
            }

            Console.WriteLine("");

            if (inputNCctvOption == "Y" || inputNCctvOption == "y")
            {
                Console.Write("input number of CCTV: ");
                nCctv = Convert.ToInt32(Console.ReadLine());
            }

            if (inputNPedOption == "Y" || inputNPedOption == "y")
            {
                Console.Write("input number of Pedestrian: ");
                nPed = Convert.ToInt32(Console.ReadLine());
            }

            if (inputNCarOption == "Y" || inputNCarOption == "y")
            {
                Console.Write("input number of Car: ");
                nCar = Convert.ToInt32(Console.ReadLine());
            }


            Simulator[] sims = new Simulator[simulationTimesForCCTVSet];
            // s.simulateAll(cctvMode);
            for(int i = 0; i < simulationTimesForCCTVSet; i++)
            {
                sims[i] = new Simulator();
                sims[i].setIdx(i);

                sims[i].setgetCCTVNumFromUser(inputNCctvOption);
                sims[i].setgetPedNumFromUser(inputNPedOption);
                sims[i].setgetCarNumFromUser(inputNCarOption);

                sims[i].initNCctv(nCctv);
                sims[i].initNPed(nPed);
                sims[i].initNCar(nCar);

                sims[i].initVariables();

                sims[i].initTimer();

                // sims[i].startTimer();
                sims[i].initMap(cctvMode);
                roadAtSim.Add(road);
                pedestrianAtSim.Add(peds);
                sims[i].writeInitialPedsToXML();
                sims[i].writePedsToXML();
                carAtSim.Add(cars);
                sims[i].writeInitialCarsToXML();
                sims[i].writeCarsToXML();
                // sims[i].stopTimer();
            }

            for (int i = 0; i < numberOfCCTVSet; i++)
            {
                double successRateForCCTVSet = 0.0;
                for (int j = 0; j < simulationTimesForCCTVSet; j++)
                {
                    peds = pedestrianAtSim[j];
                    cars = carAtSim[j];
                    if (j == 0)
                    {
                        if (i == 0 && numberOfCCTVSet != 1)
                        {
                            road.setCCTV(sims[j].getNCCTV(), road.width, road.lane_num);
                        }
                        else
                        {
                            switch (cctvMode)
                            {
                                case 0:
                                    road.setCCTV(sims[j].getNCCTV(), road.width, road.lane_num);
                                    break;
                                case 1:
                                    road.setCCTVForBrute(sims[j].getNCCTV());
                                    break;
                            }
                        }

                        cctvAtSim.Add(cctvs);
                        sims[j].writeInitialCctvsToXML();
                        sims[j].writeCctvsToXML();
                    }
                    else
                    {
                        cctvs = cctvAtSim[i];
                    }

                    sims[j].startTimer();
                    Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Simulatioin Start " + i + " - " + j);
                    sims[j].operateSim();
                    sims[j].stopTimer();
                    sims[j].printResultAsCSV();
                    double successRate = sims[j].printResultRate();
                    successRateForCCTVSet += successRate;
                    // s.printDetectedResults();

                    sims[j].resetTimer();
                }
                successRates.Add(successRateForCCTVSet / simulationTimesForCCTVSet);
            }

            Console.WriteLine("\n\n====== Simulation Results ======");
            Console.WriteLine("print index of CCTV set and Target detected Rate\n");
            for (int i = 0; i< successRates.Count ; i++)
            {
                Console.WriteLine("CCTV set {0}\t{1:F2}%", i, successRates[i]);
                Console.WriteLine();
            }

            Console.WriteLine("\n\n====== Best CCTV set ======");
            int bestCCTVIdx = successRates.IndexOf(successRates.Max());

            for (int i = 0; i < 52; i++)
            {
                Console.Write("{0, 2}", i);
            }
            Console.WriteLine();
            for (int i = 0; i < 52; i++)
            {
                Console.Write("{0, 2}", i);

                for (int j = 0; j < 52; j++)
                {
                    if (road.cctvPos[i, j] <= 0)
                        // Console.Write("{0, 2}", " ", cctvPos[i,j]);
                        Console.Write("{0, 2}", " ");
                    else
                        // Console.Write("{0, 2}", "*", cctvPos[i,j]);
                        Console.Write("{0, 2}", roadAtSim[bestCCTVIdx].cctvPos[i, j]);

                }
                Console.WriteLine();
            }
        }
    }
}
