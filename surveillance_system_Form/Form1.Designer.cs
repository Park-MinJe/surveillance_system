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
            this.gisInfoPage = new System.Windows.Forms.Panel();
            this.cb_srsName = new System.Windows.Forms.ComboBox();
            this.btn_gisConfirm = new System.Windows.Forms.Button();
            this.cb_methodNM = new System.Windows.Forms.ComboBox();
            this.cb_resultType = new System.Windows.Forms.ComboBox();
            this.lb_gisMethodName = new System.Windows.Forms.Label();
            this.cb_maxFeature = new System.Windows.Forms.ComboBox();
            this.lb_serviceKey = new System.Windows.Forms.Label();
            this.tb_pnu = new System.Windows.Forms.TextBox();
            this.cb_typeName = new System.Windows.Forms.ComboBox();
            this.tb_bbox = new System.Windows.Forms.TextBox();
            this.lb_srsName = new System.Windows.Forms.Label();
            this.tb_serviceKey = new System.Windows.Forms.TextBox();
            this.lb_resultType = new System.Windows.Forms.Label();
            this.lb_typeName = new System.Windows.Forms.Label();
            this.lb_bbox = new System.Windows.Forms.Label();
            this.lb_pnu = new System.Windows.Forms.Label();
            this.lb_maxFeature = new System.Windows.Forms.Label();
            this.simOptionInfoPage = new System.Windows.Forms.Panel();
            this.tb_carNumber = new System.Windows.Forms.TextBox();
            this.tb_pedNumber = new System.Windows.Forms.TextBox();
            this.lb_carNumber = new System.Windows.Forms.Label();
            this.lb_pedNumber = new System.Windows.Forms.Label();
            this.btn_simulationSettingConfirm = new System.Windows.Forms.Button();
            this.tb_cctvNumber = new System.Windows.Forms.TextBox();
            this.cb_cctvLocMode = new System.Windows.Forms.ComboBox();
            this.tb_simNum = new System.Windows.Forms.TextBox();
            this.tb_cctvSetNum = new System.Windows.Forms.TextBox();
            this.lb_cctvNumber = new System.Windows.Forms.Label();
            this.lb_cctvLocMode = new System.Windows.Forms.Label();
            this.lb_simNum = new System.Windows.Forms.Label();
            this.lb_cctvSetNum = new System.Windows.Forms.Label();
            this.gisInfoPage.SuspendLayout();
            this.simOptionInfoPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // gisInfoPage
            // 
            this.gisInfoPage.Controls.Add(this.cb_srsName);
            this.gisInfoPage.Controls.Add(this.btn_gisConfirm);
            this.gisInfoPage.Controls.Add(this.cb_methodNM);
            this.gisInfoPage.Controls.Add(this.cb_resultType);
            this.gisInfoPage.Controls.Add(this.lb_gisMethodName);
            this.gisInfoPage.Controls.Add(this.cb_maxFeature);
            this.gisInfoPage.Controls.Add(this.lb_serviceKey);
            this.gisInfoPage.Controls.Add(this.tb_pnu);
            this.gisInfoPage.Controls.Add(this.cb_typeName);
            this.gisInfoPage.Controls.Add(this.tb_bbox);
            this.gisInfoPage.Controls.Add(this.lb_srsName);
            this.gisInfoPage.Controls.Add(this.tb_serviceKey);
            this.gisInfoPage.Controls.Add(this.lb_resultType);
            this.gisInfoPage.Controls.Add(this.lb_typeName);
            this.gisInfoPage.Controls.Add(this.lb_bbox);
            this.gisInfoPage.Controls.Add(this.lb_pnu);
            this.gisInfoPage.Controls.Add(this.lb_maxFeature);
            this.gisInfoPage.Location = new System.Drawing.Point(12, 12);
            this.gisInfoPage.Name = "gisInfoPage";
            this.gisInfoPage.Size = new System.Drawing.Size(483, 316);
            this.gisInfoPage.TabIndex = 0;
            // 
            // cb_srsName
            // 
            this.cb_srsName.FormattingEnabled = true;
            this.cb_srsName.Items.AddRange(new object[] {
            "EPSG:5174"});
            this.cb_srsName.Location = new System.Drawing.Point(251, 228);
            this.cb_srsName.Name = "cb_srsName";
            this.cb_srsName.Size = new System.Drawing.Size(121, 23);
            this.cb_srsName.TabIndex = 33;
            // 
            // btn_gisConfirm
            // 
            this.btn_gisConfirm.Location = new System.Drawing.Point(199, 269);
            this.btn_gisConfirm.Name = "btn_gisConfirm";
            this.btn_gisConfirm.Size = new System.Drawing.Size(75, 23);
            this.btn_gisConfirm.TabIndex = 18;
            this.btn_gisConfirm.Text = "다음";
            this.btn_gisConfirm.UseVisualStyleBackColor = true;
            // 
            // cb_methodNM
            // 
            this.cb_methodNM.FormattingEnabled = true;
            this.cb_methodNM.Items.AddRange(new object[] {
            "국토교통부 GIS건물일반정보WMS조회",
            "국토교통부 GIS건물일반정보WFS조회",
            "국토교통부 GIS건물집합정보WMS조회",
            "국토교통부 GIS건물집합정보WFS조회",
            "GetFeature"});
            this.cb_methodNM.Location = new System.Drawing.Point(251, 32);
            this.cb_methodNM.Name = "cb_methodNM";
            this.cb_methodNM.Size = new System.Drawing.Size(121, 23);
            this.cb_methodNM.TabIndex = 21;
            // 
            // cb_resultType
            // 
            this.cb_resultType.FormattingEnabled = true;
            this.cb_resultType.Items.AddRange(new object[] {
            "results",
            "hits"});
            this.cb_resultType.Location = new System.Drawing.Point(251, 199);
            this.cb_resultType.Name = "cb_resultType";
            this.cb_resultType.Size = new System.Drawing.Size(121, 23);
            this.cb_resultType.TabIndex = 32;
            // 
            // lb_gisMethodName
            // 
            this.lb_gisMethodName.AutoSize = true;
            this.lb_gisMethodName.Location = new System.Drawing.Point(130, 35);
            this.lb_gisMethodName.Name = "lb_gisMethodName";
            this.lb_gisMethodName.Size = new System.Drawing.Size(71, 15);
            this.lb_gisMethodName.TabIndex = 20;
            this.lb_gisMethodName.Text = "사용할 함수";
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
            this.cb_maxFeature.Location = new System.Drawing.Point(251, 168);
            this.cb_maxFeature.Name = "cb_maxFeature";
            this.cb_maxFeature.Size = new System.Drawing.Size(121, 23);
            this.cb_maxFeature.TabIndex = 31;
            // 
            // lb_serviceKey
            // 
            this.lb_serviceKey.AutoSize = true;
            this.lb_serviceKey.Location = new System.Drawing.Point(146, 60);
            this.lb_serviceKey.Name = "lb_serviceKey";
            this.lb_serviceKey.Size = new System.Drawing.Size(55, 15);
            this.lb_serviceKey.TabIndex = 19;
            this.lb_serviceKey.Text = "서비스키";
            // 
            // tb_pnu
            // 
            this.tb_pnu.Location = new System.Drawing.Point(251, 139);
            this.tb_pnu.Name = "tb_pnu";
            this.tb_pnu.Size = new System.Drawing.Size(100, 23);
            this.tb_pnu.TabIndex = 29;
            // 
            // cb_typeName
            // 
            this.cb_typeName.FormattingEnabled = true;
            this.cb_typeName.Items.AddRange(new object[] {
            "F171"});
            this.cb_typeName.Location = new System.Drawing.Point(251, 82);
            this.cb_typeName.Name = "cb_typeName";
            this.cb_typeName.Size = new System.Drawing.Size(121, 23);
            this.cb_typeName.TabIndex = 30;
            // 
            // tb_bbox
            // 
            this.tb_bbox.Location = new System.Drawing.Point(251, 110);
            this.tb_bbox.Name = "tb_bbox";
            this.tb_bbox.Size = new System.Drawing.Size(100, 23);
            this.tb_bbox.TabIndex = 28;
            // 
            // lb_srsName
            // 
            this.lb_srsName.AutoSize = true;
            this.lb_srsName.Location = new System.Drawing.Point(146, 231);
            this.lb_srsName.Name = "lb_srsName";
            this.lb_srsName.Size = new System.Drawing.Size(55, 15);
            this.lb_srsName.TabIndex = 27;
            this.lb_srsName.Text = "좌표체계";
            // 
            // tb_serviceKey
            // 
            this.tb_serviceKey.Location = new System.Drawing.Point(251, 57);
            this.tb_serviceKey.Name = "tb_serviceKey";
            this.tb_serviceKey.Size = new System.Drawing.Size(100, 23);
            this.tb_serviceKey.TabIndex = 17;
            // 
            // lb_resultType
            // 
            this.lb_resultType.AutoSize = true;
            this.lb_resultType.Location = new System.Drawing.Point(145, 202);
            this.lb_resultType.Name = "lb_resultType";
            this.lb_resultType.Size = new System.Drawing.Size(55, 15);
            this.lb_resultType.TabIndex = 26;
            this.lb_resultType.Text = "응답형태";
            // 
            // lb_typeName
            // 
            this.lb_typeName.AutoSize = true;
            this.lb_typeName.Location = new System.Drawing.Point(133, 85);
            this.lb_typeName.Name = "lb_typeName";
            this.lb_typeName.Size = new System.Drawing.Size(68, 15);
            this.lb_typeName.TabIndex = 22;
            this.lb_typeName.Text = "Type Name";
            // 
            // lb_bbox
            // 
            this.lb_bbox.AutoSize = true;
            this.lb_bbox.Location = new System.Drawing.Point(142, 113);
            this.lb_bbox.Name = "lb_bbox";
            this.lb_bbox.Size = new System.Drawing.Size(59, 15);
            this.lb_bbox.TabIndex = 23;
            this.lb_bbox.Text = "검색 범위";
            // 
            // lb_pnu
            // 
            this.lb_pnu.AutoSize = true;
            this.lb_pnu.Location = new System.Drawing.Point(90, 142);
            this.lb_pnu.Name = "lb_pnu";
            this.lb_pnu.Size = new System.Drawing.Size(111, 15);
            this.lb_pnu.TabIndex = 24;
            this.lb_pnu.Text = "필지고유번호(PNU)";
            // 
            // lb_maxFeature
            // 
            this.lb_maxFeature.AutoSize = true;
            this.lb_maxFeature.Location = new System.Drawing.Point(129, 171);
            this.lb_maxFeature.Name = "lb_maxFeature";
            this.lb_maxFeature.Size = new System.Drawing.Size(71, 15);
            this.lb_maxFeature.TabIndex = 25;
            this.lb_maxFeature.Text = "응답 최대값";
            // 
            // simOptionInfoPage
            // 
            this.simOptionInfoPage.Controls.Add(this.tb_carNumber);
            this.simOptionInfoPage.Controls.Add(this.tb_pedNumber);
            this.simOptionInfoPage.Controls.Add(this.lb_carNumber);
            this.simOptionInfoPage.Controls.Add(this.lb_pedNumber);
            this.simOptionInfoPage.Controls.Add(this.btn_simulationSettingConfirm);
            this.simOptionInfoPage.Controls.Add(this.tb_cctvNumber);
            this.simOptionInfoPage.Controls.Add(this.cb_cctvLocMode);
            this.simOptionInfoPage.Controls.Add(this.tb_simNum);
            this.simOptionInfoPage.Controls.Add(this.tb_cctvSetNum);
            this.simOptionInfoPage.Controls.Add(this.lb_cctvNumber);
            this.simOptionInfoPage.Controls.Add(this.lb_cctvLocMode);
            this.simOptionInfoPage.Controls.Add(this.lb_simNum);
            this.simOptionInfoPage.Controls.Add(this.lb_cctvSetNum);
            this.simOptionInfoPage.Location = new System.Drawing.Point(12, 12);
            this.simOptionInfoPage.Name = "simOptionInfoPage";
            this.simOptionInfoPage.Size = new System.Drawing.Size(483, 316);
            this.simOptionInfoPage.TabIndex = 1;
            // 
            // tb_carNumber
            // 
            this.tb_carNumber.Location = new System.Drawing.Point(251, 173);
            this.tb_carNumber.Name = "tb_carNumber";
            this.tb_carNumber.Size = new System.Drawing.Size(100, 23);
            this.tb_carNumber.TabIndex = 14;
            // 
            // tb_pedNumber
            // 
            this.tb_pedNumber.Location = new System.Drawing.Point(251, 144);
            this.tb_pedNumber.Name = "tb_pedNumber";
            this.tb_pedNumber.Size = new System.Drawing.Size(100, 23);
            this.tb_pedNumber.TabIndex = 13;
            // 
            // lb_carNumber
            // 
            this.lb_carNumber.AutoSize = true;
            this.lb_carNumber.Location = new System.Drawing.Point(197, 176);
            this.lb_carNumber.Name = "lb_carNumber";
            this.lb_carNumber.Size = new System.Drawing.Size(47, 15);
            this.lb_carNumber.TabIndex = 12;
            this.lb_carNumber.Text = "차량 수";
            // 
            // lb_pedNumber
            // 
            this.lb_pedNumber.AutoSize = true;
            this.lb_pedNumber.Location = new System.Drawing.Point(185, 147);
            this.lb_pedNumber.Name = "lb_pedNumber";
            this.lb_pedNumber.Size = new System.Drawing.Size(59, 15);
            this.lb_pedNumber.TabIndex = 11;
            this.lb_pedNumber.Text = "보행자 수";
            // 
            // btn_simulationSettingConfirm
            // 
            this.btn_simulationSettingConfirm.Location = new System.Drawing.Point(199, 269);
            this.btn_simulationSettingConfirm.Name = "btn_simulationSettingConfirm";
            this.btn_simulationSettingConfirm.Size = new System.Drawing.Size(75, 23);
            this.btn_simulationSettingConfirm.TabIndex = 10;
            this.btn_simulationSettingConfirm.Text = "다음";
            this.btn_simulationSettingConfirm.UseVisualStyleBackColor = true;
            // 
            // tb_cctvNumber
            // 
            this.tb_cctvNumber.Location = new System.Drawing.Point(251, 115);
            this.tb_cctvNumber.Name = "tb_cctvNumber";
            this.tb_cctvNumber.Size = new System.Drawing.Size(100, 23);
            this.tb_cctvNumber.TabIndex = 9;
            // 
            // cb_cctvLocMode
            // 
            this.cb_cctvLocMode.FormattingEnabled = true;
            this.cb_cctvLocMode.Items.AddRange(new object[] {
            "CCTV as Grid",
            "CCTV at DST",
            "CCTV as Random"});
            this.cb_cctvLocMode.Location = new System.Drawing.Point(251, 90);
            this.cb_cctvLocMode.Name = "cb_cctvLocMode";
            this.cb_cctvLocMode.Size = new System.Drawing.Size(121, 23);
            this.cb_cctvLocMode.TabIndex = 8;
            // 
            // tb_simNum
            // 
            this.tb_simNum.Location = new System.Drawing.Point(251, 63);
            this.tb_simNum.Name = "tb_simNum";
            this.tb_simNum.Size = new System.Drawing.Size(100, 23);
            this.tb_simNum.TabIndex = 7;
            // 
            // tb_cctvSetNum
            // 
            this.tb_cctvSetNum.Location = new System.Drawing.Point(251, 35);
            this.tb_cctvSetNum.Name = "tb_cctvSetNum";
            this.tb_cctvSetNum.Size = new System.Drawing.Size(100, 23);
            this.tb_cctvSetNum.TabIndex = 6;
            // 
            // lb_cctvNumber
            // 
            this.lb_cctvNumber.AutoSize = true;
            this.lb_cctvNumber.Location = new System.Drawing.Point(181, 118);
            this.lb_cctvNumber.Name = "lb_cctvNumber";
            this.lb_cctvNumber.Size = new System.Drawing.Size(65, 15);
            this.lb_cctvNumber.TabIndex = 5;
            this.lb_cctvNumber.Text = "CCTV 대수";
            // 
            // lb_cctvLocMode
            // 
            this.lb_cctvLocMode.AutoSize = true;
            this.lb_cctvLocMode.Location = new System.Drawing.Point(153, 93);
            this.lb_cctvLocMode.Name = "lb_cctvLocMode";
            this.lb_cctvLocMode.Size = new System.Drawing.Size(93, 15);
            this.lb_cctvLocMode.TabIndex = 4;
            this.lb_cctvLocMode.Text = "CCTV 배치 유형";
            // 
            // lb_simNum
            // 
            this.lb_simNum.AutoSize = true;
            this.lb_simNum.Location = new System.Drawing.Point(105, 68);
            this.lb_simNum.Name = "lb_simNum";
            this.lb_simNum.Size = new System.Drawing.Size(141, 15);
            this.lb_simNum.TabIndex = 3;
            this.lb_simNum.Text = "CCTV당 시뮬레이션 횟수";
            // 
            // lb_cctvSetNum
            // 
            this.lb_cctvSetNum.AutoSize = true;
            this.lb_cctvSetNum.Location = new System.Drawing.Point(124, 43);
            this.lb_cctvSetNum.Name = "lb_cctvSetNum";
            this.lb_cctvSetNum.Size = new System.Drawing.Size(121, 15);
            this.lb_cctvSetNum.TabIndex = 2;
            this.lb_cctvSetNum.Text = "CCTV 배치 세트 개수";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.simOptionInfoPage);
            this.Controls.Add(this.gisInfoPage);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gisInfoPage.ResumeLayout(false);
            this.gisInfoPage.PerformLayout();
            this.simOptionInfoPage.ResumeLayout(false);
            this.simOptionInfoPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Panel gisInfoPage;
        private ComboBox cb_srsName;
        private Button btn_gisConfirm;
        private ComboBox cb_methodNM;
        private ComboBox cb_resultType;
        private Label lb_gisMethodName;
        private ComboBox cb_maxFeature;
        private Label lb_serviceKey;
        private TextBox tb_pnu;
        private ComboBox cb_typeName;
        private TextBox tb_bbox;
        private Label lb_srsName;
        private TextBox tb_serviceKey;
        private Label lb_resultType;
        private Label lb_typeName;
        private Label lb_bbox;
        private Label lb_pnu;
        private Label lb_maxFeature;
        private Panel simOptionInfoPage;
        private Label lb_cctvNumber;
        private Label lb_cctvLocMode;
        private Label lb_simNum;
        private Label lb_cctvSetNum;
        private TextBox tb_cctvNumber;
        private ComboBox cb_cctvLocMode;
        private TextBox tb_simNum;
        private TextBox tb_cctvSetNum;
        private Button btn_simulationSettingConfirm;
        private Label lb_pedNumber;
        private Label lb_carNumber;
        private TextBox tb_carNumber;
        private TextBox tb_pedNumber;
    }
}