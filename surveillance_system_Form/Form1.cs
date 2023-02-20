using static surveillance_system.Program;

namespace surveillance_system_form
{
    public partial class Form1 : Form
    {
        List<Panel> panelList = new List<Panel>();

        public Form1()
        {
            InitializeComponent();

            this.btn_gisConfirm.Click += gisConfirmClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelList.Add(gisInfoPage);
            panelList.Add(simOptionInfoPage);

            panelList[0].BringToFront();
        }

        private void gisConfirmClick(object sender, EventArgs e)
        {
            string message = string.Empty;

            message = string.Format(cb_methodNM.Text);
            gmfgbs.setGisMethodNameByGui(message);

            message = string.Format(tb_serviceKey.Text);
            gmfgbs.setServiceKeyByGui(message);
            
            message = string.Format(cb_typeName.Text);
            gmfgbs.setTypeNameByGui(message);

            message = string.Format(tb_bbox.Text);
            gmfgbs.setBBoxByGui(message);

            message = string.Format(tb_pnu.Text);
            gmfgbs.setPnuByGui(message);

            message = string.Format(cb_maxFeature.Text);
            gmfgbs.setMaxFeatureByGui(message);
            
            message = string.Format(cb_resultType.Text);
            gmfgbs.setResultTypeByGui(message);

            message = string.Format(cb_srsName.Text);
            gmfgbs.setSrsNameByGui(message);

            gbs.setEndPointUrl();
            gbs.loadBuildingDataFromApiAsXml();

            panelList[1].BringToFront();
        }
    }
}