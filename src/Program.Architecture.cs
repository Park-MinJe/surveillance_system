using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace surveillance_system
{
    public partial class Program
    {
        public class Architecture
        {
            public double X;
            public double Y;

            public double W;
            public double W2;
            public double H;

            public double[] Pos_H1 = new double[2];
            public double[] Pos_H2 = new double[2];
            public double[] Pos_V1 = new double[2];
            public double[] Pos_V2 = new double[2];

            public void define_Architecture(
                double Width, 
                double Height
            )
            {
                this.W = Width;
                this.W2 = Width / 2;
                this.H = Height;

                //this.Pos_H1[0] = 
            }
        }
    }
}
