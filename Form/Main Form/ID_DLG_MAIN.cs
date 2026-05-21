using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Resources;
using System.Threading;

namespace MLCCInspectionMC
{
    public partial class FormMain : Form
    {
        #region //Variable
        //System.Windows.Forms.Timer TimerUpdate = new System.Windows.Forms.Timer();
        //Timer TimerUpdate = new Timer();
        Thread TimerUpdate;
        TimerDelay _InitTime = new TimerDelay();
        System.Windows.Forms.Timer TimerOrigin = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer TimeCheckSimu = new System.Windows.Forms.Timer();
        #endregion
        public static Point P_Pos = new Point();
        #region //Form Initial
        public FormMain()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            /*Init all system mode*/
           
            MSystem.SysMode = ModeRun.Auto;
            MSystem.SysStatus = StatusRun.STOP;
            
            /*Init Servo Board IO*/
            MSystem.m_pAxisManager.Initial();
            MSystem.m_pDatabaseLog.CreateTB();
            MSystem.m_pDatabaseProduct.CreateTB_Product();

            MSystem.RunAllThread();

            if(MSystem.SIMULATION)
                MessageBox.Show("RUN SIMULATION","Start Program",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            /******************************************/
            _InitTime.StartTimer();

            _InitTime.StartTimer();

            MSystem.m_pDIO.m_iIOConnection = false;

            MSystem.m_pDIO.CheckSlaveConnect();

            TimerUpdate = new Thread(UpdateStatus);
            TimerUpdate.IsBackground = true;
            TimerUpdate.Start();

            TimerOrigin.Tick += new EventHandler(Originfirst);

            this.FormClosing += new FormClosingEventHandler(Formclose);

            TimeCheckSimu.Tick += new EventHandler(CheckSIM);
            TimeCheckSimu.Interval = 1;
            TimeCheckSimu.Start();
        }
        public static Point GetLocation()
        {
            Point T = new Point(P_Pos.X, P_Pos.Y);
            return T;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //Initial Form
            // MSystem.
            MSystem.InitialMainForm(PanelMain);
            MSystem.DisplayMain(PanelMain, MSystem.NumDisplay);
            MSystem.DisplayForm(PanelTitle, MSystem.TitleForm);
            MSystem.DisplayForm(PanelButton, MSystem.BottomForm);

            //MSystem.m_pLogSave.DevLogSave($"Start Program .......");

            MSystem.BottomForm.SetImgSelect((int)NumViewMain.eViewAuto);
            
        }
        private void Formclose(object sender, EventArgs e)
        {
            MSystem.m_bLife = false;
            //MSystem.m_pLogSave.DevLogSave($"Exit program");
        }
        #endregion

        #region //Update Status
        //private void UpdateStatus(object Sender, EventArgs e)
        private void UpdateStatus()
        {
            while (MSystem.m_bLife)
            {
                Thread.Sleep(100);
                try
                {
                   
                    if (MSystem.NumDisplay != 0 && MSystem.NumDisplay != MSystem.NumDisplayOld)
                    {
                        if (MSystem.NumDisplay != 5)
                        {
                            MSystem.NumDisplayOld = MSystem.NumDisplay;
                        }

                        if (MSystem.NumDisplay == 6)
                        {
                            Environment.Exit(0);
                        }
                        else if (MSystem.NumDisplay == 5)
                        {
                            MSystem.NumDisplay = 0;
                            this.WindowState = FormWindowState.Minimized;
                        }
                        else
                            MSystem.DisplayMain(PanelMain, MSystem.NumDisplay);
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }

        private void CheckSIM(object Sender, EventArgs e)
        {
            TimeCheckSimu.Stop();
            if (MSystem.SIMULATION) 
            {
                //MSystem.MyMsgMemo("RUN SIMULATION", "Memo", msgButton.OK, msgIcon.Infor);
            }
            TimerOrigin.Interval = 1000;
            TimerOrigin.Enabled = true;
            TimerOrigin.Start();
        }
        private void Originfirst(object Sender, EventArgs e)
        {
            FormOrigin formOrigin = new FormOrigin();
            if (MSystem.SysStatus == StatusRun.ERROR)
            {
                TimerOrigin.Stop();
                return;
            }
            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                formOrigin.m_BtAxis[i].SetValue(CBbutton2.SelectColor);
            }
            TimerOrigin.Stop();
            if (MSystem.SysStatus == StatusRun.ERROR) return;
            
            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                formOrigin.m_BtAxis[i].SetValue(CBbutton2.NormalColor);
            }
            formOrigin.ShowDialog();
            MSystem.m_bIsTeachDlg = false;
        }
        #endregion
    }

}
