using System.Windows.Forms;

namespace MLCCInspectionMC
{
    partial class FormTeach : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTeach));
            this.PanelTeach = new System.Windows.Forms.Panel();
            this.BT_TEACH_ARR_POS = new AxBTNENHLib4.AxBtnEnh();
            ((System.ComponentModel.ISupportInitialize)(this.BT_TEACH_ARR_POS)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelTeach
            // 
            this.PanelTeach.BackColor = System.Drawing.Color.White;
            this.PanelTeach.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.PanelTeach.Location = new System.Drawing.Point(0, 34);
            this.PanelTeach.Name = "PanelTeach";
            this.PanelTeach.Size = new System.Drawing.Size(1280, 800);
            this.PanelTeach.TabIndex = 790;
            // 
            // BT_TEACH_ARR_POS
            // 
            this.BT_TEACH_ARR_POS.Location = new System.Drawing.Point(0, 0);
            this.BT_TEACH_ARR_POS.Name = "BT_TEACH_ARR_POS";
            this.BT_TEACH_ARR_POS.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("BT_TEACH_ARR_POS.OcxState")));
            this.BT_TEACH_ARR_POS.Size = new System.Drawing.Size(1280, 35);
            this.BT_TEACH_ARR_POS.TabIndex = 802;
            this.BT_TEACH_ARR_POS.ClickEvent += new System.EventHandler(this.BT_TEACH_ARR_POS_ClickEvent);
            // 
            // FormTeach
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1280, 835);
            this.Controls.Add(this.BT_TEACH_ARR_POS);
            this.Controls.Add(this.PanelTeach);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTeach";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormLog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTeach_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTeach_FormClosed);
            this.Load += new System.EventHandler(this.FormTeach_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BT_TEACH_ARR_POS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Panel PanelTeach;
        private AxBTNENHLib4.AxBtnEnh BT_TEACH_ARR_POS;
    }
}