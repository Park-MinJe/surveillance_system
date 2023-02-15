using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    // 230209 팩토리 메소드 패턴 적용 by mj
    // SurveillanceTargetFactory 하위의 class 생성 시 사용
    public partial class Program
    {
        public class WorldFactory
        {
            public World createWorld()
            {
                // Map 초기화에 사용하는 데이터 출처에 따라 initWorld interface를 상속하는 class 중 하나를 선택
                // 또는 사용자에게 그 출처의 선택을 맡기는 경우, WorldFactory 안에 createWorld 메소드를 여러개 생성할 수도 있음.
                // ex) ~ByGis / ~ByCsv / ...
                return new World();
            }
        }

        public class RoadFactory
        {
            public Road createRoad()
            {
                return new Road();
            }
        }

        public class ArchFactory
        {
            public Architecture createArch()
            {
                return new Architecture();
            }

            public List<Architecture> createArchList()
            {
                return new List<Architecture>();
            }
        }

        public class CctvFactory
        {
            public CCTV createCctv()
            {
                return new CCTV();
            }

            public CCTV[] createCctvArr(int n)
            {
                return new CCTV[n];
            }
        }

        public class SurveillanceTargetFactory
        {
            // SurveillanceTarget
            public SurveillanceTarget[] createSurvArr(int n)
            {
                return new SurveillanceTarget[n];
            }

            // Pedestrian
            public Pedestrian createPed()
            {
                return new Pedestrian();
            }
            public Pedestrian[] createPedArr(int n)
            {
                return new Pedestrian[n];
            }

            // Car
            public Car createCar()
            {
                return new Car();
            }
            public Car[] createCarArr(int n)
            {
                return new Car[n];
            }
        }
    }
}
