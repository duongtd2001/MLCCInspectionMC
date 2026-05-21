using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FormPortSetting : Form
    {
        public FormPortSetting()
        {
            InitializeComponent();
            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + ((MSystem.Resolution_X - this.Width) / 2)
                                    , FormMain.GetLocation().Y + ((MSystem.Resolution_Y - this.Height) / 2));
            LoadUI();
        }
        public void LoadUI()
        {
            //TB_JOG_COM.Text         = InforManager.Instance.JogPort;
            //TB_BCR_COM.Text   = InforManager.Instance.BCR_Port;
            //TB_GMES_COM.Text      = InforManager.Instance.GMESPort;
            //BT_INTERFACE_NEXT_MC.Text = InforManager.Instance.Interface_NextMCPort;
        }

        void PreLoadEvent(object sender, EventArgs e)
        {
            //float _fValue = 0;
            int _IValue = 0;
            switch ((sender as SUserControls.ColorButton).Name)
            {
                case "BtSave":
                    SaveDataUI();
                    InforManager.Instance.SaveData();
                    MSystem.MyMsgMemo("Save Data Complete", "Save Data", msgButton.OK, msgIcon.Infor);
                    this.Close();
                    break;
                case "BtExit":
                    this.Close();
                    break;
                case "TB_BCR_COM":
                case "TB_GMES_COM":
                case "TB_JOG_COM":
                case "BT_INTERFACE_NEXT_MC":
                    if (MSystem.GetIn(sender, ref _IValue))
                    {
                        (sender as SUserControls.ColorButton).Text = $"COM{_IValue}";
                    }
                    break;
                default: break;
            }
        }
        private void SaveDataUI() 
        {
            //comport Data
            //InforManager.Instance.JogPort                   = TB_JOG_COM.Text;
            //InforManager.Instance.BCR_Port                  = TB_BCR_COM.Text;
            //InforManager.Instance.GMESPort                  = TB_GMES_COM.Text;
            //InforManager.Instance.Interface_NextMCPort      = BT_INTERFACE_NEXT_MC.Text;
        }

        private void BtExit_ClickEvent(object sender, EventArgs e)
        {
            this.Close();
            return;
        }

        private void BtSave_ClickEvent(object sender, EventArgs e)
        {
            SaveDataUI();
            InforManager.Instance.SaveData();
            MSystem.MyMsgMemo("Save Data Complete", "Save Data", msgButton.OK, msgIcon.Infor);
            //this.Close();
            return;
        }
    }
}
