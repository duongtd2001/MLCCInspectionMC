using System.Threading;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public class MTrsAutoManager : MSystem
    {
        TimerDelay TimDetectEMC = new TimerDelay();
        public TimerDelay TimeDetectFullTrayOut = new TimerDelay();
        public TimerDelay TimeCallTrayEmtyInput = new TimerDelay();
        public TimerDelay TimeOutNGCVFull = new TimerDelay();
        public TimerDelay TimerTopInTray = new TimerDelay();
        public TimerDelay TimerBotInTray = new TimerDelay();
        public TimerDelay TimerTopOutTray = new TimerDelay();
        public TimerDelay TimerBotOutTray = new TimerDelay();

        public bool m_bRequestDoorOpenAlert;
        public bool m_bCallTrayEmtyAlert;
        public bool m_bOutTrayAlert;
        public DialogResult m_bVinylDetectFailAnser;

        public MTrsAutoManager()
        {
            m_bRequestDoorOpenAlert = false;
            m_bCallTrayEmtyAlert = false;
            m_bOutTrayAlert = false;
            m_bVinylDetectFailAnser = DialogResult.None;
            System.Threading.Thread t = new System.Threading.Thread(new ThreadStart(MessengerPoll));
            t.IsBackground = true;
            t.Start();
        }
        ~MTrsAutoManager()
        {

        }
        void MessengerPoll()
        {
            while (true)
            {
                try
                {
                    if (!MSystem.m_bLife) break;
                    if (SysStatus != StatusRun.RUN)
                    {
                        TimeCallTrayEmtyInput.ResetTimer();
                        TimeDetectFullTrayOut.ResetTimer();
                        TimeOutNGCVFull.ResetTimer();
                        TimerTopInTray.ResetTimer();
                        TimerBotInTray.ResetTimer();
                        TimerTopOutTray.ResetTimer();
                        TimerBotOutTray.ResetTimer();
                        m_bCallTrayEmtyAlert = false;
                        m_bOutTrayAlert = false;
                    }

                    if (SysStatus == StatusRun.RUN)
                    {
                        if (!IsDry())
                        {
                            
                        }
                    }
                }
                finally
                {
                    System.Threading.Thread.Sleep(15);
                }
            }
        }
        public void dorunStep()
        {
            while (true)
            {
                switch (SysStatus)
                {
                    case StatusRun.ERROR:
                        MSystem.SysStatus = StatusRun.ERROR;
                        if (IsDetectEmergency())
                        {
                            OnEmergency();
                            break;
                        }
                        if (MSystem.m_bIsServoDisconnect)
                        {
                            MSystem.m_bIsServoDisconnect = false;
                            MSystem.MyMsgMemo("Servo Connection Fail", "Servo Error", msgButton.OK, msgIcon.Error);
                            MSystem.SysStatus = StatusRun.STOP;
                        }
                        break;
                    case StatusRun.STOP:
                        if (IsDetectEmergency())
                        {
                            OnEmergency();
                            break;
                        }
                        break;

                    case StatusRun.RUN:
                        if (IsDetectEmergency())
                        {
                            //MSystem.m_pTrsLoader.NextMC_SafetyOff();
                            m_pLogSave.AddTail(LogIndex.eDeviceLog, "EMC Press Detect");
                            OnEmergency();
                            break;
                        }
                        if (!MSystem.m_pAxisManager.m_bAllServoConnected)
                        {
                            MSystem.SysStatus = StatusRun.ERROR;
                            StopAllServo();
                            m_InTrayStop = m_OutTrayStop = true;
                            MSystem.m_pLogSave.AddTail(LogIndex.eDeviceLog, "Servo Connection Fail");
                            MSystem.m_bIsServoDisconnect = true;
                        }
                        break;

                    default:
                        break;
                }
                if (m_bRequestDoorOpenAlert)
                {
                    m_pLogSave.AddTail(LogIndex.eDeviceLog, $"DOOR OPEN!");
                    MSystem.MyMsgMemo("Door Open!! (X004) \n Lỗi cửa mở (X004)!!", "Error", msgButton.OK, msgIcon.Error);
                    MSystem.SysStatus = StatusRun.STOP;
                    m_bRequestDoorOpenAlert = false;
                }
                System.Threading.Thread.Sleep(15);
            }
        }

        public void OnEmergency()
        {
            if (IsDetectEmergency() && TimDetectEMC.MoreThan(0.2))
            {
                if (SysStatus == StatusRun.RUN)
                    m_pLogSave.AddTail(LogIndex.eDeviceLog, $"{MSystem.SysStatus} --> SW EMC Press");

                SysStatus = StatusRun.ERROR;
                AllServoOff();
                MSystem.MyMsgMemo("E-Stop Button Presed (X000)!!\n Lỗi dừng khẩn cấp (X000)!!", "Emergency Stop", msgButton.SAFETY, msgIcon.Error);
                if (IsDetectEmergency()) return;
                TimDetectEMC.StartTimer();
            }
        }
        public void StopAllServo()
        {
            MSystem.SetAllStop();
        }
        public void AllServoOff()
        {
            for (int i = 0; i < eAXIS_MAX; i++)
            {
                if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null) continue;

                MSystem.m_pAxisManager.m_pFastechAxis[i].EStop();
                MSystem.m_pAxisManager.m_pFastechAxis[i].ResetOrigin();
            }
            for (int i = 0; i < eAXIS_MAX; i++)
            {
                if (MSystem.m_pAxisManager.m_pFastechAxis[i] == null) continue;
                MSystem.m_pAxisManager.m_pFastechAxis[i].SetAmpEnable(false);
            }
        }
        public void DoorLock()
        {
        }
        public void DoorUnlock()
        {

        }

        public void SetStepForUnit()
        {
            //if (MSystem.m_pTrsShuttle.GetStep() == 10100)
            //{
            //    MSystem.m_pTrsShuttle.SetStep(10000);
            //}

            //if (MSystem.m_pTrsLoader.GetStep() == 10100)
            //{
            //    MSystem.m_pTrsLoader.SetStep(10000);
            //}
            //if (MSystem.m_pTrsLoader.GetStep() == 10300)
            //{
            //    MSystem.m_pTrsLoader.SetStep(10200);
            //}
            //if (MSystem.m_pTrsLoader.GetStep() == 10500)
            //{
            //    MSystem.m_pTrsLoader.SetStep(10400);
            //}
            //if (MSystem.m_pTrsLoader.GetStep() == 10700)
            //{
            //    MSystem.m_pTrsLoader.SetStep(10600);
            //}

            //if (MSystem.m_pTrsVilynRemove.GetStep() == 10100)
            //{
            //    MSystem.m_pTrsVilynRemove.SetStep(10000);
            //}
            //if (MSystem.m_pTrsVilynRemove.GetStep() == 10300)
            //{
            //    MSystem.m_pTrsVilynRemove.SetStep(10200);
            //}
            //if (MSystem.m_pTrsVilynRemove.GetStep() == 10500)
            //{
            //    MSystem.m_pTrsVilynRemove.SetStep(10400);
            //}
            //if (MSystem.m_pTrsVilynRemove.GetStep() == 3520)
            //{
            //    MSystem.m_pTrsVilynRemove.SetStep(3510);
            //}
            //if (MSystem.m_pTrsVilynRemove.GetStep() == 3540)
            //{
            //    MSystem.m_pTrsVilynRemove.SetStep(3530);
            //}
            //if (MSystem.m_pTrsVilynRemove.GetStep() == 3036)
            //{
            //    MSystem.m_pTrsVilynRemove.SetStep(3035);
            //}
            //if (MSystem.m_pTrsVilynRemove.GetStep() == 3060)
            //{
            //    MSystem.m_pTrsVilynRemove.SetStep(3050);
            //}
        }
    }
}
