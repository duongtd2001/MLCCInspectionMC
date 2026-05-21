using FASTECH;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace MLCCInspectionMC
{
    public partial class FastechAxis : MSystem
    {
        //Note

        bool m_ibStatus;
        TimerDelay m_ttOriginTimer = new TimerDelay();

        public sAxisInfo m_sAxisInfo = new sAxisInfo();
        public CBbutton2[] m_BtAxis = new CBbutton2[(int)Axis.eAXIS_MAX];
        public int m_iOriginStep = 100;
        public int m_iOriginPrevStep;
        public bool m_bOriginFlag;
        public int m_iOriginError;
        #region // Variable
        public string m_strAxisName;
        private byte m_iAxisID = 0;
        private byte m_iPort;
        public double m_dScale;
        public int m_iPrior;
        public bool m_bFlagOrigin = false;
        #endregion
        
        #region //Axis Initial
        public FastechAxis(string strName, int port, byte id, double scale, int prior) {
            m_strAxisName = strName;
            m_iPort = (byte)port;
            m_iAxisID = id;
            m_dScale = scale;
            m_iPrior = prior;
            m_bOriginFlag = false;
            LoadData();
        }
        public FastechAxis(string strName, byte id, double scale, int prior)
        {
            m_strAxisName = strName;
            m_iAxisID = id;
            m_dScale = scale;
            m_iPrior = prior;
            m_bOriginFlag = false;
            LoadData();
        }
        ~FastechAxis() {

        }
        public void Init(string strName, int port, byte id, double scale, int prior)
        {
            m_strAxisName = strName;
            m_iPort = (byte)port;
            m_iAxisID = id;
            m_dScale = scale;
            m_iPrior = prior;
            LoadData();
            m_bOriginFlag = false;
        }
        public void Init(string strName, byte id, double scale, int prior)
        {
            m_strAxisName = strName;
            m_iAxisID = id;
            m_dScale = scale;
            m_iPrior = prior;
            LoadData();
            m_bOriginFlag = false;
        }
        #endregion

        #region //Parameter and data
        public void LoadData() {
            ServoParam.Instance.LoadSetting();
            m_sAxisInfo = ServoParam.Instance.m_sAxisInfo[m_iAxisID];
            SaveData();
        }
        public void SaveData() {
            //ServoParam.Instance.SaveSettings();
            SetParameter(3, (int)m_sAxisInfo.dAxisAcc);
            SetParameter(4, (int)m_sAxisInfo.dAxisAcc);
            SetParameter(8, (int)m_sAxisInfo.dAxisAcc);
            SetParameter(9, (int)m_sAxisInfo.dLimitPluseValue);
            SetParameter(10, (int)m_sAxisInfo.dLimitMinusValue);
           // SetParameter(13, 0);
        }
        #endregion

        #region //Tool function
        public int OriginReturn(bool bMove)
        {
            int iResult = 0;

            switch (m_iOriginStep)
            {
                case 100:
                    m_iOriginError = 0;
                    m_bOriginFlag = false;
                    SetOriginStep(110);
                    break;
                case 110:
                    //AmpDisable
                    ResetOrigin();
                    if (GetAmpFault())
                    {
                        ClearAxis();
                        Thread.Sleep(2000);
                    }
                    SetOriginStep(115);
                    m_ttOriginTimer.StartTimer();
                    break;
                case 115:
                    if (GetAmpFault())
                        SetAmpEnable(false);

                    SetOriginStep(120);
                    m_ttOriginTimer.StartTimer();
                    break;

                case 120:
                    if (GetAmpFault())
                    {
                        ClearAxis();
                        Thread.Sleep(2000);
                    }
                    SetOriginStep(140);
                    m_ttOriginTimer.StartTimer();
                    break;

                case 140:
                    SetOriginStep(150);
                    break;

                case 150:
                    if (GetAmpFault())
                    {
                        ClearAxis();
                        Thread.Sleep(2000);
                    }

                    m_ttOriginTimer.StartTimer();
                    SetOriginStep(200);
                    break;

                case 200:
                    if (!GetAmpEnable())
                    {
                        SetAmpEnable(true);
                    }
                    if (!IsAxisReady())
                    {
                        SetOriginStep(2000);
                        break;
                    }
                    m_ttOriginTimer.StartTimer();
                    SetOriginStep(220);
                    break;
                case 220:
                    if (EziMOTIONPlusELib.FMM_OK != (iResult = EziMOTIONPlusELib.FAS_MoveOriginSingleAxis(m_iAxisID)))
                    {
                        m_iOriginError = iResult;
                        SetOriginStep(1000);
                        break;
                    }
                    m_ttOriginTimer.StartTimer();
                    SetOriginStep(305);
                    break;

                case 305:
                    if (m_ttOriginTimer.MoreThan(1.0))
                    {
                        SetOriginStep(310);
                        m_ttOriginTimer.StartTimer();
                    }
                    break;

                case 310:
                    if ((IsOriginOK() && IsAxisDone()) || (m_ttOriginTimer.MoreThan(10) && MSystem.SIMULATION))
                    {
                        SetOrigin(true);
                        SetOriginStep(2000);
                        break;
                    }
                    else if (m_ttOriginTimer.MoreThan(m_sAxisInfo.dOriginLimitTime))
                    {
                        m_iOriginError = 200;
                        SetOriginStep(1000);
                        break;
                    }
                    break;

                case 315:
                    if (m_ttOriginTimer.MoreThan(0.2))
                        SetOriginStep(2000);
                    break;

                case 1000:
                    ResetOrigin();
                    EStop();
                    Thread.Sleep(50);
                    SetOriginStep(999);
                    break;

                case 2000:
                    SetOrigin(true);
                    break;
                default:
                    m_iOriginError = 105051;
                    SetOriginStep(1000);
                    break;
            }
            return m_iOriginStep;
        }
        public bool IsAxisReady()
        {
            return IsAxisDone() && !GetAmpFault() && GetAmpEnable();
        }
        public bool IsMinusLimit()
        {
            int iResult;
            bool flag1;
            if (EziMOTIONPlusELib.FMM_OK != (iResult = (EziMOTIONPlusELib.FAS_GetAxisStatus(m_iAxisID, ref m_pAxisManager.m_AxisStatus))))
            {
                return false;
            }
            flag1 = (m_pAxisManager.m_AxisStatus & 0x00000004) != 0;//Limit-
            return flag1;
        }
        public bool IsHome()
        {
            int iResult;
            bool flag1;
            if (EziMOTIONPlusELib.FMM_OK != (iResult = (EziMOTIONPlusELib.FAS_GetAxisStatus(m_iAxisID, ref m_pAxisManager.m_AxisStatus))))
            {
                return false;
            }
            flag1 = (m_pAxisManager.m_AxisStatus & 0x00800000) != 0;//Home
            return flag1;
        }
        public int ClearAxis(bool bEnable)
        {
            int iResult = 0;

            /** AMP */
            if (EziMOTIONPlusELib.FAS_ServoAlarmReset((byte)m_iAxisID) != EziMOTIONPlusELib.FMM_OK)
            {
                return iResult;
            }
            return 0;
        }
        public int SetAmpEnable(bool bEnable)
        {
            int iResult = 0;

            /** AMP */
            if ((EziMOTIONPlusELib.FAS_ServoEnable( (byte)m_iAxisID, bEnable?1:0) != EziMOTIONPlusELib.FMM_OK))
            {
                return iResult;
            }
            return 0;
        }

        public bool GetAmpEnable()
        {
            int iResult = 0;
            iResult = EziMOTIONPlusELib.FAS_GetAxisStatus(m_iAxisID, ref m_pAxisManager.m_AxisStatus);
            if (EziMOTIONPlusELib.FMM_OK != iResult)
            {
                return false;
            }
            return (m_pAxisManager.m_AxisStatus & 0x00100000) != 0 ? true : false;
        }
        #endregion
        public bool IsMovingPauseServo() {

            bool flagServOn = false;
            int iResult = EziMOTIONPlusELib.FMM_OK;
            if ((iResult = EziMOTIONPlusELib.FAS_GetAxisStatus( m_iAxisID, ref m_pAxisManager.m_AxisStatus)) != EziMOTIONPlusELib.FMM_OK) {
                return false;
            }
            flagServOn = (m_pAxisManager.m_AxisStatus & 0x10000000) != 0 ? true : false;

            if (flagServOn) return true;
            else return false;

        }
        public int ClearAxis() {

            Thread.Sleep(300);
            int iResult = 0;

            if (EziMOTIONPlusELib.FMM_OK != (iResult = EziMOTIONPlusELib.FAS_ServoAlarmReset( m_iAxisID))) {
                return iResult;
            }
            return 0;
        }
        public void SetOriginStep(int iStep) {
            m_iOriginPrevStep = m_iOriginStep;
            m_iOriginStep = iStep;
        }
        public bool GetAmpFault()
        {
            int iResult;
            bool flag1, flag2, flag3;
            if (EziMOTIONPlusELib.FMM_OK != (iResult = EziMOTIONPlusELib.FAS_GetAxisStatus(m_iAxisID, ref m_pAxisManager.m_AxisStatus)))
            {
                return false;
            }
            flag1 = (m_pAxisManager.m_AxisStatus & 0x00000001) != 0 ? true : false;//ERRORALL
            flag2 = (m_pAxisManager.m_AxisStatus & 0x00000040) != 0 ? true : false;//ERROVERCURRENT
            flag3 = (m_pAxisManager.m_AxisStatus & 0x00000200) != 0 ? true : false;//ERROVERLOAD

            return (flag1 || flag2 || flag3);
        }
        public bool IsOrgReturning()
        {
            if (EziMOTIONPlusELib.FMM_OK != EziMOTIONPlusELib.FAS_GetAxisStatus(m_iAxisID, ref m_pAxisManager.m_AxisStatus))
            {
                return false;
            }

            return (m_pAxisManager.m_AxisStatus & 0x00040000) != 0;
        }

        public bool IsAxisDone() {
            if (MSystem.SIMULATION)
                return true;

            int iResult = 0;
            bool flag1, flag2, flag3;
            //Thread.Sleep(1);
            if (EziMOTIONPlusELib.FMM_OK != (iResult = (EziMOTIONPlusELib.FAS_GetAxisStatus(m_iAxisID, ref m_pAxisManager.m_AxisStatus)))) {
                return false;
            }
            flag1 = (m_pAxisManager.m_AxisStatus & 0x00080000) != 0 ? true : false;//INPOSITION
            flag2 = (m_pAxisManager.m_AxisStatus & 0x08000000) != 0 ? true : false;//MOTIONING
            if (flag1 || !flag2) 
                return true;
            else 
                return false;
        }

        public int EStop() {
            int iResult;
            iResult = EziMOTIONPlusELib.FAS_EmergencyStop(m_iAxisID);
            return iResult;
        }
        public int VStop()
        {
            int iResult;
            iResult = EziMOTIONPlusELib.FAS_MoveStop(m_iAxisID);
            return iResult;
        }
        public int Stop()
        {
           
            int iResult;
            iResult = EziMOTIONPlusELib.FAS_MoveStop(m_iAxisID);
            return iResult;
        }
        public void SetOrigin(bool flag)
        {
            m_bOriginFlag = flag;
        }
        public int SetPositionClear()
        {
            int iResult;
            if (EziMOTIONPlusELib.FMM_OK != (iResult = EziMOTIONPlusELib.FAS_ClearPosition(m_iAxisID)))
            {
                return iResult;
            }
            return 0;
        }
        public void ResetOrigin()
        {
            m_bOriginFlag = false;
            m_iOriginStep = 100;
            m_iOriginPrevStep = 100;
            m_iOriginError = 0;
        }

        public int StartMove(double position)
        {
            uint vel = (uint)(m_sAxisInfo.dAxisMaxSpeed * m_dScale); //m_sAxisInfo
            int pos = (int)(position * m_dScale);
            //°¡°¨¼Ó Ãß°¡ - 2017.11.4 »èÁ¦
            if (EziMOTIONPlusELib.FMM_OK != EziMOTIONPlusELib.FAS_MoveSingleAxisAbsPos(m_iAxisID, pos, vel))
            {
                return 100001 + (m_iAxisID * 1000);
            }
            if (GetAmpFault())
            {
                return -1;
            }
            return 0;
        }
        public int StartMove(double position, double speed = 0.0)
        {
            uint vel;
            if (speed == 0.0)
                vel = (uint)(m_sAxisInfo.dAxisMaxSpeed * m_dScale);
            else
                vel = (uint)(speed * m_dScale);
            int pos = (int)(position * m_dScale);

            if (EziMOTIONPlusELib.FMM_OK != EziMOTIONPlusELib.FAS_MoveSingleAxisAbsPos(m_iAxisID, pos, vel))
            {
                return 100001 + (m_iAxisID * 1000);
            }

            if (GetAmpFault())
            {
                return -1;
            }

            return 0;
        }

        public int StartLinerMove(int axisX_Id, double posX, int axisY_Id, double posY)
        {
            uint vel = (uint)(m_sAxisInfo.dAxisMaxSpeed * m_dScale);
            ushort accTime = (ushort)m_sAxisInfo.dAxisAcc;
            int[] pos = { 0, 0 };
            int[] iSlaveNo = { (byte)axisX_Id, (byte)axisY_Id };

            pos[0] = (int)(posX * m_dScale);
            pos[1] = (int)(posY * m_dScale);

            if (EziMOTIONPlusELib.FMM_OK != EziMOTIONPlusELib.FAS_MoveLinearAbsPos(2, iSlaveNo, pos, vel, accTime))
            {
                return 100001 + (m_iAxisID * 1000);
            }

            if (GetAmpFault())
            {
                return -1;
            }
            return 0;
        }

        public int Move(double position)
        {
            uint vel = (uint)(m_sAxisInfo.dAxisMaxSpeed * m_dScale); //m_sAxisInfo
            int pos = (int)(position * m_dScale);

            //°¡°¨¼Ó Ãß°¡
            //FAS_SetParameter(m_iPort, m_iAxisID, 3, (long)m_sAxisInfo.dAxisAcc);
            //FAS_SetParameter(m_iPort, m_iAxisID, 4, (long)m_sAxisInfo.dAxisDec);

            if (EziMOTIONPlusELib.FMM_OK != EziMOTIONPlusELib.FAS_MoveSingleAxisAbsPos(m_iAxisID, pos, vel))
            {
                return 10000 + (m_iAxisID * 1000);
            }
            TimerDelay _Tim = new TimerDelay();   
            while (true)
            {
                Thread.Sleep(100);
                if (IsAxisDone()) break;
                if (_Tim.MoreThan(10.0)) return 100000 + (m_iAxisID * 1000);
            }
            return 0;
        }

        public int Move(double position, double v = 0.0)
        {
            uint vel;
            if (v == 0.0)
                vel = (uint)(m_sAxisInfo.dAxisMaxSpeed * m_dScale);//m_sAxisInfo
            else
                vel = (uint)(v * m_dScale);
            int pos = (int)(position * m_dScale);
            //FAS_SetParameter(m_iPort, m_iAxisID, 3, (long)m_sAxisInfo.dAxisAcc);
            //FAS_SetParameter(m_iPort, m_iAxisID, 4, (long)m_sAxisInfo.dAxisDec);

            if (EziMOTIONPlusELib.FMM_OK != EziMOTIONPlusELib.FAS_MoveSingleAxisAbsPos(m_iAxisID, pos, vel))
            {
                return 10000 + (m_iAxisID * 1000);
            }

            TimerDelay _Tim = new TimerDelay();
            while (true)
            {
                Thread.Sleep(100);
                if (IsAxisDone()) break;
                if (_Tim.MoreThan(10.0)) return 100000 + (m_iAxisID * 1000);
            }
            return 0;
        }
        int StartVelOverMove(double dPos, double dOffsetPos, double dChangeVel)
        {
            int rtn;
            //BOOL bStatus;
            uint vel = (uint)(m_sAxisInfo.dAxisMaxSpeed * m_dScale);//m_sAxisInfo
            uint changeVel;
            changeVel = (uint)(dChangeVel * m_dScale);
            int pos = (int)(dPos * m_dScale);
            int offsetPos = (int)(dOffsetPos * m_dScale);

            if (EziMOTIONPlusELib.FMM_OK != EziMOTIONPlusELib.FAS_MoveSingleAxisAbsPos(m_iAxisID, pos, vel))
            {
                return 100001 + (m_iAxisID * 1000);
            }

            TimerDelay _Tim = new TimerDelay();
            while (true)
            {
                Thread.Sleep(10);
                double dCurr = GetCurrentPos();
                if (IsMovingPauseServo())
                {
                    _Tim.SetTimer();
                    continue;
                }

                if (dCurr >= (dPos + dOffsetPos))
                {
                    //TRACE(_T("curr = %.2f , OverridePos = %.2f\n"), dCurr, dPos + dOffsetPos);
                    rtn = EziMOTIONPlusELib.FAS_VelocityOverride(m_iAxisID, changeVel);
                    if (rtn != EziMOTIONPlusELib.FMM_OK) return 100001 + (m_iAxisID * 1000);
                    break;
                }
                if (IsAxisDone()) break;
                if (_Tim.MoreThan(10.0)) return 100001 + (m_iAxisID * 1000);
            }

            if (GetAmpFault())
            {
                return -1;
            }
            return 0;

        }

       public int GetOriginPriority()
        {
            return m_iPrior;
        }

        public int GetOriginStep()
        {
            return m_iOriginStep;
        }

        public string GetAxisName()
        {
            return m_strAxisName;
        }
        public double GetCurrentPos()
        {
            int lActPos = 0;
           // EziMOTIONPlusELib.FAS_GetCommandPos(m_iAxisID, ref lActPos);
            // EziMOTIONPlusELib.FAS_GetActualPos(m_iPort, m_iAxisID, ref lActPos);
            EziMOTIONPlusELib.FAS_GetActualPos(m_iAxisID, ref lActPos);
            return (double)(lActPos / m_dScale);
        }
        public bool IsOrigin()
        {
            return (IsOriginOK());
            // return (m_bOriginFlag && IsOriginOK());
        }

        public bool IsOriginOK()
        {
            if (MSystem.SIMULATION) 
                return true;
            int iResult = 0;
            bool flag;
            if (EziMOTIONPlusELib.FMM_OK != (iResult = (short)EziMOTIONPlusELib.FAS_GetAxisStatus(m_iAxisID, ref m_pAxisManager.m_AxisStatus)))
            {
                return false;
            }
            flag = (m_pAxisManager.m_AxisStatus & 0x02000000) != 0 ? true : false;//ERRORALL
            if (flag)
                return true;
            else
                return false;
        }
        public int GetIOStatus(ref bool MinusLimit, ref bool PlusLimit, ref bool Home)
        {
            int iResult;
            bool flag1, flag2, flag3;
            if (EziMOTIONPlusELib.FMM_OK != (iResult = EziMOTIONPlusELib.FAS_GetAxisStatus(m_iAxisID, ref m_pAxisManager.m_AxisStatus)))
            {
                return iResult;
            }
            flag1 = (m_pAxisManager.m_AxisStatus & 0x00000004) != 0;//Limit-
            flag2 = (m_pAxisManager.m_AxisStatus & 0x00000002) != 0;//Limit+
            flag3 = (m_pAxisManager.m_AxisStatus & 0x00800000) != 0;//Home

            if (flag1) MinusLimit = true;
            else MinusLimit = false;
            if (flag2) PlusLimit = true;
            else PlusLimit = false;
            if (flag3) Home = true;
            else Home = false;
            return EziMOTIONPlusELib.FMM_OK;
        }
        public int JogMoveSlow(int dir)
        {
            uint speed = (uint)(m_sAxisInfo.dJogSlowSpeed * m_dScale);//m_sAxisInfo
            return EziMOTIONPlusELib.FAS_MoveVelocity(m_iAxisID, speed, dir);
        }

        public int JogMoveMid(int dir)
        {
            uint speed = (uint)(m_sAxisInfo.dJogMidSpeed * m_dScale);//m_sAxisInfo
            return EziMOTIONPlusELib.FAS_MoveVelocity(m_iAxisID, speed, dir);
        }

        public int JogMoveFast(int dir)
        {
            uint speed = (uint)(m_sAxisInfo.dJogFastSpeed * m_dScale);//m_sAxisInfo
            return EziMOTIONPlusELib.FAS_MoveVelocity(m_iAxisID, speed, dir);
        }

        public int OriginStop()
        {
            return EziMOTIONPlusELib.FMM_OK;
        }

        public void GetOriginError(ref int iError)
        {
            iError = m_iOriginError;
        }

        public double GetTargetPos()
        {
            int lCmdPos = 0;
            EziMOTIONPlusELib.FAS_GetCommandPos(m_iAxisID, ref lCmdPos);
            return (double)(lCmdPos / m_dScale);
        }

        public bool IsDetectSafetySensor(int axisid)
        {
            return false;
        }

        public int SetParameter(byte iParamNo, int lParamValue)
        {
            int iResult = 0;

            if (EziMOTIONPlusELib.FMM_OK != (iResult = EziMOTIONPlusELib.FAS_SetParameter(m_iAxisID, iParamNo, lParamValue)))
            {
                return iResult;
            }

            if (EziMOTIONPlusELib.FMM_OK != (iResult = EziMOTIONPlusELib.FAS_SaveAllParameters( m_iAxisID)))
            {
                return iResult;
            }

            return 0;
        }

        ////////////////////////////////////////////

        public int Origin_WaitDone()
        {
            int iSkipMode = 1;
            double dTolerance = 1.0;
            double dStabilityTime = 0.1;

            while (!IsAxisDone())
            {
                Thread.Sleep(10);
                if (GetAmpFault())
                {
                    Stop();
                    return -1;
                }
            }
            #region //Skip Mode
            if (iSkipMode != 0)
            {
                double dCurPos = GetCurrentPos();
                double dTarPos = GetTargetPos();
                int iRetryCount = 0;

                while (Math.Abs(dCurPos - dTarPos) > dTolerance)
                {
                    if (iRetryCount < 10)
                        Thread.Sleep((int)(dStabilityTime * 1000));
                    else if (iRetryCount >= 10 && iRetryCount < 20)
                        Thread.Sleep((int)(dStabilityTime * 5000));
                    else if (iRetryCount <= 20)
                        Thread.Sleep((int)(dStabilityTime * 10000));

                    if (iRetryCount > 30)
                    {
                        Stop();
                        return 100;
                    }
                    dCurPos = GetCurrentPos();
                    dTarPos = GetTargetPos();

                    iRetryCount++;
                    Thread.Sleep(1);
                }
            }
            #endregion
            return 0;//Sucess
        }

        ///////////////////////////////////////
        public int WaitDone()
        {
            bool bState = false;
            int iSkipMode = 1;//»ç¿ë
            double dTolerance = 0.5;//0.5
            double dStabilityTime = 0.1;
            Thread.Sleep(100);
            while (!IsAxisDone())
            {
                Thread.Sleep(10);

                if (GetAmpFault())
                {
                    Stop();
                    return -1;
                }
            }

            if (iSkipMode != 0)
            {
                double dCurPos = GetCurrentPos();
                double dTarPos = GetTargetPos();
                int iRetryCount = 0;

                while (Math.Abs(dCurPos - dTarPos) > dTolerance)
                {
                    if (iRetryCount < 50)
                        Thread.Sleep((int)(dStabilityTime * 200));
                    else if (iRetryCount >= 50 && iRetryCount < 70)
                        Thread.Sleep((int)(dStabilityTime * 5000));
                    else if (iRetryCount <= 80)
                        Thread.Sleep((int)(dStabilityTime * 10000));

                    if (iRetryCount > 80)
                    {
                        Stop();
                        return 100;
                    }

                    dCurPos = GetCurrentPos();
                    dTarPos = GetTargetPos();

                    iRetryCount++;
                    Thread.Sleep(1);
                }
            }

            return 0;//Sucess
        }

        public void GetAxisInfo(ref sAxisInfo pax1info)
        {
            pax1info = m_sAxisInfo;
        }

        public int SetAxisInfo(sAxisInfo pax1info)
        {
            m_sAxisInfo = pax1info;
            return 0;
        }


        //public void CFastechAxis::DoEvents()
        //{
        //    MSG message;

        //    if (::PeekMessage(&message, NULL, 0, 0, PM_REMOVE))
        //    {

        //::TranslateMessage(&message);

        //::DispatchMessage(&message);
        //    }
        //}

    }
}
