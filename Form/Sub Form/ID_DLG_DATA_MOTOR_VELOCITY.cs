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
    public partial class FormMotorVelocity : Form
    {
        #region // Form Init
        SUserControls.ColorButton[] m_btnVelocity = new SUserControls.ColorButton[(int)Axis.eAXIS_MAX];
        SUserControls.ColorButton[] m_btnAccelerate = new SUserControls.ColorButton[(int)Axis.eAXIS_MAX];
        public FormMotorVelocity()
        {
            int _index = 0;
            InitializeComponent();
            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);
            m_btnVelocity[_index++] = Axis0WorkSpeed;
            m_btnVelocity[_index++] = Axis1WorkSpeed;
            //m_btnVelocity[_index++] = Axis2WorkSpeed;
            //m_btnVelocity[_index++] = Axis3WorkSpeed;
            //m_btnVelocity[_index++] = Axis4WorkSpeed;
            //m_btnVelocity[_index++] = Axis5WorkSpeed;
            //m_btnVelocity[_index++] = Axis6WorkSpeed;
            //m_btnVelocity[_index++] = Axis7WorkSpeed;
            //m_btnVelocity[_index++] = Axis8WorkSpeed;
            //m_btnVelocity[_index++] = Axis9WorkSpeed;
            //m_btnVelocity[_index++] = Axis10WorkSpeed;
            //m_btnVelocity[_index++] = Axis11WorkSpeed;

            _index = 0;
            m_btnAccelerate[_index++] = Axis0AccSpeed;
            m_btnAccelerate[_index++] = Axis1AccSpeed;
            //m_btnAccelerate[_index++] = Axis2AccSpeed;
            //m_btnAccelerate[_index++] = Axis3AccSpeed;
            //m_btnAccelerate[_index++] = Axis4AccSpeed;
            //m_btnAccelerate[_index++] = Axis5AccSpeed;
            //m_btnAccelerate[_index++] = Axis6AccSpeed;
            //m_btnAccelerate[_index++] = Axis7AccSpeed;
            //m_btnAccelerate[_index++] = Axis8AccSpeed;
            //m_btnAccelerate[_index++] = Axis9AccSpeed;
            //m_btnAccelerate[_index++] = Axis10AccSpeed;
            //m_btnAccelerate[_index++] = Axis11AccSpeed;

        }
        private void FLoad(object sender, EventArgs e)
        {
            LoadUI();
        }
        #endregion
        private void PreLoadEvent(object sender, EventArgs e) {
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
            //else if ((sender as SUserControls.ColorButton).Name == "bt_speedAttach"
            //     || (sender as SUserControls.ColorButton).Name == "bt_SpeedVinyl")
            //{
            //    KeyboardNum getValue = new KeyboardNum();
            //    //
            //    getValue.TxCurrent.Text = (sender as SUserControls.ColorButton).Text;
            //    DialogResult Result = getValue.ShowDialog();
            //    if (Result == DialogResult.OK)
            //    {
            //        double TEMP;
            //        if (double.TryParse(getValue.TxValue.Text, out TEMP))
            //            (sender as SUserControls.ColorButton).Text = TEMP.ToString();//getValue.TxValue.Text;
            //    }
            //}
            //else 
            {
                KeyboardNum getValue = new KeyboardNum();
                //
                getValue.TxCurrent.Text = (sender as SUserControls.ColorButton).Text;
                DialogResult Result = getValue.ShowDialog();
                if (Result == DialogResult.OK)
                {
                    int TEMP;
                    if (int.TryParse(getValue.TxValue.Text, out TEMP))
                        (sender as SUserControls.ColorButton).Text = TEMP.ToString();//getValue.TxValue.Text;
                }
                //
                getValue.Dispose();
                getValue = null;
            }
            
        }
        private void SaveData()
        {
            if (MSystem.MyMsgMemo("Do you want saves all parameter?", "Save Data", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
            {
                Task.Run(async () => {
                    await Task.Delay(100);
                    for (int i = 0; i < (int)Axis.eAXIS_MAX; i++)
                    {
                        ServoParam.Instance.m_sAxisInfo[i].dAxisMaxSpeed = double.Parse(m_btnVelocity[i].Text.Trim());
                        ServoParam.Instance.m_sAxisInfo[i].dAxisAcc = double.Parse(m_btnAccelerate[i].Text.Trim());
                        if (MSystem.m_pAxisManager.m_pFastechAxis[i] != null) 
                        {
                            MSystem.SetMsgDisplay($"Save Data {ServoParam.Instance.m_sAxisInfo[i].AxisName} ...");
                            MSystem.m_pAxisManager.m_pFastechAxis[i].SetAxisInfo(ServoParam.Instance.m_sAxisInfo[i]);
                            MSystem.m_pAxisManager.m_pFastechAxis[i].SaveData();
                        }
                    }

                    ServoParam.Instance.SaveSettings();
                    MSystem.MyMessagerBottom("Change Parameter Servo Run Speed");
                    MSystem.KillMsgDisplay();
                    await Task.Delay(100);
                    MSystem.MyMsgMemo("Save Data OK", "Save Data", msgButton.OK, msgIcon.Infor);
                });
                MSystem.MyMsgDisplay("Wait a moment ... ");
            }
        }
        private void LoadUI()
        {
            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                m_btnVelocity[i].Text   = ServoParam.Instance.m_sAxisInfo[i].dAxisMaxSpeed.ToString();
                m_btnAccelerate[i].Text = ServoParam.Instance.m_sAxisInfo[i].dAxisAcc.ToString();
            }
        }

        private void colorButton3_ClickEvent(object sender, EventArgs e)
        {
            SaveData();
            return;
        }

        private void colorButton5_ClickEvent(object sender, EventArgs e)
        {
            this.Close();
            return;
        }
    }
}
