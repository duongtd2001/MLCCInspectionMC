using FASTECH;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FormOrigin : Form
    {
        public CBbutton2[] m_BtAxis = new CBbutton2[(int)Axis.eAXIS_MAX];
        Thread _Timupdate;
        int m_iOriginError = 0;
        int _index = 0;
        int Axis_Count = 0;
        bool UpdateDis = false;
        bool UpdateAlarmFlag = true;
        bool[] flagLimit = new bool[] { false, false};
        #region//Form Initial
        public FormOrigin()
        {
            InitializeComponent();

            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);

            //convert type button
            m_BtAxis[_index++] = new CBbutton2(BT_AXIS_0, false);
            m_BtAxis[_index++] = new CBbutton2(BT_AXIS_1, false);

            for (int i = 0; i < (int)Axis.eAXIS_MAX; i++)
            {
                m_BtAxis[i].SetValue(CBbutton2.NormalColor);
                m_BtAxis[i].Button.ClickEvent += new EventHandler(PreLoadEvent);
            }

            //CMD button
            ORG_BT_SELECT_ALL.ClickEvent += new EventHandler(PreLoadEvent);
            ORG_BT_CANCLE.ClickEvent += new EventHandler(PreLoadEvent);
            ORG_BT_SERVO_ON.ClickEvent += new EventHandler(PreLoadEvent);
            ORG_BT_SERVO_OFF.ClickEvent += new EventHandler(PreLoadEvent);
            ORG_BT_ORIGIN.ClickEvent += new EventHandler(PreLoadEvent);
            ORG_BT_AL_RESET.ClickEvent += new EventHandler(PreLoadEvent);

            this.FormClosed += new FormClosedEventHandler(CloseForm);
            MSystem.m_bIsTeachDlg = true;
            MSystem.m_bOrigin = true;
            UpdateDis = false;
            UpdateAlarmFlag = true;

            _Timupdate = new Thread(UpdateStatus);
            _Timupdate.IsBackground = true;
            _Timupdate.Start();
        }
        ~FormOrigin()
        {
            MSystem.m_bIsTeachDlg = false;
            MSystem.m_bOrigin = false;
        }
        private void CloseForm(object sender, EventArgs e)
        {
            MSystem.m_bIsTeachDlg = false;
            MSystem.m_bOrigin = false;
            UpdateAlarmFlag = false;
            _Timupdate.Join();
        }
        private void FormOrigin_FormClosed(object sender, FormClosedEventArgs e)
        {
            MSystem.m_bOrigin = false;
        }
        private void UpdateStatus()
        {
            while (UpdateAlarmFlag)
            {
                Thread.Sleep(10);
                try
                {
                    if (!UpdateDis)
                        UpdateAlarm();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }
        #endregion

        #region //Event Form
        private void PreLoadEvent(object sender, EventArgs e)
        {
            switch ((sender as AxBTNENHLib4.AxBtnEnh).Name.ToString())
            {
                case "ORG_BT_SELECT_ALL":

                    for (int i = 0; i < MSystem.eAXIS_MAX; i++)
                    {
                        m_BtAxis[i].SetValue(CBbutton2.SelectColor);
                    }
                    break;
                case "ORG_BT_CANCLE":
                    for (int i = 0; i < MSystem.eAXIS_MAX; i++)
                    {
                        m_BtAxis[i].SetValue(CBbutton2.NormalColor);
                    }
                    break;
                case "ORG_BT_SERVO_ON":
                    ServorOn();
                    break;
                case "ORG_BT_SERVO_OFF":
                    ServorOFF();
                    break;
                case "ORG_BT_ORIGIN":
                    flagLimit[0] = flagLimit[1] = false;
                    ServoOrigin();
                    break;
                case "ORG_BT_AL_RESET":
                    ServoReset();
                    break;
                case "BT_STOP_ORIGIN":
                    StopOrigin();
                    break;
                case "BT_AXIS_0":
                    if (!m_BtAxis[0].m_iSel) m_BtAxis[0].SetValue(CBbutton2.SelectColor);
                    else m_BtAxis[0].SetValue(CBbutton2.NormalColor);
                    UpdateDis = true;
                    break;
                case "BT_AXIS_1":
                    if (!m_BtAxis[1].m_iSel) m_BtAxis[1].SetValue(CBbutton2.SelectColor);
                    else m_BtAxis[1].SetValue(CBbutton2.NormalColor);
                    UpdateDis = true;
                    break;

                default: break;
            }
        }
        
        void ServorOn()
        {
            // SERVO ON
            bool bSelectFlag = false;
            UpdateDis = false;
            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                if (m_BtAxis[i].m_iSel)
                    bSelectFlag = true;
            }

            if (!bSelectFlag)
            {
                MSystem.MyMsgMemo("No Select.", "Select Servo", msgButton.OK, msgIcon.Infor);
                return;
            }

            DialogResult dlg = MSystem.MyMsgMemo("Do you want to ON Servo?", "Servo On", msgButton.YESNO, msgIcon.Question);
            if (dlg != DialogResult.Yes)
            {
                return;
            }

            Task.Run((Func<Task>)(async () =>
            {
                await Task.Delay(100);
                for (int i = 0; i < MSystem.eAXIS_MAX; i++)
                {
                    if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null)
                        continue;

                    if (m_BtAxis[i].m_iSel)
                    {
                        // MSystem.SetMsgDisplay($"Servo On {MSystem.m_pAxisManager.m_pFastechAxis[i].m_sAxisInfo.AxisName} ... ");
                        MSystem.SetMsgDisplay($"Servo On {((Axis)i).ToString()} ... ");
                        if (MSystem.m_pAxisManager.m_pFastechAxis[i].SetAmpEnable(true) != 0)
                        {
                            MSystem.KillMsgDisplay();
                            MSystem.MyMsgMemo("Servo On Fail!", "Servo Error", msgButton.OK, msgIcon.Error);
                            return;
                        }
                        m_BtAxis[i].SetValue(CBbutton2.NormalColor);
                    }
                    await Task.Delay(300);
                }
                MSystem.KillMsgDisplay();
                await Task.Delay(10);
            }));
            MSystem.MyMsgDisplay("Servo On ....");
        }
        void ServorOFF()
        {
            bool bSelectFlag = false;
            UpdateDis = false;
            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                if (m_BtAxis[i].m_iSel)
                    bSelectFlag = true;
            }

            if (!bSelectFlag)
            {
                MSystem.MyMsgMemo("No Select.", "Select Servo", msgButton.OK, msgIcon.Infor);
                return;
            }

            if (MSystem.MyMsgMemo("Do you want to OFF Servo?", "Servo OFF", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            Task.Run((Func<Task>)(async () =>
            {
                await Task.Delay(100);
                for (int i = 0; i < MSystem.eAXIS_MAX; i++)
                {
                    if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null)
                    {
                        continue;
                    }
                    if (m_BtAxis[i].m_iSel)
                    {
                        //MSystem.SetMsgDisplay($"Servo OFF {((Axis)i).ToString()} ... ");
                        MSystem.SetMsgDisplay($"Servo OFF {MSystem.m_pAxisManager.m_pFastechAxis[i].m_sAxisInfo.AxisName} ... ");
                        if (MSystem.m_pAxisManager.m_pFastechAxis[i].SetAmpEnable(false) != 0)
                        {
                            MSystem.KillMsgDisplay();
                            MSystem.MyMsgMemo("Servo OFF Fail!", "Servo Error", msgButton.OK, msgIcon.Error);
                            return;
                        }
                        MSystem.m_pAxisManager.m_pFastechAxis[i].ClearAxis();
                        MSystem.m_pAxisManager.m_pFastechAxis[i].ResetOrigin();
                        m_BtAxis[i].SetValue(CBbutton2.NormalColor);
                    }
                    await Task.Delay(300);
                }
                MSystem.KillMsgDisplay();
            }));
            MSystem.MyMsgDisplay("Servo OFF ....");
        }
        void ServoReset()
        {
            // SERVO ON
            bool bSelectFlag = false;
            UpdateDis = false;
            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                if (m_BtAxis[i].m_iSel || m_BtAxis[i].GetColor == CBbutton2.ErrorColor)
                    bSelectFlag = true;
            }

            if (!bSelectFlag)
            {
                MSystem.MyMsgMemo("No Select", "Select Servo", msgButton.OK, msgIcon.Question);
                return;
            }

            if (MSystem.MyMsgMemo("Do you want to Reset Servo?", "Reset Servo", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            Task.Run((Func<Task>)(async () => {
                await Task.Delay(300);
                for (int i = 0; i < MSystem.eAXIS_MAX; i++)
                {
                    if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null)
                        continue;
                    if (m_BtAxis[i].m_iSel || m_BtAxis[i].GetColor == CBbutton2.ErrorColor)
                    {
                        //MSystem.SetMsgDisplay($"Servo Reset {MSystem.m_pAxisManager.m_pFastechAxis[i].m_sAxisInfo.AxisName} ... ");
                        MSystem.SetMsgDisplay($"Servo Reset {((Axis)i).ToString()} ... ");
                        if (MSystem.m_pAxisManager.m_pFastechAxis[i].ClearAxis() != 0)
                        {
                            MSystem.MyMsgMemo("Reset Servo Error !", "Servo Error", msgButton.YESNO, msgIcon.Question);
                            MSystem.KillMsgDisplay();
                            return;
                        }
                        MSystem.m_pAxisManager.m_pFastechAxis[i].ClearAxis();
                        MSystem.m_pAxisManager.m_pFastechAxis[i].ResetOrigin();
                        m_BtAxis[i].SetValue(CBbutton2.NormalColor);
                    }
                    await Task.Delay(300);
                }
                MSystem.KillMsgDisplay();
            }));
            MSystem.MyMsgDisplay("Servo Start Reset ...");
        }
        public void ServoOrigin()
        {
            bool bSelectFlag = false;

            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                if (m_BtAxis[i].m_iSel)
                    bSelectFlag = true;
            }
            if (!bSelectFlag)
            {
                MSystem.MyMsgMemo("No Select Axis", "Select Servo", msgButton.OK, msgIcon.Question);
                return;
            }
            if (MSystem.MyMsgMemo("Are you want Origins Axis Selected?", "Origin Axis", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (!ReturnOrigin())
            {
                MSystem.MyMsgMemo("Origin Error", "Origin", msgButton.OK, msgIcon.Error);
                return;
            }

            MSystem.MyMsgMemo("Origin Done! Ready for run", "Origin", msgButton.OK, msgIcon.Infor);

        }
        private void BtExit_Click(object sender, EventArgs e)
        {
            UpdateDis = false; ;
            MSystem.m_bIsTeachDlg = false;
            this.Close();
        }
        #endregion

        #region Return Origin 
        
        public bool ReturnOrigin()
        {
            TimerDelay _timeWait = new TimerDelay();
            bool needExits = false;
            int iError = 0;
            int iComplete = 0;
            bool[] bAxisSel = new bool[MSystem.eAXIS_MAX];
            bool[] bMove = new bool[MSystem.eAXIS_MAX];

            m_iOriginError = -1;
            bool ResultOrigin = true;
            if (!MSystem.waitServoInitOk())
            {
                ResultOrigin = false;
                return ResultOrigin;
            }
            MSystem.m_bInitWorkFlag = true;
            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                if (!m_BtAxis[i].m_iSel)
                {
                    MSystem.m_bInitWorkFlag = false;
                    continue;
                }
                bAxisSel[i] = true;
            }
            var resultRun = Task.Run(async () =>
            {
                await Task.Delay(1);
                /*Clear and reset origin*/
                for (int k = 0; k < MSystem.eAXIS_MAX; k++)
                {
                    int i = k;

                    if (!m_BtAxis[i].m_iSel)
                        continue;
                    if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null)
                    {
                        ResultOrigin = false;
                        break;
                    }
                    MSystem.m_pAxisManager.m_pFastechAxis[i].ClearAxis();
                    MSystem.m_pAxisManager.m_pFastechAxis[i].ResetOrigin();

                    bMove[i] = true;
                    bAxisSel[i] = true;
                }

                int[] iPrior = new int[MSystem.eAXIS_MAX];
                int[] iStep = new int[MSystem.eAXIS_MAX];
                bool[] bDone = new bool[MSystem.eAXIS_MAX];
                for (int i = 0; i < MSystem.eAXIS_MAX; i++)
                {
                    iPrior[i] = iStep[i] = 0;
                    bDone[i] = false;
                }
                //Start Origin and check Prior
                for (int iPriority = 0; iPriority < 5; iPriority++)
                {
                    if (needExits)
                    {
                        ResultOrigin = false;
                        break;
                    }

                    while (true)
                    {
                        if (needExits)
                        {
                            ResultOrigin = false;
                            break;
                        }
                        for (int k = 0; k < MSystem.eAXIS_MAX; k++)
                        {
                            int i = k;
                            if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null)
                            {
                                MSystem.KillMsgDisplay();
                                MSystem.MyMsgMemo($"Connect to FASTECH Port Fail!", "Error", msgButton.OK, msgIcon.Error);
                                ResultOrigin = false;
                                needExits = true;
                                break;
                            }
                            iPrior[i] = MSystem.m_pAxisManager.m_pFastechAxis[i].GetOriginPriority();
                            iStep[i] = MSystem.m_pAxisManager.m_pFastechAxis[i].GetOriginStep();

                            if (m_BtAxis[i].m_iSel
                                && (iPrior[i] == iPriority)
                                && (iStep[i] != 2000))
                            {
                                if (Check_EStop())
                                {
                                    MSystem.KillMsgDisplay();
                                    ResultOrigin = false;
                                    break;
                                }
                                if (Check_Stop())
                                {
                                    MSystem.KillMsgDisplay();
                                    ResultOrigin = false;
                                    break;
                                }
                                // CHECK AMP FAULT------------------------------------------------------------------------------------------
                                if (Check_AmpFault(i))
                                {
                                    MSystem.KillMsgDisplay();
                                    ResultOrigin = false;
                                    break;
                                }

                                MSystem.SetMsgDisplay($" {((Axis)i).ToString()} is executing the Motor Origin...\n Wait a moment");
                                //flagLimit[i] = MSystem.m_pAxisManager.m_pFastechAxis[i].IsMinusLimit();
                                if (MSystem.m_pAxisManager.m_pFastechAxis[i].IsMinusLimit())
                                {
                                    if (i == 0)
                                    {
                                        MSystem.m_pAxisManager.m_pFastechAxis[i].JogMoveSlow(1);
                                        Thread.Sleep(1000);
                                        MSystem.SetAllStop();
                                        MSystem.m_pAxisManager.m_pFastechAxis[i].m_iOriginStep = 100;
                                    }
                                    else
                                    {
                                        MSystem.m_pAxisManager.m_pFastechAxis[i].JogMoveSlow(1);
                                        Thread.Sleep(3000);
                                        MSystem.SetAllStop();
                                        MSystem.m_pAxisManager.m_pFastechAxis[i].m_iOriginStep = 100;
                                    }
                                }
                                MSystem.m_pAxisManager.m_pFastechAxis[i].OriginReturn(bMove[i]);

                                iStep[i] = MSystem.m_pAxisManager.m_pFastechAxis[i].GetOriginStep();


                                if (999 == iStep[i])
                                {
                                    for (int f = 0; f < MSystem.eAXIS_MAX; f++)
                                    {
                                        int j = f;

                                        if (m_BtAxis[j].m_iSel)
                                        {
                                            MSystem.m_pAxisManager.m_pFastechAxis[i].Stop();
                                        }
                                    }
                                    MSystem.m_pAxisManager.m_pFastechAxis[i].GetOriginError(ref iError);

                                    MSystem.SetMsgDisplay($"Origin Fail!!! {((Axis)i).ToString()},0x {iError.ToString("X8")} ");
                                    Thread.Sleep(3000);
                                    MSystem.KillMsgDisplay();
                                    ResultOrigin = false;
                                    break;
                                }
                            }

                            //Reset Status button
                            if (iStep[i] == 2000)
                            {
                                if (m_BtAxis[k].m_iSel && bDone[i] == false)
                                {
                                    bDone[i] = true;
                                    OnOriginDone(i);
                                    break;
                                }
                            }
                            await Task.Delay(50);
                        }//For End

                        if ((iComplete = IsOriginComplete(iPriority)) > 0)
                        {
                            if (iComplete == 0)
                            {
                                MSystem.KillMsgDisplay();
                                ResultOrigin = false;
                                break;
                            }

                            break;
                        }
                        if (!ResultOrigin) break;
                    }
                }
                MSystem.KillMsgDisplay();
            });

            MSystem.MyMsgDisplay("Start Origin ......");

            MSystem.KillMsgDisplay();
            if (MSystem.SIMULATION) MSystem.SetAllStop();

            if (MSystem.m_bInitWorkFlag && ResultOrigin)
            {
                MSystem.m_bInitWorkFlag = false;
            }
            return ResultOrigin;
        }

        void StopOrigin(bool bReset = true)
        {
            for (int k = 0; k < MSystem.eAXIS_MAX; k++)
            {
                if (MSystem.m_pAxisManager.m_pFastechAxis[k] == null)
                    continue;
                int j = k;
                if (m_BtAxis[j].m_iSel)
                {
                    MSystem.m_pAxisManager.m_pFastechAxis[j].Stop();
                    if (bReset)
                    {
                        MSystem.m_pAxisManager.m_pFastechAxis[j].ResetOrigin();
                    }
                }
            }
        }
        public bool Check_AmpFault(int i)
        {
            string str;

            if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null)
                return true;

            bool bState = false;
            bState = MSystem.m_pAxisManager.m_pFastechAxis[i].GetAmpFault();

            int iStep = MSystem.m_pAxisManager.m_pFastechAxis[i].GetOriginStep();

            if (bState && iStep >= 220)
            {
                StopOrigin(false);
                m_iOriginError = i;
                str = ((Axis)i).ToString();
                MSystem.MyMsgMemo($"AmpFault status while Origin. {str}", "Origin Error", msgButton.OK, msgIcon.Error);

                return true;
            }

            return false;
        }
        void OnOriginDone(int iAxis)
        {
            if (MSystem.m_pAxisManager.m_pFastechAxis[iAxis] == null)
                return;

            m_BtAxis[iAxis].SetValue(CBbutton2.NormalColor);
        }
        public int IsOriginComplete(int nPriority)
        {
            int[] iStep = new int[MSystem.eAXIS_MAX];
            int[] iPrior = new int[MSystem.eAXIS_MAX];

            for (int k = 0; k < MSystem.eAXIS_MAX; k++)
            {
                if (MSystem.m_pAxisManager.m_pFastechAxis[k] == null)
                    continue;
                iPrior[k] = MSystem.m_pAxisManager.m_pFastechAxis[k].GetOriginPriority();
                if (m_BtAxis[k].m_iSel //
                    && nPriority == iPrior[k])
                {
                    iStep[k] = MSystem.m_pAxisManager.m_pFastechAxis[k].GetOriginStep();
                    if (2000 != iStep[k])   //2000
                    {
                        return 0;
                    }
                }
            }
            return 1;
        }
        void UpdateAlarm()
        {
            bool bState = false;

            for (int i = 0; i < MSystem.eAXIS_MAX; i++)
            {
                if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null)
                    continue;
                bState = MSystem.m_pAxisManager.m_pFastechAxis[i].GetAmpFault();

                // SERVO ALARM
                if (bState && !m_BtAxis[i].m_iSel)
                {
                    m_BtAxis[i].SetValue(CBbutton.ErrorColor); //RED
                    continue;
                }

                //ORIGIN DONE
                if (MSystem.m_pAxisManager.m_pFastechAxis[i].IsOrigin()
                    && MSystem.m_pAxisManager.m_pFastechAxis[i].IsAxisDone() && !m_BtAxis[i].m_iSel)
                {
                    m_BtAxis[i].SetValue(CBbutton.StatusOK); //GREEN
                }
            }
        }

        private void IDC_INIT_ALL_UNIT_Click(object sender, EventArgs e)
        {
            Thread varInit = new Thread(() =>
            {
                if (MSystem.MyMsgMemo("Are You Want Init All Unit ?", "Init Unit", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
                {
                }
            });
            varInit.IsBackground = true;
            varInit.Start();
        }
        public bool Check_EStop()
        {
            if (MSystem.SIMULATION) return false;
            if (MSystem.IsDetectEmergency())
            {
                StopOrigin();
                return true;
            }
            return false;
        }

        public bool Check_Stop()
        {
            if (MSystem.m_pDIO.IsOn(IO.IN_START_SW))
            {
                StopOrigin();
                return true;
            }
            return false;
        }
        private void ORG_BT_SELECT_ALL_MouseDownEvent(object sender, AxBTNENHLib4._DBtnEnhEvents_MouseDownEvent e)
        {
            UpdateDis = true;
        }

        private void ORG_BT_CANCLE_ClickEvent(object sender, EventArgs e)
        {
            UpdateDis = false;
        }

    }
}
#endregion