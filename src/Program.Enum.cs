using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public enum TargetType
        {
            PED,
            CAR
        }

        public enum initSurvType
        {
            RAND,
            CSV,
            XML
        }

        public enum initArchType
        {
            GIS
        }
    }
}
