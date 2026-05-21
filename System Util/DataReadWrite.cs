using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MLCCInspectionMC
{
    public class DataReadWrite
    {
        public string path;
        public string fileName;
        public string PathFile = "";
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        public DataReadWrite(string _path, string _fileName)
        {
            path = _path;
            fileName = _fileName;
            PathFile = path + fileName;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public void SaveString(string _SelectKey, string _param, string _value)
        {
            WritePrivateProfileString(_SelectKey, _param, _value, PathFile);
        }

        public void ReadString(string _SelectKey, string _param, ref string _value, string _Defaut = "")
        {
            StringBuilder temp = new StringBuilder(255);
            int Result = GetPrivateProfileString(_SelectKey, _param, "", temp, 255, PathFile);

            if (Result == 0)
                _value = _Defaut;
            else
                _value = temp.ToString();
        }

        public void SaveInt(string _SelectKey, string _param, int Value) 
        {
            WritePrivateProfileString(_SelectKey, _param, $"{Value.ToString()}", PathFile);
        }

        public void GetInt(string _SelectKey, string _param, ref int Value,int _default = 0)
        {
            StringBuilder temp = new StringBuilder(255);
            int Result = GetPrivateProfileString(_SelectKey, _param, "", temp, 255, PathFile);

            if (Result == 0)
                Value = _default;
            else 
            {
                int tmp = 0;
                if (int.TryParse(temp.ToString(), out tmp)) Value = tmp;
            }    
        }

        public void SaveFloat(string _SelectKey, string _param, float Value)
        {
            WritePrivateProfileString(_SelectKey, _param, $"{Value.ToString("f03")}", PathFile);
        }
        public void GetFloat(string _SelectKey, string _param, ref float Value, float _default = 0.00f)
        {
            StringBuilder temp = new StringBuilder(255);
            int Result = GetPrivateProfileString(_SelectKey, _param, "", temp, 255, PathFile);

            if (Result == 0)
                Value = _default;
            else
            {
                float tmp = 0;
                if (float.TryParse(temp.ToString(), out tmp)) Value = tmp;
            }
        }

        public void SaveDouble(string _SelectKey, string _param, double Value)
        {
            WritePrivateProfileString(_SelectKey, _param, $"{Value.ToString("f03")}", PathFile);
        }
        public void GetDouble(string _SelectKey, string _param, ref double Value, double _default = 0.00f)
        {
            StringBuilder temp = new StringBuilder(255);
            int Result = GetPrivateProfileString(_SelectKey, _param, "", temp, 255, PathFile);

            if (Result == 0)
                Value = _default;
            else
            {
                double tmp = 0;
                if (double.TryParse(temp.ToString(), out tmp)) Value = tmp;
            }
        }

    }
}
