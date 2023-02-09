using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    // 230209 팩토리 메소드 패턴 적용 by mj
    public partial class Program
    {
        public class WorldFactory
        {
            public World createWorld()
            {
                return new World();
            }
        }

        public class RoadFactory
        {

        }

        public class ArchFactory
        {

        }

        public class CctvFactory
        {

        }

        public class PedFactory
        {

        }

        public class CarFactory
        {

        }
    }
}
