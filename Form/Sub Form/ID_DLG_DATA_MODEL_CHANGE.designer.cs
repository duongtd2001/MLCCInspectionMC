namespace MLCCInspectionMC
{
    partial class FormModel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModel));
            this.colorButton3 = new SUserControls.ColorButton();
            this.LbModelList = new System.Windows.Forms.ListBox();
            this.colorButton1 = new SUserControls.ColorButton();
            this.colorButton2 = new SUserControls.ColorButton();
            this.colorButton4 = new SUserControls.ColorButton();
            this.TxModel = new SUserControls.ColorButton();
            this.TB_NEW_MODEL = new SUserControls.ColorButton();
            this.colorButton6 = new SUserControls.ColorButton();
            this.colorButton5 = new SUserControls.ColorButton();
            this.BtExit = new AxBTNENHLib4.AxBtnEnh();
            this.BtChange = new AxBTNENHLib4.AxBtnEnh();
            this.BtDelete = new AxBTNENHLib4.AxBtnEnh();
            this.BtCreate = new AxBTNENHLib4.AxBtnEnh();
            ((System.ComponentModel.ISupportInitialize)(this.BtExit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtChange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtCreate)).BeginInit();
            this.SuspendLayout();
            // 
            // colorButton3
            // 
            this.colorButton3.BackColor = System.Drawing.Color.Transparent;
            this.colorButton3.BorderLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colorButton3.Checked = false;
            this.colorButton3.CheckedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(238)))), ((int)(((byte)(160)))));
            this.colorButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.colorButton3.ForeColor = System.Drawing.Color.Black;
            this.colorButton3.GradientBottom = System.Drawing.Color.White;
            this.colorButton3.GradientTop = System.Drawing.Color.Transparent;
            this.colorButton3.Location = new System.Drawing.Point(7, 5);
            this.colorButton3.Name = "colorButton3";
            this.colorButton3.Size = new System.Drawing.Size(252, 525);
            this.colorButton3.TabIndex = 186;
            this.colorButton3.UseVisualStyleBackColor = false;
            // 
            // LbModelList
            // 
            this.LbModelList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.LbModelList.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbModelList.FormattingEnabled = true;
            this.LbModelList.ItemHeight = 23;
            this.LbModelList.Location = new System.Drawing.Point(9, 43);
            this.LbModelList.Name = "LbModelList";
            this.LbModelList.Size = new System.Drawing.Size(248, 487);
            this.LbModelList.TabIndex = 213;
            this.LbModelList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxModelList_DrawItem);
            this.LbModelList.SelectedIndexChanged += new System.EventHandler(this.LbModelList_SelectedIndexChanged);
            // 
            // colorButton1
            // 
            this.colorButton1.BackColor = System.Drawing.Color.Transparent;
            this.colorButton1.BorderLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colorButton1.Checked = false;
            this.colorButton1.CheckedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(238)))), ((int)(((byte)(160)))));
            this.colorButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.colorButton1.ForeColor = System.Drawing.Color.Black;
            this.colorButton1.GradientBottom = System.Drawing.Color.WhiteSmoke;
            this.colorButton1.GradientTop = System.Drawing.Color.WhiteSmoke;
            this.colorButton1.Location = new System.Drawing.Point(265, 5);
            this.colorButton1.Name = "colorButton1";
            this.colorButton1.Size = new System.Drawing.Size(416, 206);
            this.colorButton1.TabIndex = 214;
            this.colorButton1.UseVisualStyleBackColor = false;
            // 
            // colorButton2
            // 
            this.colorButton2.BackColor = System.Drawing.Color.Transparent;
            this.colorButton2.BorderLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colorButton2.Checked = false;
            this.colorButton2.CheckedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(238)))), ((int)(((byte)(160)))));
            this.colorButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorButton2.ForeColor = System.Drawing.Color.Black;
            this.colorButton2.GradientBottom = System.Drawing.Color.Gainsboro;
            this.colorButton2.GradientTop = System.Drawing.Color.WhiteSmoke;
            this.colorButton2.Location = new System.Drawing.Point(269, 10);
            this.colorButton2.Name = "colorButton2";
            this.colorButton2.Size = new System.Drawing.Size(102, 41);
            this.colorButton2.TabIndex = 216;
            this.colorButton2.Text = "CURRENT \r\nMODEL";
            this.colorButton2.UseVisualStyleBackColor = false;
            // 
            // colorButton4
            // 
            this.colorButton4.BackColor = System.Drawing.Color.Transparent;
            this.colorButton4.BorderLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colorButton4.Checked = false;
            this.colorButton4.CheckedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(238)))), ((int)(((byte)(160)))));
            this.colorButton4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorButton4.ForeColor = System.Drawing.Color.Red;
            this.colorButton4.GradientBottom = System.Drawing.Color.LightGray;
            this.colorButton4.GradientTop = System.Drawing.Color.LightGray;
            this.colorButton4.Location = new System.Drawing.Point(269, 107);
            this.colorButton4.Name = "colorButton4";
            this.colorButton4.Size = new System.Drawing.Size(403, 32);
            this.colorButton4.TabIndex = 233;
            this.colorButton4.Text = "Enter model name !";
            this.colorButton4.UseVisualStyleBackColor = false;
            // 
            // TxModel
            // 
            this.TxModel.BackColor = System.Drawing.Color.Transparent;
            this.TxModel.BorderLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TxModel.Checked = false;
            this.TxModel.CheckedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(238)))), ((int)(((byte)(160)))));
            this.TxModel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxModel.ForeColor = System.Drawing.Color.Black;
            this.TxModel.GradientBottom = System.Drawing.Color.White;
            this.TxModel.GradientTop = System.Drawing.Color.White;
            this.TxModel.Location = new System.Drawing.Point(377, 10);
            this.TxModel.Name = "TxModel";
            this.TxModel.Size = new System.Drawing.Size(295, 41);
            this.TxModel.TabIndex = 234;
            this.TxModel.UseVisualStyleBackColor = false;
            this.TxModel.Click += new System.EventHandler(this.TxModel_Click);
            // 
            // TB_NEW_MODEL
            // 
            this.TB_NEW_MODEL.BackColor = System.Drawing.Color.Transparent;
            this.TB_NEW_MODEL.BorderLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TB_NEW_MODEL.Checked = false;
            this.TB_NEW_MODEL.CheckedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(238)))), ((int)(((byte)(160)))));
            this.TB_NEW_MODEL.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_NEW_MODEL.ForeColor = System.Drawing.Color.Black;
            this.TB_NEW_MODEL.GradientBottom = System.Drawing.Color.White;
            this.TB_NEW_MODEL.GradientTop = System.Drawing.Color.White;
            this.TB_NEW_MODEL.Location = new System.Drawing.Point(377, 60);
            this.TB_NEW_MODEL.Name = "TB_NEW_MODEL";
            this.TB_NEW_MODEL.Size = new System.Drawing.Size(295, 41);
            this.TB_NEW_MODEL.TabIndex = 236;
            this.TB_NEW_MODEL.UseVisualStyleBackColor = false;
            this.TB_NEW_MODEL.Click += new System.EventHandler(this.TB_NEW_MODEL_Click);
            // 
            // colorButton6
            // 
            this.colorButton6.BackColor = System.Drawing.Color.Transparent;
            this.colorButton6.BorderLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colorButton6.Checked = false;
            this.colorButton6.CheckedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(238)))), ((int)(((byte)(160)))));
            this.colorButton6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorButton6.ForeColor = System.Drawing.Color.Black;
            this.colorButton6.GradientBottom = System.Drawing.Color.Gainsboro;
            this.colorButton6.GradientTop = System.Drawing.Color.WhiteSmoke;
            this.colorButton6.Location = new System.Drawing.Point(269, 60);
            this.colorButton6.Name = "colorButton6";
            this.colorButton6.Size = new System.Drawing.Size(102, 41);
            this.colorButton6.TabIndex = 235;
            this.colorButton6.Text = "NEW\r\nMODEL";
            this.colorButton6.UseVisualStyleBackColor = false;
            // 
            // colorButton5
            // 
            this.colorButton5.BackColor = System.Drawing.Color.Transparent;
            this.colorButton5.BorderLineColor = System.Drawing.Color.Blue;
            this.colorButton5.Checked = false;
            this.colorButton5.CheckedButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(238)))), ((int)(((byte)(160)))));
            this.colorButton5.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorButton5.ForeColor = System.Drawing.Color.Black;
            this.colorButton5.GradientBottom = System.Drawing.Color.Gainsboro;
            this.colorButton5.GradientTop = System.Drawing.Color.WhiteSmoke;
            this.colorButton5.Location = new System.Drawing.Point(9, 8);
            this.colorButton5.Name = "colorButton5";
            this.colorButton5.Size = new System.Drawing.Size(248, 34);
            this.colorButton5.TabIndex = 237;
            this.colorButton5.Text = "Model List";
            this.colorButton5.UseVisualStyleBackColor = false;
            // 
            // BtExit
            // 
            this.BtExit.Location = new System.Drawing.Point(508, 543);
            this.BtExit.Name = "BtExit";
            this.BtExit.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("BtExit.OcxState")));
            this.BtExit.Size = new System.Drawing.Size(173, 77);
            this.BtExit.TabIndex = 805;
            this.BtExit.ClickEvent += new System.EventHandler(this.BtExit_Click);
            // 
            // BtChange
            // 
            this.BtChange.Location = new System.Drawing.Point(547, 148);
            this.BtChange.Name = "BtChange";
            this.BtChange.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("BtChange.OcxState")));
            this.BtChange.Size = new System.Drawing.Size(125, 59);
            this.BtChange.TabIndex = 806;
            this.BtChange.ClickEvent += new System.EventHandler(this.BtChange_Click);
            // 
            // BtDelete
            // 
            this.BtDelete.Location = new System.Drawing.Point(408, 148);
            this.BtDelete.Name = "BtDelete";
            this.BtDelete.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("BtDelete.OcxState")));
            this.BtDelete.Size = new System.Drawing.Size(125, 59);
            this.BtDelete.TabIndex = 807;
            this.BtDelete.ClickEvent += new System.EventHandler(this.BtDelete_Click);
            // 
            // BtCreate
            // 
            this.BtCreate.Location = new System.Drawing.Point(269, 148);
            this.BtCreate.Name = "BtCreate";
            this.BtCreate.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("BtCreate.OcxState")));
            this.BtCreate.Size = new System.Drawing.Size(125, 59);
            this.BtCreate.TabIndex = 808;
            this.BtCreate.ClickEvent += new System.EventHandler(this.BtCreate_Click);
            // 
            // FormModel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(698, 632);
            this.Controls.Add(this.BtCreate);
            this.Controls.Add(this.BtDelete);
            this.Controls.Add(this.BtChange);
            this.Controls.Add(this.BtExit);
            this.Controls.Add(this.colorButton5);
            this.Controls.Add(this.TB_NEW_MODEL);
            this.Controls.Add(this.colorButton6);
            this.Controls.Add(this.TxModel);
            this.Controls.Add(this.colorButton4);
            this.Controls.Add(this.colorButton2);
            this.Controls.Add(this.colorButton1);
            this.Controls.Add(this.LbModelList);
            this.Controls.Add(this.colorButton3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1000, 800);
            this.MinimumSize = new System.Drawing.Size(700, 600);
            this.Name = "FormModel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Model Setting";
            this.Load += new System.EventHandler(this.FormModel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BtExit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtChange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtCreate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public SUserControls.ColorButton colorButton3;
        private System.Windows.Forms.ListBox LbModelList;
        public SUserControls.ColorButton colorButton1;
        public SUserControls.ColorButton colorButton2;
        public SUserControls.ColorButton colorButton4;
        public SUserControls.ColorButton TxModel;
        public SUserControls.ColorButton TB_NEW_MODEL;
        public SUserControls.ColorButton colorButton6;
        public SUserControls.ColorButton colorButton5;
        private AxBTNENHLib4.AxBtnEnh BtExit;
        private AxBTNENHLib4.AxBtnEnh BtChange;
        private AxBTNENHLib4.AxBtnEnh BtDelete;
        private AxBTNENHLib4.AxBtnEnh BtCreate;
    }
}