using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FormUnitStep : Form
    {
        //Timer _updateStep = new Timer();
        Thread _updateStep;
        public FormUnitStep()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);
            this.FormClosed += new FormClosedEventHandler(FormClose);
        }

        private void FormClose(object sender, FormClosedEventArgs e)
        {

        }

        
        private void BT_EXIT_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
