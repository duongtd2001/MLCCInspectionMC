using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;
namespace MLCCInspectionMC
{
    public class LRW500
    {
        // Library for interface via ethernet
        public string IP = "169.254.0.10";
        public ushort _Port = 44818;
        EEIPClient LRW500Connect = new EEIPClient();
        public LRW500(string _IP = "169.254.0.10")
        {
            IP = _IP;
        }
        public int ReadCurrentvalue()
        {
            try
            {
                LRW500Connect.RegisterSession(IP, 44818);
                byte[] Data = LRW500Connect.GetAttributeSingle(0x66, 1, 0x325);
                if (Data.Length > 0)
                {
                    LRW500Connect.UnRegisterSession();
                    return (Data[1] * 255 + Data[0]);
                }
                else
                {
                    LRW500Connect.UnRegisterSession();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return -1;
            }

        }
        public int ReadCurrentvalue(int _Address)
        {
            try
            {
                LRW500Connect.RegisterSession(IP, 44818);
                byte[] Data = LRW500Connect.GetAttributeSingle(0x66, 1, _Address);
                if (Data.Length > 0)
                {
                    LRW500Connect.UnRegisterSession();
                    return (Data[1] * 255 + Data[0]);
                }
                else
                {
                    LRW500Connect.UnRegisterSession();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return -1;
            }
        }
        public byte[] ReadCurrentvalue2(int _AttributeID)
        {
            try
            {
                LRW500Connect.RegisterSession(IP, _Port);
                byte[] _Data = LRW500Connect.GetAttributeSingle(0x66, 1, _AttributeID);
                if (_Data.Length > 0)
                {
                    LRW500Connect.UnRegisterSession();
                    return _Data;
                }
                else
                {
                    LRW500Connect.UnRegisterSession();
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }
        public int SetValueTest(int _Value)
        {
            try 
            {
                LRW500Connect.RegisterSession(IP, _Port);
                byte[] _Result = LRW500Connect.SetAttributeSingle(0x66, 1, 833, new byte[] { (byte)(_Value % 255), (byte)(_Value/255) });
                LRW500Connect.UnRegisterSession();
                return 0;
            }
            catch (Exception ex) 
            {
                ex.ToString();
            }
            
            return -1;
        }
    }
}
