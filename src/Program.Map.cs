using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static surveillance_system.Program;
using OpenTK.Windowing.Desktop;
using static surveillance_system.Program.OsmReader;

namespace surveillance_system
{
   public partial class Program
    {
        public enum RoadType
        {
            /** Small road or residential street */
            Street,

            /** Major road or minor state highway */
            MajorRoad,

            /** Highway */
            Highway,

            /** Other (path, bus route, etc) */
            Other,
        };
        static public string[] RoadTypeName =
        {
            "Street",
            "MajorRoad",
            "Highway",
            "Other"
        };
        // 도로 폭
        static public double[] RoadWidth =
        {
            // mm
            // Street
            8000.0,
            // MajorRoad
            10000.0,
            // Highway
            14000.0
        };
        
        // 도로를 이루는 node
        public class Node : Point
        {
            // x: lon, y: lat
            public Node(double x, double y, double z)
            {
                this.setPoint(x, y, z);
            }

            public Node(Point p)
            {
                this.setPoint(p);
            }

            public override string ToString()
            {
                return String.Format("x: {0}\ty: {1}\tz: {2}", this.x, this.y, this.z);
            }
        }

        public class PartOfRoad
        {
            public Segment NodeToNode { set; get; }
            public Segment UpperSeg { set; get; }
            public Segment LowerSeg { set; get; }
        }

        // node로 이루어진 도로
        public class Road
        {
            public string RoadName { set; get; }
            public string RoadRef { set; get; }
            /** 230801 박민제
             * 도로 폭
             * Street: 
             */
            public double Width { set; get; }
            // 도로를 구성하는 점
            public List<int> NodeIndices { set; get; }
            // 점과 점사이 부분
            public List<PartOfRoad> Parts { set; get; }
            public RoadType RoadType { set; get; }
            public EOSMWayType EOSMWayType { set; get; }
            public bool bIsOneWay { set; get; } = false;

            public Road()
            {
                NodeIndices = new List<int>();
                Parts = new List<PartOfRoad>();
            }

            public override string ToString()
            {
                string rt = "Road Name - " + RoadName;
                rt += "\nRoad Ref - " + RoadRef;
                rt += "\nRoad Type - " + RoadTypeName[Convert.ToInt32(RoadType)];
                rt += "\nWidth - " + Width;
                rt += "\nNode Indices\n=====>\n";
                foreach(int idx in NodeIndices)
                {
                    rt += idx + "/";
                }
                return rt;
            }
        }

        // 교차로 정보
        public class RoadRef
        {
            public int DSTNodeIndex { set; get; }
            public List<int> roadIndices { set; get; }
            public List<Point> intersectionArea { set; get; }

            public double minWidth { set; get; }
            public double maxWidth { set; get; }
            
            public RoadRef()
            {
                roadIndices = new List<int>();
                intersectionArea = new List<Point>();
            }

            public override string ToString()
            {
                string rt = "\nDST Node Index - " + DSTNodeIndex;
                rt += "\nRelated Roads' Indices\n";
                foreach(int idx in roadIndices) { rt += idx + "/"; }
                return rt;
            }
        }

        public class Map
        {
            // Map의 실제 범위
            public Point lowerCorner;
            public Point upperCorner;

            // 지도 위의 점
            public List<Node> nodes;
            // node로 이루어진 도로
            public List<Road> roads;
            // 교차로
            public List<RoadRef> roadsRefs;

            // 230206 쓰이지 않고 있음 _Minje
            //public double[] laneVector;

            // 가로 도로 좌표
            // 첫번째 인덱스 = 도로 번호,  두번째 인덱스 = y 값
            //public double[,] lane_h; // 가로 - 중앙선 y 값 
            //public double[,] lane_h_upper; // 가로 - 중앙선 위 라인 y값 
            //public double[,] lane_h_lower; // 가로 - 중앙선 아래 라인 y값 

            // 세로 도로 좌표
            // 첫번째 인덱스 = 도로 번호,  두번째 인덱스 = x 값
            //public double[,] lane_v; // 세로 - 중앙선 x값
            //public double[,] lane_v_left; // 세로- 중앙선 왼쪽 라인 x값
            //public double[,] lane_v_right; // 세로 중앙선 오른쪽 라인 x값

            //public double[,] DST; // 도로 교차점
            //public double[,] intersectionArea; // 도로 교차구간
            public double X_mapSize;
            public double Y_mapSize;
            public int X_grid_num;
            public int Y_grid_num;
            //public int lane_num;
            //public double X_interval;
            //public double Y_interval;
            //public int width;

            // 건물 위치
            public int[,] buildingPos;
            // CCTV 위치
            public int[,] cctvPos;
            // Ped 위치
            public int[,] pedPos;
            // Car 위치
            public int[,] carPos;

            public Map() {
                nodes = new List<Node>();
                roads = new List<Road>();
                roadsRefs = new List<RoadRef>();
            }

            // 230504 pmj
            // initalizer used to clone
            public Map(Map r)
            {
                // Debug
                //r.printRoadInfo();

                if (r.lowerCorner != null)
                {
                    this.lowerCorner = new Point(r.lowerCorner);
                }
                if (r.upperCorner != null)
                {
                    this.upperCorner = new Point(r.upperCorner);
                }

                if(nodes == null) this.nodes = new List<Node> ();
                foreach (Node nd in r.nodes) this.nodes.Add(nd);

                if(roads == null) this.roads = new List<Road>();
                foreach (Road rd in r.roads) this.roads.Add(rd);

                if (roadsRefs == null) this.roadsRefs = new List<RoadRef>();
                foreach (RoadRef rRefs in r.roadsRefs) this.roadsRefs.Add(rRefs);

                // 230206 쓰이지 않고 있음 _Minje
                /*
                this.laneVector = new double[r.laneVector.Length];
                for(int i = 0; i < r.laneVector.Length; i++)
                {
                    this.laneVector[i] = r.laneVector[i];
                }
                */

                //if (r.lane_h != null)
                //{
                //    this.lane_h = new double[r.lane_h.GetUpperBound(0) + 1, r.lane_h.GetUpperBound(1) + 1];
                //    for (int i = 0; i < r.lane_h.GetUpperBound(0); i++)
                //    {
                //        for (int j = 0; j < r.lane_h.GetUpperBound(1); j++)
                //        {
                //            this.lane_h[i, j] = r.lane_h[i, j];
                //        }
                //    }
                //}

                //if (r.lane_v_left != null)
                //{
                //    this.lane_v_left = new double[r.lane_v_left.GetUpperBound(0) + 1, r.lane_v_left.GetUpperBound(1) + 1];
                //    for (int i = 0; i < r.lane_v_left.GetUpperBound(0); i++)
                //    {
                //        for (int j = 0; j < r.lane_v_left.GetUpperBound(1); j++)
                //        {
                //            this.lane_v_left[i, j] = r.lane_v_left[i, j];
                //        }
                //    }
                //}

                //if (r.lane_v_right != null)
                //{
                //    this.lane_v_right = new double[r.lane_v_right.GetUpperBound(0) + 1, r.lane_v_right.GetUpperBound(1) + 1];
                //    for (int i = 0; i < r.lane_v_right.GetUpperBound(0); i++)
                //    {
                //        for (int j = 0; j < r.lane_v_right.GetUpperBound(1); j++)
                //        {
                //            this.lane_v_right[i, j] = r.lane_v_right[i, j];
                //        }
                //    }
                //}

                //if (r.DST != null)
                //{
                //    this.DST = new double[r.DST.GetUpperBound(0) + 1, r.DST.GetUpperBound(1) + 1];
                //    for (int i = 0; i < r.DST.GetUpperBound(0); i++)
                //    {
                //        for (int j = 0; j < r.DST.GetUpperBound(1); j++)
                //        {
                //            this.DST[i, j] = r.DST[i, j];
                //        }
                //    }
                //}

                //if (r.intersectionArea != null)
                //{
                //    this.intersectionArea = new double[r.intersectionArea.GetUpperBound(0) + 1, r.intersectionArea.GetUpperBound(1) + 1];
                //    for (int i = 0; i < r.intersectionArea.GetUpperBound(0); i++)
                //    {
                //        for (int j = 0; j < r.intersectionArea.GetUpperBound(1); j++)
                //        {
                //            this.intersectionArea[i, j] = r.intersectionArea[i, j];
                //        }
                //    }
                //}

                this.X_mapSize = r.X_mapSize;
                this.Y_mapSize = r.Y_mapSize;
                this.X_grid_num = r.X_grid_num;
                this.Y_grid_num = r.Y_grid_num;
                //this.lane_num = r.lane_num;
                //this.X_interval = r.X_interval;
                //this.Y_interval = r.Y_interval;
                //this.width = r.width;

                if (r.buildingPos != null)
                {
                    this.buildingPos = new int[r.buildingPos.GetUpperBound(0) + 1, r.buildingPos.GetUpperBound(1) + 1];
                    for (int i = 0; i < r.buildingPos.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < r.buildingPos.GetUpperBound(1); j++)
                        {
                            this.buildingPos[i, j] = r.buildingPos[i, j];
                        }
                    }
                }
                else
                {
                    this.buildingPos = new int[this.Y_grid_num, this.X_grid_num];
                }

                if (r.cctvPos != null)
                {
                    this.cctvPos = new int[r.cctvPos.GetUpperBound(0) + 1, r.cctvPos.GetUpperBound(1) + 1];
                    for (int i = 0; i < r.buildingPos.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < r.cctvPos.GetUpperBound(1); j++)
                        {
                            this.cctvPos[i, j] = r.cctvPos[i, j];
                        }
                    }
                }
                else
                {
                    this.cctvPos = new int[this.Y_grid_num, this.X_grid_num];
                }

                if (r.pedPos != null)
                {
                    this.pedPos = new int[r.pedPos.GetUpperBound(0) + 1, r.pedPos.GetUpperBound(1) + 1];
                    for (int i = 0; i < r.pedPos.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < r.pedPos.GetUpperBound(1); j++)
                        {
                            this.pedPos[i, j] = r.pedPos[i, j];
                        }
                    }
                }
                else
                {
                    this.pedPos = new int[this.Y_grid_num, this.X_grid_num];
                }

                if (r.carPos != null)
                {
                    this.carPos = new int[r.carPos.GetUpperBound(0) + 1, r.carPos.GetUpperBound(1) + 1];
                    for (int i = 0; i < r.carPos.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < r.carPos.GetUpperBound(1); j++)
                        {
                            this.carPos[i, j] = r.carPos[i, j];
                        }
                    }
                }
                else
                {
                    this.carPos = new int[this.Y_grid_num, this.X_grid_num];
                }
            }

            public void roadBuilder(int wd, int intvl, int n_interval, Point upperCorner, Point lowerCorner)
            {
                //this.lane_num = n_interval + 1;
                //DST = new double[lane_num * lane_num, 2];
                //intersectionArea = new double[lane_num * lane_num, 4];
                ////this.interval = intvl;
                //this.width = wd;

                ////this.mapSize = n_interval * intvl + wd * lane_num;
                ////this.grid_num = this.mapSize / 10000 + 2;
                //// 실제 데이터로부터의 mapSize
                ////lowerCorner = buildingfromApi.getMapLowerCorner();
                ////upperCorner = buildingfromApi.getMapUpperCorner();

                //this.lowerCorner = TransformCoordinate(lowerCorner, 3857, 4326);
                //this.upperCorner = TransformCoordinate(upperCorner, 3857, 4326);

                //this.X_mapSize = getDistanceBetweenPointsOfepsg4326(this.lowerCorner.x, this.lowerCorner.y, this.upperCorner.x, this.lowerCorner.y);
                ////Console.WriteLine("x map size: {0}", this.X_mapSize);
                //this.Y_mapSize = getDistanceBetweenPointsOfepsg4326(this.lowerCorner.x, this.lowerCorner.y, this.lowerCorner.x, this.upperCorner.y);
                ////Console.WriteLine("y map size: {0}", this.Y_mapSize);

                //this.X_grid_num = (int)Math.Truncate(X_mapSize) / 10000 + 2;
                ////Console.WriteLine("x grid num: {0}", this.X_grid_num);
                //this.Y_grid_num = (int)Math.Truncate(Y_mapSize) / 10000 + 2;
                ////Console.WriteLine("y grid num: {0}", this.Y_grid_num);

                //this.X_interval = (this.X_mapSize - lane_num * this.width) / n_interval;
                //this.Y_interval = (this.Y_mapSize - lane_num * this.width) / n_interval;

                //// 교차점, 교차구간 설정
                //int idx = 0;
                //for (int i = 0; i < lane_num; i++)
                //{
                //    for (int j = 0; j < lane_num; j++)
                //    {
                //        DST[idx, 0] = (this.X_interval + wd) * i + (wd / 2);
                //        DST[idx, 1] = (this.Y_interval + wd) * j + (wd / 2);

                //        intersectionArea[idx, 0] = DST[idx, 0] - (wd / 2); // x_min
                //        intersectionArea[idx, 1] = DST[idx, 0] + (wd / 2); // x_max
                //        intersectionArea[idx, 2] = DST[idx, 1] - (wd / 2); // y_min
                //        intersectionArea[idx, 3] = DST[idx, 1] + (wd / 2); // y_max
                //        idx++;
                //    }
                //}

                //// 230206 쓰이지 않고 있음 _Minje
                //// 도로 벡터 초기화
                ///*double incr = 100;
                //int laneVectorSize = (int)((intvl + wd) * (n_interval) / incr);
                ////Console.WriteLine("laneSize = {0}", laneSize);
                //laneVector = new double[laneVectorSize];

                //for (int i = 0; i < laneVectorSize; i++)
                //{
                //    laneVector[i] = i * incr;
                //}*/

                //// 가로 도로 좌표 설정
                //lane_h = new double[lane_num, 1];
                //lane_h_upper = new double[lane_num, 1];
                //lane_h_lower = new double[lane_num, 1];

                //for (int i = 0; i < lane_num; i++)
                //{
                //    lane_h[i, 0] = i * (intvl + wd) + (wd / 2);
                //    lane_h_upper[i, 0] = lane_h[i, 0] + wd / 2;
                //    lane_h_lower[i, 0] = lane_h[i, 0] - wd / 2;
                //}

                //// 세로 도로 좌표 설정
                //lane_v = new double[lane_num, 1];
                //lane_v_left = new double[lane_num, 1];
                //lane_v_right = new double[lane_num, 1];
                //for (int i = 0; i < lane_num; i++)
                //{
                //    lane_v[i, 0] = i * (intvl + wd) + (wd / 2);
                //    lane_v_left[i, 0] = lane_h[i, 0] - wd / 2;
                //    lane_v_right[i, 0] = lane_h[i, 0] + wd / 2;
                //}

                //// mode 0: pos cctv as grid    1: pos cctv as random    2: use prepared as random cctv set
                ///* switch (cctvMode)
                //{
                //    case 0:
                //        setCCTV(n_cctv, wd, lane_num); break;
                //    case 1:
                //        setCCTVForBrute(n_cctv); break;
                //    case 2:
                //        break;

                //}*/

                ////setPed(n_ped);
                ////setCar(n_car);
            }
            public void roadBuilderWithOsm(Point upperCorner, Point lowerCorner)
            {
                // Coordination of osm file is epsg4326
                this.lowerCorner = TransformCoordinate(lowerCorner, 3857, 4326);
                //this.lowerCorner.printString();
                this.upperCorner = TransformCoordinate(upperCorner, 3857, 4326);
                //this.upperCorner.printString();

                this.X_mapSize = getDistanceBetweenPointsOfepsg4326(this.lowerCorner.x, this.lowerCorner.y, this.upperCorner.x, this.lowerCorner.y);
                //Console.WriteLine("x map size: {0}", this.X_mapSize);
                this.Y_mapSize = getDistanceBetweenPointsOfepsg4326(this.lowerCorner.x, this.lowerCorner.y, this.lowerCorner.x, this.upperCorner.y);
                //Console.WriteLine("y map size: {0}", this.Y_mapSize);

                this.X_grid_num = (int)Math.Truncate(X_mapSize) / 10000 + 2;
                //Console.WriteLine("x grid num: {0}", this.X_grid_num);
                this.Y_grid_num = (int)Math.Truncate(Y_mapSize) / 10000 + 2;
                //Console.WriteLine("y grid num: {0}", this.Y_grid_num);

                foreach (FOSMWayInfo wayFromOsm in osmReader.Ways)
                {
                    if (wayFromOsm.WayType != EOSMWayType.Other && wayFromOsm.WayType != EOSMWayType.Building
                        && wayFromOsm.Nodes.Count > 1)
                    {
                        Road newRoad = new Road();
                        newRoad.RoadName = wayFromOsm.Name;
                        newRoad.RoadRef = wayFromOsm.Ref;
                        newRoad.bIsOneWay = wayFromOsm.bIsOneWay;

                        // define Road Type and Width
                        newRoad.EOSMWayType = wayFromOsm.WayType;
                        {
                            if (wayFromOsm.WayType == EOSMWayType.Motorway
                                || wayFromOsm.WayType == EOSMWayType.Motorway_Link
                                || wayFromOsm.WayType == EOSMWayType.Trunk
                                || wayFromOsm.WayType == EOSMWayType.Trunk_Link
                                || wayFromOsm.WayType == EOSMWayType.Primary
                                || wayFromOsm.WayType == EOSMWayType.Primary_Link)
                            {
                                newRoad.RoadType = RoadType.Highway;
                                newRoad.Width = RoadWidth[Convert.ToInt32(newRoad.RoadType)];
                            }
                            else if (wayFromOsm.WayType == EOSMWayType.Secondary
                                || wayFromOsm.WayType == EOSMWayType.Secondary_Link
                                || wayFromOsm.WayType == EOSMWayType.Tertiary
                                || wayFromOsm.WayType == EOSMWayType.Tertiary_Link)
                            {
                                newRoad.RoadType = RoadType.MajorRoad;
                                newRoad.Width = RoadWidth[Convert.ToInt32(newRoad.RoadType)];
                            }
                            else if (wayFromOsm.WayType == EOSMWayType.Residential
                                || wayFromOsm.WayType == EOSMWayType.Service
                                || wayFromOsm.WayType == EOSMWayType.Unclassified
                                || wayFromOsm.WayType == EOSMWayType.Road)
                            {
                                newRoad.RoadType = RoadType.Street;
                                newRoad.Width = RoadWidth[Convert.ToInt32(newRoad.RoadType)];
                            }
                        }

                        // 도로를 이루는 node 탐색
                        foreach (FOSMNodeInfo nodeFromOsm in wayFromOsm.Nodes)
                        {
                            Node newNode = new Node(nodeFromOsm.Longitude, nodeFromOsm.Latitude, 0.0);
                            //debug
                            {
                                //Console.WriteLine(newNode.ToString());
                                //Console.WriteLine(newNode.x <= this.upperCorner.x && newNode.x >= this.lowerCorner.x
                                //    && newNode.y <= this.upperCorner.y && newNode.x >= this.lowerCorner.y);
                                //Console.ReadLine();
                            }

                            // 지도 영역 안에 있는 Node만 취급
                            if (newNode.x < this.upperCorner.x && newNode.x > this.lowerCorner.x
                                && newNode.y < this.upperCorner.y && newNode.x > this.lowerCorner.y)
                            {
                                newNode = new Node(calcIndexOnProg(newNode, this.lowerCorner, this.upperCorner));

                                bool isNodeExist = false;
                                int indexOfExistingNode = 0;
                                for (int i = 0; i < this.nodes.Count; i++)
                                {
                                    if (this.nodes[i].Equals(newNode))
                                    {
                                        isNodeExist = true;
                                        indexOfExistingNode = i;
                                        break;
                                    }
                                }

                                if (!isNodeExist)
                                {
                                    this.nodes.Add(newNode);
                                    newRoad.NodeIndices.Add(this.nodes.Count - 1);
                                }
                                else
                                {
                                    newRoad.NodeIndices.Add(indexOfExistingNode);
                                }
                            }
                        }

                        roads.Add(newRoad);
                    }
                }

                // Road class의 Parts, LeftLinek, RightLine 정의
                foreach (Road road in roads)
                {
                    if (road.Width == 0.0) road.Width = 8000.0;

                    for(int i = 0; i < road.NodeIndices.Count - 1; i++)
                    {
                        PartOfRoad newPart = new PartOfRoad();

                        Point p1 = nodes[road.NodeIndices[i]];
                        Point p2 = nodes[road.NodeIndices[i + 1]];

                        newPart.NodeToNode = new Segment(p1, p2);

                        lVector p1p2Vector = new lVector(p2.x - p1.x, p2.y - p1.y);

                        lVector verticalVector = new lVector(-p1p2Vector.componet_y, p1p2Vector.componet_x);

                        lVector XunitVector = new lVector(0.001, 0);
                        double angle = Math.Round(Math.Acos(InnerProduct(verticalVector, XunitVector) / (Norm(verticalVector) * Norm(XunitVector))), 8);
                        if (verticalVector.componet_y < 0)
                        {
                            angle = Math.Round(2 * Math.PI - angle, 8);
                        }

                        Point p1_left = new Point(p1.x + (road.Width) / 2 * Math.Cos(angle), p1.y + (road.Width) / 2 * Math.Sin(angle), 0.0);
                        Point p1_right = new Point(p1.x - (road.Width) / 2 * Math.Cos(angle), p1.y - (road.Width) / 2 * Math.Sin(angle), 0.0);
                        newPart.LowerSeg = new Segment(p1_left, p1_right);

                        Point p2_left = new Point(p2.x + (road.Width) / 2 * Math.Cos(angle), p2.y + (road.Width) / 2 * Math.Sin(angle), 0.0);
                        Point p2_right = new Point(p2.x - (road.Width) / 2 * Math.Cos(angle), p2.y - (road.Width) / 2 * Math.Sin(angle), 0.0);
                        newPart.UpperSeg = new Segment(p2_left, p2_right);

                        //debug
                        {
                            //Console.Write("p1 -> ");
                            //p1.printString();
                            //Console.Write("p2 -> ");
                            //p2.printString();
                            //Console.WriteLine("* Lower Segment - " + newPart.LowerSeg.ToString());
                            //Console.WriteLine("* Upper Segment - " + newPart.UpperSeg.ToString());
                            //Console.WriteLine();
                        }
                    }
                }

                //debug roads
                {
                    //foreach (Road road in roads)
                    //{
                    //    //if (road.Width == 0)
                    //    {
                    //        Console.WriteLine(road.ToString());
                    //        Console.WriteLine();
                    //    }
                    //}
                }

                // 교차로 정보 roadsRefs에 저장
                for (int i = 0; i < roads.Count; i++)
                {
                    for (int j = i + 1; j < roads.Count; j++)
                    {
                        foreach (int nodeIdx in roads[i].NodeIndices)
                        {
                            if (roads[j].NodeIndices.Contains(nodeIdx))
                            {
                                int dstIdx = nodeIdx;
                                bool isDstExist = false;
                                foreach(RoadRef dstRef in roadsRefs)
                                {
                                    if(dstRef.DSTNodeIndex == dstIdx)
                                    {
                                        isDstExist = true;
                                        if (!dstRef.roadIndices.Contains(i)) dstRef.roadIndices.Add(i);
                                        if (!dstRef.roadIndices.Contains(j)) dstRef.roadIndices.Add(j);
                                    }
                                }
                                if (!isDstExist)
                                {
                                    RoadRef newRef = new RoadRef();
                                    newRef.DSTNodeIndex = dstIdx;
                                    newRef.roadIndices.Add(i);
                                    newRef.roadIndices.Add(j);

                                    newRef.minWidth = (roads[i].Width < roads[j].Width) ? roads[i].Width : roads[j].Width;
                                    newRef.maxWidth = (roads[i].Width > roads[j].Width) ? roads[i].Width : roads[j].Width;

                                    roadsRefs.Add(newRef);
                                }
                            }
                        }
                    }
                }

                //debug roadsRefs
                {
                    //foreach (RoadRef dst in roadsRefs)
                    //{
                    //    //Console.WriteLine(dst.ToString());

                    //    if (dst.roadIndices.Count > 1)
                    //    {
                    //        Console.WriteLine(dst.ToString());
                    //        for (int i = 0; i < dst.roadIndices.Count; i++)
                    //        {
                    //            Console.WriteLine(roads[dst.roadIndices[i]].ToString());
                    //        }
                    //    }
                    //    Console.WriteLine();
                    //}
                }
            }

            /* --------------------------------------
             * update map size
            -------------------------------------- */
            public void updateMapMinSize_X(double x)
            {

            }
            public void updateMapMaxSize_X(double x)
            {

            }
            public void updateMapMinSize_Y(double y)
            {

            }
            public void updateMapMaxSize_Y(double y)
            {

            }

            /* --------------------------------------
             * set coordinate of objects with csv
            -------------------------------------- */
            public void setPosZeros(int[,] pos, int y, int x)
            {
                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < x; j++)
                    {
                        pos[i, j] = 0;
                    }
                }
            }

            /* --------------------------------------
             * set coordinate of objects with csv
            -------------------------------------- */
            public void setPedswithCSV(string filename, Pedestrian[] peds)
            {
                tr.PedsFromCSV(filename, peds);

                this.setPosZeros(pedPos, this.Y_grid_num, this.X_grid_num);

                for(int i = 0;i < peds.Length ; i++)
                {
                    pedPos[Convert.ToInt32((peds[i].Y) / 10000), Convert.ToInt32((peds[i].X / 10000))] += 1;
                }
            }

            public void setCarswithCSV(string filename, Car[] cars)
            {
                tr.CarsFromCSV(filename, cars);

                this.setPosZeros(carPos, this.Y_grid_num, this.X_grid_num);

                for (int i = 0; i < cars.Length; i++)
                {
                    carPos[Convert.ToInt32((cars[i].Y) / 10000), Convert.ToInt32((cars[i].X / 10000))] += 1;
                }
            }

            public void setCctvswithCSV(string filename, CCTV[] cctvs)
            {
                cr.CctvsFromCSV(filename, cctvs);

                this.setPosZeros(cctvPos, this.Y_grid_num, this.X_grid_num);

                for (int i = 0; i < cctvs.Length; i++)
                {
                    cctvPos[Convert.ToInt32((cctvs[i].Y) / 10000), Convert.ToInt32((cctvs[i].X / 10000))] += 1;
                }

                foreach (CCTV cctv in cctvs)
                {
                    clog.clearCctvLog();

                    cctv.get_PixelDensity(Dist,
                            cctv.WD,
                            cctv.HE,
                            cctv.Focal_Length,
                            cctv.imW,
                            cctv.imH);

                    cctv.get_H_FOV(Dist, cctv.WD, cctv.Focal_Length, cctv.ViewAngleH, cctv.X, cctv.Y);
                    cctv.get_V_FOV(Dist, cctv.HE, cctv.Focal_Length, cctv.ViewAngleV, cctv.X, cctv.Z);
                }
            }

            /* --------------------------------------
             * set coordinate of objects
            -------------------------------------- */

            // set Architecture object positions
            public void setBuildingsArrPos(int n_building, Building[] buildings)
            {
                if (buildingPos == null)
                {
                    //debug
                    buildingPos = new int[this.Y_grid_num, this.X_grid_num];
                }
                else
                {
                    this.setPosZeros(buildingPos, this.Y_grid_num, this.X_grid_num);
                }

                for (int i = 0; i < n_building; i++)
                {
                    int grid_Y = Convert.ToInt32(buildings[i].Y / 10000),
                        grid_X = Convert.ToInt32(buildings[i].X / 10000);

                    if (grid_Y >= this.Y_grid_num)
                    {
                        grid_Y = this.Y_grid_num - 1;
                    }
                    if(grid_X >= this.X_grid_num)
                    {
                        grid_X = this.X_grid_num - 1;
                    }
                    if(grid_Y < 0)
                    {
                        grid_Y = 0;
                    }
                    if(grid_X < 0)
                    {
                        grid_X = 0;
                    }

                    // debug
                    //Console.WriteLine("Attempting add on grid({0}, {1})", grid_Y, grid_X);
                    //Console.WriteLine("- (grid_Y_Size, grid_X_Size) = ({0}, {1})", this.Y_grid_num, this.X_grid_num);
                    buildingPos[grid_Y, grid_X] += 1;
                    //Console.WriteLine("Sucess init grid");
                }
            }

            // set CCTV positions
            // 일정한 간격으로 
            public void setCCTVsArrPos(CCTV[] cctvs)
            {
                if (cctvPos == null)
                {
                    cctvPos = new int[this.Y_grid_num, this.X_grid_num];
                }
                else
                {
                    this.setPosZeros(cctvPos, this.Y_grid_num, this.X_grid_num);
                }

                double x_range = X_mapSize;
                double y_range = Y_mapSize;
                int rootN = (int)Math.Sqrt((double)cctvs.Length);

                // x좌표가 int 형식이라 캐스팅해서 완벽한 그리드는 아닐 수 있음
                int x_intvl = (int)x_range / (rootN + 1);
                int y_intvl = (int)y_range / (rootN + 1);
                Console.WriteLine("x_mapsize y_mapsize x_range y_range rootN x_intvl y_intvl {0} {1} {2} {3} {4} {5} {6}", 
                                    this.X_mapSize, this.Y_mapSize, x_range, y_range, rootN, x_intvl, y_intvl);
                double startX = x_intvl;
                double startY = y_intvl;

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Set CCTV Position");
                int cctvIdx = 0;
                for(int i = 0; i < rootN; i ++)
                {
                    startX = x_intvl;
                    for (int j = 0; j < rootN; j++)
                    {
                        cctvs[cctvIdx].X = (int)startX;
                        cctvs[cctvIdx].Y = (int)startY;
                        // Console.WriteLine("cctv{0}\t{1, 6} {2, 6} ", i * rootN + j, cctvs[cctvIdx].X , cctvs[cctvIdx].Y);
                        // Console.WriteLine("pos arr {0} {1} ", cctvs[i].Y / 10000, cctvs[i].X / 10000);   // 사용 안함
                        // Console.WriteLine();
                        startX += x_intvl;

                        //debug
			            cctvPos[(cctvs[cctvIdx].Y)/10000, (cctvs[cctvIdx].X)/10000] += 1;
                        
                        cctvIdx++;

                    }

                    startY += y_intvl;
                			// 여기는 cctv 값 넣는 for문 안쪽
                }
                // for문 끝나고

                // this.printCctvPos();
            }
            // 교차로에만 설치하는 경우
            // 교차로 중 설치 위치는 랜덤
            public void setCCTVsArrPosbyRandomInDST(CCTV[] cctvs)
            {
                if (cctvPos == null)
                {
                    cctvPos = new int[this.Y_grid_num, this.X_grid_num];
                }
                else
                {
                    this.setPosZeros(cctvPos, this.Y_grid_num, this.X_grid_num);
                }

                Console.WriteLine("x_mapsize y_mapsize {0} {1} ", this.X_mapSize, this.Y_mapSize);

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Set CCTV Position by Random DST");
                for (int i = 0; i < cctvs.Length; i++)
                {
                    Random rand = new Random();
                    int intersectidx = rand.Next(roadsRefs.Count);
                    Node dst = nodes[roadsRefs[intersectidx].DSTNodeIndex];
                    cctvs[i].X = (int)Math.Truncate(dst.x);
                    cctvs[i].Y = (int)Math.Truncate(dst.y);

                    //cctvs[i].X = (int)Math.Truncate(DST[intersectidx, 0]);
                    //cctvs[i].Y = (int)Math.Truncate(DST[intersectidx, 1]);

                    // Console.WriteLine("cctv{0}\t{1, 6} {2, 6} ", i, cctvs[i].X, cctvs[i].Y);
                    // Console.WriteLine();

                    //debug
                    cctvPos[(cctvs[i].Y) / 10000, (cctvs[i].X) / 10000] += 1;
                }

                // this.printCctvPos();
            }

            // 무작위 좌표에 cctv 배치
            public void setCCTVsArrPosbyRandomInInt(CCTV[] cctvs)
            {
                if (cctvPos == null)
                {
                    cctvPos = new int[this.Y_grid_num, this.X_grid_num];
                }
                else
                {
                    this.setPosZeros(cctvPos, this.Y_grid_num, this.X_grid_num);
                }

                Console.WriteLine("x_mapsize y_mapsize {0} {1} ", this.X_mapSize, this.Y_mapSize);

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Set CCTV Position by Random Int");
                for (int i = 0; i < cctvs.Length; i++)
                {
                    Random rand = new Random();
                    cctvs[i].X = rand.Next(Convert.ToInt32(Math.Truncate(this.X_mapSize)));
                    cctvs[i].Y = rand.Next(Convert.ToInt32(Math.Truncate(this.Y_mapSize)));

                    // Console.WriteLine("cctv{0}\t{1, 6} {2, 6} ", i, cctvs[i].X, cctvs[i].Y);
                    // Console.WriteLine();

                    //debug
                    cctvPos[(cctvs[i].Y) / 10000, (cctvs[i].X) / 10000] += 1;
                }

                // this.printCctvPos();
            }
            public void setCCTVPosbyRandomInInt(int idx, CCTV cctv, int x, int y)
            {
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Set CCTV Position by Random Int");
                //Random rand = new Random();
                //cctv.X = rand.Next(Convert.ToInt32(Math.Truncate(this.X_mapSize)));
                cctv.X = x;
                //cctv.Y = rand.Next(Convert.ToInt32(Math.Truncate(this.Y_mapSize)));
                cctv.Y = y;

                Console.WriteLine("cctv{0}\t{1, 6} {2, 6} ", idx, cctv.X, cctv.Y);
                Console.WriteLine();

                //debug
                cctvPos[(cctv.Y) / 10000, (cctv.X) / 10000] += 1;

                // this.printCctvPos();
            }

            // 파일로부터 읽은 cctv 위치
            public void setCCTVsArrPosbyRealWorldData(CCTV[] cctvs)
            {
                if (cctvPos == null)
                {
                    cctvPos = new int[this.Y_grid_num, this.X_grid_num];
                }
                else
                {
                    this.setPosZeros(cctvPos, this.Y_grid_num, this.X_grid_num);
                }

                Console.WriteLine("x_mapsize y_mapsize {0} {1} ", this.X_mapSize, this.Y_mapSize);

                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Set CCTV Position by Real World Data");
                for (int i = 0, j = 0; i < cr.realWorldCctvData.Count(); i++)
                {
                    Point cctvPositionAsWGS84 = new Point(Convert.ToDouble(cr.realWorldCctvData[i][8]), Convert.ToDouble(cr.realWorldCctvData[i][7]), 0);
                    Point cctvLocationOnSystem = calcIndexOnProg(cctvPositionAsWGS84, this.lowerCorner, this.upperCorner);

                    int cctvNumAtPosition = Convert.ToInt32(cr.realWorldCctvData[i][2]);
                    for (int k = 0; k < cctvNumAtPosition; j++, k++)
                    {
                        cctvs[j].X = Convert.ToInt32(cctvLocationOnSystem.x);
                        cctvs[j].Y = Convert.ToInt32(cctvLocationOnSystem.y);

                        //debug
                        //Console.WriteLine("cctv{0}\t{1, 6} {2, 6} ", j, cctvs[i].X, cctvs[i].Y);
                        //Console.WriteLine();

                        //debug
                        cctvPos[(cctvs[j].Y) / 10000, (cctvs[j].X) / 10000] += 1;
                    }
                }

                //debug
                //this.printCctvPos();
            }

            // 보행자 위치 처음 설정
            public void setPedsArrPos(Pedestrian[] peds, int randSeed)
            {
                if (pedPos == null)
                {
                    pedPos = new int[this.Y_grid_num, this.X_grid_num];
                }
                else
                {
                    this.setPosZeros(pedPos, this.Y_grid_num, this.X_grid_num);
                }

                for (int i = 0; i < peds.Length; i++)
                {
                    Random rand = new Random(randSeed);
                    // int intersectidx = rand.Next(9);
                    int intersectidx = rand.Next(roadsRefs.Count);

                    // Console.WriteLine(intersectidx);
                    double[,] newPos = getPointOfAdjacentRoad(intersectidx);
                    peds[i].X = Math.Round(newPos[0, 0]);
                    peds[i].Y = Math.Round(newPos[0, 1]);

                    /*Random rand = new Random();
                    double opt = rand.NextDouble();

                    if (opt > 0.5) {
                        peds[i].X = Math.Round(laneVector.Max() * opt);
                        peds[i].Y = lane_h[rand.Next(0, lane_h.GetLength(0)), 0];
                    }
                    else
                    {
                        peds[i].X =lane_v[rand.Next(0, lane_v.GetLength(0)), 0];
                        peds[i].Y = Math.Round(laneVector.Max() * opt);
                    }*/
                    pedPos[Convert.ToInt32((peds[i].Y) / 10000), Convert.ToInt32((peds[i].X / 10000))] += 1;
                }
                // for문 끝나고

                this.printPedPos();
            }
            public void setPedPos(Pedestrian ped, int intersectidx)
            {
                //Random rand = new Random();
                // int intersectidx = rand.Next(9);
                //int intersectidx = rand.Next(lane_num * lane_num);

                // Console.WriteLine(intersectidx);
                double[,] newPos = getPointOfAdjacentRoad(intersectidx);
                ped.X = Math.Round(newPos[0, 0]);
                ped.Y = Math.Round(newPos[0, 1]);
                Console.WriteLine("ped.Y -> {0} / ped.X -> {1}", 
                    Convert.ToInt32((ped.Y) / 10000), Convert.ToInt32((ped.X / 10000)));

                /*Random rand = new Random();
                double opt = rand.NextDouble();

                if (opt > 0.5) {
                    peds[i].X = Math.Round(laneVector.Max() * opt);
                    peds[i].Y = lane_h[rand.Next(0, lane_h.GetLength(0)), 0];
                }
                else
                {
                    peds[i].X =lane_v[rand.Next(0, lane_v.GetLength(0)), 0];
                    peds[i].Y = Math.Round(laneVector.Max() * opt);
                }*/
                pedPos[Convert.ToInt32((ped.Y) / 10000), Convert.ToInt32((ped.X / 10000))] += 1;

                //this.printPedPos();
            }

            // set Car object
            public void setCarsArrPos(Car[] cars, int randSeed)
            {
                if (carPos == null)
                {
                    carPos = new int[this.Y_grid_num, this.X_grid_num];
                }
                else
                {
                    this.setPosZeros(carPos, this.Y_grid_num, this.X_grid_num);
                }

                for (int i = 0; i < cars.Length; i++)
                {
                    Random rand = new Random(randSeed);
                    int intersectidx = rand.Next(roadsRefs.Count);
                    Node dst = nodes[roadsRefs[intersectidx].DSTNodeIndex];
                    cars[i].X = dst.x;
                    cars[i].Y = dst.y;

                    //cars[i].X = DST[intersectidx, 0];
                    //cars[i].Y = DST[intersectidx, 1];

                    // 230802 박민제
                    // 교차로를 지나는 도로 폭 중 가장 작은 것을 width로
                    double width = roadsRefs[intersectidx].minWidth;

                    int carintersectidx = rand.Next(4); // 0, 1, 2, 3
                    if (carintersectidx == 0)
                    {// down left
                        cars[i].X -= width / 4;
                        cars[i].Y += width / 4;
                    }
                    else if (carintersectidx == 1)
                    {// up left
                        cars[i].X += width / 4;
                        cars[i].Y += width / 4;
                    }
                    else if (carintersectidx == 2)
                    {// up right
                        cars[i].X += width / 4;
                        cars[i].Y -= width / 4;
                    }
                    else if (carintersectidx == 3)
                    {// down right
                        cars[i].X -= width / 4;
                        cars[i].Y -= width / 4;
                    }
                    carPos[Convert.ToInt32((cars[i].Y) / 10000), Convert.ToInt32((cars[i].X / 10000))] += 1;
                }
                // for문 끝나고

                this.printCarPos();
            }
            public void setCarPos(Car car, int intersectidx, int carintersectidx)
            {
                //Random rand = new Random();
                //int intersectidx = rand.Next(lane_num * lane_num);
                //car.X = DST[intersectidx, 0];
                //car.Y = DST[intersectidx, 1];

                Node dst = nodes[roadsRefs[intersectidx].DSTNodeIndex];
                car.X = dst.x;
                car.Y = dst.y;

                // 230802 박민제
                // 교차로를 지나는 도로 폭 중 가장 작은 것을 width로
                double width = roadsRefs[intersectidx].minWidth;

                //int carintersectidx = rand.Next(4); // 0, 1, 2, 3
                if (carintersectidx == 0)
                {// down left
                    car.X -= width / 4;
                    car.Y += width / 4;
                }
                else if (carintersectidx == 1)
                {// up left
                    car.X += width / 4;
                    car.Y += width / 4;
                }
                else if (carintersectidx == 2)
                {// up right
                    car.X += width / 4;
                    car.Y -= width / 4;
                }
                else if (carintersectidx == 3)
                {// down right
                    car.X -= width / 4;
                    car.Y -= width / 4;
                }
                carPos[Convert.ToInt32((car.Y) / 10000), Convert.ToInt32((car.X / 10000))] += 1;
                

                //this.printCarPos();
            }

            /* --------------------------------------
             * get coordinate of objects
            -------------------------------------- */
            public void getPosOfCctvs(CCTV[] cctvs)
            {
                if (cctvPos == null)
                {
                    cctvPos = new int[this.Y_grid_num, this.X_grid_num];
                }
                else
                {
                    this.setPosZeros(cctvPos, this.Y_grid_num, this.X_grid_num);
                }

                for (int i = 0; i < cctvs.Length; i++)
                {
                    cctvPos[(cctvs[i].Y) / 10000, (cctvs[i].X) / 10000] += 1;
                }
            }

            /* --------------------------------------
             * get Point and index
            -------------------------------------- */
            public double[,] getPointOfAdjacentRoad(int currAreaIdx)
            {
                if (currAreaIdx == -1)
                {
                    return new double[,] { { 0, 0 } };
                }

                int i, j;
                Random rand = new Random();

                //do
                //{
                //    i = currAreaIdx / lane_num;
                //    j = currAreaIdx % lane_num;

                //    int opt = rand.Next(0, 4);
                //    if (opt == 0) j += 1; // up
                //    else if (opt == 1) j -= 1; // down
                //    else if (opt == 2) i -= 1; // left
                //    else if (opt == 3) i += 1; // right

                //} while (i < 0 || i >= lane_num || j < 0 || j >= lane_num);

                /** 230802 박민제
                 * 현재 위치에서 가장 가까운 node로 목적지 설정
                 */
                // currAreaIdx에 해당하는 교차로 node
                int dstNodeIdx = this.roadsRefs[currAreaIdx].DSTNodeIndex;

                List<Node> adjacentNodes = new List<Node>();
                foreach(int roadIdx in this.roadsRefs[currAreaIdx].roadIndices)
                {
                    int currentNodeIdx = -1;
                    for (currentNodeIdx = 0; currentNodeIdx < this.roads[roadIdx].NodeIndices.Count; currentNodeIdx++)
                    {
                        if (this.roads[roadIdx].NodeIndices[currentNodeIdx] == dstNodeIdx)
                        {
                            break;
                        }
                    }

                    if(currentNodeIdx - 1 >= 0)
                        adjacentNodes.Add(this.nodes[this.roads[roadIdx].NodeIndices[currentNodeIdx - 1]]);
                    if(currentNodeIdx + 1 < this.roads[roadIdx].NodeIndices.Count)
                        adjacentNodes.Add(this.nodes[this.roads[roadIdx].NodeIndices[currentNodeIdx + 1]]);
                }

                //int idx = lane_num * i + j;
                double[,] newPos = new double[1, 2];
                // newPos[0,0] = DST[idx, 0] + rand.Next(-width, width) * rand.NextDouble();
                // newPos[0,1] = DST[idx, 1] + rand.Next(-width, width) * rand.NextDouble();

                //20220512
                // newPos[0, 0] = rand.Next((int)intersectionArea[idx, 0], (int)intersectionArea[idx, 1]);
                // newPos[0, 1] = rand.Next((int)intersectionArea[idx, 2], (int)intersectionArea[idx, 3]);

                Node nodeForNewPos = adjacentNodes[rand.Next(adjacentNodes.Count)];
                newPos[0, 0] = nodeForNewPos.x;
                newPos[0, 1] = nodeForNewPos.y;

                // Console.WriteLine("newpos {0} {1}", newPos[0, 0], newPos[0, 1]);
                return newPos;
            }

            public int getIdxOfIntersection(double x, double y)
            {
                //for (int i = 0; i < intersectionArea.GetLength(0); i++)
                //{
                //    if (x >= intersectionArea[i, 0] && x <= intersectionArea[i, 1] && y >= intersectionArea[i, 2] && y <= intersectionArea[i, 3])
                //    {
                //        // Console.WriteLine("getIdxIntersection return {0}", i);
                //        return i;
                //    }
                //}

                // 교차로의 큰 반지름 안의 점인지.
                for (int i = 0; i < roadsRefs.Count; i++)
                {
                    Node dst = nodes[roadsRefs[i].DSTNodeIndex];
                    double powOfDist = Math.Pow(x - dst.x, 2) + Math.Pow(y - dst.y, 2);
                    if (powOfDist <= Math.Pow(roadsRefs[i].maxWidth / 2, 2))
                    {
                        // Console.WriteLine("getIdxIntersection return {0}", i);
                        return i;
                    }
                }

                return -1;
            }

            public double[,] getPointOfAdjacentIntersection(int currAreaIdx, double x, double y)
            {
                if(currAreaIdx == -1){
                    return new double[,] { { 0, 0 } };
                }

                int i, j;
                double curX, curY;

                Node dst = this.nodes[this.roadsRefs[currAreaIdx].DSTNodeIndex];
                double midX = dst.x;
                double midY = dst.y;

                double width = (this.roadsRefs[currAreaIdx].minWidth + this.roadsRefs[currAreaIdx].maxWidth) / 2;

                Random rand = new Random();

                // Console.WriteLine("getPointOfAdjacentIntersection x: {0}, y: {1}, midX: {2}, midY: {3}", x, y, midX, midY);
                //do
                //{
                //    i = currAreaIdx / lane_num;
                //    j = currAreaIdx % lane_num;
                //    curX = x;
                //    curY = y;

                //    int opt = rand.Next(0, 2);

                //    // 이 부분 수정 필요
                //    if ( x < midX && y >= midY ){ // 0 down left
                //        if (opt == 0)// down
                //        {
                //            curY -= width/2;
                //        }
                //        else if (opt == 1) //left
                //        { 
                //            curX -= (this.X_interval + width/2);
                //            i -= 1;
                //        }
                //    }
                //    else if ( x >= midX && y >= midY ){ // 1 up left
                //        if (opt == 0) // up
                //        {   
                //            curY += (this.Y_interval + width/2);
                //            j += 1; 
                //        }
                //        else if (opt == 1) // left
                //        {
                //            curX -= width/2;
                //        }
                //    }
                //    else if ( x >= midX && y < midY ){ // 2 up right
                //        if (opt == 0) // up
                //        {
                //            curY += width/2;
                //        }
                //        else if (opt == 1) // right
                //        {
                //            curX += (this.X_interval + width/2);
                //            i += 1;
                //        }                        
                //    }
                //    else if( x < midX && y < midY ){ // 3 down right
                //        if (opt == 0) // down
                //        {
                //            curY -= (this.Y_interval + width/2);
                //            j -= 1;
                //        }
                //        else if (opt == 1) // right
                //        {
                //            curX += width/2;
                //        }
                //    }

                //} while (i< 0 || i >= lane_num || j < 0|| j >=  lane_num);

                /** 230802 박민제
                 * 현재 위치에서 가장 가까운 node로 목적지 설정
                 */
                // currAreaIdx에 해당하는 교차로 node
                int dstNodeIdx = this.roadsRefs[currAreaIdx].DSTNodeIndex;

                List<Node> adjacentNodes = new List<Node>();
                foreach (int roadIdx in this.roadsRefs[currAreaIdx].roadIndices)
                {
                    int currentNodeIdx = -1;
                    for (currentNodeIdx = 0; currentNodeIdx < this.roads[roadIdx].NodeIndices.Count; currentNodeIdx++)
                    {
                        if (this.roads[roadIdx].NodeIndices[currentNodeIdx] == dstNodeIdx)
                        {
                            break;
                        }
                    }

                    if (currentNodeIdx - 1 >= 0)
                        adjacentNodes.Add(this.nodes[this.roads[roadIdx].NodeIndices[currentNodeIdx - 1]]);
                    if (currentNodeIdx + 1 < this.roads[roadIdx].NodeIndices.Count)
                        adjacentNodes.Add(this.nodes[this.roads[roadIdx].NodeIndices[currentNodeIdx + 1]]);
                }

                Node nodeForNewPos = adjacentNodes[rand.Next(adjacentNodes.Count)];

                //int idx = lane_num * i + j;
                double[,] newPos = new double[1, 2];

                newPos[0, 0] = nodeForNewPos.x;
                newPos[0, 1] = nodeForNewPos.y;

                return newPos;
            }

            /* --------------------------------------
             * print info
            -------------------------------------- */
            public void printRoadInfo()
            {
                //Console.WriteLine("\n=================== {0, 25} ==========================================\n", "DST");
                //for (int i = 0; i < DST.GetLength(0); i++)
                //{
                //    for (int j = 0; j < 2; j++)
                //    {
                //        if (j == 0) Console.Write("DST[{0, 2}].X = {1, 6}\t", i, DST[i, j]);
                //        if (j == 1) Console.Write("DST[{0, 2}].Y = {1, 6}\t", i, DST[i, j]);
                //    }
                //    Console.WriteLine();
                //}
                //Console.WriteLine("\n========================================================================================\n");

                /* -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
                   Console.WriteLine("\n========================lane Vector========================================");
                   for (int i = 0; i < laneVector.Length; i++)
                   {
                       Console.Write(laneVector[i] + "  ");
                       if (i % 99 == 0) Console.WriteLine();
                   }
                   Console.WriteLine("\n===========================================================================\n");
                ------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

                //Console.WriteLine("\n=================== {0, 25} ==========================================\n", "lane horizontal");
                //for (int i = 0; i < lane_h.GetLength(0); i++)
                //{
                //    Console.WriteLine("\n{0}번째 가로 도로 정보", i);
                //    Console.WriteLine("y좌표 : 위 - 중앙 - 아래");
                //    Console.WriteLine("       {0}   {1}   {2}", lane_h_upper[i,0],lane_h[i,0],lane_h_lower[i,0]);
                //}
                //Console.WriteLine("\n========================================================================================\n");

                

                //Console.WriteLine("\n=================== {0, 25} ==========================================\n", "lane vertical");
                //for (int i = 0; i < lane_h.GetLength(0); i++)
                //{
                //    Console.WriteLine("\n{0}번째 세로 도로 정보", i);
                //    Console.WriteLine("x좌표 : 왼쪽 - 중앙 - 오른쪽");
                //    Console.WriteLine("       {0}   {1}   {2}", lane_v_left[i, 0], lane_v[i, 0], lane_v_right[i, 0]);
                //}
                //Console.WriteLine("\n========================================================================================\n");
            }

            public void printCctvInfo(CCTV[] cctvs)
            {
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Set CCTV Position");

                for (int i = 0; i < cctvs.Length; i++)
                {
                    Console.WriteLine("cctv{0}\t{1, 6} {2, 6} ", i, cctvs[i].X, cctvs[i].Y);
                }
            }

            /* --------------------------------------
             * print Pos on grid
            -------------------------------------- */
            public void printPos(int[,] pos)
            {
                Console.Write("{0, 4}", 0);
                for (int i = 1; i < this.X_grid_num; i++)
                {
                    Console.Write("{0, 2}", i);
                }
                Console.WriteLine();
                for (int i = 0; i < this.Y_grid_num; i++)
                {
                    Console.Write("{0, 2}", i);

                    for (int j = 0; j < this.X_grid_num; j++)
                    {
                        if (pos[i, j] <= 0)
                            Console.Write("{0, 2}", " ");
                        else
                            Console.Write("{0, 2}", pos[i, j]);

                    }
                    Console.WriteLine();
                }
            }

            public void printBuildingPos()
            {
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Print Building Position");
                printPos(this.buildingPos);
            }

            public void printCctvPos()
            {
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Print CCTV Position");
                printPos(this.cctvPos);
            }

            public void printPedPos()
            {
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Print Pedestrian Position");
                printPos(this.pedPos);
            }

            public void printCarPos()
            {
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Print Car Position");
                printPos(this.carPos);
            }

            public void printAllPos()
            {
                Console.WriteLine("\n=================== {0, 25} ==========================================\n", "Print All Position");
                Console.Write("{0, 10}", 0);
                for (int i = 1; i < this.X_grid_num; i++)
                {
                    Console.Write("{0, 8}", i);
                }
                Console.WriteLine();
                for (int i = 0; i < this.Y_grid_num; i++)
                {
                    Console.Write("{0, 2}", i);

                    for (int j = 0; j < this.X_grid_num; j++)
                    {
                        string tmp = "";
                        if (this.cctvPos[i, j] > 0) tmp += "&" + cctvPos[i, j];

                        if (this.buildingPos[i, j] > 0) tmp += "A" + buildingPos[i, j];

                        if (this.pedPos[i, j] > 0) tmp += "P" + pedPos[i, j];

                        if (this.carPos[i, j] > 0) tmp += "C" + carPos[i, j];

                        Console.Write("{0, 8}", tmp);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}