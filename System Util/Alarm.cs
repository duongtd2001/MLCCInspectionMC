using MLCCInspectionMC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public class Alarm : MSystem
    {
        public string path = $"{MSystem.Root}Data\\";
        public string fileName = "ErrorMessage.ini";
        public string PathFile = "";
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        public List<string[]> AlarmList = new List<string[]>();
        #region//Read Alarm Messager
        public Alarm()
        {
            PathFile = $"{path}{fileName}";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!File.Exists(PathFile)) File.Create(PathFile);
        }

        public void ReadError(string _SelectKey, ref string[] _msgError)
        {
            StringBuilder[] temp = { new StringBuilder(255), new StringBuilder(255), new StringBuilder(255), new StringBuilder(255), new StringBuilder(255), new StringBuilder(255) };

            int Result = GetPrivateProfileString(_SelectKey, $"messager", "", temp[0], 255, PathFile);
            int Result1 = GetPrivateProfileString(_SelectKey, $"messager2", "", temp[1], 255, PathFile);
            int Result2 = GetPrivateProfileString(_SelectKey, $"messager3", "", temp[2], 255, PathFile);

            int Result3 = GetPrivateProfileString(_SelectKey, $"recovery1", "", temp[3], 255, PathFile);
            int Result4 = GetPrivateProfileString(_SelectKey, $"recovery2", "", temp[4], 255, PathFile);
            int Result5 = GetPrivateProfileString(_SelectKey, $"recovery3", "", temp[5], 255, PathFile);

            string resion = (Result != 0) ? (temp[0].ToString()) : ("");
            resion += "\r\n" + ((Result1 != 0) ? (temp[1].ToString()) : (""));
            resion += "\r\n" + ((Result2 != 0) ? (temp[2].ToString()) : (""));

            string action = (Result3 != 0) ? (temp[3].ToString()) : ("");
            action += "\r\n" + ((Result4 != 0) ? (temp[4].ToString()) : (""));
            action += "\r\n" + ((Result5 != 0) ? (temp[5].ToString()) : (""));

            _msgError[0] = resion;
            _msgError[1] = action;
        }
        public void LoadAlarmList(string Path)
        {
            bool tmp;
            if (!File.Exists(Path))
            {
                System.Windows.Forms.MessageBox.Show("Can't find Alarm List File ..... ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Load Alarm List
            string[] line = File.ReadAllLines(Path, Encoding.Unicode);
            for (int i = 0; i < line.Length; i++)
            {
                foreach (char ckeckstr in line[i])
                {
                    if (line[i] == "") break;
                    if (ckeckstr.ToString() == "[")
                    {
                        tmp = true;
                        string[] strListAlarm = new string[5];
                        for (int j = 0; j < 5; j++)
                        {
                            if ((i + j) < line.Length)
                            {
                                if (line[i + j] == "") break;
                                if (line[i + j][0] == '[' && !tmp) break;
                                strListAlarm[j] = line[i + j].Trim();
                                tmp = false;
                            }
                            //////////////////////////////////////
                        }
                        AlarmList.Add(strListAlarm);
                    }
                    else
                    {
                        continue;
                    }
                    break;
                }
            }
        }
        public void SetErrorMsg(long number, string strunit)
        {
            lock (LockInsertAlarm)
            {
                if (SysStatus == StatusRun.ERROR || (SysStatus == StatusRun.STOP))
                    return;

                SysStatus = StatusRun.ERROR;

                string[] _MsgError = new string[2];
                ReadError($"{number}", ref _MsgError);
               
            }
        }
        public void InserLogAlarm(string Path, string[] content, string NameFile)
        {
            lock (LockInsertAlarm)
            {
                string fileFolder = Path + "/" + DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM") + "/" + DateTime.Now.ToString("dd") + "/";
                string filePath = fileFolder + DateTime.Now.ToString("yyyyMMdd_HH_") + NameFile + ".xlsx";
                if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            }
        }
        #endregion
    }
}
