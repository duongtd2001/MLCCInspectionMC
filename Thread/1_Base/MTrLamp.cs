using System.Threading;

namespace MLCCInspectionMC
{
    public class MTrLamp : MSystem
    {
        readonly object LockLamp = new object();

        public void Lamp_Poll()
        {
            while (true)
            {
                switch (SysStatus)
                {
                    case StatusRun.RUN:
                        if (IsWaitInput() && SysMode != ModeRun.DryRun)
                        {
                            LoadlampStop();
                            break;
                        }
                        LampRun();
                        break;
                    case StatusRun.STOP:
                        LoadlampStop();
                        break;

                    case StatusRun.ERROR:
                        LoadlampError();
                        break;
                    case StatusRun.OPRATOR_CALL:
                        LoadlampOperator();
                        break;
                    default: break;
                }
                Thread.Sleep(50);
            }
        }
        public void LampRun()
        {
            if (m_pTrsAutoManager.m_bCallTrayEmtyAlert)
            {
                SetLampStateAndLog(green: true, yellow: true, red: false);
            }
            else
            {
                SetLampStateAndLog(green: true, yellow: false, red: false);
            }
        }
        public void LoadlampStop()
        {
            SetLampStateAndLog(green: false, yellow: true, red: false);
        }
        public void LoadlampError()
        {
            SetLampStateAndLog(green: false, yellow: false, red: true);
        }
        public void LoadlampOperator()
        {
            SetLampStateAndLog(green: false, yellow: true, red: true);
        }
        public bool IsWaitInput()
        {
            return false;
        }
        /// <summary>
        /// Set lamp state and log
        /// </summary>
        /// <param name="green"> Lamp Green (true=ON, false=OFF)</param>
        /// <param name="yellow">LAmp Yellow (true=ON, false=OFF)</param>
        /// <param name="red">Lamp Red (true=ON, false=OFF)</param>
        public void SetLampStateAndLog(bool green, bool yellow, bool red)
        {
            lock (LockLamp)
            {
                int[] newState = { green ? 1 : 0, yellow ? 1 : 0, red ? 1 : 0 };

                if (newState[0] != m_iTowerLampGeim[0] ||
                    newState[1] != m_iTowerLampGeim[1] ||
                    newState[2] != m_iTowerLampGeim[2])
                {
                    m_pDIO.SetOutput(IO.OUT_TOWER_LAMP_GREEN, green);
                    m_pDIO.SetOutput(IO.OUT_TOWER_LAMP_YELLOW, yellow);
                    m_pDIO.SetOutput(IO.OUT_TOWER_LAMP_RED, red);

                    string lampStateStringForLog = $"{newState[0]}{newState[1]}{newState[2]}";

                    if (lampStateStringForLog == "000" || lampStateStringForLog == "111")
                    {
                        return;
                    }

                    m_iTowerLampGeim = newState;
                }
            }
        }
    }
}
