using OpenCvSharp.Dnn;
using stdole;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static OpenCvSharp.Stitcher;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace MLCCInspectionMC
{
    public partial class FormAuto : Form
    {
        public Color _clON = Color.Lime;
        public Color _clOFF = Color.Gainsboro;
        public Color _CheckOK = Color.Gainsboro;
        public Color _CheckNG = Color.Red;
        private int[] displayNumberMap;
        #region //Timer Update Status
        //System.Windows.Forms.Timer TimUpdate = new System.Windows.Forms.Timer();
        Thread TimUpdate;//= new Thread(new ThreadStart(UpDateData));
        #endregion

        PictureBox[,] m_btPostBGLASS = new PictureBox[4, 4];
        #region // Form Init
        public FormAuto()
        {
            InitializeComponent();
            initTrayMap();
            Control.CheckForIllegalCrossThreadCalls = false;
            Load += new EventHandler(LoadData);

            TimUpdate = new Thread(UpDateData);
            TimUpdate.IsBackground = true;
            TimUpdate.Start();
            if (InforManager.Instance.ProductInfor)
            {
                IDC_PL_INFO.Visible = true;
                MAIN_BT_PRODUCT.Visible = true;
                IDC_STT_LABEL_OUTPUT_TIME.Visible = true;
            }

            else
            {
                IDC_PL_INFO.Visible = false;
                MAIN_BT_PRODUCT.Visible = false;
                IDC_STT_LABEL_OUTPUT_TIME.Visible = false;
            }
        }
        #region Table Data
        public void InitializeTLayoutPanel(TableLayoutPanel tbllp, int rowcount, int colcount)
        {
            tbllp.Visible = false;
            tbllp.Controls.Clear();

            tbllp.ColumnStyles.Clear();
            tbllp.RowStyles.Clear();

            tbllp.ColumnCount = colcount;
            tbllp.RowCount = rowcount;

            float collpcent = colcount / 100.0f;
            float rowpcent = rowcount / 100.0f;

            tbllp.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tbllp.AutoScroll = true;
            tbllp.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            for (int col = 0; col <= tbllp.ColumnCount - 1; col++)
            {
                tbllp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, collpcent));

                for (int rows = 0; rows <= tbllp.RowCount - 1; rows++)
                {
                    tbllp.RowStyles.Add(new ColumnStyle(SizeType.Percent, rowpcent));
                    MyButton.ButtonPress _bt = new MyButton.ButtonPress();
                    _bt.GradientTop = Color.Gainsboro;
                    _bt.GradientBottom = Color.WhiteSmoke;
                    _bt.BorderLineColor = Color.Blue;
                    _bt.ColorPress = _clON;
                    _bt.Font = new System.Drawing.Font("Arial", 9, FontStyle.Bold);
                    string _name = rows.ToString() + col.ToString();
                    string _text = (InforTeaching.Instance.TrayRows - rows).ToString() + (col + 1).ToString();
                    _bt.Name = _name;
                    _bt.Text = _text;
                    _bt.Size = new Size(10, 10);
                    _bt.RectCornerRadius = 3;
                    _bt.BorderLineColor = Color.Goldenrod;
                    _bt.Dock = DockStyle.Fill;
                    tbllp.Controls.Add(_bt, col, rows);
                }
            }
            tbllp.Visible = true;
        }

        public void SetColor(MyButton.ButtonPress buttonPress, bool _ON)
        {
            buttonPress.ColorPress = _clON;
            if (_ON)
            {
                buttonPress.GradientBottom = _clON;
                buttonPress.GradientTop = _clON;
                buttonPress.SetPress = true;
            }
            else
            {
                buttonPress.GradientTop = Color.Gainsboro;
                buttonPress.GradientBottom = Color.WhiteSmoke;
                buttonPress.SetPress = false;
            }
        }

        public void SetColorNG(MyButton.ButtonPress buttonPress, bool _ON)
        {
            buttonPress.ColorPress = _CheckNG;
            if (_ON)
            {
                buttonPress.SetPress = true;
                buttonPress.GradientBottom = _CheckNG;
                buttonPress.GradientTop = _CheckNG;
            }
            else
            {
                buttonPress.GradientTop = Color.Gainsboro;
                buttonPress.GradientBottom = Color.WhiteSmoke;
                buttonPress.SetPress = false;
            }
        }
        public void SetColorOK(MyButton.ButtonPress buttonPress, bool _ON)
        {
            buttonPress.ColorPress = _CheckOK;
            if (_ON)
            {
                buttonPress.SetPress = true;
                buttonPress.GradientBottom = _CheckNG;
                buttonPress.GradientTop = _CheckNG;
            }
            else
            {
                buttonPress.GradientTop = Color.Gainsboro;
                buttonPress.GradientBottom = Color.WhiteSmoke;
                buttonPress.SetPress = false;
            }
        }
        #endregion
        void DisplayMode()
        {
            if (MSystem.SysMode == ModeRun.Auto)
            {
                if (MAIN_BT_SLECT_MODE.Caption != "Auto")
                    MAIN_BT_SLECT_MODE.Caption = "Auto";
            }
            else if (MSystem.SysMode == ModeRun.DryRun)
            {
                if (MAIN_BT_SLECT_MODE.Caption != "Dry")
                    MAIN_BT_SLECT_MODE.Caption = "Dry";
            }
            else if (MSystem.SysMode == ModeRun.ByPass)
            {
                if (MAIN_BT_SLECT_MODE.Caption != "Bypass")
                    MAIN_BT_SLECT_MODE.Caption = "Bypass";
            }
            else if (MSystem.SysMode == ModeRun.ByPassTest)
            {
                if (MAIN_BT_SLECT_MODE.Caption != "Bypass Deco")
                    MAIN_BT_SLECT_MODE.Caption = "Bypass Deco";
            }
        }
        private void LoadData(object sender, EventArgs e)
        {

        }
        private void FormAuto_VisibleChanged(object sender, EventArgs e)
        {

        }
        private void UpDateData()
        {
            while (MSystem.m_bLife)
            {
                Thread.Sleep(50);
                try
                {
                    DisplayMode();
                    /*******************************************/
                    UpdateProductDisplay();
                    /*******************************************/
                    InforProduct.Instance.UpdateTactimeDisplay();
                    /*******************************************/
                    DisplayBCR();
                    /*******************************************/
                    DisplayTimePress();
                    /*******************************************/
                    DisplayPressMCStatus();

                }
                catch (Exception)
                {
                    Thread.Sleep(10);
                }

            }
        }
        public void DisplayPressMCStatus()
        {

        }
        public void DisplayBCR()
        {

            //MSystem.DisPlayString(IDC_SHUTTLE_REAR_BCR_1, MSystem.m_pTrsShuttle[MSystem.eREAR]._Infor[MSystem.eLEFT].BCR_Right);

        }
        public void DisplayTimePress()
        {

        }

        int _dateold;
        void UpdateProductDisplay()
        {
            if ((DateTime.Now.Hour == 8 || DateTime.Now.Hour == 20) && DateTime.Now.Second < 10 && DateTime.Now.Minute == 0)
            {
                if (_dateold != DateTime.Now.Hour)
                {
                    InforProduct.Instance.ProductPass = 0;
                    InforProduct.Instance.ProductNG = 0;
                    _dateold = DateTime.Now.Hour;
                }
            }
        }
        #endregion
        public static bool IsUnicode(string input)
        {
            var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
            var unicodBytesCount = Encoding.UTF8.GetByteCount(input);
            return asciiBytesCount != unicodBytesCount;
        }

        #region Manual Call Form
        private void MAIN_BT_IO_MONITOR_Click(object sender, EventArgs e)
        {
            FormIOMonitor dlg = new FormIOMonitor();
            dlg.ShowDialog();
        }

        private void MAIN_BT_SLECT_MODE_Click(object sender, EventArgs e)
        {
            if (MSystem.SysStatus != StatusRun.STOP) return;
            FormMode dlg = new FormMode();
            dlg.ShowDialog();
        }

        private void MAIN_BT_PRODUCT_Click(object sender, EventArgs e)
        {
            FormproductInfor dlg = new FormproductInfor();
            dlg.ShowDialog();
        }
        private void MAIN_BT_PRODUCT_2_Click(object sender, EventArgs e)
        {
            FormproductInfor dlg = new FormproductInfor();
            dlg.ShowDialog();
        }
        private void MAIN_BT_ORIGIN_Click(object sender, EventArgs e)
        {
            if (MSystem.SysStatus != StatusRun.STOP) return;
            FormOrigin dlg = new FormOrigin();
            dlg.ShowDialog();
        }

        private void MAIN_BT_UNIT_INIT_Click(object sender, EventArgs e)
        {
            if (MSystem.SysStatus != StatusRun.STOP) return;
            FormUnitInit dlg = new FormUnitInit();
            dlg.ShowDialog();
        }
        #endregion

        #region Event Form

        private void BT_OUTOF_PLAN_Click(object sender, EventArgs e)
        {

        }
        public void StartBottonOn()
        {
            MSystem.m_pTrsOP.OnStartButton();
        }
        public void StopBottonOn()
        {
            MSystem.m_pTrsOP.OnStopButton();
        }
        public void AllCVStop()
        {
            
        }
        private void BT_START_Click(object sender, EventArgs e)
        {
            if (!MSystem.IsOriginAll())
            {
                MSystem.MyMsgMemo("Please Origin All Servo before run Program", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            else
            {
                StartBottonOn();
            }    
                

        }

        private void BT_STOP_Click(object sender, EventArgs e)
        {
            StopBottonOn();
        }
        private void colorButton17_Click(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo("Do you want Reset Tactime ?", "Reset", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
            {
                InforProduct.Instance.ResetTactime();
            }
        }
        private void colorButton14_Click(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo("Do you want Reset Tactime ?", "Reset", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
            {
                InforProduct.Instance.ResetTactime();
            }
        }

        private void colorButton19_Click(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo("Are You Want Reset All Data ?", "Memo", msgButton.YESNO, msgIcon.Question) == DialogResult.No)
                return;
            /***************************************************/
            InforProduct.Instance.Total_product = 0;
            InforProduct.Instance.ProductPass = 0;
            InforProduct.Instance.ProductNG = 0;
            InforProduct.Instance.SaveSettings();
        }

        private void FormAuto_Load(object sender, EventArgs e)
        {
            // MSystem.m_pITV.SendStartCMD(InforManager.Instance.ReguratorValue);
        }

        #endregion

        private void IDC_IN_LIFT_Click(object sender, EventArgs e)
        {
           
        }

        private void IDC_OUT_LIFT_Click(object sender, EventArgs e)
        {
           
        }
        private void IDC_TRAY_TRANSFER_Click(object sender, EventArgs e)
        {
            
        }

        private void TB_COMPLETE_ROW_Click(object sender, EventArgs e)
        {
            
        }

        private void TB_COMPLETE_COL_Click(object sender, EventArgs e)
        {
           
        }

        private void IDC_INPUT_STOP_Click(object sender, EventArgs e)
        {
            if (IDC_INPUT_STOP.Value == 1)
                MSystem.m_bInputStop = true;
            else
                MSystem.m_bInputStop = false;
        }

        private void BT_OUTPUT_STOP_Click(object sender, EventArgs e)
        {
            if (IDC_OUTPUT_STOP.Value == 1)
                MSystem.m_bOutputStop = true;
            else
                MSystem.m_bOutputStop = false;
        }

        private void IDC_TEST_RUN_Click(object sender, EventArgs e)
        {
            if (MSystem.SysStatus == StatusRun.RUN) return;
            MSystem.m_bTestRun = !MSystem.m_bTestRun;
        }

        private void IDC_TRAY_FULL_DblClick(object sender, EventArgs e)
        {
           
        }

        private void IDC_TRAY_COMPLTE_DblClick(object sender, EventArgs e)
        {
            
        }
        private void IDC_TRAY_EMTY_DblClick(object sender, EventArgs e)
        {
            
        }
        private void TB_TRAY_COUNTER_Click(object sender, EventArgs e)
        {
            
        }

        private void IDC_LOADER_Click(object sender, EventArgs e)
        {
            
        }

        private void IDC_SHUTTLE_Click(object sender, EventArgs e)
        {
           
        }

        private void IDC_VILYN_Click(object sender, EventArgs e)
        {
           
        }

        #region Vision
        public class ItemDisplayData : IDisposable
        {
            public Bitmap Cam0Image { get; set; }
            public Bitmap Cam1Image { get; set; }

            public void Dispose()
            {
                Cam0Image?.Dispose();
                Cam1Image?.Dispose();
            }
        }


        public void UpdateCameraImageResult(PictureBox pb, Bitmap rawImage)
        {
            if (pb.InvokeRequired)
            {
                pb.Invoke(new Action(() => UpdateCameraImageResult(pb, rawImage)));
                return;
            }
            pb.Image?.Dispose();
            pb.Image = null;

            if (rawImage == null)
            {
                return;
            }
            pb.Image = rawImage;
        }
        private int totaltraymap;
        int rows = 0;
        int cols = 0;
        private ItemDisplayData[] _trayImages;     // current tray (live)
        private ItemDisplayData[] _trayImagesOld;  // previous tray

        private int GetTrayUiIndex(int index) => index;

        private Button[] UIShortedCell;     // current tray cells
        private Button[] UIShortedCellOld;  // previous tray cells

        private void initTrayMap()
        {
            rows = InforTeaching.Instance.TrayRows;
            cols = InforTeaching.Instance.TrayColumns;
            totaltraymap = rows * cols;

            _trayImages = new ItemDisplayData[totaltraymap];
            _trayImagesOld = new ItemDisplayData[totaltraymap];

            // Build the current (live) grid and the previous-tray grid with the same layout.
            UIShortedCell = BuildTrayGrid(groupboxtraymap, _trayImages);
            UIShortedCellOld = BuildTrayGrid(groupboxtraymapold, _trayImagesOld);

            MSystem.m_pYoloManager.OnItemInspected -= YoloManager_OnItemInspected;
            MSystem.m_pYoloManager.OnItemInspected += YoloManager_OnItemInspected;

        }

        /// <summary>
        /// Builds a tray grid of buttons inside <paramref name="box"/>. Clicking a cell shows the
        /// stored images from <paramref name="imageStore"/> on the two preview picture boxes.
        /// Returns the button array indexed by (displayNumber - 1).
        /// </summary>
        private Button[] BuildTrayGrid(GroupBox box, ItemDisplayData[] imageStore)
        {
            Button[] cells = new Button[totaltraymap];
            box.Controls.Clear();

            int padding = 3;
            Rectangle safeArea = box.DisplayRectangle;

            int cellWidth = (safeArea.Width - (cols + 1) * padding) / cols;
            int cellHeight = (safeArea.Height - (rows + 1) * padding) / rows;

            for (int i = 0; i < totaltraymap; i++)
            {
                Button btn = new Button();

                int row = i / cols;
                int col = i % cols;

                int reverseRow = (rows - 1) - row;

                int displayNumber;
                if (reverseRow % 2 == 0)
                {
                    displayNumber = reverseRow * cols + (cols - col);
                }
                else
                {
                    displayNumber = reverseRow * cols + col + 1;
                }

                btn.Text = displayNumber.ToString();
                btn.Tag = displayNumber.ToString();
                int uiIndex = GetTrayUiIndex(displayNumber - 1);
                btn.BackColor = Color.Gray;
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.Margin = new Padding(0);

                int uiRow = i / rows;
                int uiCol = i % cols;

                btn.Size = new Size(cellWidth, cellHeight);

                btn.Location = new Point(
                    safeArea.X + padding + uiCol * (cellWidth + padding),
                    safeArea.Y + padding + uiRow * (cellHeight + padding)
                );

                int capturedIndex = displayNumber - 1;
                btn.Click += (s, e) =>
                {
                    var data = imageStore[capturedIndex];

                    if (data != null)
                    {
                        Bitmap clone0 = data.Cam0Image != null ? new Bitmap(data.Cam0Image) : null;
                        Bitmap clone1 = data.Cam1Image != null ? new Bitmap(data.Cam1Image) : null;

                        UpdateCameraImageResult(pictureBox1, clone0);
                        UpdateCameraImageResult(pictureBox2, clone1);
                    }
                };

                box.Controls.Add(btn);
                cells[uiIndex] = btn;
            }

            return cells;
        }

        

        private void YoloManager_OnItemInspected(int trayIndex, bool isOK, Bitmap img0, Bitmap img1)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => YoloManager_OnItemInspected(trayIndex, isOK, img0, img1)));
                return;
            }

            if (trayIndex >= 0 && trayIndex < totaltraymap)
            {
                UIShortedCell[trayIndex].BackColor = isOK ? Color.LimeGreen : Color.Red;

                _trayImages[trayIndex]?.Dispose();

                _trayImages[trayIndex] = new ItemDisplayData
                {
                    Cam0Image = img0,
                    Cam1Image = img1
                };
                Bitmap display0 = img0 != null ? new Bitmap(img0) : null;
                Bitmap display1 = img1 != null ? new Bitmap(img1) : null;

                UpdateCameraImageResult(pictureBox1, display0);
                UpdateCameraImageResult(pictureBox2, display1);
            }
            else
            {
                img0?.Dispose();
                img1?.Dispose();
            }
        }

        public void ResetTrayMap()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ResetTrayMap));
                return;
            }

            for (int i = 0; i < totaltraymap; i++)
            {
                // Promote current tray -> old: transfer image ownership (do NOT dispose current).
                _trayImagesOld[i]?.Dispose();
                _trayImagesOld[i] = _trayImages[i];
                _trayImages[i] = null;

                // Copy cell color current -> old.
                if (UIShortedCellOld[i] != null && UIShortedCell[i] != null)
                {
                    UIShortedCellOld[i].BackColor = UIShortedCell[i].BackColor;
                }

                // Reset current cell to empty/gray.
                if (UIShortedCell[i] != null)
                {
                    UIShortedCell[i].BackColor = Color.Gray;
                }
            }
            GC.Collect();
        }

        int currentindextest = 0;
        private void RunOnce(object sender, EventArgs e)
        {
            Bitmap picbox1 = pictureBox1.Tag as Bitmap;
            Bitmap picbox2 = pictureBox2.Tag as Bitmap;

            var results0 = MSystem.m_pYoloManager.CheckImageDetails(picbox1);
            var results1 = MSystem.m_pYoloManager.CheckImageDetails(picbox2);

            bool isCam0_OK = results0.Count > 0;
            bool isCam1_OK = results1.Count > 0;
            bool isItemOK = isCam0_OK && isCam1_OK;

            Bitmap drawn0 = MSystem.m_pYoloManager.DrawResults(picbox1, results0, isCam0_OK);
            Bitmap drawn1 = MSystem.m_pYoloManager.DrawResults(picbox2, results1, isCam1_OK);

            MSystem.m_pYoloManager.sendEvent(currentindextest, isItemOK, drawn0, drawn1);
            currentindextest++;
           // ResetTrayMap();

        }


        #region Load and Clear Image Test
        public static Bitmap LoadBitmapWithoutLocking(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var tempImg = new Bitmap(fs))
                {
                    return (Bitmap)tempImg.Clone();
                }
            }
        }

        public void UpdateCameraImage(PictureBox pb, Bitmap rawImage)
        {
            if (pb.InvokeRequired)
            {
                pb.Invoke(new Action(() => UpdateCameraImage(pb, rawImage)));
                return;
            }
            if (rawImage == null)
            {
                pb.Image = null;
                pb.Tag = null;
                return;
            }

            pb.Image?.Dispose();
            pb.Image = null;

            if (pb.Tag is Bitmap oldTag)
            {
                oldTag.Dispose();
                pb.Tag = null;
            }

            pb.Image = new Bitmap(rawImage);
            pb.Tag = rawImage;
        }

        private void Improt_Image_Click(object sender, EventArgs e) 
        {
            if (sender is ToolStripMenuItem btn)
            {
                PictureBox targetPB = null;

                switch (btn.Name)
                {
                    case "ImportImage1": targetPB = pictureBox1; break;
                    case "ImportImage2": targetPB = pictureBox2; break;
                }

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";
                    ofd.Title = $"Select Image for PictureBox";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {

                        Bitmap bmp = LoadBitmapWithoutLocking(ofd.FileName);
                        UpdateCameraImage(targetPB, bmp);
                    }
                }
            }
        }

        private void Clear_Image_Click(Object sender, EventArgs e)
        {
            try
            {
                if (sender is ToolStripMenuItem btn)
                {
                    PictureBox targetPB = null;
                    switch (btn.Name)
                    {
                        case "ClearImage1": targetPB = pictureBox1; break;
                        case "ClearImage2": targetPB = pictureBox2; break;
                    }

                    if (targetPB != null)
                    {
                        targetPB.Image?.Dispose();
                        targetPB.Image = null;
                        targetPB.Tag = null;
                    }
                }
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion

        #endregion

        private void BT_TRIGGER_POINT_Click(object sender, EventArgs e)
        {
            if (MSystem.SysStatus != StatusRun.STOP) return;
            ModeTriggerSelected(ModeTrigger.Point);
        }

        private void BT_TRIGGER_FLYING_Click(object sender, EventArgs e)
        {
            ModeTriggerSelected(ModeTrigger.Flying);
        }

        private void ModeTriggerSelected(ModeTrigger mode)
        {
            if(0 == mode)
            {
                MSystem.ModeTrigger = mode;
                BT_TRIGGER_POINT.GradientBottom = Color.Yellow;
                BT_TRIGGER_POINT.GradientTop = Color.Yellow;
                BT_TRIGGER_FLYING.GradientBottom = Color.DimGray;
                BT_TRIGGER_FLYING.GradientTop = Color.DimGray;
            }
            else
            {
                MSystem.ModeTrigger = mode;
                BT_TRIGGER_POINT.GradientBottom = Color.DimGray;
                BT_TRIGGER_POINT.GradientTop = Color.DimGray;
                BT_TRIGGER_FLYING.GradientBottom = Color.Yellow;
                BT_TRIGGER_FLYING.GradientTop = Color.Yellow;
            }
        }
    }
}
