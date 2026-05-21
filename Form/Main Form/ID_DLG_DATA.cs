using System;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FormData : Form
    {
        #region //Variable

        #endregion

        #region //Form Init
        public FormData()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            //////////////////////////////////////////////////////////////////////
            this.MouseClick += new MouseEventHandler(FormData_MouseClick);
        }

        private void FormData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (e.X < 1205 || e.Y > 60) return;

            string Pass = MSystem.GetPass();
            if (Pass == null || Pass != $"YJ{DateTime.Now:HHmm}") return;
            
        }
        private void LoadAllData(object sender, EventArgs e)
        {

        }
        #endregion

        private void BT_SYS_MANAGER_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonPress5_Click(object sender, EventArgs e)
        {
           
        }

        private void buttonPress4_Click(object sender, EventArgs e)
        {
            FormPortSetting dlg = new FormPortSetting();
            dlg.ShowDialog();
        }

        private void buttonPress3_Click(object sender, EventArgs e)
        {
            FormModel dlg = new FormModel();
            dlg.ShowDialog();
        }

        private void buttonPress9_Click(object sender, EventArgs e)
        {
            FormMotorVelocity dlg = new FormMotorVelocity();
            dlg.ShowDialog();
        }

        private void buttonPress8_Click(object sender, EventArgs e)
        {
            FormJogVelocity dlg = new FormJogVelocity();
            dlg.ShowDialog();
        }

        private void FormData_FormClosing(object sender, FormClosingEventArgs e)
        {
            // MSystem.m_iRSTPass_Timer._TimStop = false;
            //MSystem.m_iRSTPass_Timer.ResetTimer();
            // MSystem.m_iRSTPass_Timer.StartTimer();
        }

        private void FormData_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MSystem.m_iRSTPass_Timer.StartTimer();
        }
    }
}
