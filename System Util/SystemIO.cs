//#define YJ_IO
using FASTECH;
using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Threading;
namespace MLCCInspectionMC
{
    public class SystemIO
    {
        public static readonly object LockInput = new object();
        public static readonly object LockOutput = new object();

        #region IP for IO ethernet
        // Neu có nhieu slave thi tang chi so cuoi cua IPAdress
        public static IPAddress[] IP_Input = {
                                  new IPAddress(new byte[] { 192, 168, 0, 4 }),
                                 
                               };

        public static IPAddress[] IP_Output = {
                                  new IPAddress(new byte[] { 192, 168, 0, 4 }),
                                  
                                };
        public bool[] _inputConnect = new bool[IP_Input.Length];
        public bool[] _outputConnect = new bool[IP_Output.Length];
        public const int OUTPUTPIN = 16;
        #endregion

        #region VARIABLE LIBRARY FOR IO RS485
        string _DataIF = "";
        const int _input = 0;
        const int _output = 1;
        public static readonly object SyncData = new object();
        public bool m_iIOConnection = false;
        public bool Re_rmb = false;
        public TimerDelay LifeTime = new TimerDelay();
        public TimerDelay _TimScore = new TimerDelay();
        byte[] CMD_QQ = { 0x02, 0x51, 0x51, 0x03 };// - Command request check all modul
        #region Variable INOUT
        public const int MAX_SLAVE_COUNT = 16;
        public const int MAX_COUNT = 1;

        public const int MAX_IN_MODULE_COUNT = 0x04;
        public const int MAX_OUT_MODULE_COUNT = 0x04;
#if YJ_IO
        public const int MAX_IO = 16;
#else
        public const int MAX_IO = 32;
#endif
        public const int IO_SIZE = 4;

        public const int OUTPUT_MODULE = 2;
        public const int INPUT_MODULE = 1;

        public int[] m_iExistSlave = new int[MAX_SLAVE_COUNT * MAX_COUNT];
        public int[] m_iSlaveType = new int[MAX_SLAVE_COUNT * MAX_COUNT];
        public int[] m_iSlaveIndex = new int[MAX_SLAVE_COUNT * MAX_COUNT];

        public byte[] m_uInBuffer = new byte[MAX_SLAVE_COUNT * IO_SIZE * MAX_COUNT];
        public byte[] m_uOutBuffer = new byte[MAX_SLAVE_COUNT * IO_SIZE * MAX_COUNT];

        public bool[] m_bReplied = new bool[MAX_COUNT];

        public string[] m_strInName = new string[MAX_SLAVE_COUNT * 16 * MAX_COUNT];
        public string[] m_strOutName = new string[MAX_SLAVE_COUNT * 16 * MAX_COUNT];

        public int[] m_iInModuleCount = new int[MAX_COUNT];
        public int[] m_iOutModuleCount = new int[MAX_COUNT];

        byte[] m_bInt = new byte[MAX_COUNT];

        public SerialPort Serial;
        public string PortName = "";
        public string NameMC;
        public string m_iPort;
        public bool _bConnect;
        #endregion

        #endregion

        #region //Variable for IO Function
        public long countIF = 0;
        public long countRepare;
        bool b_reply = false;
        public int[] IO_Map = { 1, 1, 1, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int[] IO_Target = { 1, 1 };
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if YJ_IO
        public const byte MaxIN_Port = 16;
        public const byte MaxOut_Port = 16;
#else
        public const byte MaxIN_Port = 100;
        public const byte MaxOut_Port = 100;
#endif

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool[] InputDataBits;
        public bool[] OutputDataBits;
        public byte[] InputData = new byte[MAX_SLAVE_COUNT * IO_SIZE * MAX_COUNT]; // 64
        public byte[] OutputData = new byte[MAX_SLAVE_COUNT * IO_SIZE * MAX_COUNT]; // 64
        public static IO Input_Origin = (IO)1000;
        public static IO Output_Origin = (IO)2000;
        const int Output_End = 3000;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] bitOfMarks8 = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
        public static ushort[] bitOfMarks16 = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000, 0x8000 };
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum IO_RESULT
        {
            ERR_ID = 0,
            ERR_IO_SUCCESS
        };
        #endregion

        #region FUNCTION GET DATA IO

        public void SetOutput(IO number, bool _ONOFF)
        {
            if (MSystem.SIMULATION) return;
            //check range output data
            if (number < Output_Origin || number >= IO.OUT_MAX) return;
            //Process output data
            OutputDataBits[number - Output_Origin] = _ONOFF;
            return;
        }

        public void OutPutOn(IO number)
        {
            //check range output data
            if (number < Output_Origin || number >= IO.OUT_MAX) return;
            //Process output data
            OutputDataBits[number - Output_Origin] = true;
            return;
        }

        public void OutPutOff(IO number)
        {
            if (number < Output_Origin || number >= IO.OUT_MAX) return;
            //Process output data
            OutputDataBits[number - Output_Origin] = false;
            return;
        }

        public bool IsOn(IO number)
        {
            if (MSystem.m_pDIO == null) return false;
            //int bitIndex;
            //int Port;
            //int bitnum;
            //byte _data;
            try
            {
                if (number >= Input_Origin && number <= IO.IN_MAX)
                {
                    //bitIndex = number - Input_Origin;
                    //Port = bitIndex / 8;
                    //bitnum = bitIndex % 8;
                    //_data = (byte)(InputData[Port] & bitOfMarks8[bitnum]);

                    //if (_data > 0) return true;
                    //else return false;
                    return InputDataBits[number - Input_Origin];
                }
                else if (number >= Output_Origin && number <= IO.OUT_MAX)
                {
                    return OutputDataBits[number - Output_Origin];
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return false;
        }
        public bool IsOff(IO number)
        {
            if (MSystem.SIMULATION) return true;
            bool _value = false;
            if (MSystem.m_pDIO == null) return false;
            try
            {
                if (number >= Input_Origin && number <= IO.IN_MAX)
                {
                    _value = !InputDataBits[number - Input_Origin];
                }
                else if (number >= Output_Origin && number <= IO.OUT_MAX)
                {
                    _value = !OutputDataBits[number - Output_Origin];
                }
                return _value;
            }
            catch (Exception ex)
            {
                ex.ToString();
                MSystem.MyMessagerBottom("IO Logic Error");
            }
            return false;
        }
        public void UpdateDataOut()
        {
            for (int i = 0; i < IO.OUT_MAX - Output_Origin; i++)
            {
                if (OutputDataBits[i])
                    OutputData[i / 8] |= (byte)(1 << (i % 8));
            }

        }
        public void GetDataOutput()
        {
            try
            {
                int _leng = IO.OUT_MAX - Output_Origin;
                int numBytes = _leng / 8;

                if (_leng % 8 != 0) numBytes++;

                byte[] bytes = new byte[numBytes];

                for (int i = 0; i < _leng; i++)
                {
                    if (OutputDataBits[i])
                        bytes[i / 8] |= (byte)(1 << (i % 8));
                }
                Array.Copy(bytes, m_uOutBuffer, bytes.Length);
            }
            catch (Exception ex)
            {
                ex.ToString();
                MSystem.MyMessagerBottom("Get Data output not correct");
            }
        }
        public bool[] GetDataBitfromByte(byte[] bytes)
        {
            System.Collections.BitArray b = new System.Collections.BitArray(bytes);
            bool[] bitValues = new bool[b.Count];
            b.CopyTo(bitValues, 0);
            // Array.Reverse(bitValues);
            return bitValues;
        }
        #endregion

        public SystemIO()
        {

#if YJ_IO
            InputDataBits = new bool[MaxIN_Port * 2 * 8];
            OutputDataBits = new bool[MaxOut_Port * 2 * 8];
            NameMC = PortName;
            m_iPort = _Port;
            OpenPort(m_iPort, baurate);
            Initialize();
#else
            InputDataBits = new bool[(IP_Input.Length * 4 - 2) * 8];
            OutputDataBits = new bool[(IP_Input.Length * 4 - 2) * 8];
            ConnectionIP();
#endif
            Thread IOPOLL = new Thread(new ThreadStart(dorunStep));
            IOPOLL.Start();

            Thread CheckConnectPoll = new Thread(new ThreadStart(PollCheckConnection));
            CheckConnectPoll.Start();

        }
        public bool ConnectionInputIP(int index)
        {
            _bConnect = true;
            /******************************************************/
            //connect input
            byte[] addr = IP_Input[index].GetAddressBytes();
            if (!EziMOTIONPlusELib.FAS_Connect(IP_Input[index], addr[3]))
            {
                _bConnect = false;
                _inputConnect[index] = false;
            }
            else
            {
                _inputConnect[index] = true;
            }

            return _bConnect;
        }
        public bool ConnectionOutputIP(int index)
        {
            _bConnect = true;
            /******************************************************/
            //connect output
            byte[] addr = IP_Output[index].GetAddressBytes();
            if (!EziMOTIONPlusELib.FAS_Connect(IP_Output[index], addr[3]))
            {
                _bConnect = false;
                _outputConnect[index] = false;
            }
            else
                _outputConnect[index] = true;
            /******************************************************/
            return _bConnect;
        }
        public bool ConnectionIP()
        {
            _bConnect = true;
            /******************************************************/
            //connect input
            for (int i = 0; i < IP_Input.Length; i++)
            {
                byte[] addr = IP_Input[i].GetAddressBytes();
                if (!EziMOTIONPlusELib.FAS_Connect(IP_Input[i], addr[3]))
                {
                    _bConnect = false;
                    _inputConnect[i] = false;
                }
                else
                {
                    _inputConnect[i] = true;
                }
            }
            /******************************************************/
            //connect output
            //for (int i = 0; i <IP_Output.Length; i++)
            //{
            //    byte[] addr = IP_Output[i].GetAddressBytes();
            //    if (!EziMOTIONPlusELib.FAS_Connect(IP_Output[i], addr[3]))
            //    {
            //        _bConnect = false;
            //        _outputConnect[i] = false;
            //    }
            //    else
            //        _outputConnect[i] = true;

            //}
            /******************************************************/
            return _bConnect;
        }
        public void GetInputIP_R()
        {
            uint uInput = 0;
            uint uLatch = 0;
            int nBdID = 0;
            for (int i = 0; i < IP_Input.Length; i++)
            {
                uInput = 0;
                uLatch = 0;
                nBdID = IP_Input[i].GetAddressBytes()[3];

                if (EziMOTIONPlusELib.FAS_GetInput(nBdID, ref uInput, ref uLatch) != EziMOTIONPlusELib.FMM_OK)
                    _inputConnect[i] = false;
                else
                    _inputConnect[i] = true;
                if (_inputConnect[i])
                {
                    // *********** Neu chay Slave_0 16 bit, slave ke tiep chay 32 bit *********/
                    //if (i == 0)
                    //{
                    //    m_uInBuffer[i * 4 + 0] = (byte)(uInput);
                    //    m_uInBuffer[i * 4 + 1] = (byte)(uInput >> 8);
                    //}
                    //else
                    //{
                    //    m_uInBuffer[i * 4 + 0 - 2] = (byte)(uInput);
                    //    m_uInBuffer[i * 4 + 1 - 2] = (byte)(uInput >> 8);
                    //    m_uInBuffer[i * 4 + 2 - 2] = (byte)(uInput >> 16);
                    //    m_uInBuffer[i * 4 + 3 - 2] = (byte)(uInput >> 24);
                    //}

                    //********** Neu chay full salve 32bit ************/
                    m_uInBuffer[i * 4 + 0] = (byte)(uInput);
                    m_uInBuffer[i * 4 + 1] = (byte)(uInput >> 8);
                    m_uInBuffer[i * 4 + 2] = (byte)(uInput >> 16);
                    m_uInBuffer[i * 4 + 3] = (byte)(uInput >> 24);

                }
                //else 
                //{
                //    ConnectionInputIP(i);
                //}
            }
            Array.Copy(GetDataBitfromByte(m_uInBuffer), InputDataBits, InputDataBits.Length);
        }
        public void GetInputIP()
        {
            uint uInput = 0;
            uint uLatch = 0;
            int nBdID = 0;
            for (int i = 0; i < IP_Input.Length; i++)
            {
                uInput = 0;
                uLatch = 0;
                nBdID = IP_Input[i].GetAddressBytes()[3];

                if (EziMOTIONPlusELib.FAS_GetInput(nBdID, ref uInput, ref uLatch) != EziMOTIONPlusELib.FMM_OK)
                    _inputConnect[i] = false;
                else
                    _inputConnect[i] = true;
                if (_inputConnect[i])
                {
                    m_uInBuffer[i * 2 + 0] = (byte)(uInput);
                    m_uInBuffer[i * 2 + 1] = (byte)(uInput >> 8);
                }
            }
            Array.Copy(GetDataBitfromByte(m_uInBuffer), InputDataBits, InputDataBits.Length);
        }
        public void SetOutputIP()
        {
            uint uSetMask;
            uint uClrMask;
            int nBdID = 0;
            GetDataOutput();
            for (int i = 0; i < IP_Output.Length; i++)
            {
                nBdID = IP_Output[i].GetAddressBytes()[3];
                uSetMask = (uint)(m_uOutBuffer[i * 2 + 0] << 16 | (m_uOutBuffer[i * 2 + 1] << 24));
                /*********************************************************************/
                uClrMask = ~uSetMask;

                if (EziMOTIONPlusELib.FAS_SetOutput(nBdID, uSetMask, uClrMask) != EziMOTIONPlusELib.FMM_OK)
                    _outputConnect[i] = false;
                else
                    _outputConnect[i] = true;
            }
        }
        public bool OpenPort(string _Port, int baurate)
        {
            lock (SyncData)
            {
                try
                {
                    if (Serial != null)
                    {
                        if (Serial.IsOpen) Serial.Close();
                        Serial = null;
                    }
                    Serial = new SerialPort(_Port, baurate, Parity.None, 8, StopBits.One);
                    Serial.Handshake = Handshake.None;
                    Serial.ReadTimeout = 10;
                    Serial.WriteTimeout = 10;
                    Serial.NewLine = "\x03";
                    Serial.Open();
                    _bConnect = Serial.IsOpen;

                }
                catch (Exception e)
                {
                    e.ToString();
                    _bConnect = false;
                }
                return _bConnect;
            }
        }
        public void ClosePort()
        {
            lock (SyncData)
            {
                if (Serial != null)
                {
                    if (Serial.IsOpen) Serial.Close();
                    Serial = null;
                }
            }
        }
        public void Initialize()
        {
            Re_rmb = false;
            m_bReplied[0] = false;
            Serial.ReadTimeout = 1000;
            TimerDelay _tminit = new TimerDelay();
            if (Serial.IsOpen)
            {
                QuerySlave(0);
                _tminit.StartTimer();
                while (!m_bReplied[0])
                {
                    ReadDataReply();
                    if (_tminit.MoreThan(4.0))
                    {
                        Re_rmb = false;

                        return;
                    }
                    Thread.Sleep(20);
                }
                Thread.Sleep(500);
                IntReset(0, false);
                _tminit.StartTimer();
                while (!m_bReplied[0])
                {
                    ReadDataReply();
                    if (_tminit.MoreThan(4.0))
                    {
                        Re_rmb = false;
                        return;
                    }
                    Thread.Sleep(20);
                }
                Re_rmb = true;
                Serial.ReadTimeout = 50;
                ReadWrite(0);
            }
        }
        public void CheckSlaveConnect()
        {
#if YJ_IO
            bool ShowDialog = false;
            string _FindSlave = "";
            for (int i = 0; i < 16; i++)
            {
                if (IO_Map[i] == 0) continue;
                if (IO_Map[i] != GetSlaveType(i))
                {
                    if (IO_Map[i] == INPUT_MODULE)
                        _FindSlave += $"Module Input 0-{i}\r\n";
                    else
                        _FindSlave += $"Module Outnput 1-{i}\r\n";
                    ShowDialog = true;
                }
            }
            if (ShowDialog)
            {
                System.Windows.Forms.MessageBox.Show(_FindSlave, "CAN'T FIND SOME MODUL IO",
                                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
#else
            return;
#endif
        }
        public void QuerySlave(int index)
        {
            lock (SyncData)
            {
                m_bReplied[index] = false;
                Serial.Write("\x02QQ\x03");

                m_iInModuleCount[index] = 0;
                m_iOutModuleCount[index] = 0;
            }
        }
        public void IntReset(int index, bool flag)
        {
            m_bReplied[index] = false;
            string cmd = "IR00";
            Send(index, cmd);
        }
        public void AnalyzeDIOData(int port, byte[] strData)
        {
            try
            {
                int index = 0;
                int len = strData.Length;
                if (len > 1 && strData[0] == 0x02 && strData[len - 1] == 0x03)
                {
                    int count = 0;
                    switch (strData[1])
                    {
                        case 0x51: //'Q'
                            if (len < 3) break;
                            count = HexToInt(strData[2]) * 16 + HexToInt(strData[3]);
                            if (len < 6 + (count * 3) - 3) break;
                            for (int i = 0; i < count; i++)
                            {
                                SetModuleType(index, HexToInt(strData[4 + (i * 3)]),
                                    HexToInt(strData[6 + (i * 3)]), HexToInt(strData[5 + (i * 3)]));
                            }
                            break;
                        case 0x45: // Input Status - 'E'
                            b_reply = true;
                            if (len < 3) break;
                            count = HexToInt(strData[2]) * 16 + HexToInt(strData[3]);
                            if (len < 9 + (count * 6) - 3) break;
                            for (int i = 0; i < count; i++)
                            {
                                if (GetSlaveType(index * 16 + i) == INPUT_MODULE)
                                    UpdateIOStatus(index, HexToInt(strData[4 + (i * 6)]), 0,
                                        HexToInt(strData[5 + (i * 6)]),
                                        (byte)(HexToInt(strData[6 + (i * 6)]) * 16 + HexToInt(strData[7 + (i * 6)])),
                                        (byte)(HexToInt(strData[8 + (i * 6)]) * 16 + HexToInt(strData[9 + (i * 6)])));
                            }

                            countIF++;

                            break;
                        case 0x46: //'F'  // Input Status Ex | I/O 
                            b_reply = true;
                            if (len < 3) break;
                            count = (HexToInt(strData[2]) * 16) + (HexToInt(strData[3]) * 1);
                            if (len < 9 + (count * 6) - 3 + 2) break;
                            {
                                int incount = 0;
                                for (int i = 0; i < count; i++)
                                {
                                    int offset = 4 + (i * 6);
                                    if (GetSlaveType(index * 16 + i) == INPUT_MODULE)
                                    {
                                        UpdateIOStatus(index, HexToInt(strData[offset + 0]), 0,
                                            HexToInt(strData[offset + 1]),
                                            (byte)((HexToInt(strData[offset + 2]) * 16) + (HexToInt(strData[offset + 3]) * 1)),
                                            (byte)((HexToInt(strData[offset + 4]) * 16) + (HexToInt(strData[offset + 5]) * 1)));
                                        incount++;
                                    }
                                }
                                int intpos = 4 + (incount * 6) + 0;
                                byte intflag = (byte)((HexToInt(strData[intpos + 0]) * 16)
                                                      + (HexToInt(strData[intpos + 1]) * 1));
                                m_bInt[index] = intflag;
                                countIF++;
                            }
                            break;
                        case 0x49://'I' // Interrupt Reset
                            b_reply = true;
                            if (len < 4) break;
                            if (strData[2] == 'R')
                            {

                            }
                            break;
                    }
                    SetReply(index, HexToInt(strData[3]));

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                m_bReplied[0] = false;
                MSystem.MyMessagerBottom("Miss Frame With IO Modul");
            }

        }

        public void ReadWrite(int index)
        {
            string strCmd = (0x0F).ToString("X1") + m_iOutModuleCount[index].ToString("X2");
            string strSend = "";
            string strData = "";
            GetDataOutput();
            for (int i = 0; i < 16; i++)
            {
                if (GetSlaveType(index * 16 + i) == OUTPUT_MODULE)
                {
                    strSend = i.ToString("X1") + (GetSubIndex(index, i) % 16).ToString("X1") + m_uOutBuffer[GetSubIndex(index, i) * 2].ToString("X2")
                                + m_uOutBuffer[GetSubIndex(index, i) * 2 + 1].ToString("X2");
                    strData += strSend;
                }
            }

            strSend = strCmd + strData;

            Send(index, strSend);
            m_bReplied[0] = false;
            ReadDataReply();

        }
        void SetModuleType(int INDEX, byte slaveno, byte type, byte index)
        {
            //int cnt = 0;
            //bool isexist = false;

            m_iExistSlave[INDEX * 16 + slaveno] = 1;
            m_iSlaveType[INDEX * 16 + slaveno] = type == 1 ? 1 : 2;
            if (m_iSlaveType[INDEX * 16 + slaveno] == 1)
                m_iInModuleCount[INDEX]++;
            if (m_iSlaveType[INDEX * 16 + slaveno] == 2)
                m_iOutModuleCount[INDEX]++;

            switch (INDEX)
            {
                //ÀÔ·Â Æ÷Æ®°¡ 3°³...
                case 0: m_iSlaveIndex[INDEX * 16 + slaveno] = index; break;
                case 1: m_iSlaveIndex[INDEX * 16 + slaveno] = index + 16; break;
                case 2: m_iSlaveIndex[INDEX * 16 + slaveno] = index + 32; break;

                case 3: m_iSlaveIndex[INDEX * 16 + slaveno] = index; break;
                case 4: m_iSlaveIndex[INDEX * 16 + slaveno] = index + 16; break;

                default: m_iSlaveIndex[INDEX * 16 + slaveno] = index; break;
            }
        }
        public void UpdateIOStatus(int INDEX, byte slaveno, byte type, byte index, byte data1, byte data2)
        {
            if (type == 0)
            {
                byte _indexbuff = (byte)GetSubIndex(INDEX, slaveno);
                m_uInBuffer[_indexbuff * 2] = (byte)data1;
                m_uInBuffer[_indexbuff * 2 + 1] = (byte)data2;
                Array.Copy(GetDataBitfromByte(m_uInBuffer), InputDataBits, InputDataBits.Length);
                //Array.Copy(m_uInBuffer,InputData, m_uInBuffer.Length);
                m_iIOConnection = true;
            }
        }
        public int GetSubIndex(int index, int slaveno)
        {
            return m_iSlaveIndex[index * 16 + slaveno];
        }
        public int GetSlaveType(int nSlaveNo)
        {
            return m_iSlaveType[nSlaveNo];
        }
        public int IsExistSlave(int nSlaveNo)
        {
            return m_iExistSlave[nSlaveNo];
        }
        public byte HexToInt(byte ch)
        {
            if (ch >= '0' && ch <= '9')
                return (byte)(ch - 48);
            if (ch >= 'A' && ch <= 'F')
                return (byte)(ch - 65 + 10);
            return 255;
        }
        public void SetReply(int index, int slaveno)
        {
            m_bReplied[index] = true;
        }
        public int IntToHex(int v)
        {
            if (v >= 0 && v <= 9)
                return v + 30;
            if (v >= 10 && v <= 15)
                return v - 10 + 65;
            return -1;
        }
        public byte MakeCRC(string str)
        {
            int len = str.Length;
            int crc = 0;
            for (int i = 0; i < len; i++)
            {
                crc += str[i];
            }
            return (byte)(crc & 0x0f);
        }
        public void Send(int index, string str)
        {
            lock (SyncData)
            {
                byte crc = MakeCRC(str);
                str = '\x2' + str + crc.ToString("X1") + '\x3';
                Serial.Write(str);
            }
        }

        byte[] buffer = new byte[2048];
        byte[] ReceivedByte = new byte[2048];
        public int _index = 0;
        public int m_RetryRead = 0;
        public void ReadDataReply()
        {
            try
            {
                if (Serial.BytesToRead != 0)
                {
                    lock (SyncData)
                    {
                        _DataIF += Serial.ReadExisting();
                        if (_DataIF[_DataIF.Length - 1] == 0x03)
                        {
                            AnalyzeDIOData(0, ASCIIEncoding.ASCII.GetBytes(_DataIF));
                            _DataIF = "";
                        }
                        LifeTime.StartTimer();
                    }
                }
            }
            catch (TimeoutException)
            {
                Serial.DiscardInBuffer();
                m_bReplied[0] = false;
            }
            catch (IOException error)
            {
                string s = error.Message;

            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        public void PollCheckConnection()
        {
            while (MSystem.m_bLife)
            {
                //for (int i = 0; i < IP_Output.Length; i++)
                //{
                //    if (!_outputConnect[i])
                //    {
                //        ConnectionOutputIP(i);
                //    }
                //}
                for (int i = 0; i < IP_Input.Length; i++)
                {
                    if (!_inputConnect[i])
                    {
                        ConnectionInputIP(i);
                    }
                }
                Thread.Sleep(5);
            }
        }

        public void dorunStep()
        {
            while (MSystem.m_bLife)
            {
#if YJ_IO
                try
                {
                    ReadDataReply();
                    if (m_bReplied[0])
                    {
                        if (Re_rmb) 
                        {
                            ReadWrite(0);
                            m_RetryRead = 0;
                            LifeTime.StartTimer();
                        }
                    }
                    else
                    {
                        if (LifeTime.MoreThan(0.03) && Re_rmb)
                        {
                            ReadWrite(0);
                            LifeTime.StartTimer();
                            m_RetryRead++;
                        }
                        if (!Re_rmb || m_RetryRead > 5)
                        {
                            Thread.Sleep(1000);
                            Initialize();
                            if (m_bReplied[0])
                                CheckSlaveConnect();

                            LifeTime.StartTimer();
                        }
                        if (!_bConnect)
                        {
                            OpenPort(m_iPort, 115200);
                        } 
                    }         
                }
                catch(Exception) 
                {
                    MSystem.m_pLogSave.AddTail(LogIndex.eDeviceLog, "Logic Error IO Connection E1234");
                }
                Thread.Sleep(1);
#else
                try
                {
                    GetInputIP();
                    SetOutputIP();
                }
                catch (Exception)
                {

                }
#endif
                Thread.Sleep(1);
            }
        }
    }
}
