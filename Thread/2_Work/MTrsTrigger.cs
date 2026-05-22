using System.Threading;

namespace MLCCInspectionMC
{
    public class MTrsTrigger : MSystem
    {
        public MTrsTrigger()
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
            //while (true)
            //{
            //    Thread.Sleep(5);
            //    try
            //    {
            //        dorunStep();
            //    }
            //    catch
            //    {
            //    }
            //}
        }
        double Curr = 0;
        bool bPos = false;
        double Last = 0;
        private void dorunStep()
        {
            if (SysStatus != StatusRun.RUN) return;
            if (ModeTrigger != ModeTrigger.Flying) return;
            Curr = MSystem.m_pTrsTransferXY.GetCurrentPos(Axis.AXIS_X);
            if(Curr != Last)
            {
                bPos = (m_pTrsTransferXY.CalculatorPosX(index_X) - Curr) < 0.5;
            }
            if (bPos)
            {
                bPos = false;
                Last = m_pTrsTransferXY.CalculatorPosX(index_X);
                m_bCallVision[0] = m_bCallVision[1] = true;
            }
        }
    }
}