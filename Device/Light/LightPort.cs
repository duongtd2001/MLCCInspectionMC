using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLCCInspectionMC
{
    public class LightPort : MSystem
    {
        public SerialPort Serial;
        public string portName = "";
        public string m_iPortNo = "";
        public readonly object LockLightControl = new object();
        public bool _bConnect = false;
        public string _val = "0000";

        public LightPort(int _baurate, string _PortName)
        {
            MSystem.LoadParameter();
            portName = _PortName;
            m_iPortNo = InforManager.Instance.LightPort;
            if (OpenPort(m_iPortNo, _baurate))
                MSystem.MyMessagerBottom($"{_PortName} Open Comport Success ({m_iPortNo})");
            else
                //MSystem.MyMessagerBottom($"{_PortName} Open Comport Fail ({m_iPortNo})");

            Serial.DataReceived += DatarecivePort;
        }
        string strData = "";
        private void DatarecivePort(object sender, SerialDataReceivedEventArgs e)
        {
            lock (LockLightControl)
            {
                try
                {
                    {
                        strData += Serial.ReadExisting();
                        strData = "";
                    }
                }
                catch (Exception)
                {
                    MyMessagerBottom($"{portName} Recive Data Fail - Logic fail");
                }
            }
        }

        public bool OpenPort(string _Port, int baurate)
        {
            lock (LockLightControl)
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
                    Serial.ReadTimeout = 500;
                    Serial.WriteTimeout = 500;
                    Serial.NewLine = "\r\n";

                    Serial.Open();
                    _bConnect = Serial.IsOpen;
                    return Serial.IsOpen;
                }
                catch (Exception e)
                {
                    e.ToString();
                    _bConnect = false;
                    return false;
                }
            }
        }

        public string StringFormat(int channel, int val)
        {
            string ch = channels[channel - 1];
            _val = val.ToString("0000");

            return $"SL{ch}{_val}#";
        }

        public void SetLightOff(int _CH)
        {
            lock (LockLightControl)
            {
                Thread.Sleep(50);
                string str = string.Empty;
                if (Serial is null) return;
                if (Serial.IsOpen)
                {
                    Serial.Write(StringFormat(_CH, 0));
                }
                else
                {
                    MSystem.MyMessagerBottom("Light Port Not Open");
                    if (!Serial.IsOpen)
                    {
                        if (OpenPort(m_iPortNo, 115200))
                            MSystem.MyMessagerBottom($"{portName} Open Comport Success ({m_iPortNo})");
                        else
                            MSystem.MyMessagerBottom($"{portName} Open Comport Fail ({m_iPortNo})");
                        Serial.DataReceived += DatarecivePort;
                        Thread.Sleep(10);
                    }
                }
            }
        }

        public void SetLightOn(int _CH, int _Value)
        {
            lock (LockLightControl)
            {
                Thread.Sleep(50);
                string str = string.Empty;

                if (Serial.IsOpen)
                {
                    Serial.Write(StringFormat(_CH, _Value));
                }
                else
                {
                    MSystem.MyMessagerBottom("Light Port Not Open");

                    if (!Serial.IsOpen)
                    {
                        if (OpenPort(m_iPortNo, 115200))
                            MSystem.MyMessagerBottom($"{portName} Open Comport Success ({m_iPortNo})");
                        else
                            MSystem.MyMessagerBottom($"{portName} Open Comport Fail ({m_iPortNo})");

                        Serial.DataReceived += DatarecivePort;
                        Thread.Sleep(10);
                    }
                }
            }
        }

        public List<int> FormatMutilChannel()
        {
            List<int> channelList = new List<int>();
            for (int i = 0; i < channels.Length; i++)
            {
                channelList.Add(i + 1);
            }
            return channelList;
        }

        public void MutilChannelOFF()
        {
            foreach (int i in FormatMutilChannel())
            {
                SetLightOff(i);
                Thread.Sleep(5);
            }
        }

        public void MutilChannelON(int val)
        {
            foreach (int i in FormatMutilChannel())
            {
                SetLightOn(i, val);
                Thread.Sleep(5);
            }
        }
    }
}
