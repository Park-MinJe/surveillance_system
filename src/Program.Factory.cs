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
            public World createWorld(int nCctv, int nPed, int nCar)
            {
                // Map 초기화에 사용하는 데이터 출처에 따라 initWorld interface를 상속하는 class 중 하나를 선택
                // 또는 사용자에게 그 출처의 선택을 맡기는 경우, WorldFactory 안에 createWorld 메소드를 여러개 생성할 수도 있음.
                return new World(nCctv, nPed, nCar, new initWorldByGis());
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
            public Architecture createArchitecture()
            {
                return new Architecture();
            }
        }

        public class CctvFactory
        {
            public CCTV createCctv()
            {
                return new CCTV();
            }
        }

        public class SurveillanceTargetFactory
        {

        }

        public class PedFactory
        {
            public Pedestrian createPed()
            {
                return new Pedestrian();
            }
        }

        public class CarFactory
        {
            public Car createCar()
            {
                return new Car();
            }
        }
    }
}
