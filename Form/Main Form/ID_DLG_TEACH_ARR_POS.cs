using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FormTeachArrPos : Form
    {
        //private Button[] m_btPoselect;
        private int row;
        private int col;
        private int JogMode = 0;
        private Button bt_Pos;
        private int bt_idx = -1;
        private int idx_mapping = 0;



        public FormTeachArrPos()
        {
            InitializeComponent();
            this.Location = new Point(0, 0);

            InitArrPos(InforTeaching.Instance.TrayRows, InforTeaching.Instance.TrayColumns);
            UpDateData();
            numericRow.Value = InforTeaching.Instance.TrayRows;
            numericCols.Value = InforTeaching.Instance.TrayColumns;
        }

        private void InitArrPos(int _row, int _cols)
        {
            row = _row;
            col = _cols;
            MSystem.MAP_ARRAY = new int[_row, _cols];
            MSystem.m_btPoselect = new Button[_row * _cols];
            tbPanelArray.Controls.Clear();
            tbPanelArray.RowStyles.Clear();
            tbPanelArray.ColumnStyles.Clear();
            if (_row >= 15 || _cols >= 15)
            {
                grbArrayPosition.Size = new Size(750, 420);
            }
            else if (_row >= 10 || _cols >= 10)
            {
                grbArrayPosition.Size = new Size(640, 420);
            }
            else
                grbArrayPosition.Size = new Size(400, 400);
            tbPanelArray.RowCount = _row;
            tbPanelArray.ColumnCount = _cols;

            int index = 1;
            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    tbPanelArray.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / _row));
                    tbPanelArray.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / _cols));
                    Button btn = new Button();
                    btn.Tag = $"BT_POS_{index}";
                    btn.BackColor = Color.Gainsboro;
                    btn.ForeColor = Color.Black;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Text = index.ToString();
                    btn.Font = new Font("Microsoft Sans Serif", 13, FontStyle.Regular);
                    btn.Dock = DockStyle.Fill;
                    btn.Click += BT_POS_Click;
                    MSystem.m_btPoselect[index - 1] = btn;
                    MSystem.MAP_ARRAY[i, j] = index - 1;
                    tbPanelArray.Controls.Add(btn, j, i);
                    index++;
                }
            }
        }

        private void BT_POS_Click(object sender, EventArgs e)
        {
            bt_Pos = (Button)sender;
            SelectButton(bt_Pos.Tag.ToString());
            if(!InforTeaching.Instance.m_dPos.ContainsKey(bt_idx)) return;
            TARGET_POS_X.Text = InforTeaching.Instance.m_dPos[bt_idx].X.ToString("F3");
            TARGET_POS_Y.Text = InforTeaching.Instance.m_dPos[bt_idx].Y.ToString("F3");
        }

        void SelectButton(string _tag)
        {
            UnSelectButton();
            string[] idx = _tag.Split('_');
            bt_idx = Convert.ToInt16(idx[idx.Length - 1]) - 1;
            MSystem.m_btPoselect[Convert.ToInt16(idx[idx.Length - 1]) - 1].BackColor = Color.AntiqueWhite;
            MSystem.m_btPoselect[Convert.ToInt16(idx[idx.Length - 1]) - 1].FlatAppearance.BorderSize = 1;
            MSystem.m_btPoselect[Convert.ToInt16(idx[idx.Length - 1]) - 1].FlatAppearance.BorderColor = Color.DarkCyan;
        }
        void UnSelectButton()
        {
            for (int i = 0; i < MSystem.m_btPoselect.Length; i++)
            {
                MSystem.m_btPoselect[i].BackColor = Color.Gainsboro;
                MSystem.m_btPoselect[i].FlatAppearance.BorderSize = 0;
            }
        }

        private void BT_SAVE_POS_ClickEvent(object sender, EventArgs e)
        {
            if (!MSystem.IsOkeyTeaching() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All servo before Save data position", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.MyMsgMemo($"Are you want Save {bt_Pos.Tag.ToString()}?", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (MSystem.SIMULATION)
            {
                return;
            }
            InforTeaching.Instance.m_dPos[bt_idx] = (MSystem.m_pTrsTransferXY.GetCurrentPos(Axis.AXIS_X), 
                MSystem.m_pTrsTransferXY.GetCurrentPos(Axis.AXIS_Y));
            InforTeaching.Instance.SaveSettings();
        }

        private void axBT_MOVE_POS_ClickEvent(object sender, EventArgs e)
        {
            if (!MSystem.IsOkeyTeaching() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All servo before Save data position", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.MyMsgMemo($"Are you want go to {bt_Pos.Tag.ToString()}?", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            if (MSystem.SIMULATION)
            {
                return;
            }
            Task.Run(() =>
            {
                MSystem.m_pTrsTransferXY.MovePosition(Axis.AXIS_X, InforTeaching.Instance.m_dPos[bt_idx].X);
            });
            Task.Run(() =>
            {
                MSystem.m_pTrsTransferXY.MovePosition(Axis.AXIS_Y, InforTeaching.Instance.m_dPos[bt_idx].Y);
            });
        }

        private void BT_MOVE_POS_Click(object sender, EventArgs e)
        {

        }

        private void AxBT_GENERAL_ARRAY_Click(object sender, EventArgs e)
        {
            InforTeaching.Instance.TrayColumns = Convert.ToInt16(numericCols.Value);
            InforTeaching.Instance.TrayRows = Convert.ToInt16(numericRow.Value);
            MSystem.ArrayPos = new double[InforTeaching.Instance.TrayRows, InforTeaching.Instance.TrayColumns];
            GeneralArray();
            InforTeaching.Instance.SaveSettings();
        }

        public void GeneralArray()
        {
            if (MSystem.MyMsgMemo($"Do you want General Array Position?", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            if (InforTeaching.Instance.m_dTechPos_StartXY == null)
            {
                MSystem.MyMsgMemo($"Starting point not yet teaching", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            if(InforTeaching.Instance.m_dTechPos_EndXY ==  null)
            {
                MSystem.MyMsgMemo($"Starting point not yet teaching", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            idx_mapping = 0;
            //for (int i = 0; i < InforTeaching.Instance.TrayRows; i++)
            //{
            //    if(i % 2 == 0)
            //    {
            //        for (int j = 0; j < InforTeaching.Instance.TrayColumns; j++)
            //        {
            //            InforTeaching.Instance.m_dPos[idx_mapping] =
            //            (CalculatorPosX(Convert.ToInt32(MSystem.m_btPoselect[j].Text) - 1),
            //             CalculatorPosY(Convert.ToInt32(MSystem.m_btPoselect[i].Text) - 1));
            //            idx_mapping++;
            //        }
            //    }
            //    else
            //    {
            //        for (int k = InforTeaching.Instance.TrayColumns - 1; k >= 0; k--)
            //        {
            //            InforTeaching.Instance.m_dPos[idx_mapping] =
            //            (CalculatorPosX(Convert.ToInt32(MSystem.m_btPoselect[k].Text) - 1),
            //             CalculatorPosY(Convert.ToInt32(MSystem.m_btPoselect[i].Text) - 1));
            //            idx_mapping++;
            //        }
            //    }

            //}
            for (int row = 0; row < InforTeaching.Instance.TrayRows; row++)
            {
                if (row % 2 == 0)
                {
                    for (int col = 0; col < InforTeaching.Instance.TrayColumns; col++)
                    {
                        InforTeaching.Instance.m_dPos[idx_mapping] =
                        (
                            CalculatorPosX(col),
                            CalculatorPosY(row)
                        );

                        idx_mapping++;
                    }
                }
                else
                {
                    for (int col = InforTeaching.Instance.TrayColumns - 1; col >= 0; col--)
                    {
                        InforTeaching.Instance.m_dPos[idx_mapping] =
                        (
                            CalculatorPosX(col),
                            CalculatorPosY(row)
                        );

                        idx_mapping++;
                    }
                }
            }
        }
        public double CalculatorPosY(int idx)
        {
            double dY = InforTeaching.Instance.m_dTechPos_StartXY[1] - idx * ((InforTeaching.Instance.m_dTechPos_StartXY[1] -
               InforTeaching.Instance.m_dTechPos_EndXY[1]) / (InforTeaching.Instance.TrayRows - 1));
            return dY;
        }
        public double CalculatorPosX(int idx)
        {
            double dX = InforTeaching.Instance.m_dTechPos_StartXY[0] + idx * ((InforTeaching.Instance.m_dTechPos_EndXY[0] -
                InforTeaching.Instance.m_dTechPos_StartXY[0]) / (InforTeaching.Instance.TrayColumns - 1));
            return dX;
        }
        private void JogActive(object sender, MouseEventArgs e)
        {
            switch ((sender as Button).Name.ToString())
            {
                case "BT_Y_JOG_UP":
                    JogYMinus();
                    break;
                case "BT_Y_JOG_DOWN":
                    JogYPlus();
                    break;
                case "BT_X_JOG_LEFT":
                    //JogXPlus();
                    JogXMinus();
                    break;
                case "BT_X_JOG_RIGHT":
                    //JogXMinus();
                    JogXPlus();
                    break;

                default: break;
            }
        }

        void UpDateData()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);
                    if (this.Visible)
                    {
                        try
                        {
                            UpdatePosition();
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        void UpdatePosition()
        {
            if (MSystem.m_pAxisManager.m_pFastechAxis[(int)Axis.AXIS_X] != null)
                CURRENT_POS_X.Text = MSystem.GetCurrentPos((int)Axis.AXIS_X).ToString("f3");

            if (MSystem.m_pAxisManager.m_pFastechAxis[(int)Axis.AXIS_Y] != null)
                CURRENT_POS_Y.Text = MSystem.GetCurrentPos((int)Axis.AXIS_Y).ToString("f3");

            BT_POS_STARTXY.Text = $"({InforTeaching.Instance.m_dTechPos_StartXY[0].ToString("f3")}, {InforTeaching.Instance.m_dTechPos_StartXY[1].ToString("f3")})";
            BT_POS_ENDXY.Text = $"({InforTeaching.Instance.m_dTechPos_EndXY[0].ToString("f3")}, {InforTeaching.Instance.m_dTechPos_EndXY[1].ToString("f3")})";
            BT_POS_IN_OUT.Text = $"({InforTeaching.Instance.m_dTechPos_In_Out_Tray[0].ToString("f3")}, {InforTeaching.Instance.m_dTechPos_In_Out_Tray[1].ToString("f3")})";
        }

        private void JogXMinus()
        {
            JogAxis(Axis.AXIS_X, 0);
        }
        private void JogXPlus()
        {
            JogAxis(Axis.AXIS_X, 1);
        }
        private void JogYMinus()
        {
            JogAxis(Axis.AXIS_Y, 1);
        }
        private void JogYPlus()
        {
            JogAxis(Axis.AXIS_Y, 0);
        }

        private void JogDisable(object sender, MouseEventArgs e)
        {
            MSystem.SetAllStop();
        }

        public void JogAxis(Axis _Axis, int _Dir)
        {
            if (!MSystem.m_pAxisManager.m_bConnected)
                return;
            switch (JogMode)
            {
                case 0:
                    MSystem.m_pAxisManager.m_pFastechAxis[(int)_Axis].JogMoveSlow(_Dir);
                    break;
                case 1:
                    MSystem.m_pAxisManager.m_pFastechAxis[(int)_Axis].JogMoveMid(_Dir);
                    break;
                case 2:
                    MSystem.m_pAxisManager.m_pFastechAxis[(int)_Axis].JogMoveFast(_Dir);
                    break;
                default: 
                    break;
            }
        }

        private void BT_SPEED_Click(object sender, EventArgs e)
        {
            JogMode++;
            if (JogMode <= 2)
            {
                LoadJogModeDisplay();
            }
            else
            {
                JogMode = 0;
                LoadJogModeDisplay();
            }    
        }
        public void LoadJogModeDisplay()
        {
            if (JogMode == 0 && BT_SPEED.Text != "Low") BT_SPEED.Text = "Low";
            else if (JogMode == 1 && BT_SPEED.Text != "Mid") BT_SPEED.Text = "Mid";
            else if (JogMode == 2 && BT_SPEED.Text != "Hight") BT_SPEED.Text = "Hig";
        }

        private void AxBT_Write_Pos_StartXY_ClickEvent(object sender, EventArgs e)
        {
            if (!MSystem.IsOkeyTeaching() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All servo before Save data position", "Error", msgButton.OK, msgIcon.Error);
                return;
            }

            if (MSystem.MyMsgMemo($"Are you want Save Position POS START RUN?", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (MSystem.SIMULATION)
            {
                return;
            }
            InforTeaching.Instance.m_dTechPos_StartXY = new double[] { MSystem.GetCurrentPos((int)Axis.AXIS_X),
            MSystem.GetCurrentPos((int)Axis.AXIS_Y)};
            InforTeaching.Instance.SaveSettings();
        }

        private void AxBT_Write_Pos_EndXY_ClickEvent(object sender, EventArgs e)
        {
            if (!MSystem.IsOkeyTeaching() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All servo before Save data position", "Error", msgButton.OK, msgIcon.Error);
                return;
            }

            if (MSystem.MyMsgMemo($"Are you want Save Position POS START RUN?", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (MSystem.SIMULATION)
            {
                return;
            }

            InforTeaching.Instance.m_dTechPos_EndXY = new double[] { MSystem.GetCurrentPos((int)Axis.AXIS_X),
            MSystem.GetCurrentPos((int)Axis.AXIS_Y)};
            InforTeaching.Instance.SaveSettings();
        }

        private void AxBT_GO_POS_START_ClickEvent(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo($"Do you want go to POSITION START", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (MSystem.SIMULATION)
            {
                return;
            }

            Task.Run(() =>
            {
                MSystem.m_pTrsTransferXY.MovePosition(Axis.AXIS_X, MSystem.m_pTrsTransferXY.PosStartX());
            });

            Task.Run(() =>
            {
                MSystem.m_pTrsTransferXY.MovePosition(Axis.AXIS_Y, MSystem.m_pTrsTransferXY.PosStartY());
            });
        }

        private void AxBT_GO_POS_END_ClickEvent(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo($"Do you want go to POSITION END", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            if (MSystem.SIMULATION)
            {
                return;
            }
            Task.Run(() =>
            {
                MSystem.m_pTrsTransferXY.MovePosition(Axis.AXIS_X, MSystem.m_pTrsTransferXY.PosEndX());
            });
            Task.Run(() =>
            {
                MSystem.m_pTrsTransferXY.MovePosition(Axis.AXIS_Y, MSystem.m_pTrsTransferXY.PosEndY());
            });
        }

        private void Ax_BT_GO_POS_IN_OUT_ClickEvent(object sender, EventArgs e)
        {
            if (MSystem.MyMsgMemo($"Do you want go to POSITION IN OUT TRAY", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            if (MSystem.SIMULATION)
            {
                return;
            }
            Task.Run(() =>
            {
                MSystem.m_pTrsTransferXY.MovePosition(Axis.AXIS_X, InforTeaching.Instance.m_dTechPos_In_Out_Tray[0]);
            });
            Task.Run(() =>
            {
                MSystem.m_pTrsTransferXY.MovePosition(Axis.AXIS_Y, InforTeaching.Instance.m_dTechPos_In_Out_Tray[1]);
            });
        }

        private void Ax_BT_POS_IN_OUT_ClickEvent(object sender, EventArgs e)
        {
            InforTeaching.Instance.m_dTechPos_In_Out_Tray = new double[] { MSystem.GetCurrentPos((int)Axis.AXIS_X),
            MSystem.GetCurrentPos((int)Axis.AXIS_Y)};
            InforTeaching.Instance.SaveSettings();
        }

        private void axBtnGrabImage1_ClickEvent(object sender, EventArgs e)
        {
            PictureCamera1.Image = null;
            MSystem._camera[0].Trigger();
            Bitmap bmp = PictureCamera1.Image as Bitmap;
            PictureCamera1.Image = MSystem._camera[0].m_bitmap;
            if (bmp != null)
            {
                bmp.Dispose();
            }
        }

        private void axBtnLiveCamera1_ClickEvent(object sender, EventArgs e)
        {
            MSystem._camera[0].ContinuesMode();
            Bitmap bmp = PictureCamera1.Image as Bitmap;
            PictureCamera1.Image = MSystem._camera[0].m_bitmap;
            if (bmp != null)
            {
                bmp.Dispose();
            }
        }

        private void axBtnGrabImage2_ClickEvent(object sender, EventArgs e)
        {
            PictureCamera2.Image = null;
            MSystem._camera[1].Trigger();
            Bitmap bmp = PictureCamera2.Image as Bitmap;
            PictureCamera2.Image = MSystem._camera[1].m_bitmap;
            if (bmp != null)
            {
                bmp.Dispose();
            }
        }

        private void axBtnLiveCamera2_ClickEvent(object sender, EventArgs e)
        {
            MSystem._camera[1].ContinuesMode();
            Bitmap bmp = PictureCamera2.Image as Bitmap;
            PictureCamera2.Image = MSystem._camera[1].m_bitmap;
            if (bmp != null)
            {
                bmp.Dispose();
            }
        }

        public void GrabImage(int idx)
        {
            MSystem._camera[idx].Trigger();
            Bitmap bmp = PictureCamera2.Image as Bitmap;
            PictureCamera2.Image = MSystem._camera[idx].m_bitmap;
            if (bmp != null)
            {
                bmp.Dispose();
            }
        }

        public void LiveCamera(int idx)
        {
            MSystem._camera[idx].ContinuesMode();
            Bitmap bmp = PictureCamera1.Image as Bitmap;
            PictureCamera1.Image = MSystem._camera[idx].m_bitmap;
            if (bmp != null)
            {
                bmp.Dispose();
            }
        }

        private void axBtnCheckCam1_ClickEvent(object sender, EventArgs e)
        {

        }

        private void axBtnCheckCam2_ClickEvent(object sender, EventArgs e)
        {

        }
    }
}
