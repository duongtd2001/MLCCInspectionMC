using System.Drawing;
using System.Threading;

namespace MLCCInspectionMC
{
    public class MTrsVision : MSystem
    {
        public int m_iNextStep = 0;
        public int m_iPreViewStep = 0;
        public int m_iCurrentStep = 0;

        int idx;
        public MTrsVision(int _idx)
        {
            idx = _idx;
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
                Thread.Sleep(100);
                try
                {
                    dorunStep();
                }
                catch
                {
                }
            }
        }
        private void dorunStep()
        {
            if (SysStatus != StatusRun.RUN) return;
            if (m_bCallVision[idx])
            {
                m_bCallVision[idx] = false;
                _camera[idx].Trigger();
                int currentIdx = MSystem.CurrentTrayIndex;
                if (_camera[idx].m_bitmap != null)
                {
                    Bitmap clonedBmp = (Bitmap)_camera[idx].m_bitmap.Clone();
                    m_qImage[idx].Enqueue(new ImageData { TrayIndex = currentIdx, Image = clonedBmp });
                }
                else
                {
                    m_qImage[idx].Enqueue(new ImageData { TrayIndex = currentIdx, Image = null });
                }
            }
        }
    }
}
