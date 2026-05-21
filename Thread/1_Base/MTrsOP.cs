using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLCCInspectionMC
{
    public class MTrsOP : MSystem
    {
        public TimerDelay m_tInputTimer = new TimerDelay();
        public TimerDelay m_tInputOffTimer = new TimerDelay();
        public bool m_bInputButton;
        public bool m_bInputState;
        public bool m_bStopButton;
        public bool m_bStartButton;
        public bool m_bTrayInButton;
        public bool m_bTrayInState;
        public bool m_bTrayOutButton;
        public bool m_bTrayOutState;

        public MTrsOP()
        {
            m_tInputTimer.PauseTimer();
            m_tInputTimer.ResetTimer();

            m_tInputOffTimer.PauseTimer();
            m_tInputOffTimer.ResetTimer();
            m_tInputOffTimer.RunTimer();

            m_bInputButton = m_bInputState = false;
            m_bTrayInButton = m_bTrayOutButton = false;
            m_bTrayInState = m_bTrayOutState = false;
        }
        ~MTrsOP()
        {

        }

        public void dorunStep()
        {
            while (true)
            {
                OPLamp();
                switch (MSystem.SysStatus)
                {
                    case StatusRun.ERROR:
                        break;
                    case StatusRun.STOP:
                        break;
                    case StatusRun.RUN:
                        break;
                    default: break;
                }
                Thread.Sleep(15);
            }
        }
        void OPLamp()
        {
            if (MSystem.SysStatus == StatusRun.RUN)
            {
                if (AutoForm.BT_STOP.Value == 1 || AutoForm.BT_START.Value == 0)
                {
                    AutoForm.BT_START.Value = 1;
                    AutoForm.BT_STOP.Value = 0;
                }
            }
            else
            {
                if (AutoForm.BT_STOP.Value == 0 || AutoForm.BT_START.Value == 1)
                {
                    AutoForm.BT_START.Value = 0;
                    AutoForm.BT_STOP.Value = 1;
                }
            }
            if (MSystem.SysStatus == StatusRun.ERROR)
            {
                if (AutoForm.BT_STOP.Value == 0 || AutoForm.BT_START.Value == 1)
                {
                    AutoForm.BT_START.Value = 0;
                    AutoForm.BT_STOP.Value = 1;
                }
            }
        }

        public void OnStartButton()
        {
            if (!MSystem.IsOriginAll())
            {
                MSystem.MyMsgMemo("Please Origin All Servo before run Program", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.SysStatus == StatusRun.RUN) return;
            /******************************************************************/
            if (m_bIsTeachDlg || m_bInitManual)
            {
                return;
            }
            /******************************************************************/

            MSystem.m_bInitWorkFlag = false;
            MSystem.SysStatus = StatusRun.RUN;
        }
        public void OnStopButton()
        {
            if (MSystem.SysStatus == StatusRun.STOP) return;
            MSystem.SetAllStop();
            MSystem.SysStatus = StatusRun.STOP;
            //Tacttime.PauseTimer();
        }
        public void OnResetButton()
        {
            if (MSystem.SysStatus == StatusRun.RUN) return;

            
            MSystem.SysStatus = StatusRun.STOP;
        }
        
        public void InputStart()
        {
        }
        public void InputStop()
        {
        }
    }
}
