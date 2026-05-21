using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class ID_DLG_PASSWORD : Form
    {
        public ID_DLG_PASSWORD()
        {
            InitializeComponent();
            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);
            BT_OK.DialogResult = DialogResult.OK;
            BT_EXIT.DialogResult = DialogResult.Cancel;
           
        }

        private void buttonPress2_Click(object sender, EventArgs e)
        {
            TXT_PASSWORD.Text =  MSystem.GetPass();
        }
    }
}
