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
    public partial class FormJogVelocity : Form
    {
        #region//Form Init
        
        SUserControls.ColorButton[] m_btnLow = new SUserControls.ColorButton[(int)Axis.eAXIS_MAX];
        SUserControls.ColorButton[] m_btnMiddle = new SUserControls.ColorButton[(int)Axis.eAXIS_MAX];
        SUserControls.ColorButton[] m_btnHigh = new SUserControls.ColorButton[(int)Axis.eAXIS_MAX];
        public FormJogVelocity()
        {
            int _index = 0;
            InitializeComponent();

            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);

            m_btnLow[_index++] = Axis0SpeedLow;
            m_btnLow[_index++] = Axis1SpeedLow;
            //m_btnLow[_index++] = Axis2SpeedLow;
            //m_btnLow[_index++] = Axis3SpeedLow;
            //m_btnLow[_index++] = Axis4SpeedLow;
            //m_btnLow[_index++] = Axis5SpeedLow;
            //m_btnLow[_index++] = Axis6SpeedLow;
            //m_btnLow[_index++] = Axis7SpeedLow;

            _index = 0;
            m_btnMiddle[_index++] = Axis0SpeedMiddle;
            m_btnMiddle[_index++] = Axis1SpeedMiddle;
            //m_btnMiddle[_index++] = Axis2SpeedMiddle;
            //m_btnMiddle[_index++] = Axis3SpeedMiddle;
            //m_btnMiddle[_index++] = Axis4SpeedMiddle;
            //m_btnMiddle[_index++] = Axis5SpeedMiddle;
            //m_btnMiddle[_index++] = Axis6SpeedMiddle;
            //m_btnMiddle[_index++] = Axis7SpeedMiddle;

            _index = 0;
            m_btnHigh[_index++] = Axis0SpeedHight;
            m_btnHigh[_index++] = Axis1SpeedHight;
            //m_btnHigh[_index++] = Axis2SpeedHight;
            //m_btnHigh[_index++] = Axis3SpeedHight;
            //m_btnHigh[_index++] = Axis4SpeedHight;
            //m_btnHigh[_index++] = Axis5SpeedHight;
            //m_btnHigh[_index++] = Axis6SpeedHight;
            //m_btnHigh[_index++] = Axis7SpeedHight;
        }
        private void FLoad(object sender, EventArgs e)
        {
            LoadUI();
        }
        #endregion

        private void PreLoadEvent(object sender, EventArgs e)
        {
           // if ((sender as SUserControls.ColorButton) == null) return;
            //if ((sender as SUserControls.ColorButton).Text == "Exit")
            //{
            //    this.Close();
            //    return;
            //}
            //else if ((sender as SUserControls.ColorButton).Text == "Save")
            //{
            //    SaveData();
            //    return;
            //}
            KeyboardNum getValue = new KeyboardNum();
            
            getValue.TxCurrent.Text = (sender as SUserControls.ColorButton).Text;
            DialogResult Result = getValue.ShowDialog();
            if (Result == DialogResult.OK){   
                int TEMP;
                if (int.TryParse(getValue.TxValue.Text, out TEMP))
                    (sender as SUserControls.ColorButton).Text = TEMP.ToString();//   getValue.TxValue.Text;
            }
            getValue.Dispose();
            getValue = null;
        }
        private void SaveData() {
            if (MSystem.MyMsgMemo("Do you want saves all parameter?","Save Data", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(100);

                    for (int i = 0; i < (int)Axis.eAXIS_MAX; i++)
                    {
                        ServoParam.Instance.m_sAxisInfo[i].dJogSlowSpeed    = double.Parse(m_btnLow[i].Text.Trim());
                        ServoParam.Instance.m_sAxisInfo[i].dJogMidSpeed     = double.Parse(m_btnMiddle[i].Text.Trim());
                        ServoParam.Instance.m_sAxisInfo[i].dJogFastSpeed    = double.Parse(m_btnHigh[i].Text.Trim());

                        if (MSystem.m_pAxisManager.m_pFastechAxis[i] != null && !MSystem.SIMULATION)
                        {
                            MSystem.SetMsgDisplay($"Save Data {ServoParam.Instance.m_sAxisInfo[i].AxisName} ...");
                            MSystem.m_pAxisManager.m_pFastechAxis[i].SetAxisInfo(ServoParam.Instance.m_sAxisInfo[i]);
                            MSystem.m_pAxisManager.m_pFastechAxis[i].SaveData();
                        }
                    }

                    ServoParam.Instance.SaveSettings();
                    MSystem.MyMessagerBottom("Change Parameter Servo Jog Speed");
                    MSystem.KillMsgDisplay();
                    await Task.Delay(100);
                    MSystem.MyMsgMemo("Save Data OK", "Save Data", msgButton.OK, msgIcon.Infor);
                    
                });
                MSystem.MyMsgDisplay("Wait a moment...");
            }
        
        }
        private void LoadUI() {
            //if (MSystem.SIMULATION) return;
            for (int i = 0; i < (int)Axis.eAXIS_MAX; i++)
            {
                m_btnLow[i].Text = ServoParam.Instance.m_sAxisInfo[i].dJogSlowSpeed.ToString();
                m_btnMiddle[i].Text = ServoParam.Instance.m_sAxisInfo[i].dJogMidSpeed.ToString();
                m_btnHigh[i].Text = ServoParam.Instance.m_sAxisInfo[i].dJogFastSpeed.ToString();
            }
        }

        private void BtExit_ClickEvent(object sender, EventArgs e)
        {
            this.Close();
            return;
        }

        private void BtSave_ClickEvent(object sender, EventArgs e)
        {
            SaveData();
            return;
        }
    }
}
