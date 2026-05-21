using System;
using System.Data.SqlClient;
using System.Threading;

namespace FrontAutoLoaderV2
{
    public class MTrsAGVInTray : MSystem
    {/******************************************************/
        #region Variable
        public bool m_bReady;
        public bool m_bCallAgv;
        public bool m_iHoldCV = false;
        public bool m_bRCVAble = false;
        public bool m_bRCVStart = false;
        public UNITINFOR _Infor = new UNITINFOR();

        private int m_iCurrentStep = 0;
        public int m_iNextStep = 0;
        public int m_iPreviewStep = 0;
        public int m_iRetry = 0;

        public TimerDelay m_iTimer = new TimerDelay();
        public TimerDelay m_iTimerCallAgv = new TimerDelay();
        public TimerDelay m_iCVTimeOut = new TimerDelay();
        public TimerDelay m_iInputOffDelay = new TimerDelay();
        public TimerDelay m_iOutputOffDelay = new TimerDelay();
        #endregion
        /******************************************************/
        #region Define STEP Run
        const int STEP_WORK = 1000;
        const int STEP_CALL_AGV = 2000;
        const int STEP_WAIT_AGV = 3000;
        const int STEP_AGV_TRANSFERING = 4000;
        const int STEP_AGV_TRANSFER_END = 5000;
        #endregion
        /******************************************************/
        #region Class Initial
        public MTrsAGVInTray()
        {
            _Infor = new UNITINFOR();
            m_bReady = false;
            InitData();

        }
        public int SetUnitInitialize()
        {
            try
            {
                InitData();
                return (int)InitResult.UNIT_INIT_SUCCESS;
            }
            catch (Exception)
            {
                MSystem.MyMsgMemo("Out Conveyor Init Fail", "Init Error");
                return 1;
            }
        }
        public void InitData()
        {
            m_iNextStep = 0;
            m_iCurrentStep = 0;
            m_iPreviewStep = 0;
            m_iRetry = 0;
            _Infor = new UNITINFOR();
            m_bRCVAble = false;
            m_bRCVStart = false;
            SetStep(0);
        }
        #endregion
        /******************************************************/
        #region // Tool Get Step and Timer
        public void SetStep(int _step)
        {
            m_iCurrentStep = _step;
            m_iTimer.StartTimer();
        }
        public int GetStep()
        {
            return m_iCurrentStep;
        }
        public virtual void SetError(int _idError)
        {
            MSystem.SetError(_idError + 0, $"Out Conveyor");
        }
        public bool IsOverTime(double _tim)
        {
            if (m_iTimer.MoreThan(_tim))
                return true;
            else
                return false;
        }
        #endregion
        /******************************************************/
        #region Main Run
        public void ThreadJob()
        {
            while (true)
            {
                Thread.Sleep(10);
                try
                {
                    dorunStep();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }
        public void dorunStep()
        {
            if (SysStatus != StatusRun.RUN || InforManager.Instance.m_iInput_Mode != (int)Input_Mode.AGV)
            {
                ReadyOff();
                m_iCVTimeOut.ResetTimer();
                m_iInputOffDelay.ResetTimer();
                SetStep(0);
                return;
            }

            if (InforManager.Instance.m_iAGV_Moving_Path == MSystem.eLEFT)
            {
                m_pDIO.OutPutOn(IO.OUT_INPUT_TRAY_AGV_LEFT_MOVING_PATH);
                m_pDIO.OutPutOff(IO.OUT_INPUT_TRAY_AGV_RIGHT_MOVING_PATH);
            }
            else
            {
                m_pDIO.OutPutOff(IO.OUT_INPUT_TRAY_AGV_LEFT_MOVING_PATH);
                m_pDIO.OutPutOn(IO.OUT_INPUT_TRAY_AGV_RIGHT_MOVING_PATH);
            }

            switch (m_iCurrentStep)
            {
                case 0:
                {
                    InitData();
                    SetStep(STEP_WORK);
                    break;
                }
                case STEP_WORK:
                {
                    if (m_pTrsInTray_Bot_InCV.IsDetectIn() ||
                        m_pTrsInTray_Bot_InCV.IsDetectOut() ||
                        m_pTrsInTray_Bot_OutCV.IsDetectOut() ||
                        m_pTrsInTray_Top_InCV.IsDetectIn() ||
                        m_pTrsInTray_Top_InCV.IsDetectOut() ||
                        m_pTrsInTray_Top_OutCV.IsDetectOut())
                    {
                        ReadyOff();
                        m_bCallAgv = false;
                        m_iTimerCallAgv.ResetTimer();
                    }

                    if (m_iTimerCallAgv.MoreThan(20))
                    {
                        m_iRetry = 0;
                        m_bCallAgv = true;
                        SetStep(STEP_CALL_AGV);
                    }
                    break;
                }
                case STEP_CALL_AGV:
                {
                    if (CallAGV(ON))
                    {
                        m_iRetry = 0;
                        SetStep(STEP_WAIT_AGV);
                    }
                    else
                    {
                        m_iRetry++;
                        if (m_iRetry >= 5)
                        {
                            m_iRetry = 0;
                            SetError(9999);
                            break;
                        }
                        SetStep(STEP_CALL_AGV + 10);
                    }
                    break;
                }
                case STEP_CALL_AGV + 10:
                {
                    if (!IsOverTime(20)) break;
                    SetStep(STEP_CALL_AGV);
                    break;
                }
                case STEP_WAIT_AGV:
                {
                    m_pTrsInTray_Bot_InCV.CVStop();
                    m_pTrsInTray_Top_InCV.CVStop();

                    ReadyOn();
                    if (IsAGV_Ready())
                        SetStep(STEP_AGV_TRANSFERING);
                    break;
                }
                case STEP_AGV_TRANSFERING:
                {
                    if (!IsAGV_Ready())
                    {
                        SetError(9999);
                        break;
                    }
                    if (IsAGV_TransferRequest())
                    {
                        m_pTrsInTray_Bot_InCV.CVRun();
                        m_pTrsInTray_Top_InCV.CVRun();
                        SetStep(STEP_AGV_TRANSFERING + 10);
                        break;
                    }
                    break;
                }
                case STEP_AGV_TRANSFERING + 10:
                {
                    break;
                }
                default: break;
            }
        }
        #endregion
        /******************************************************/
        #region Control IO Function
        public void ReadyOn()
        {
            m_pDIO.OutPutOn(IO.OUT_INPUT_TRAY_AGV_UPPER_READY);
            m_pDIO.OutPutOn(IO.OUT_INPUT_TRAY_AGV_LOWER_READY);
        }
        public void ReadyOff()
        {
            m_pDIO.OutPutOff(IO.OUT_INPUT_TRAY_AGV_UPPER_READY);
            m_pDIO.OutPutOff(IO.OUT_INPUT_TRAY_AGV_LOWER_READY);
        }
        public void DoneOn()
        {
            m_pDIO.OutPutOn(IO.OUT_INPUT_TRAY_AGV_MOVE_PERMIT);
        }
        public void DoneOff()
        {
            m_pDIO.OutPutOff(IO.OUT_INPUT_TRAY_AGV_MOVE_PERMIT);
        }
        public bool IsAGV_Ready()
        {
            return m_pDIO.IsOn(IO.IN_INPUT_TRAY_AGV_GO_READY);
        }
        public bool IsAGV_TransferRequest()
        {
            return m_pDIO.IsOff(IO.IN_INPUT_TRAY_AGV_REQUEST_TRANSFER);
        }
        public bool IsAGV_UpperTransfering()
        {
            return m_pDIO.IsOn(IO.IN_INPUT_TRAY_AGV_UPPER_TRANSFERING);
        }
        public bool IsAGV_LowerTransfering()
        {
            return m_pDIO.IsOn(IO.IN_INPUT_TRAY_AGV_LOWER_TRANSFERING);
        }

        public bool CallAGV(bool OnOff)
        {
            try
            {
                string sqlConection = @"Data Source=107.124.46.147;Initial Catalog=SW;User ID=sa;Password=123";
                using (var con = new SqlConnection(sqlConection))
                {
                    con.Open();
                    var strQuery = "SELECT * FROM S_WIP_CALL WHERE PAN_ID=01 AND ADDRESS=02";
                    using (var cmd = new SqlCommand(strQuery, con))
                    {
                        if (cmd.ExecuteScalar() == null)
                            strQuery = "INSERT ";
                        else
                            strQuery = "UPDATE ";
                    }
                    strQuery += "";
                    using (var cmd = new SqlCommand(strQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@TIME", $"{DateTime.Now:yyyyMMddHHmmss}");
                        cmd.Parameters.AddWithValue("@PAN_ID", "01");
                        cmd.Parameters.AddWithValue("@ADDRESS", "02");
                        cmd.Parameters.AddWithValue("@INPUT1", OnOff ? "1" : "0");
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch { return false; }
        }
        #endregion
    }
}
