using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FormModel : Form
    {
        public string NameCurrentModel = "";
        private string CopyModelName = "";

        #region //Form Initial
        public FormModel()
        {
            InitializeComponent();
            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);
            //this.BtExit.DialogResult = System.Windows.Forms.DialogResult.OK;

            //LoadData();
        }
        private void FormModel_Load(object sender, EventArgs e)
        {
            LoadAllModel();
        }
        #endregion

        #region //Event Form
        private void listBoxModelList_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (!string.IsNullOrEmpty(CopyModelName))
            {
                if (CopyModelName == LbModelList.Items[e.Index].ToString())
                {
                    e.Graphics.FillRectangle(Brushes.HotPink, e.Bounds);
                }
            }

            if ((DrawItemState.Selected & e.State) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(Brushes.MediumSpringGreen, e.Bounds);
            }

            using (SolidBrush solid = new SolidBrush(e.ForeColor))
            {
                if (e.Index != -1)
                {
                    SizeF size = e.Graphics.MeasureString(LbModelList.Items[e.Index].ToString(), e.Font);
                    //e.Graphics.DrawString(listBoxModelList.GetItemText(listBoxModelList.Items[e.Index].ToString()), e.Font, solid, e.Bounds);
                    e.Graphics.DrawString(LbModelList.GetItemText(LbModelList.Items[e.Index].ToString()), e.Font, solid, e.Bounds.Left + (e.Bounds.Width / 2 - size.Width / 2), e.Bounds.Top + (e.Bounds.Height / 2 - size.Height / 2));
                }

            }
        }
        #endregion
        private void BtExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void LbModelList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LbModelList.SelectedIndex < 0) return;
            TB_NEW_MODEL.Text = LbModelList.Items[LbModelList.SelectedIndex].ToString();
            if (TB_NEW_MODEL.Text.Trim() == InforManager.Instance.ModelName)
                TB_NEW_MODEL.Text = "";
            TxModel.Text = InforManager.Instance.ModelName;
        }
        private void LoadAllModel() {
            string[] strmodel = InforTeaching.Instance.LoadAllModell();
            if (strmodel != null && strmodel.Length > 0) {
                LbModelList.Items.Clear();
                foreach (string _tmp in strmodel) {
                    if (_tmp == "") continue;
                    LbModelList.Items.Add(_tmp.Replace(".json","").Trim());
                }
            }
            if (LbModelList.Items.Contains(InforManager.Instance.ModelName)) {
                TxModel.Text = InforManager.Instance.ModelName;
                LbModelList.SelectedIndex = LbModelList.Items.IndexOf(TxModel.Text);
            }

        }
        private void BtChange_Click(object sender, EventArgs e)
        {
            if (LbModelList.SelectedIndex < 0) return;
            /*********************************************************************************************/
            if (LbModelList.Items[LbModelList.SelectedIndex].ToString() == InforManager.Instance.ModelName)
            {
                MSystem.MyMsgMemo("Current model is model selected", "Memo", msgButton.OK, msgIcon.Infor);
                return;
            }
            /**********************************************************************************************/
            try {
                InforManager.Instance.ModelName = LbModelList.Items[LbModelList.SelectedIndex].ToString().Trim();
                InforManager.Instance.SaveData();
                InforTeaching.Instance.LoadSetting();
            }
            catch (Exception ex) {
                ex.ToString();
            }
            TxModel.Text = InforManager.Instance.ModelName;
            TB_NEW_MODEL.Text = "";
            MSystem.MyMsgMemo("Change Model Okey! Please init All data", "Memo", msgButton.OK, msgIcon.Infor);

        }
        private void BtDelete_Click(object sender, EventArgs e)
        {
            if (LbModelList.SelectedIndex < 0) return;
            /*********************************************************************************************/
            if (LbModelList.Items[LbModelList.SelectedIndex].ToString() == InforManager.Instance.ModelName)
            {
                MSystem.MyMsgMemo("You Can't Delete Current Model", "Memo", msgButton.OK, msgIcon.Infor);
                return;
            }
            /*********************************************************************************************/
            string filepath = $"{Config.ModelSavePath}{LbModelList.Items[LbModelList.SelectedIndex].ToString()}.json";
            if (DialogResult.Yes != MSystem.MyMsgMemo("Do you want delete this model?", "Memo", msgButton.YESNO, msgIcon.Question)) {
                return;
            }
            if (File.Exists(filepath)) {
                File.Delete(filepath);
            }
            LoadAllModel();
            TB_NEW_MODEL.Text = "";
        }
        private void TxModel_Click(object sender, EventArgs e)
        {
           
        }
        private void BtCreate_Click(object sender, EventArgs e)
        {
            if (TB_NEW_MODEL.Text.Trim() == "") return;
            if (TB_NEW_MODEL.Text.Trim() == InforManager.Instance.ModelName)
            {
                MSystem.MyMsgMemo("This model is exited!!", "Memo", msgButton.OK, msgIcon.Infor);
            }
            else {
                if (DialogResult.Yes != MSystem.MyMsgMemo($"Do you want create model {TB_NEW_MODEL.Text.Trim()}??", "Question", msgButton.YESNO, msgIcon.Infor)) {
                    return;
                }
                InforTeaching.Instance.SaveSettings(TB_NEW_MODEL.Text.Trim());
                LoadAllModel();
                TB_NEW_MODEL.Text = "";
                MSystem.MyMsgMemo("Create Model complete !!", "Memo", msgButton.OK, msgIcon.Infor);
            }
        }
        private void TB_NEW_MODEL_Click(object sender, EventArgs e)
        {
            MSystem.GetCharKeyboard(sender);
        }
    }
}
