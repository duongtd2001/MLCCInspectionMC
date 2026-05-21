using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace MLCCInspectionMC
{
    public class InforManager : MSystem
    {
        private static InforManager instance;
        public static InforManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InforManager();
                }
                return instance;
            }
        }

        /*System data*/
        [JsonProperty]
        public string ModelName = "MODEL DEFAULT";
        public string Password = "7891";
        [JsonIgnore]
        public bool m_iDoor = true;
        [JsonIgnore]
        public bool m_iBuzzer = true;


        /*Comport*/
        [JsonProperty]
        public string LightPort = "COM2";
        /*Delay Time*/

        /*System Menager*/
        [JsonProperty]
        public bool ProductInfor = false;
        public void LoadSetting()
        {
            if (File.Exists(Config.SystemFilePath))
            {
                string _s = File.ReadAllText(Config.SystemFilePath, Encoding.UTF8);
                try
                {
                    instance = JsonConvert.DeserializeObject<InforManager>(_s);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SystemFile need to check" + ex.ToString());
                    SaveData();
                }
            }
            else
            {
                Directory.CreateDirectory(Config.SystemSavePath);
                SaveData();
            }
        }
        public void SaveData()
        {
            if (!Directory.Exists(Config.SystemSavePath))
                Directory.CreateDirectory(Config.SystemSavePath);
            if (!File.Exists(Config.SystemFilePath))
                using (var myfile = File.Create(Config.SystemFilePath)) { }
            try
            {
                string _sTestSpecString = JsonConvert.SerializeObject(Instance);
                string _sTestSpecStringIndeneted = JToken.Parse(_sTestSpecString).ToString(Formatting.Indented);
                File.WriteAllText(Config.SystemFilePath, _sTestSpecStringIndeneted);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
    public class Config
    {
        //Infor manager
        public static string FolderPath = @"C:\FA\MLCCInspectionMC\";
        public static string SystemSavePath = FolderPath + @"System\Manager\";
        public static string SystemFilePath = SystemSavePath + "SystemManager.json";

        //servo parameter
        public static string ServoSavePath = FolderPath + @"Infor\Axis\";
        public static string ServoFilePath = ServoSavePath + "axis.json";

        //Model position
        public static string ModelSavePath = FolderPath + @"Infor\Model\";
        public static string ModelFilePath = ModelSavePath + "MODEL DEFAULT.json";

        // GMES
        public static string DGSLogPath = @"C:\DGS\LOGS\";
        //Log Data
        public static string LogDevice = $@"{FolderPath}Log\DevLog\";
        public static string LogError = $@"{FolderPath}Log\Error\";
        public static string LogDataSave = $@"{FolderPath}Log\DataSave\";
        public static string LogBCRSave = $@"{FolderPath}Log\DataBCR\";
        public static string LogGMESSave = $@"{FolderPath}Log\DataGMESRun\";

        //Data Running
        public static string SysTemRunning = $@"{FolderPath}System\DataRunning\";
        //DataBase 
        public static string DataLogError = $@"{FolderPath}Log\Error\";
        public static string ProductLog = $@"{FolderPath}Log\Product\";
        //
        public static string PathLogGEIM = @"C:\FA\LOG\";
        public static string PathLogGEIM_D1 = @"C:\FA\LOG_D1\";
        public static string PathLogGEIM_C1 = @"C:\FA\LOG_C1\";
        // Vision Save
        public static string VS_IMG_PASS = @"D:\FA\HRMAssyVision\Image\";
        public static string VS_IMG_FAIL = @"D:\FA\HRMAssyVision\Image\";
        public static string VS_LOG = @"D:\FA\HRMAssyVision\LOG\";
    }
}
