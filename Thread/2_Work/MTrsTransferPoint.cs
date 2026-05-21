using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MLCCInspectionMC
{
    public class MTrsTransferPoint : MSystem
    {
        #region Variable
        public int m_iNextStep = 0;
        public int m_iPreViewStep = 0;
        public int m_iCurrentStep = 0;

        public TimerDelay m_iTimer = new TimerDelay();
        public TimerDelay m_iFakeVision = new TimerDelay();
        public TimerDelay m_iTimerWorking  = new TimerDelay();
        public TimerDelay m_iTimerInit = new TimerDelay();

        public bool m_bReady = false;

        public int COUNT_MAX;

        #endregion
        /******************************************************/
        #region Define STEP Run

        const int STEP_WAIT = 100;
        const int STEP_MOVE_XY = 1000;
        const int STEP_MOVE_X = 2000;
        const int STEP_MOVE_Y = 3000;
        const int STEP_CHECK_READY = 110;
        const int STEP_IN_OUT_TRAY = 5000;

        #endregion
        /******************************************************/
        #region Class Initial
        public MTrsTransferPoint()
        {
            ThreadRun();
            InitData();
        }
        public void ThreadRun()
        {
            _mRun = new Thread(() => ThreadJob());
            _mRun.Priority = ThreadPriority.Highest;
            _mRun.SetApartmentState(ApartmentState.STA);
            _mRun.IsBackground = true;
            _mRun.Start();
        }
        public void InitData()
        {
            index_SW_Start = 0;
            index_X = 0;
            index_Y = 0;
            m_bCallVision[0] = m_bCallVision[1] = false;
            m_iNextStep = 0;
            m_iCurrentStep = 0;
            m_iPreViewStep = 0;
            CurrentTrayIndex = 0;
            SetStep(0);
        }
        #endregion
        /******************************************************/
        #region Tool Get Step and Timer
        public void SetStep(int _step)
        {
            m_iTimer.StartTimer();
            m_iCurrentStep = _step;
            m_iTimerWorking.StartTimer();
            m_iFakeVision.StartTimer();
        }
        public int GetStep()
        {
            return m_iCurrentStep;
        }
        public virtual void SetError(int _idError)
        {
            MSystem.SetError(_idError, $"Vision DATA");
        }
        public bool IsOverTime(double _tim)
        {
            if (m_iTimer.MoreThan(_tim))
                return true;
            else
                return false;
        }
        #endregion

        Stopwatch sw = new Stopwatch();
        /**********************************************************/
        #region Main Run
        public void ThreadJob()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(10);
                try
                {
                    dorunStep();
                }
                catch (Exception ex)
                {
                }
            }
        }
       
        public void dorunStep()
        {
            if (SysStatus != StatusRun.RUN) return;
            if (ModeTrigger != ModeTrigger.Point) return;
            switch (m_iCurrentStep)
            {
                case 0:
                    InitData();
                    SetStep(STEP_WAIT);
                    break;
                case STEP_WAIT:
                    InitData();
                    if (!m_bReady)
                    {
                        SetStep(STEP_CHECK_READY);
                    }
                    else
                    {
                        SetStep(STEP_MOVE_XY);
                    }
                    break;
                case STEP_IN_OUT_TRAY:
                    break;
                case STEP_IN_OUT_TRAY + 100:

                    break;
                case STEP_MOVE_XY:
                    Task.Run(() =>
                    {
                        MovePosition(Axis.AXIS_X, PosStartX());
                    });
                    Task.Run(() =>
                    {
                        MovePosition(Axis.AXIS_Y, PosStartY());
                    });
                    SetStep(STEP_MOVE_XY + 100);
                    break;
                case STEP_WAIT + 100:
                    if (IsStartSW())
                    {
                        index_SW_Start++;
                        if (index_SW_Start > 10)
                        {
                            MSystem.AutoForm.ResetTrayMap();
                            m_pLightControl.SetLightOn(0, InforTeaching.Instance.m_iLight[0]);
                            m_pLightControl.SetLightOn(1, InforTeaching.Instance.m_iLight[1]);
                            Thread.Sleep(50);
                            SetStep(STEP_MOVE_X);
                        }
                    }
                    else
                    {
                        index_SW_Start = 0;
                    }
                    //MSystem.AutoForm.ResetTrayMap();
                    //m_pLightControl.SetLightOn(0, InforTeaching.Instance.m_iLight[0]);
                    //m_pLightControl.SetLightOn(1, InforTeaching.Instance.m_iLight[1]);
                    //Thread.Sleep(1000);
                    SetStep(STEP_MOVE_X);
                    break;
                case STEP_MOVE_XY + 100:
                    if (IsOverTime(5))
                    {
                        SysStatus = StatusRun.STOP;
                        SetStep(STEP_MOVE_XY);
                        break;
                    }
                    if (!IsMoveComplete(Axis.AXIS_X, CalculatorPosX(index_X)) || !IsMoveComplete(Axis.AXIS_Y, CalculatorPosY(index_Y)))
                    {
                        SetStep(STEP_MOVE_XY);
                        break;
                    } 
                    SetStep(STEP_WAIT + 100);
                    break;
                case STEP_MOVE_X:
                    MoveTransferX(index_X);
                    SetStep(STEP_MOVE_X + 100);
                    break;
                case STEP_MOVE_X + 100:
                    if (IsOverTime(5))
                    {
                        SysStatus = StatusRun.STOP;
                        SetStep(STEP_MOVE_X);
                        break;
                    }
                    if (!IsMoveComplete(Axis.AXIS_X, CalculatorPosX(index_X)))
                    {
                        SetStep(STEP_MOVE_X);
                        MyMessagerBottom("Move set step X");
                        break;
                    } 
                    
                    if (index_Y % 2 == 0)
                    {
                        MyMessagerBottom($"Move step X: {index_X}");
                        index_X++;
                        if (index_X == InforTeaching.Instance.TrayColumns)
                        {
                            index_Y++;
                            SetStep(STEP_MOVE_Y);
                            MyMessagerBottom("Move set step Y");
                            index_X = InforTeaching.Instance.TrayColumns - 1;
                        }
                        else
                        {
                            m_bCallVision[0] = true;
                            m_bCallVision[1] = true;
                            Thread.Sleep(200);
                            MSystem.CurrentTrayIndex++;
                            SetStep(STEP_MOVE_X);
                        } 
                    }
                    else
                    {
                        MyMessagerBottom($"Move step X: {index_X}");
                        index_X--;
                        if (index_X < 0)
                        {
                            index_Y++;
                            SetStep(STEP_MOVE_Y);
                            index_X = 0;
                        }
                        else
                        {
                            m_bCallVision[0] = true;
                            m_bCallVision[1] = true;
                            Thread.Sleep(200);
                            MSystem.CurrentTrayIndex++;
                            SetStep(STEP_MOVE_X);
                        }
                    }
                    break;

                case STEP_MOVE_Y:
                    if (index_Y < InforTeaching.Instance.TrayRows)
                    {
                        MoveTransferY(index_Y);
                        MyMessagerBottom($"Move step Y: {index_Y}");
                        SetStep(STEP_MOVE_Y + 100);
                    }
                    else
                    {
                        SetStep(STEP_WAIT);
                    }
                        break;
                case STEP_MOVE_Y + 100:
                    if (IsOverTime(5))
                    {
                        SysStatus = StatusRun.STOP;
                        break;
                    }
                    if (!IsMoveComplete(Axis.AXIS_Y, CalculatorPosY(index_Y)))
                    {
                        SetStep(STEP_MOVE_Y);
                        break;
                    }  
                    SetStep(STEP_MOVE_X);
                    break;
                case STEP_CHECK_READY:
                    m_bReady = true;
                    m_pLightControl.SetLightOff(0);
                    m_pLightControl.SetLightOff(1);
                    SetStep(STEP_WAIT);
                    break;
            }
        }
        #endregion
        
        #region Servo Function
        //NEW
        public void AllServoStop()
        {
            for (int i = 0; i < (int)Axis.eAXIS_MAX; i++)
            {
                MSystem.m_pAxisManager.m_pFastechAxis[i].Stop();
            }
        }

        public int MoveTransferX(int idx)
        {
            return MovePosition(Axis.AXIS_X, CalculatorPosX(idx));
        }
        public int MoveTransferY(int idx)
        {
            return MovePosition(Axis.AXIS_Y, CalculatorPosY(idx));
        }
        public int MovePosition(Axis iAxis, double dPos)
        {
            return SetMove((int)iAxis, dPos);
        }
        public bool IsMoveComplete(Axis iAxis, double dPos)
        {
            return IsAxisPositionCheck((int)iAxis, dPos);
        }
        public double GetCurrentPos(Axis iAxis)
        {
            return IsAxisCurrentPos((int)iAxis);
        }
        // Move each point
        public double CalculatorPosX(int idx)
        {
            double dX = InforTeaching.Instance.m_dTechPos_StartXY[0] +  idx * ((InforTeaching.Instance.m_dTechPos_EndXY[0] - 
                InforTeaching.Instance.m_dTechPos_StartXY[0]) / (InforTeaching.Instance.TrayColumns - 1));
            return dX;
        }
        public double CalculatorPosY(int idx)
        {
            double dY = InforTeaching.Instance.m_dTechPos_StartXY[1] - idx * ((InforTeaching.Instance.m_dTechPos_StartXY[1] -
               InforTeaching.Instance.m_dTechPos_EndXY[1]) / (InforTeaching.Instance.TrayRows - 1));
            return dY;
        }
        // move all point
        public double PosStartX()
        {
            return InforTeaching.Instance.m_dTechPos_StartXY[0];
        }
        public double PosStartY()
        {
            return InforTeaching.Instance.m_dTechPos_StartXY[1];
        }
        public double PosEndX()
        {
            return InforTeaching.Instance.m_dTechPos_EndXY[0];
        }
        public double PosEndY()
        {
            return InforTeaching.Instance.m_dTechPos_EndXY[1];
        }

        public void TransferServoStop()
        {
            for (int i = 0; i < (int)Axis.eAXIS_MAX; i++)
            {
                MSystem.m_pAxisManager.m_pFastechAxis[i].Stop();
            }
        }
        public bool IsStartSW()
        {
            return MSystem.m_pDIO.IsOn(IO.IN_START_SW);
        }
        public int SetUnitInitialize()
        {
            InitData();
            return 0;
        }
        public bool IsLightCurtainDetect()
        {
            return m_pDIO.IsOff(IO.IN_LIGHT_CURTAIN_SENS);
        }

        #endregion
    }
}
