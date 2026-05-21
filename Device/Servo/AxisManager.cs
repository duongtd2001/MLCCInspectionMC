#define _UseEziPlusE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FASTECH;

namespace MLCCInspectionMC
{
    public partial class AxisManager : MSystem
    {
        public const double PI = 3.1415926535897;
        public static IPAddress[] IP_Servo = {
                                                new IPAddress(new byte[] { 192, 168, 0, 2 }),
                                                new IPAddress(new byte[] { 192, 168, 0, 3 }),
                                               

        };
        public FastechAxis[] m_pFastechAxis = new FastechAxis[(int)Axis.eAXIS_MAX];
        public bool m_bConnected = false;
        public bool m_bAllServoConnected = true;
        public int iAxiscount = 0;
        public uint m_AxisStatus;
        private byte m_iAxisID = 0;

        #region Axis Name
        public string[] strAxisName = {
            "AXIS X",
            "AXIS Y",
        };
        #endregion

        #region //Scale && Prior Data
        double[] scale_FAS = {
		                        (10000.0 / (10)*3),
                                  //Axis Load X		        /S5M * 19 = 200mm 
		                        (10000.0 / (10)),
                                  //Axis Load Y		        /S5M * 19 = 200mm 

        };
        int[] prior_FAS = {
                                0,  //Axis In lift		
                                0,  //Axis Out Lift
	    };
        #endregion

        #region //Variable of class 

        private byte m_iPort;
        public string PortName;
        #endregion


        #region //Initial Form
        public AxisManager(/*string port*/)
        {
            if (!ConnectionIP()) SIMULATION = true;
            try
            {
                if (Connect_FAS() == true)
                {
                    MSystem.MyMessagerBottom("Connect Servo OK");
                }
                else
                {
                    string str = "Some Servo Connection Fail";
                    MSystem.MyMessagerBottom(str);
                }
            }
            catch (Exception)
            {
                string str = "Some Servo Connection Fail";
                MSystem.MyMessagerBottom(str);
                MSystem.MyMsgMemo("Some Servo Connection Fail", "Error", msgButton.OK, msgIcon.Error);
            }
        }
        ~AxisManager()
        {
            try
            {
                Close_FAS();
            }
            catch (Exception)
            {

            }
        }
        public void Initial()
        {
            Thread _refreshConnectServo = new Thread(RefreshConnectServo);
            _refreshConnectServo.IsBackground = true;
            _refreshConnectServo.Start();

            if (Connect_FAS())
            {
                try
                {
                    for (byte i = 0; i < eAXIS_MAX; i++)
                    {
                        m_pFastechAxis[i] = new FastechAxis(strAxisName[i], i, scale_FAS[i], prior_FAS[i]);
                        m_pFastechAxis[i].SetAmpEnable(true);
                    }
                }
                catch
                {
                    if (!SIMULATION)
                    {
                        FMsgMemo.Show($"Some Servo Connection Fail.\r\nProgram Will Exit...", "Error", msgButton.OK, msgIcon.Error);
                        NumDisplay = (int)NumViewMain.eViewExit;
                    }

                }
            }
            else
            {
                if (!SIMULATION)
                {
                    FMsgMemo.Show($"Some Servo Connection Fail.\r\nProgram Will Exit...", "Error", msgButton.OK, msgIcon.Error);
                    NumDisplay = (int)NumViewMain.eViewExit;
                }

            }
        }
        #endregion


        #region //Connect
        public bool Connect_FAS()
        {
            if (m_bConnected) return true;
            try
            {
                for (int i = 0; i < eAXIS_MAX; i++)
                {
                    if (!EziMOTIONPlusELib.FAS_Connect(IP_Servo[i], i))
                    {
                        m_bConnected = false;
                        return false;
                    }
                }

            }
            catch (Exception)
            {
                m_bConnected = false;
                return false;
            }

            //slave check
            for (byte i = 0; i < eAXIS_MAX; i++)
            {
                if (EziMOTIONPlusELib.FAS_IsSlaveExist(i) == 0)
                {
                    m_bConnected = false;
                    return false;
                }
            }

            Thread.Sleep(200);
            m_bConnected = true;
            return true;
        }
        public bool ConnectionIP()
        {
            string failIP = string.Empty;
            bool _bConnect = true;
            /******************************************************/
            //connect input
            for (int i = 0; i < IP_Servo.Length; i++)
            {
                byte[] addr = IP_Servo[i].GetAddressBytes();
                if (!EziMOTIONPlusELib.FAS_Connect(IP_Servo[i], i))
                {
                    _bConnect = false;
                    failIP += $"{IP_Servo[i].ToString()} : {((Axis)i).ToString()}  {Environment.NewLine}";
                }
            }

            if (!_bConnect)
            {
                MessageBox.Show("CHANGE MODE TO RUN SIMULATION\n**************************************\n\nSome Servo Connection Fail : \n**************************************\n" + failIP, "Error", MessageBoxButton.OK);
            }
            /******************************************************/
            return _bConnect;
        }
        
        public void Close_FAS()
        {
            for (int i = 0; i < eAXIS_MAX; i++)
                EziMOTIONPlusELib.FAS_Close(i);
        }
        #endregion

        #region Reconnet when Disconnect
        public void RefreshConnectServo()
        {
            while (true)
            {
                Thread.Sleep(1000);

                for (byte i = 0; i < eAXIS_MAX; i++)
                {
                    //if(MSystem.SysStatus != StatusRun.RUN && !SIMULATION)
                    {
                        if (EziMOTIONPlusELib.FAS_IsSlaveExist(i) == 0  )
                        {
                            MyMessagerBottom("Disconnect to " + strAxisName[i]);
                            m_bAllServoConnected = false;
                            EziMOTIONPlusELib.FAS_Close(i);
                            Thread.Sleep(100);
                            EziMOTIONPlusELib.FAS_Connect(IP_Servo[i], i);
                        }
                        else
                        {
                            iAxiscount++;
                        }
                    }
                    
                }
                if (!m_bAllServoConnected && iAxiscount == MSystem.eAXIS_MAX)
                {
                    MyMessagerBottom("All Servos Reconnected!");
                    m_bAllServoConnected = true;
                }
                iAxiscount = 0;

            }
        }
        #endregion
    }
}