﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static surveillance_system.Program;

namespace surveillance_system_Form
{
    public partial class startSimulation : Form
    {
        public startSimulation()
        {
            InitializeComponent();

            this.btn_startSim.Click -= btn_startSim_Click;
            this.btn_startSim.Click += btn_startSim_Click;

            this.btn_simCancel.Click -= btn_simCancel_Click;
            this.btn_simCancel.Click += btn_simCancel_Click;
        }

        private void btn_startSim_Click(object sender, EventArgs e)
        {
            gm.startSimulationByGui();
            this.Close();
        }

        private void btn_simCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
