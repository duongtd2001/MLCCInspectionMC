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
    public class InforProduct : MSystem
    {
        private static InforProduct instance;
        public static InforProduct Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InforProduct();
                }
                return instance;
            }
        }
        
        [JsonProperty]
        public long Total_product = 0;
        [JsonProperty]
        public long ProductPass = 0;
        [JsonProperty]
        public long ProductNG = 0;
        [JsonProperty]
        public long TrayOut = 0;

        [JsonProperty]
        public long ProductBCR_PASS = 0;
        public long ProductBCR_FAIL = 0;
        public long Product_GMES_FAIL = 0;
        public long Product_RemoveVinyl_FAIL = 0;
        public long Product_RemoveVinyl_FAIL_Lower = 0;

        [JsonProperty]
        public bool ProductInforShow = true;
        public void LoadSetting()
        {
            if (File.Exists(Config.SysTemRunning + "DataRunning.json"))
            {
                string _s = File.ReadAllText(Config.SysTemRunning + "DataRunning.json", Encoding.UTF8);
                try
                {
                    instance = JsonConvert.DeserializeObject<InforProduct>(_s);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SystemFile need to check" + ex.ToString());
                    SaveSettings();
                }
            }
            else
            {
                Directory.CreateDirectory(Config.SysTemRunning);
                SaveSettings();
            }
        }
        public void SaveSettings()
        {
            try
            {
                string _sTestSpecString = JsonConvert.SerializeObject(Instance);
                string _sTestSpecStringIndeneted = JToken.Parse(_sTestSpecString).ToString(Formatting.Indented);
                File.WriteAllText(Config.SysTemRunning + "DataRunning.json", _sTestSpecStringIndeneted);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public void ResetTactime()
        {
            lock (LockTactime)
            {
                _tactime.Clear();
            }

        }
        public void UpdateTactimeDisplay()
        {
            lock (LockTactime)
            {
                if (_tactime.Count > 50) _tactime.RemoveAt(0);
                if (_tactime.Count > 0)
                {
                    double total = 0;
                    foreach (double _tmp in _tactime)
                    {
                        total += _tmp;
                    }
                    total = total / _tactime.Count;
                   
                }
                else
                {
                    
                }
                DisplayString(AutoForm.TXT_TOTAL_PRODUCT, (InforProduct.Instance.ProductPass + InforProduct.Instance.ProductNG).ToString());
                DisplayString(AutoForm.TXT_PASS_PRODUCT, InforProduct.Instance.ProductPass.ToString());
                DisplayString(AutoForm.TXT_NG_PRODUCT, InforProduct.Instance.ProductNG.ToString());
            }
        }
    }
}
