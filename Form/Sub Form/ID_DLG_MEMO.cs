using System;
using System.Drawing;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FMsgMemo : Form
    {
        Image _ImgQuestion = Image.FromFile("Image/res/question.bmp");
        Image _ImgInfor = Image.FromFile("Image/res/warning.bmp");
        Image _ImgError = Image.FromFile("Image/res/error.bmp");
        public msgIcon _Icon = new msgIcon();
        public msgButton _Btmsg = new msgButton();
        public string strmsg = "";
        private static FMsgMemo MsgBox;
        public static bool KillLife = false;
        public static bool CVNG_Full = false;

        Timer CheckCondition = new Timer();
        Timer CheckCondition2 = new Timer();
        Timer TimerBlink = new Timer();
        public FMsgMemo(msgIcon _icon)
        {
            InitializeComponent();
            Point check = FormMain.GetLocation();
            //this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);

            BT_YES.DialogResult = DialogResult.Yes;
            BT_NO.DialogResult = DialogResult.No;
            BT_OK.DialogResult = DialogResult.OK;
            BT_RESET.DialogResult = DialogResult.OK;
            MSystem.m_bIsTeachDlg = true;

            //TimerBlink.Tick += new EventHandler(Blink);
            //TimerBlink.Interval = 1000;
            //TimerBlink.Start();

        }

        void Blink(object sender, EventArgs e)
        {
            if (MsgBox.Text.Contains("Emergency") || MsgBox.Text.Contains("Light Curtain") ||
                MsgBox.Text.Contains("Door"))
            {
                if (MsgBox.LB_MSG.ForeColor == Color.Yellow)
                    MsgBox.LB_MSG.ForeColor = Color.Red;
                else
                    MsgBox.LB_MSG.ForeColor = Color.Yellow;

                if (MsgBox.BackColor == Color.Yellow)
                    MsgBox.BackColor = Color.Red;
                else
                    MsgBox.BackColor = Color.Yellow;
            }
        }
       
        public static DialogResult Show(string contentErr, string strTitle, msgButton _bt, msgIcon _icon)
        {
            MsgBox = new FMsgMemo(_icon);
            MsgBox.strmsg = contentErr;
            MsgBox._Btmsg = _bt;
            MsgBox._Icon = _icon;
            MsgBox.Text = strTitle;
            MsgBox.InitMsg(_icon);
            if (MsgBox.Text.Contains("Emergency") || MsgBox.Text.Contains("Light Curtain") ||
                MsgBox.Text.Contains("Door"))
            {
                MsgBox.Size = MsgBox.MinimumSize = MsgBox.MaximumSize = new Size(1100, 800);
                MsgBox.LB_TITTLE.Text = MsgBox.Text;
                MsgBox.LB_TITTLE.Visible = true;
                MsgBox.LB_TITTLE.Location = new Point(0, 0);
                MsgBox.LB_TITTLE.Size = new Size(1100, 123);
                //MsgBox.BT_OK.Location = new Point(393, 680);
                MsgBox.BT_OK.Location = new Point(MsgBox.Size.Width / 2 + MsgBox.BT_OK.Size.Width / 4, 680);
                MsgBox.BT_BUZZ_OFF.Location = new Point(MsgBox.Size.Width / 2 - MsgBox.BT_BUZZ_OFF.Size.Width - MsgBox.BT_BUZZ_OFF.Size.Width / 4, 680);
                //MsgBox.BT_RESET.Location = new Point(666, 680);
                MsgBox.LB_MSG.Size = new Size(1100, 159);
                MsgBox.LB_MSG.Location = new Point(0, 480);
                MsgBox.LB_MSG.ForeColor = Color.Blue;
                MsgBox.LB_MSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                MsgBox.BackColor = Color.Yellow;
                //MsgBox.BT_IMAGE.BackgroundImage = Image.FromFile("Image/res/Safety_Alarm1.png");
                MsgBox.BT_IMAGE.Visible = true;
                MsgBox.BT_IMAGE.Size = new Size(350, 329);
                MsgBox.BT_IMAGE.Location = new Point(MsgBox.Size.Width / 2 - MsgBox.BT_IMAGE.Size.Width / 2, 180);
                MsgBox.Location = new Point(FormMain.GetLocation().X + (1280 - MsgBox.Width) / 2, FormMain.GetLocation().Y + (1024 - MsgBox.Height) / 2);
            }

            else
            {
                MsgBox.LB_TITTLE.Visible = false;
                MsgBox.Size = new Size(720, 283);
                MsgBox.BT_OK.Location = new Point(408, 178);
                MsgBox.LB_MSG.Location = new Point(97, 15);
                MsgBox.LB_MSG.ForeColor = Color.Black;
                MsgBox.LB_MSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                MsgBox.BackColor = Color.White;
                MsgBox.BT_IMAGE.Visible = true;
                MsgBox.BT_IMAGE.Location = new Point(26, 60);
                MsgBox.Location = new Point(FormMain.GetLocation().X + (1280 - MsgBox.Width) / 2, FormMain.GetLocation().Y + (1024 - MsgBox.Height) / 2);
            }
            return MsgBox.ShowDialog();
        }
        private void MyMessenger_Load(object sender, EventArgs e)
        {

        }
        public void InitMsg(msgIcon _icon)
        {
            //Image Memo
            switch (_icon)
            {
                case msgIcon.Question:
                    BT_IMAGE.BackgroundImage = _ImgQuestion;
                    break;
                case msgIcon.Infor:
                    BT_IMAGE.BackgroundImage = _ImgInfor;
                    break;
                case msgIcon.Error:
                    BT_IMAGE.BackgroundImage = _ImgError;
                    break;
                case msgIcon.eMessgMax:
                default:
                    BT_IMAGE.BackgroundImage = null;
                    break;
            }
            //Button Result
            switch (_Btmsg)
            {
                case msgButton.YESNO:
                    BT_YES.Visible = true;
                    BT_NO.Visible = true;
                    BT_OK.Visible = false;
                    BT_BUZZ_OFF.Visible = false;
                    BT_RESET.Visible = false;
                    break;
                case msgButton.OK:
                    BT_YES.Visible = false;
                    BT_NO.Visible = false;
                    BT_OK.Visible = true;
                    BT_BUZZ_OFF.Visible = false;
                    BT_RESET.Visible = false;
                    break;
                case msgButton.SAFETY:
                    BT_YES.Visible = false;
                    BT_NO.Visible = false;
                    BT_OK.Visible = true;
                    BT_BUZZ_OFF.Visible = true;
                    BT_RESET.Visible = false;
                    break;
                case msgButton.eBtMax:
                default:
                    BT_YES.Visible = false;
                    BT_NO.Visible = false;
                    BT_OK.Visible = true;
                    BT_BUZZ_OFF.Visible = false;
                    BT_RESET.Visible = false;
                    break;
            }
            LB_MSG.Text = strmsg;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (MSystem.SysStatus == StatusRun.RUN && MSystem.SysMode != ModeRun.DryRun)
            {
                ///*Detect Door Tray Open*/
                // if (MSystem.IsDetectDoorTrayUnit() && MSystem.m_pTrsAutoManager.TimeDoorTray.MoreThan(30.0))
                //{
                //    MSystem.m_pTrsBuzzer.SetBuzzerPattern(0);
                //    MSystem.m_pTrsAutoManager.TimeDoorTray.StartTimer();
                //}


                //if (MSystem.m_pTrsBGTrayUnload.IsDetectTrayZone2() && MSystem.m_pTrsBGTrayUnload.IsDetectTrayZone1()
                //    && MSystem.m_pTrsBGTrayUnload.IsDetectTrayLift() && MSystem.m_pTrsAutoManager.TimeDetectFullTrayOut.MoreThan(30))
                //{
                //    MSystem.m_pTrsAutoManager.TimeDetectFullTrayOut.StartTimer();
                //    MSystem.m_pTrsBuzzer.SetBuzzerPattern(0);
                //}

            }

        }

        private void FMsgMemo_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimerBlink.Stop();
            CheckCondition.Stop();
            CheckCondition.Dispose();
            CheckCondition2.Stop();
            CheckCondition2.Dispose();
            CVNG_Full = false;
            MSystem.m_bIsTeachDlg = false;
        }

        private void BT_BUZZ_OFF_Click(object sender, EventArgs e)
        {
            
        }
    }
}
