namespace surveillance_system_Form
{
    partial class startSimulation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_startSimulation = new System.Windows.Forms.Label();
            this.btn_startSim = new System.Windows.Forms.Button();
            this.btn_simCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lb_startSimulation
            // 
            this.lb_startSimulation.AutoSize = true;
            this.lb_startSimulation.Location = new System.Drawing.Point(113, 47);
            this.lb_startSimulation.Name = "lb_startSimulation";
            this.lb_startSimulation.Size = new System.Drawing.Size(149, 15);
            this.lb_startSimulation.TabIndex = 0;
            this.lb_startSimulation.Text = "시뮬레이션을 시작할까요?";
            // 
            // btn_startSim
            // 
            this.btn_startSim.Location = new System.Drawing.Point(113, 103);
            this.btn_startSim.Name = "btn_startSim";
            this.btn_startSim.Size = new System.Drawing.Size(75, 23);
            this.btn_startSim.TabIndex = 1;
            this.btn_startSim.Text = "시작";
            this.btn_startSim.UseVisualStyleBackColor = true;
            this.btn_startSim.Click += new System.EventHandler(this.btn_startSim_Click);
            // 
            // btn_simCancel
            // 
            this.btn_simCancel.Location = new System.Drawing.Point(194, 103);
            this.btn_simCancel.Name = "btn_simCancel";
            this.btn_simCancel.Size = new System.Drawing.Size(75, 23);
            this.btn_simCancel.TabIndex = 2;
            this.btn_simCancel.Text = "취소";
            this.btn_simCancel.UseVisualStyleBackColor = true;
            this.btn_simCancel.Click += new System.EventHandler(this.btn_simCancel_Click);
            // 
            // startSimulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 163);
            this.Controls.Add(this.btn_simCancel);
            this.Controls.Add(this.btn_startSim);
            this.Controls.Add(this.lb_startSimulation);
            this.Name = "startSimulation";
            this.Text = "startSimulation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lb_startSimulation;
        private Button btn_startSim;
        private Button btn_simCancel;
    }
}