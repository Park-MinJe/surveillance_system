namespace surveillance_system_form
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_serviceKey = new System.Windows.Forms.TextBox();
            this.btn_Confirm = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lb_serviceKey = new System.Windows.Forms.Label();
            this.lb_gisMethodName = new System.Windows.Forms.Label();
            this.cb_methodNM = new System.Windows.Forms.ComboBox();
            this.lb_typeName = new System.Windows.Forms.Label();
            this.lb_bbox = new System.Windows.Forms.Label();
            this.lb_pnu = new System.Windows.Forms.Label();
            this.lb_maxFeature = new System.Windows.Forms.Label();
            this.lb_resultType = new System.Windows.Forms.Label();
            this.lb_srsName = new System.Windows.Forms.Label();
            this.tb_bbox = new System.Windows.Forms.TextBox();
            this.tb_pnu = new System.Windows.Forms.TextBox();
            this.cb_typeName = new System.Windows.Forms.ComboBox();
            this.cb_maxFeature = new System.Windows.Forms.ComboBox();
            this.cb_resultType = new System.Windows.Forms.ComboBox();
            this.cb_srsName = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // tb_serviceKey
            // 
            this.tb_serviceKey.Location = new System.Drawing.Point(346, 72);
            this.tb_serviceKey.Name = "tb_serviceKey";
            this.tb_serviceKey.Size = new System.Drawing.Size(100, 23);
            this.tb_serviceKey.TabIndex = 0;
            // 
            // btn_Confirm
            // 
            this.btn_Confirm.Location = new System.Drawing.Point(346, 333);
            this.btn_Confirm.Name = "btn_Confirm";
            this.btn_Confirm.Size = new System.Drawing.Size(75, 23);
            this.btn_Confirm.TabIndex = 1;
            this.btn_Confirm.Text = "확인";
            this.btn_Confirm.UseVisualStyleBackColor = true;
            // 
            // lb_serviceKey
            // 
            this.lb_serviceKey.AutoSize = true;
            this.lb_serviceKey.Location = new System.Drawing.Point(262, 73);
            this.lb_serviceKey.Name = "lb_serviceKey";
            this.lb_serviceKey.Size = new System.Drawing.Size(55, 15);
            this.lb_serviceKey.TabIndex = 2;
            this.lb_serviceKey.Text = "서비스키";
            // 
            // lb_gisMethodName
            // 
            this.lb_gisMethodName.AutoSize = true;
            this.lb_gisMethodName.Location = new System.Drawing.Point(246, 42);
            this.lb_gisMethodName.Name = "lb_gisMethodName";
            this.lb_gisMethodName.Size = new System.Drawing.Size(71, 15);
            this.lb_gisMethodName.TabIndex = 3;
            this.lb_gisMethodName.Text = "사용할 함수";
            // 
            // cb_methodNM
            // 
            this.cb_methodNM.FormattingEnabled = true;
            this.cb_methodNM.Items.AddRange(new object[] {
            "국토교통부 GIS건물일반정보WMS조회",
            "국토교통부 GIS건물일반정보WFS조회",
            "국토교통부 GIS건물집합정보WMS조회",
            "국토교통부 GIS건물집합정보WFS조회"});
            this.cb_methodNM.Location = new System.Drawing.Point(346, 39);
            this.cb_methodNM.Name = "cb_methodNM";
            this.cb_methodNM.Size = new System.Drawing.Size(121, 23);
            this.cb_methodNM.TabIndex = 4;
            // 
            // lb_typeName
            // 
            this.lb_typeName.AutoSize = true;
            this.lb_typeName.Location = new System.Drawing.Point(249, 105);
            this.lb_typeName.Name = "lb_typeName";
            this.lb_typeName.Size = new System.Drawing.Size(68, 15);
            this.lb_typeName.TabIndex = 5;
            this.lb_typeName.Text = "Type Name";
            // 
            // lb_bbox
            // 
            this.lb_bbox.AutoSize = true;
            this.lb_bbox.Location = new System.Drawing.Point(258, 139);
            this.lb_bbox.Name = "lb_bbox";
            this.lb_bbox.Size = new System.Drawing.Size(59, 15);
            this.lb_bbox.TabIndex = 6;
            this.lb_bbox.Text = "검색 범위";
            // 
            // lb_pnu
            // 
            this.lb_pnu.AutoSize = true;
            this.lb_pnu.Location = new System.Drawing.Point(206, 165);
            this.lb_pnu.Name = "lb_pnu";
            this.lb_pnu.Size = new System.Drawing.Size(111, 15);
            this.lb_pnu.TabIndex = 7;
            this.lb_pnu.Text = "필지고유번호(PNU)";
            // 
            // lb_maxFeature
            // 
            this.lb_maxFeature.AutoSize = true;
            this.lb_maxFeature.Location = new System.Drawing.Point(242, 194);
            this.lb_maxFeature.Name = "lb_maxFeature";
            this.lb_maxFeature.Size = new System.Drawing.Size(71, 15);
            this.lb_maxFeature.TabIndex = 8;
            this.lb_maxFeature.Text = "응답 최대값";
            // 
            // lb_resultType
            // 
            this.lb_resultType.AutoSize = true;
            this.lb_resultType.Location = new System.Drawing.Point(258, 225);
            this.lb_resultType.Name = "lb_resultType";
            this.lb_resultType.Size = new System.Drawing.Size(55, 15);
            this.lb_resultType.TabIndex = 9;
            this.lb_resultType.Text = "응답형태";
            // 
            // lb_srsName
            // 
            this.lb_srsName.AutoSize = true;
            this.lb_srsName.Location = new System.Drawing.Point(262, 254);
            this.lb_srsName.Name = "lb_srsName";
            this.lb_srsName.Size = new System.Drawing.Size(55, 15);
            this.lb_srsName.TabIndex = 10;
            this.lb_srsName.Text = "좌표체계";
            // 
            // tb_bbox
            // 
            this.tb_bbox.Location = new System.Drawing.Point(346, 131);
            this.tb_bbox.Name = "tb_bbox";
            this.tb_bbox.Size = new System.Drawing.Size(100, 23);
            this.tb_bbox.TabIndex = 11;
            // 
            // tb_pnu
            // 
            this.tb_pnu.Location = new System.Drawing.Point(346, 157);
            this.tb_pnu.Name = "tb_pnu";
            this.tb_pnu.Size = new System.Drawing.Size(100, 23);
            this.tb_pnu.TabIndex = 12;
            // 
            // cb_typeName
            // 
            this.cb_typeName.FormattingEnabled = true;
            this.cb_typeName.Items.AddRange(new object[] {
            "F171"});
            this.cb_typeName.Location = new System.Drawing.Point(346, 101);
            this.cb_typeName.Name = "cb_typeName";
            this.cb_typeName.Size = new System.Drawing.Size(121, 23);
            this.cb_typeName.TabIndex = 13;
            // 
            // cb_maxFeature
            // 
            this.cb_maxFeature.FormattingEnabled = true;
            this.cb_maxFeature.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59",
            "60",
            "61",
            "62",
            "63",
            "64",
            "65",
            "66",
            "67",
            "68",
            "69",
            "70",
            "71",
            "72",
            "73",
            "74",
            "75",
            "76",
            "77",
            "78",
            "79",
            "80",
            "81",
            "82",
            "83",
            "84",
            "85",
            "86",
            "87",
            "88",
            "89",
            "90",
            "91",
            "92",
            "93",
            "94",
            "95",
            "96",
            "97",
            "98",
            "99",
            "100"});
            this.cb_maxFeature.Location = new System.Drawing.Point(346, 191);
            this.cb_maxFeature.Name = "cb_maxFeature";
            this.cb_maxFeature.Size = new System.Drawing.Size(121, 23);
            this.cb_maxFeature.TabIndex = 14;
            // 
            // cb_resultType
            // 
            this.cb_resultType.FormattingEnabled = true;
            this.cb_resultType.Items.AddRange(new object[] {
            "results",
            "hits"});
            this.cb_resultType.Location = new System.Drawing.Point(346, 222);
            this.cb_resultType.Name = "cb_resultType";
            this.cb_resultType.Size = new System.Drawing.Size(121, 23);
            this.cb_resultType.TabIndex = 15;
            // 
            // cb_srsName
            // 
            this.cb_srsName.FormattingEnabled = true;
            this.cb_srsName.Items.AddRange(new object[] {
            "EPSG:5174"});
            this.cb_srsName.Location = new System.Drawing.Point(346, 251);
            this.cb_srsName.Name = "cb_srsName";
            this.cb_srsName.Size = new System.Drawing.Size(121, 23);
            this.cb_srsName.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cb_srsName);
            this.Controls.Add(this.cb_resultType);
            this.Controls.Add(this.cb_maxFeature);
            this.Controls.Add(this.cb_typeName);
            this.Controls.Add(this.tb_pnu);
            this.Controls.Add(this.tb_bbox);
            this.Controls.Add(this.lb_srsName);
            this.Controls.Add(this.lb_resultType);
            this.Controls.Add(this.lb_maxFeature);
            this.Controls.Add(this.lb_pnu);
            this.Controls.Add(this.lb_bbox);
            this.Controls.Add(this.lb_typeName);
            this.Controls.Add(this.cb_methodNM);
            this.Controls.Add(this.lb_gisMethodName);
            this.Controls.Add(this.lb_serviceKey);
            this.Controls.Add(this.btn_Confirm);
            this.Controls.Add(this.tb_serviceKey);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tb_serviceKey;
        private Button btn_Confirm;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Label lb_serviceKey;
        private Label lb_gisMethodName;
        private ComboBox cb_methodNM;
        private Label lb_typeName;
        private Label lb_bbox;
        private Label lb_pnu;
        private Label lb_maxFeature;
        private Label lb_resultType;
        private Label lb_srsName;
        private TextBox tb_bbox;
        private TextBox tb_pnu;
        private ComboBox cb_typeName;
        private ComboBox cb_maxFeature;
        private ComboBox cb_resultType;
        private ComboBox cb_srsName;
    }
}