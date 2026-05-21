namespace MLCCInspectionMC
{
    partial class FormMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMode));
            this.btauto = new SUserControls.ColorButton();
            this.btdry = new SUserControls.ColorButton();
            this.btbypass = new SUserControls.ColorButton();
            this.BtExit = new AxBTNENHLib4.AxBtnEnh();
            this.ChangeMode = new AxBTNENHLib4.AxBtnEnh();
            ((System.ComponentModel.ISupportInitialize)(this.BtExit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChangeMode)).BeginInit();
            this.SuspendLayout();
            // 
            // btauto
            // 
            this.btauto.BackColor = System.Drawing.Color.Transparent;
            this.btauto.BorderLineColor = System.Drawing.Color.Gray;
            this.btauto.Checked = false;
            this.btauto.CheckedButtonColor = System.Drawing.Color.Black;
            this.btauto.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btauto.ForeColor = System.Drawing.Color.Black;
            this.btauto.GradientBottom = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btauto.GradientTop = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btauto.Location = new System.Drawing.Point(68, 26);
            this.btauto.Name = "btauto";
            this.btauto.RectCornerRadius = 5;
            this.btauto.Size = new System.Drawing.Size(206, 99);
            this.btauto.TabIndex = 214;
            this.btauto.Text = "Auto";
            this.btauto.UseVisualStyleBackColor = false;
            // 
            // btdry
            // 
            this.btdry.BackColor = System.Drawing.Color.Transparent;
            this.btdry.BorderLineColor = System.Drawing.Color.Gray;
            this.btdry.Checked = false;
            this.btdry.CheckedButtonColor = System.Drawing.Color.Black;
            this.btdry.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btdry.ForeColor = System.Drawing.Color.Black;
            this.btdry.GradientBottom = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btdry.GradientTop = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btdry.Location = new System.Drawing.Point(299, 26);
            this.btdry.Name = "btdry";
            this.btdry.RectCornerRadius = 5;
            this.btdry.Size = new System.Drawing.Size(206, 99);
            this.btdry.TabIndex = 215;
            this.btdry.Text = "Dry";
            this.btdry.UseVisualStyleBackColor = false;
            // 
            // btbypass
            // 
            this.btbypass.BackColor = System.Drawing.Color.Transparent;
            this.btbypass.BorderLineColor = System.Drawing.Color.Gray;
            this.btbypass.Checked = false;
            this.btbypass.CheckedButtonColor = System.Drawing.Color.Black;
            this.btbypass.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btbypass.ForeColor = System.Drawing.Color.Black;
            this.btbypass.GradientBottom = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btbypass.GradientTop = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.btbypass.Location = new System.Drawing.Point(529, 26);
            this.btbypass.Name = "btbypass";
            this.btbypass.RectCornerRadius = 5;
            this.btbypass.Size = new System.Drawing.Size(206, 99);
            this.btbypass.TabIndex = 222;
            this.btbypass.Text = "ByPass";
            this.btbypass.UseVisualStyleBackColor = false;
            // 
            // BtExit
            // 
            this.BtExit.Location = new System.Drawing.Point(409, 151);
            this.BtExit.Name = "BtExit";
            this.BtExit.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("BtExit.OcxState")));
            this.BtExit.Size = new System.Drawing.Size(173, 77);
            this.BtExit.TabIndex = 805;
            this.BtExit.ClickEvent += new System.EventHandler(this.BtExit_Click);
            // 
            // ChangeMode
            // 
            this.ChangeMode.Location = new System.Drawing.Point(230, 151);
            this.ChangeMode.Name = "ChangeMode";
            this.ChangeMode.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("ChangeMode.OcxState")));
            this.ChangeMode.Size = new System.Drawing.Size(173, 77);
            this.ChangeMode.TabIndex = 806;
            this.ChangeMode.ClickEvent += new System.EventHandler(this.ChangeMode_Click);
            // 
            // FormMode
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(797, 240);
            this.Controls.Add(this.ChangeMode);
            this.Controls.Add(this.BtExit);
            this.Controls.Add(this.btbypass);
            this.Controls.Add(this.btauto);
            this.Controls.Add(this.btdry);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Select Mode Run";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormOrigin_FormClosed);
            this.Load += new System.EventHandler(this.FormMode_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BtExit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChangeMode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public SUserControls.ColorButton btauto;
        public SUserControls.ColorButton btdry;
        public SUserControls.ColorButton btbypass;
        private AxBTNENHLib4.AxBtnEnh BtExit;
        private AxBTNENHLib4.AxBtnEnh ChangeMode;
    }
}