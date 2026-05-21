using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace MLCCInspectionMC
{
    public partial class FormTeach : Form
    {
        public FormTeach()
        {
            InitializeComponent();
            
            Control.CheckForIllegalCrossThreadCalls = false;
            //this.Location = new Point(FormMain.GetLocation().X + ((MSystem.Resolution_X - this.Width) / 2), FormMain.GetLocation().Y + ((MSystem.Resolution_Y - this.Height) / 2));
            this.Location = new Point(0,0);
            //////////////////////////////////////////////////////////////////////
            this.VisibleChanged += FormTeach_VisibleChanged;
        }
        private void FormTeach_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                MSystem.HideAllFormTeach(PanelTeach);
                MSystem.ArrPosForm.Visible = true;
                BT_TEACH_ARR_POS.Value = 1;
            }
        }

        private void FormTeach_FormClosing(object sender, FormClosingEventArgs e)
        {
            //MSystem.m_iRSTPass_Timer._TimStop = false;
            //MSystem.m_iRSTPass_Timer.ResetTimer();
            //MSystem.m_iRSTPass_Timer.StartTimer();
        }

        private void FormTeach_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MSystem.m_iRSTPass_Timer.StartTimer();
        }

        private void FormTeach_Load(object sender, EventArgs e)
        {
            MSystem.InitialMainFormTeach(PanelTeach);
            MSystem.HideAllFormTeach(PanelTeach);
            //MSystem.ArrPosForm
        }

        private void BT_TEACH_ARR_POS_ClickEvent(object sender, EventArgs e)
        {
            if (MSystem.ArrPosForm.Visible) return;
            MSystem.HideAllFormTeach(PanelTeach);
            MSystem.ArrPosForm.Visible = true;
        }

        private void BT_TECH_VISION_ClickEvent(object sender, EventArgs e)
        {
            if (MSystem.ArrPosForm.Visible) return;
            MSystem.HideAllFormTeach(PanelTeach);
            MSystem.ArrPosForm.Visible = true;
        }
    }
}
