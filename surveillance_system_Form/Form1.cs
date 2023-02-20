using surveillance_system_Form;
using System.ComponentModel;
using static surveillance_system.Program;

namespace surveillance_system_form
{
    public partial class Form1 : Form
    {
        private List<Panel> panelList = new List<Panel>();

        public Form1()
        {
            InitializeComponent();

            this.btn_gisConfirm.Click += gisConfirmClick;
            this.btn_simulationSettingConfirm.Click += btn_simulationSettingConfirm_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelList.Add(gisInfoPage);
            panelList.Add(simOptionInfoPage);

            panelList[0].BringToFront();
        }

        private void gisConfirmClick(object sender, EventArgs e)
        {
            string methodNm = string.Empty,
                serviceKye = string.Empty,
                typeNm = string.Empty,
                bbox = string.Empty,
                pnu = string.Empty,
                maxFeature = string.Empty,
                resultType = string.Empty,
                srsNm = string.Empty;

            methodNm = string.Format(cb_methodNM.Text);
            serviceKye = string.Format(tb_serviceKey.Text);
            typeNm = string.Format(cb_typeName.Text);
            bbox = string.Format(tb_bbox.Text);
            pnu = string.Format(tb_pnu.Text);
            maxFeature = string.Format(cb_maxFeature.Text);
            resultType = string.Format(cb_resultType.Text);
            srsNm = string.Format(cb_srsName.Text);

            gm.setEndPointUrlByGui(methodNm, serviceKye, typeNm, bbox, pnu, maxFeature, resultType, srsNm);
            gm.loadBuildingDataFromApiAsXmlByGui();

            panelList[1].BringToFront();
        }

        private void btn_simulationSettingConfirm_Click(object sender, EventArgs e)
        {
            string numberOfCCTVSet = string.Empty,
                simulationTimesForCCTVSet = string.Empty,
                cctvArrangementMode = string.Empty,
                N_Cctv = string.Empty,
                N_Ped = string.Empty,
                N_Car = string.Empty;

            numberOfCCTVSet = string.Format(tb_cctvSetNum.Text);
            simulationTimesForCCTVSet = string.Format(tb_simNum.Text);
            cctvArrangementMode = string.Format(cb_cctvLocMode.Text);
            N_Cctv = string.Format(tb_cctvNumber.Text);
            N_Ped = string.Format(tb_pedNumber.Text);
            N_Car = string.Format(tb_carNumber.Text);

            gm.setSimulationSettingByGui(numberOfCCTVSet, simulationTimesForCCTVSet, cctvArrangementMode, N_Cctv, N_Ped, N_Car);
            
            startSimulation modal = new startSimulation();
            modal.ShowDialog();
        }
    }
}