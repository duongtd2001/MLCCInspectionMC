using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static MLCCInspectionMC.FormLog;

namespace MLCCInspectionMC
{
    public partial class FormproductInfor : Form
    {
        Thread _updateStatus;
        //Timer _updateStatus = new Timer();
        public static MyMonthCalendar monthCalendar = new MyMonthCalendar();
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
        string[] _DateSelect_Weekly = new string[7];
        string _DateSelect_Detail = "";
        int _indexControl = -1;
        long[] _data_A = new long[3];
        long[] _data_B = new long[3];
        long weekly_total_A;
        long weekly_total_B;

        public CBbutton[] m_btDateSelect = new CBbutton[7];
        public CBbutton[] m_btWeek_A = new CBbutton[7];
        public CBbutton[] m_btWeek_B = new CBbutton[7];
        public CBbutton[] m_btWeek_SubTotal = new CBbutton[7];

        //private Color ColumnBackground = Color.FromArgb(255, 224, 192);
        private System.Drawing.Color ColumnBackground = System.Drawing.Color.Bisque;
        public FormproductInfor()
        {
            InitializeComponent();
            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);

            _updateStatus = new Thread(UpdateAllData);
            _updateStatus.IsBackground = true;
            _updateStatus.Start();

            int i = 0;
            m_btWeek_A[i++] = new CBbutton(BT_DATE_01_A, false);
            m_btWeek_A[i++] = new CBbutton(BT_DATE_02_A, false);
            m_btWeek_A[i++] = new CBbutton(BT_DATE_03_A, false);
            m_btWeek_A[i++] = new CBbutton(BT_DATE_04_A, false);
            m_btWeek_A[i++] = new CBbutton(BT_DATE_05_A, false);
            m_btWeek_A[i++] = new CBbutton(BT_DATE_06_A, false);
            m_btWeek_A[i++] = new CBbutton(BT_DATE_07_A, false);

            i = 0;
            m_btWeek_B[i++] = new CBbutton(BT_DATE_01_B, false);
            m_btWeek_B[i++] = new CBbutton(BT_DATE_02_B, false);
            m_btWeek_B[i++] = new CBbutton(BT_DATE_03_B, false);
            m_btWeek_B[i++] = new CBbutton(BT_DATE_04_B, false);
            m_btWeek_B[i++] = new CBbutton(BT_DATE_05_B, false);
            m_btWeek_B[i++] = new CBbutton(BT_DATE_06_B, false);
            m_btWeek_B[i++] = new CBbutton(BT_DATE_07_B, false);

            i = 0;
            m_btWeek_SubTotal[i++] = new CBbutton(BT_DATE_01_TOTAL, false);
            m_btWeek_SubTotal[i++] = new CBbutton(BT_DATE_02_TOTAL, false);
            m_btWeek_SubTotal[i++] = new CBbutton(BT_DATE_03_TOTAL, false);
            m_btWeek_SubTotal[i++] = new CBbutton(BT_DATE_04_TOTAL, false);
            m_btWeek_SubTotal[i++] = new CBbutton(BT_DATE_05_TOTAL, false);
            m_btWeek_SubTotal[i++] = new CBbutton(BT_DATE_06_TOTAL, false);
            m_btWeek_SubTotal[i++] = new CBbutton(BT_DATE_07_TOTAL, false);

            i = 0;
            m_btDateSelect[i++] = new CBbutton(BT_DATE_01, false);
            m_btDateSelect[i++] = new CBbutton(BT_DATE_02, false);
            m_btDateSelect[i++] = new CBbutton(BT_DATE_03, false);
            m_btDateSelect[i++] = new CBbutton(BT_DATE_04, false);
            m_btDateSelect[i++] = new CBbutton(BT_DATE_05, false);
            m_btDateSelect[i++] = new CBbutton(BT_DATE_06, false);
            m_btDateSelect[i++] = new CBbutton(BT_DATE_07, false);

            for (i = 0; i < m_btDateSelect.Length; i++)
                m_btDateSelect[i].Button.MouseDown += new MouseEventHandler(PreLoadEventPos);

            if (DateTime.Now.Hour >= 20)
            {
                _DateSelect_Detail = $"{DateTime.Now:yyyyMMdd}";
                BT_DETAIL_DATE.Text = $"{DateTime.Now:dd-MMM}";
                BT_DETAIL_DOW.Text = $"{DateTime.Today.DayOfWeek.ToString().Substring(0, 3)}";
            }
            else if (DateTime.Now.Hour < 8)
            {
                _DateSelect_Detail = $"{DateTime.Now.AddDays(-1):yyyyMMdd}";
                BT_DETAIL_DATE.Text = $"{DateTime.Now.AddDays(-1):dd-MMM}";
                if (DateTime.Today.DayOfWeek != 0)
                    BT_DETAIL_DOW.Text = $"{(DateTime.Today.DayOfWeek - 1).ToString().Substring(0, 3)}";
                else
                    BT_DETAIL_DOW.Text = $"{DayOfWeek.Saturday.ToString().Substring(0, 3)}";
            }
            else
            {
                _DateSelect_Detail = $"{DateTime.Now:yyyyMMdd}";
                BT_DETAIL_DATE.Text = $"{DateTime.Now:dd-MMM}";
                BT_DETAIL_DOW.Text = $"{DateTime.Today.DayOfWeek.ToString().Substring(0, 3)}";
            }
        }

        private void UpdateAllData()
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    UpdateData();
                }
                catch (Exception)
                {
                    Thread.Sleep(10);
                }
            }
        }

        private void PreLoadEventPos(object sender, MouseEventArgs e)
        {
            _indexControl = GetButtonClick(sender);
            if (_indexControl < 0) return;
            UnSelectButton();
            int _dts = DateTime.Today.DayOfWeek - DayOfWeek.Monday;
            if (DateTime.Today.DayOfWeek != 0)
            {
                if ((int)DateTime.Today.DayOfWeek == 1 && DateTime.Now.Hour < 8)
                {
                    _DateSelect_Detail = $"{DateTime.Now.AddDays(-_dts + _indexControl - 8):yyyyMMdd}";
                    BT_DETAIL_DATE.Text = $"{DateTime.Now.AddDays(-_dts + _indexControl - 8):dd-MMM}";
                }
                else
                {
                    _DateSelect_Detail = $"{DateTime.Now.AddDays(-_dts + _indexControl - 1):yyyyMMdd}";
                    BT_DETAIL_DATE.Text = $"{DateTime.Now.AddDays(-_dts + _indexControl - 1):dd-MMM}";
                }

            }
            else
            {
                _DateSelect_Detail = $"{DateTime.Now.AddDays(-_dts + _indexControl - 8):yyyyMMdd}";
                BT_DETAIL_DATE.Text = $"{DateTime.Now.AddDays(-_dts + _indexControl - 8):dd-MMM}";
            }
            if (_indexControl < 7)
                BT_DETAIL_DOW.Text = $"{(DateTime.Today.DayOfWeek + _indexControl - _dts - 1).ToString().Substring(0, 3)}";
            else
                BT_DETAIL_DOW.Text = $"{DayOfWeek.Sunday.ToString().Substring(0, 3)}";
            SelectDateColumn(_indexControl);
        }
        private void SelectDateColumn(int _ColSel)
        {
            m_btDateSelect[_ColSel - 1].SetValue(ColumnBackground);
            m_btWeek_A[_ColSel - 1].SetValue(ColumnBackground);
            m_btWeek_B[_ColSel - 1].SetValue(ColumnBackground);
            m_btWeek_SubTotal[_ColSel - 1].SetValue(ColumnBackground);
        }
        void UnSelectButton()
        {
            for (int i = 0; i < 7; i++)
            {
                m_btDateSelect[i].SetValeUnSelect_3();
                m_btWeek_A[i].SetValeUnSelect_2();
                m_btWeek_B[i].SetValeUnSelect_2();
                m_btWeek_SubTotal[i].SetValeUnSelect_2();
            }
        }
        private int GetButtonClick(object sender)
        {
            if (sender is SUserControls.ColorButton btn)
            {
                switch (btn.Name)
                {
                    case "BT_DATE_01":
                        return 1;
                    case "BT_DATE_02":
                        return 2;
                    case "BT_DATE_03":
                        return 3;
                    case "BT_DATE_04":
                        return 4;
                    case "BT_DATE_05":
                        return 5;
                    case "BT_DATE_06":
                        return 6;
                    case "BT_DATE_07":
                        return 7;
                }
            }
            return -1;
        }

        private void UpdateData()
        {
            int _dts = DateTime.Today.DayOfWeek - DayOfWeek.Monday;
            /*Show Hide Button*/
            MSystem.IsSensorOn(IDC_PRODUCT_INFOR_SHOW_1, InforManager.Instance.ProductInfor, 0);
            MSystem.IsSensorOn(IDC_PRODUCT_INFOR_HIDE_1, !InforManager.Instance.ProductInfor, 0);

            if (InforManager.Instance.ProductInfor)
            {
                IDC_PRODUCT_INFOR_SHOW.Value = 1;
                IDC_PRODUCT_INFOR_HIDE.Value = 0;
                IDC_PRODUCT_INFOR_SHOW.BackColorInterior = System.Drawing.Color.Yellow;
                IDC_PRODUCT_INFOR_SHOW.BackColorPressed = System.Drawing.Color.Yellow;
                IDC_PRODUCT_INFOR_HIDE.BackColorInterior = System.Drawing.Color.White;
                IDC_PRODUCT_INFOR_HIDE.BackColorPressed = System.Drawing.Color.White;
            }
            else
            {
                IDC_PRODUCT_INFOR_SHOW.Value = 0;
                IDC_PRODUCT_INFOR_HIDE.Value = 1;
                IDC_PRODUCT_INFOR_SHOW.BackColorInterior = System.Drawing.Color.White;
                IDC_PRODUCT_INFOR_SHOW.BackColorPressed = System.Drawing.Color.White;
                IDC_PRODUCT_INFOR_HIDE.BackColorInterior = System.Drawing.Color.Yellow;
                IDC_PRODUCT_INFOR_HIDE.BackColorPressed = System.Drawing.Color.Yellow;
            }

            //Tinh toan Thu - Ngay - Thang
            for (int i = 0; i < 6; i++)
            {
                if (DateTime.Today.DayOfWeek != 0)
                {
                    if ((int)DateTime.Today.DayOfWeek == 1 && DateTime.Now.Hour < 8)
                        m_btDateSelect[i].Button.Text = $"{DateTime.Now.AddDays(-_dts + i - 7):dd-MMM}\n" + $"{(DateTime.Today.DayOfWeek + i - _dts).ToString().Substring(0, 3)}";
                    else
                        m_btDateSelect[i].Button.Text = $"{DateTime.Now.AddDays(-_dts + i):dd-MMM}\n" + $"{(DateTime.Today.DayOfWeek + i - _dts).ToString().Substring(0, 3)}";
                }
                else
                    m_btDateSelect[i].Button.Text = $"{DateTime.Now.AddDays(-_dts + i - 7):dd-MMM}\n" + $"{(DateTime.Today.DayOfWeek + i - _dts).ToString().Substring(0, 3)}";
            }

            if (DateTime.Today.DayOfWeek != 0)
            {
                if ((int)DateTime.Today.DayOfWeek == 1 && DateTime.Now.Hour < 8)
                    m_btDateSelect[6].Button.Text = $"{DateTime.Now.AddDays(-_dts - 1):dd-MMM}\n" + $"{DayOfWeek.Sunday.ToString().Substring(0, 3)}";
                else
                    m_btDateSelect[6].Button.Text = $"{DateTime.Now.AddDays(-_dts + 6):dd-MMM}\n" + $"{DayOfWeek.Sunday.ToString().Substring(0, 3)}";
            }
            else
                m_btDateSelect[6].Button.Text = $"{DateTime.Today:dd-MMM}\n" + $"{DayOfWeek.Sunday.ToString().Substring(0, 3)}";

            //Update Weekly
            weekly_total_A = 0;
            weekly_total_B = 0;
            if ((int)DateTime.Today.DayOfWeek == 1 && DateTime.Now.Hour < 8)
            {
                for (int i = 0; i < 7; i++)
                {
                    _DateSelect_Weekly[i] = $"{DateTime.Now.AddDays(i - 7):yyyyMMdd}";
                    _data_A = UpdateProductFromDB_Daily(_DateSelect_Weekly[i] + "_" + "A");
                    _data_B = UpdateProductFromDB_Daily(_DateSelect_Weekly[i] + "_" + "B");
                    m_btWeek_A[i].Button.Text = $"{_data_A[0]}";
                    m_btWeek_B[i].Button.Text = $"{_data_B[0]}";
                    m_btWeek_SubTotal[i].Button.Text = $"{_data_A[0] + _data_B[0]}";
                    weekly_total_A += _data_A[0];
                    weekly_total_B += _data_B[0];
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    if (DateTime.Today.DayOfWeek != 0)
                        _DateSelect_Weekly[i] = $"{DateTime.Now.AddDays(-_dts + i):yyyyMMdd}";
                    else
                        _DateSelect_Weekly[i] = $"{DateTime.Now.AddDays(-_dts + i - 7):yyyyMMdd}";
                    _data_A = UpdateProductFromDB_Daily(_DateSelect_Weekly[i] + "_" + "A");
                    _data_B = UpdateProductFromDB_Daily(_DateSelect_Weekly[i] + "_" + "B");
                    m_btWeek_A[i].Button.Text = $"{_data_A[0]}";
                    m_btWeek_B[i].Button.Text = $"{_data_B[0]}";
                    m_btWeek_SubTotal[i].Button.Text = $"{_data_A[0] + _data_B[0]}";
                    weekly_total_A += _data_A[0];
                    weekly_total_B += _data_B[0];
                }
            }

            MSystem.DisPlayString(BT_WEEK_A_TOTAL, $"{weekly_total_A}"); ;
            MSystem.DisPlayString(BT_WEEK_B_TOTAL, $"{weekly_total_B}");
            MSystem.DisPlayString(BT_WEEK_TOTAL, $"{weekly_total_A + weekly_total_B}");

            //Update Detail
            if (this.TAB_DAILY.Visible)
            {
                //_indexControl = 0;
                UnSelectButton();

                if (DateTime.Now.Hour >= 20)
                {
                    _DateSelect_Detail = $"{DateTime.Now:yyyyMMdd}";
                    BT_DETAIL_DATE.Text = $"{DateTime.Now:dd-MMM}";
                    BT_DETAIL_DOW.Text = $"{DateTime.Today.DayOfWeek.ToString().Substring(0, 3)}";
                    if (DateTime.Today.DayOfWeek != 0) _indexControl = (int)DateTime.Today.DayOfWeek;
                    else _indexControl = 7;
                }
                else if (DateTime.Now.Hour < 8)
                {
                    if (DateTime.Today.DayOfWeek != 0)
                    {
                        _DateSelect_Detail = $"{DateTime.Now.AddDays(-1):yyyyMMdd}";
                        BT_DETAIL_DATE.Text = $"{DateTime.Now.AddDays(-1):dd-MMM}";
                        BT_DETAIL_DOW.Text = $"{(DateTime.Today.DayOfWeek - 1).ToString().Substring(0, 3)}";
                        if ((int)DateTime.Today.DayOfWeek != 1)
                            _indexControl = (int)DateTime.Today.DayOfWeek - 1;
                        else
                            _indexControl = 7;
                    }
                    else
                    {
                        _DateSelect_Detail = $"{DateTime.Now.AddDays(-1):yyyyMMdd}";
                        BT_DETAIL_DATE.Text = $"{DateTime.Now.AddDays(-1):dd-MMM}";
                        BT_DETAIL_DOW.Text = $"{DayOfWeek.Saturday.ToString().Substring(0, 3)}";
                        _indexControl = 6;
                    }
                }
                else
                {
                    _DateSelect_Detail = $"{DateTime.Now:yyyyMMdd}";
                    BT_DETAIL_DATE.Text = $"{DateTime.Now:dd-MMM}";
                    BT_DETAIL_DOW.Text = $"{DateTime.Today.DayOfWeek.ToString().Substring(0, 3)}";
                    if (DateTime.Today.DayOfWeek != 0) _indexControl = (int)DateTime.Today.DayOfWeek;
                    else _indexControl = 7;
                }
                SelectDateColumn(_indexControl);
            }
            _data_A = UpdateProductFromDB_Daily(_DateSelect_Detail + "_" + "A");
            _data_B = UpdateProductFromDB_Daily(_DateSelect_Detail + "_" + "B");
            MSystem.DisPlayString(BT_DETAIL_TOTAL_A, $"{_data_A[0]}");
            MSystem.DisPlayString(BT_DETAIL_PASS_A, $"{_data_A[1]}");
            MSystem.DisPlayString(BT_DETAIL_FAIL_A, $"{_data_A[2]}");
            MSystem.DisPlayString(BT_DETAIL_TOTAL_B, $"{_data_B[0]}");
            MSystem.DisPlayString(BT_DETAIL_PASS_B, $"{_data_B[1]}");
            MSystem.DisPlayString(BT_DETAIL_FAIL_B, $"{_data_B[2]}");
            if (_data_A[0] > 0)
                MSystem.DisPlayString(BT_DETAIL_NG_RATE_A, $"{((float)(_data_A[2]) / (_data_A[0])) * 100:f02} %");
            else
                MSystem.DisPlayString(BT_DETAIL_NG_RATE_A, "0.00%");
            if (_data_B[0] > 0)
                MSystem.DisPlayString(BT_DETAIL_NG_RATE_B, $"{(float)(_data_B[2]) / (_data_B[0]) * 100:f02} %");
            else
                MSystem.DisPlayString(BT_DETAIL_NG_RATE_B, "0.00%");

            MSystem.DisPlayString(TB_NG_TOTAL, $"{InforProduct.Instance.ProductNG}");
            MSystem.DisPlayString(LB_OUTPUT_TOTAL, $"{InforProduct.Instance.ProductPass}");
            MSystem.DisPlayString(LB_INPUT_TOTAL, $"{InforProduct.Instance.ProductPass + InforProduct.Instance.ProductNG}");
            MSystem.DisPlayString(TB_NG_GMES, $"{InforProduct.Instance.Product_GMES_FAIL}");
            MSystem.DisPlayString(TB_NG_BCR, $"{InforProduct.Instance.ProductBCR_FAIL}");
            MSystem.DisPlayString(TB_NG_VINYL_REMOVE, $"{InforProduct.Instance.Product_RemoveVinyl_FAIL}");
            MSystem.DisPlayString(TB_NG_VINYL_REMOVE_LOWER, $"{InforProduct.Instance.Product_RemoveVinyl_FAIL_Lower}");

            if (InforProduct.Instance.ProductPass + InforProduct.Instance.ProductNG > 0)
                MSystem.DisPlayString(LB_NG_RATE, $"{(((float)InforProduct.Instance.ProductNG / (InforProduct.Instance.ProductPass + InforProduct.Instance.ProductNG)) * 100).ToString("f02")} %");
            else
                MSystem.DisPlayString(LB_NG_RATE, "0.00%");
        }
        private void CloseAllData(object sender, EventArgs e)
        {
            _updateStatus.Join();
            _updateStatus = null;
        }

        private void BT_EXIT_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void IDC_PRODUCT_INFOR_SHOW_Click(object sender, EventArgs e)
        {
            InforManager.Instance.ProductInfor = true;
            MSystem.AutoForm.IDC_PL_INFO.Visible = true;
            MSystem.AutoForm.MAIN_BT_PRODUCT.Visible = true;
            InforManager.Instance.SaveData();
        }

        private void IDC_PRODUCT_INFOR_HIDE_Click(object sender, EventArgs e)
        {
            InforManager.Instance.ProductInfor = false;
            MSystem.AutoForm.IDC_PL_INFO.Visible = false;
            MSystem.AutoForm.MAIN_BT_PRODUCT.Visible = false;
            InforManager.Instance.SaveData();
        }

        private void BT_ALL_RESET_Click(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo("Do you want to Reset All Data Product ?", "Question", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
            {
                MSystem.ResetAllProductInfor();
            }
        }

        private void BT_RESET_BCR_Click(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo("Do you want to Reset BCR Fail Data ?", "Question", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
            {
                MSystem.ResetBCRInfor();
            }
        }

        private void BT_RESET_PRESS_LEFT_Click(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo("Do you want to Reset GMES Fail Data ?", "Question", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
            {
                MSystem.ResetGMESInfor();
            }
        }
        private long[] UpdateProductFromDB_Daily(string _str)
        {
            long[] _rtn = new long[3];
            string _strtotal;
            string _strpass;
            string _strfail;
            long _total = 0;
            long _pass = 0;
            long _fail = 0;

            MSystem.m_plisView.TB_Product_Update_Daily_A = MSystem.m_pDatabaseProduct.GetDataSearchProduct(_str);
            if (MSystem.m_plisView.TB_Product_Update_Daily_A.Rows.Count == 0)
            {
                _strtotal = "0";
                _strpass = "0";
                _strfail = "0";
            }
            else
            {
                _strtotal = MSystem.m_plisView.TB_Product_Update_Daily_A.Rows[0]["TOTAL"].ToString();
                _strpass = MSystem.m_plisView.TB_Product_Update_Daily_A.Rows[0]["TOTAL_PASS"].ToString();
                _strfail = MSystem.m_plisView.TB_Product_Update_Daily_A.Rows[0]["TOTAL_FAIL"].ToString();
            }
            _rtn[0] = _total = long.Parse(_strtotal);
            _rtn[1] = _pass = long.Parse(_strpass);
            _rtn[2] = _fail = long.Parse(_strfail);

            return _rtn;
        }

        private void buttonPress1_Click(object sender, EventArgs e)
        {
            //MSystem.INC_ProductPass_DB();
            return;
            string _DateNow;
            if (DateTime.Now.Hour >= 20)
                _DateNow = $"{DateTime.Now.ToString("yyyyMMdd")}" + "_" + "B";
            else if (DateTime.Now.Hour < 8)
                _DateNow = $"{DateTime.Now.AddDays(-1).ToString("yyyyMMdd")}" + "_" + "B";
            else
                _DateNow = $"{DateTime.Now.ToString("yyyyMMdd")}" + "_" + "A";

            string[] _strtmp = new string[4];
            _strtmp[0] = _DateNow;
            _strtmp[1] = "54";
            _strtmp[2] = "47";
            _strtmp[3] = "7";

            MSystem.m_plisView.TB_Product = MSystem.m_pDatabaseProduct.GetDataSearchProduct(_DateNow);
            if (MSystem.m_plisView.TB_Product.Rows.Count == 0)
                MSystem.m_pDatabaseProduct.InsertData_Product(_strtmp);
            else
                MSystem.m_pDatabaseProduct.UpdateData_Product(_strtmp);
        }



        private void TB_PRODUCT_INFOR_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (_indexControl > 0 && e.Column == _indexControl)
                e.Graphics.FillRectangle(new SolidBrush(System.Drawing.Color.Bisque), e.CellBounds);
        }

        private void BT_TEST_ADD_Click(object sender, EventArgs e)
        {
            //MSystem.INC_ProductPass_DB();
            return;
        }

        private void BT_TIME_RESET_ClickEvent(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo("Do you want to Reset Tactime ?", "Reset", msgButton.YESNO, msgIcon.Question) == DialogResult.Yes)
            {
                InforProduct.Instance.ResetTactime();
            }
        }
    }
}
