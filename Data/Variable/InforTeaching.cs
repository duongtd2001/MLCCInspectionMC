using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MLCCInspectionMC
{
    public class InforTeaching
    {
        private static InforTeaching instance;
        public static InforTeaching Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InforTeaching();
                }
                return instance;
            }
        }
        public int TrayRows = 0;
        public int TrayColumns = 0;

        public double[] m_dTechPos_StartXY = new double[2];
        public double[] m_dTechPos_EndXY = new double[2];
        public double[] m_dTechPos_In_Out_Tray = new double[2];
        public Dictionary<int, (double X, double Y)> m_dPos = new Dictionary<int, (double X, double Y)>();
        public int m_iModeTrigger = 0;

        public int[] m_iLight = new int[2];

        public void LoadSetting()
        {
            if (File.Exists(Config.ModelSavePath + InforManager.Instance.ModelName + ".json"))
            {
                string _s = File.ReadAllText(Config.ModelSavePath + InforManager.Instance.ModelName + ".json", Encoding.UTF8);
                try
                {
                    instance = JsonConvert.DeserializeObject<InforTeaching>(_s);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SystemFile need to check" + ex.ToString());
                }
            }
            else
            {
                Directory.CreateDirectory(Config.ModelSavePath);
                SaveSettings();
            }
        }
        public void SaveSettings()
        {
            if (!File.Exists(Config.ModelFilePath))
            {
                InforManager.Instance.ModelName = "MODEL DEFAULT";
                InforManager.Instance.SaveData();
                using (var myfile = File.Create(Config.ModelSavePath + InforManager.Instance.ModelName + ".json")) { }
            }
            try
            {
                string _sTestSpecString = JsonConvert.SerializeObject(Instance);
                string _sTestSpecStringIndeneted = JToken.Parse(_sTestSpecString).ToString(Formatting.Indented);
                File.WriteAllText(Config.ModelSavePath + InforManager.Instance.ModelName + ".json", _sTestSpecStringIndeneted);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public void SaveSettings(string filepath)
        {
            if (!Directory.Exists(Config.ModelSavePath))
                Directory.CreateDirectory(Config.ModelSavePath);

            try
            {
                string _sTestSpecString = JsonConvert.SerializeObject(Instance);
                string _sTestSpecStringIndeneted = JToken.Parse(_sTestSpecString).ToString(Formatting.Indented);
                File.WriteAllText($"{Config.ModelSavePath + filepath + ".json"}", _sTestSpecStringIndeneted);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }
        public string[] LoadAllModell()
        {
            DirectoryInfo d = new DirectoryInfo(Config.ModelSavePath);
            try
            {
                FileInfo[] Files = d.GetFiles("*.json");
                string str = "";
                if (Files.Length > 0)
                {
                    foreach (FileInfo file in Files)
                    {
                        str = str + "," + file.Name;
                    }
                    string[] strModel = str.Split(',');
                    return strModel;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

    }
}
