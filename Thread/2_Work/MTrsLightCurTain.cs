using System;
using System.Threading;

namespace MLCCInspectionMC
{
    public class MTrsLightCurTain : MSystem
    {
        public MTrsLightCurTain()
        {
            ThreadRun();
        }
        private void ThreadRun()
        {
            _mRun = new Thread(() => ThreadJob());
            _mRun.SetApartmentState(ApartmentState.STA);
            _mRun.Priority = ThreadPriority.Highest;
            _mRun.IsBackground = true;
            _mRun.Start();
        }

        public void ThreadJob()
        {
            while (true)
            {
                Thread.Sleep(5);
                try
                {
                    dorunStep();
                }
                catch
                {
                }
            }
        }
        double Curr = 0;
        bool bPosX = false;
        bool bPosY = false;
        double Last = 0;
        //private void dorunStep()
        //{
        //    if (SysStatus != StatusRun.RUN) return;
        //    Curr = MSystem.m_pTrsTransferXY.GetCurrentPos(Axis.AXIS_Y);
        //    bPosY = InforTeaching.Instance.m_dTechPos_In_Out_Tray[1] - Curr < 150;
        //    if (!m_pTrsTransferXY.IsLightCurtainDetect())
        //    {
        //        if (bPosY)
        //        {
        //            MSystem.SetAllStop();
        //            MSystem.SysStatus = StatusRun.STOP;
        //            MSystem.MyMsgMemo("LIGHT CURTAIN DETECT!! (X005)!!", "Error", msgButton.OK, msgIcon.Error);
        //        }
        //    }
        //}

        private void dorunStep()
        {
            if (SysStatus != StatusRun.RUN) return;
            Curr = GetCurrentPos((int)Axis.AXIS_X);
            bPosX = Math.Abs(Curr - InforTeaching.Instance.m_dTechPos_In_Out_Tray[0]) < 0.5;
            Curr = GetCurrentPos((int)Axis.AXIS_Y);
            bPosY = Math.Abs(Curr - InforTeaching.Instance.m_dTechPos_In_Out_Tray[1]) < 0.5;
            if (bPosX && bPosY) return;
            if (!m_pTrsTransferXY.IsLightCurtainDetect())
            {
                MSystem.SetAllStop();
                MSystem.SysStatus = StatusRun.STOP;
                MSystem.MyMsgMemo("LIGHT CURTAIN DETECT!! (X005)!!", "Error", msgButton.OK, msgIcon.Error);
            }
        }


    }
}

