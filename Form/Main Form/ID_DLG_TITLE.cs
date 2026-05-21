using System;
using System.Threading;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FormTitle : Form
    {
        #region//Variable Form
        //Timer timerUpdate = new Timer();
        //System.Windows.Forms.Timer timerUpdate = new System.Windows.Forms.Timer();
        Thread timerUpdate;
        #endregion

        #region //Form Initial
        public FormTitle()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            MSystem.LoadParameter();

            this.BtGetStepJob.Click += new System.EventHandler(BtGetStepJobMain);
            this.Closed += new EventHandler(CloseAllData);
            timerUpdate = new Thread(UpdateStatus);
            timerUpdate.IsBackground = true;
            timerUpdate.Start();
            //BtGetStepJob.Text = MSystem.PROGRAM_NAME;
        }
        private void CloseAllData(object sender, EventArgs e)
        {
            timerUpdate = null;
        }
        #endregion

        #region //Event Form
        private void BtGetStepJobMain(object sender, EventArgs e)
        {

        }
        #endregion

        #region //Update Status
        private void UpdateStatus()
        {
            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    // MSystem.AutoForm.BT_SCORE.Text = $"{MSystem.m_pDIO.countIF}";
                    MSystem.m_pDIO.countIF = 0;
                    MSystem.m_pDIO._TimScore.StartTimer();

                    MSystem.DisPlayString(LB_MODEL, InforManager.Instance.ModelName);
                    MSystem.DisPlayString(Txt_DateTime, DateTime.Now.ToString("yyyy-MM-dd") + ", " + DateTime.Now.ToString("HH:mm:ss"));

                    BtGetStepJob.Text = MSystem.PROGRAM_NAME;
                    MSystem.DisPlayString(Txt_Ver, MSystem.PROGRAM_VER);


                }
                catch (Exception)
                {
                    MessageBox.Show("Title Form Error Logic");
                }
            }

        }

        #endregion

        private void IDC_THREAD_STEP_Click(object sender, EventArgs e)
        {
            if (!Visible) return;
            this.Invoke((MethodInvoker)delegate
            {
                FormUnitStep dlgUnitStep = new FormUnitStep();
                dlgUnitStep.ShowDialog();
                dlgUnitStep = null;
            });

        }
    }
}
