using FASTECH;
using SUserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace IRSensorAttachV2
{
    public partial class FormAttachJigTeach : Form
    {
        public enum PosTeach
        {
            //Shuttle Rear
            ePOS_SHUTTLE_REAR_LOAD                 ,
            ePOS_SHUTTLE_REAR_BCR_1                 ,
            ePOS_SHUTTLE_REAR_BCR_2                 ,
            ePOS_SHUTTLE_REAR_UNLOAD                ,

            //Shuttle Front
            ePOS_SHUTTLE_FRONT_LOAD                 ,
            ePOS_SHUTTLE_FRONT_BCR_1                ,
            ePOS_SHUTTLE_FRONT_BCR_2                ,
            ePOS_SHUTTLE_FRONT_UNLOAD               ,

            //TRANSFER REAR
            ePOS_TRANSFER_REAR_LOAD                 ,
            ePOS_TRANSFER_REAR_VISION_JIG_LEFT      ,
            ePOS_TRANSFER_REAR_JIG_LEFT_1           ,
            ePOS_TRANSFER_REAR_JIG_LEFT_2           ,
            
            ePOS_TRANSFER_REAR_VISION_JIG_RIGHT     ,
            ePOS_TRANSFER_REAR_JIG_RIGHT_1          ,
            ePOS_TRANSFER_REAR_JIG_RIGHT_2          ,

            ePOS_TRANSFER_REAR_UNLOAD               ,
            ePOS_TRANSFER_NG_CV_LEFT,
            ePOS_TRANSFER_NG_CV_RIGHT,

            //TRANSFER FRONT
            ePOS_TRANSFER_FRONT_LOAD                ,

            ePOS_TRANSFER_FRONT_VISION_JIG_LEFT     ,
            ePOS_TRANSFER_FRONT_JIG_LEFT_1          ,
            ePOS_TRANSFER_FRONT_JIG_LEFT_2          ,

            ePOS_TRANSFER_FRONT_VISION_JIG_RIGHT    ,
            ePOS_TRANSFER_FRONT_JIG_RIGHT_1         ,
            ePOS_TRANSFER_FRONT_JIG_RIGHT_2         ,

            ePOS_TRANSFER_FRONT_UNLOAD              ,

            //JIg Attach
            ePOS_JIG_LEFT_LOAD                      ,
            ePOS_JIG_LEFT_TRANFER_REAR              ,
            ePOS_JIG_LEFT_TRANFER_FRONT             ,
            ePOS_JIG_LEFT_ATTACH                    ,

            ePOS_JIG_RIGHT_LOAD                    ,
            ePOS_JIG_RIGHT_TRANFER_REAR             ,
            ePOS_JIG_RIGHT_TRANFER_FRONT            ,
            ePOS_JIG_RIGHT_ATTACH                   ,

            ePOS_MAX
        }
        public enum AxisTEACHING
        {
            eAXIS_SHUTTLE_REAR                      = 0,
            eAXIS_SHUTTLE_FRONT                     = 1,
            eAXIS_TRANSFER_REAR                     = 2,
            eAXIS_TRANSFER_FRONT                    = 3,
            eAXIS_JIG_LEFT                          = 4,
            eAXIS_JIG_RIGHT                         = 5,
            eAXIS_MAX
        }
        #region Variable Image Button jog
        //Up
        Image Jog_left_dis = Image.FromFile("Image/res/jog_1_up.bmp");
        Image Jog_left_en  = Image.FromFile("Image/res/jog_1_down.bmp");
        //Up
        Image Jog_right_dis = Image.FromFile("Image/res/jog_2_up.bmp");
        Image Jog_right_en  = Image.FromFile("Image/res/jog_2_down.bmp");
        //Up
        Image Jog_up_dis = Image.FromFile("Image/res/jog_3_up.bmp");
        Image Jog_up_en  = Image.FromFile("Image/res/jog_3_down.bmp");
        //Up
        Image Jog_dow_dis = Image.FromFile("Image/res/jog_4_up.bmp");
        Image Jog_dow_en  = Image.FromFile("Image/res/jog_4_down.bmp");


        Image IO_ON  = Image.FromFile("Image/res/On.ico");
        Image IO_OFF = Image.FromFile("Image/res/Off.ico");
        Image IO_DIS = Properties.Resources.Dis3;

        public static bool ResetButtonJog = false;
        #endregion

        #region Variable data
        bool JogRemember = false;
        public int m_SelecPos = 0;
        int _SelectType = MSystem.eFRONT;
        #endregion

        #region //Timer Update Status
       // System.Windows.Forms.Timer TimUpdate = new System.Windows.Forms.Timer();
        Thread TimUpdate;
        #endregion

        #region //Button Tech
        //Position
        SUserControls.ColorButton[] m_btCurrentPos = new SUserControls.ColorButton[(int)Axis.eAXIS_MAX];
        SUserControls.ColorButton[] m_btTargetPos = new SUserControls.ColorButton[(int)Axis.eAXIS_MAX];

        // private const int MaxPOS = 15;
        CBbutton[] m_btPoselect = new CBbutton[(int)PosTeach.ePOS_MAX];
        #endregion

        public FormAttachJigTeach()
        {
            InitializeComponent();
            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);
            int i = 0;
            #region Init button jog
            /*Load Image default*/
            BT_Y_JOG_UP.BackgroundImage = Jog_up_dis;
            BT_Y_JOG_DOWN.BackgroundImage = Jog_dow_dis;
            BT_X_JOG_LEFT.BackgroundImage = Jog_left_dis;
            BT_X_JOG_RIGHT.BackgroundImage = Jog_right_dis;

            BT_Z_JOG_UP.BackgroundImage = Jog_up_dis;
            BT_Z_JOG_DOWN.BackgroundImage = Jog_dow_dis;

            BT_SHUTTLE_JOG_LEFT.BackgroundImage = Jog_left_dis;
            BT_SHUTTLE_JOG_RIGHT.BackgroundImage = Jog_right_dis;

            /*Reset text on button*/
            BT_Y_JOG_UP.Text = "";
            BT_Y_JOG_DOWN.Text = "";
            BT_X_JOG_LEFT.Text = "";
            BT_X_JOG_RIGHT.Text = "";

            BT_Z_JOG_UP.Text = "";
            BT_Z_JOG_DOWN.Text = "";

            BT_SHUTTLE_JOG_LEFT.Text = "";
            BT_SHUTTLE_JOG_RIGHT.Text = "";

            #endregion

            #region Button Select Position
            i = 0;
            //shuttle Rear
            m_btPoselect[i++] = new CBbutton(BT_SHUTTLE_REAR_LOAD, false);
            m_btPoselect[i++] = new CBbutton(BT_SHUTTLE_REAR_BCR_1, false);
            m_btPoselect[i++] = new CBbutton(BT_SHUTTLE_REAR_BCR_2, false);
            m_btPoselect[i++] = new CBbutton(BT_SHUTTLE_REAR_UNLOAD, false);

            //shuttle Front
            m_btPoselect[i++] = new CBbutton(BT_SHUTTLE_FRONT_LOAD, false);
            m_btPoselect[i++] = new CBbutton(BT_SHUTTLE_FRONT_BCR_1, false);
            m_btPoselect[i++] = new CBbutton(BT_SHUTTLE_FRONT_BCR_2, false);
            m_btPoselect[i++] = new CBbutton(BT_SHUTTLE_FRONT_UNLOAD, false);


            //Transfer Rear
            m_btPoselect[i++] = new CBbutton(BT_TF_REAR_LOAD, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_REAR_VISION_JIG_LEFT, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_REAR_JIG_LEFT_L, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_REAR_JIG_LEFT_R, false);

            m_btPoselect[i++] = new CBbutton(BT_TF_REAR_VISION_JIG_RIGHT, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_REAR_JIG_RIGHT_L, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_REAR_JIG_RIGHT_R, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_REAR_UNLOAD, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_NG_CV_LEFT, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_NG_CV_RIGHT, false);

            //Transfer Front
            m_btPoselect[i++] = new CBbutton(BT_TF_FRONT_LOAD, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_FRONT_VISION_JIG_LEFT, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_FRONT_JIG_LEFT_L, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_FRONT_JIG_LEFT_R, false);

            m_btPoselect[i++] = new CBbutton(BT_TF_FRONT_VISION_JIG_RIGHT, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_FRONT_JIG_RIGHT_L, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_FRONT_JIG_RIGHT_R, false);
            m_btPoselect[i++] = new CBbutton(BT_TF_FRONT_UNLOAD, false);

            //Jig Left
            m_btPoselect[i++] = new CBbutton(BT_JIG_LEFT_LOAD, false);
            m_btPoselect[i++] = new CBbutton(BT_JIG_LEFT_TRANSFER_REAR, false);
            m_btPoselect[i++] = new CBbutton(BT_JIG_LEFT_TRANSFER_FRONT, false);
            m_btPoselect[i++] = new CBbutton(BT_JIG_LEFT_ATTACH, false);

            //Jig RIGHT
            m_btPoselect[i++] = new CBbutton(BT_JIG_RIGHT_LOAD, false);
            m_btPoselect[i++] = new CBbutton(BT_JIG_RIGHT_TRANSFER_REAR, false);
            m_btPoselect[i++] = new CBbutton(BT_JIG_RIGHT_TRANSFER_FRONT, false);
            m_btPoselect[i++] = new CBbutton(BT_JIG_RIGHT_ATTACH, false);

            #endregion

            #region BT Current Position and Target position
            i = 0;
            m_btCurrentPos[i++] = TEACH_CURENT_SHUTTER_REAR;
            m_btCurrentPos[i++] = TEACH_CURENT_SHUTTER_FRONT;
            m_btCurrentPos[i++] = TEACH_CURENT_TF_REAR;
            m_btCurrentPos[i++] = TEACH_CURENT_TF_FRONT;
            m_btCurrentPos[i++] = TEACH_CURENT_JIG_LEFT;
            m_btCurrentPos[i++] = TEACH_CURENT_JIG_RIGHT;

            i = 0;
            m_btTargetPos[i++] = TEACH_TARGET_SHUTTER_REAR;
            m_btTargetPos[i++] = TEACH_TARGET_SHUTTER_FRONT;
            m_btTargetPos[i++] = TEACH_TARGET_TF_REAR;
            m_btTargetPos[i++] = TEACH_TARGET_TF_FRONT;
            m_btTargetPos[i++] = TEACH_TARGET_JIG_LEFT;
            m_btTargetPos[i++] = TEACH_TARGET_JIG_RIGHT;
            #endregion

            if ((int)PosTeach.ePOS_MAX > 0)
            {
                for (i = 0; i < m_btPoselect.Length; i++)
                    m_btPoselect[i].Button.MouseDown += new MouseEventHandler(PreLoadEventPos);
            }


            MSystem.m_pTrsJog._JogMode = 0;
            LoadJogModeDisplay();

            SelectButton(0);
            SelectTargetPost((PosTeach)GetButtonSelected());

            MSystem.m_bIsTeachDlg = true;
            TimUpdate = new Thread(UpDateData);
            TimUpdate.IsBackground = true;
            TimUpdate.Start();

            if (_SelectType == MSystem.eFRONT)
            {
                BT_SELECT_TYPE.Text = "Rear";
               // GR_UNLOADER.Text = "Transfer Rear";
                BT_SELECT_TYPE.ForeColor = Color.Crimson;
                _SelectType = MSystem.eREAR;
            }
            else
            {
                BT_SELECT_TYPE.Text = "Front";
                _SelectType = MSystem.eFRONT;
                BT_SELECT_TYPE.ForeColor = Color.BlueViolet;
               // GR_UNLOADER.Text = "Transfer Front";
            }

        }

        private void FormAttachJigTeach_FormClosed(object sender, FormClosedEventArgs e)
        {
            MSystem.m_bIsTeachDlg = false;
            TimUpdate.Abort();
        }
        private void FormAttachJigTeach_FormClosing(object sender, FormClosingEventArgs e)
        {
            MSystem.m_bIsTeachDlg = false;
        }
       
        #region Event Form 
        void UpDateData()
        {
            while (true) 
            {
                Thread.Sleep(1);
                try
                {
                    /*Disable Jog*/
                    if (!BT_Y_JOG_UP.Press && !BT_Y_JOG_DOWN.Press &&
                       !BT_X_JOG_RIGHT.Press && !BT_X_JOG_LEFT.Press &&
                       !BT_Z_JOG_UP.Press && !BT_Z_JOG_DOWN.Press && 
                       !BT_SHUTTLE_JOG_LEFT.Press && !BT_SHUTTLE_JOG_RIGHT.Press&&
                        JogRemember)
                    {
                        JogRemember = false;
                        DisableJog();
                    }
                    /*Update position*/
                    UpdatePosition();
                    /*Update Sensor and control manual*/
                    JogTP();
                    /**********************************/
                    SensorUpdate();
                    LoadJogModeDisplay();
                }
                catch (Exception ex) 
                {
                    ex.ToString();
                }
            }
        }
        void UpdatePosition()
        {
            for (int i = 0; i < (int)Axis.eAXIS_MAX; i++)
            {
                if (MSystem.m_pAxisManager.m_pFastechAxis[i] != null)
                {
                    m_btCurrentPos[i].Text = MSystem.GetCurrentPos(i).ToString("f3");
                }
            }
        }
        private void JogActive(object sender, MouseEventArgs e)
        {
            JogRemember = true;
            switch ((sender as Button).Name.ToString())
            {
                case "BT_Y_JOG_UP":
                    JogYPlus();
                    MSystem.m_pTrsJog.FlagY1 = true;
                    BT_Y_JOG_UP.BackgroundImage = Jog_up_en;
                    break;
                case "BT_Y_JOG_DOWN":
                    JogYMinus();
                    MSystem.m_pTrsJog.FlagY1 = true;
                    BT_Y_JOG_DOWN.BackgroundImage = Jog_dow_en;
                    break;

                case "BT_Z_JOG_UP":
                    JogZMinus();
                    MSystem.m_pTrsJog.FlagZ1 = true;
                    BT_Z_JOG_UP.BackgroundImage = Jog_up_en;
                    break;
                case "BT_Z_JOG_DOWN":
                    JogZPlus();
                    MSystem.m_pTrsJog.FlagZ1 = true;
                    BT_Z_JOG_DOWN.BackgroundImage = Jog_dow_en;
                    break;

                case "BT_X_JOG_LEFT":
                    JogXMinus();
                    MSystem.m_pTrsJog.FlagX1 = true;
                    BT_X_JOG_LEFT.BackgroundImage = Jog_left_en;
                    break;
                case "BT_X_JOG_RIGHT":
                    JogXPlus();
                    MSystem.m_pTrsJog.FlagX11 = true;
                    BT_X_JOG_RIGHT.BackgroundImage = Jog_right_en;
                    break;
                case "BT_SHUTTLE_JOG_LEFT":
                    JogShuttleMinus();
                    MSystem.m_pTrsJog.FlagX1 = true;
                    BT_SHUTTLE_JOG_LEFT.BackgroundImage = Jog_left_en;
                    break;
                case "BT_SHUTTLE_JOG_RIGHT":
                    JogShuttlePlus();
                    MSystem.m_pTrsJog.FlagX11 = true;
                    BT_SHUTTLE_JOG_RIGHT.BackgroundImage = Jog_right_en;
                    break;
                default: break;
            }
        }
        private void JogDisable(object sender, MouseEventArgs e)
        {
            if (JogRemember)
            {
                JogRemember = false;
                DisableJog();
            }
        }
        void DisableJog()
        {
            try
            {
                BT_Y_JOG_UP.BackgroundImage = Jog_up_dis;
                BT_Y_JOG_DOWN.BackgroundImage = Jog_dow_dis;
                BT_X_JOG_LEFT.BackgroundImage = Jog_left_dis;
                BT_X_JOG_RIGHT.BackgroundImage = Jog_right_dis;

                BT_Z_JOG_UP.BackgroundImage = Jog_up_dis;
                BT_Z_JOG_DOWN.BackgroundImage = Jog_dow_dis;

                BT_SHUTTLE_JOG_LEFT.BackgroundImage = Jog_left_dis;
                BT_SHUTTLE_JOG_RIGHT.BackgroundImage = Jog_right_dis;

                MSystem.SetAllStop();
            }
            catch (Exception)
            {
                BT_Y_JOG_UP.BackgroundImage = Jog_up_dis;
                BT_Y_JOG_DOWN.BackgroundImage = Jog_dow_dis;
                BT_X_JOG_LEFT.BackgroundImage = Jog_left_dis;
                BT_X_JOG_RIGHT.BackgroundImage = Jog_right_dis;

                BT_Z_JOG_UP.BackgroundImage = Jog_up_dis;
                BT_Z_JOG_DOWN.BackgroundImage = Jog_dow_dis;

                BT_SHUTTLE_JOG_LEFT.BackgroundImage = Jog_left_dis;
                BT_SHUTTLE_JOG_RIGHT.BackgroundImage = Jog_right_dis;

                MSystem.MyMsgMemo("Reconnect to servo Motor!", "Servo Error", msgButton.eBtMax, msgIcon.eMessgMax);

            }
            finally 
            {
                MSystem.m_pTrsJog.FlagX1 = MSystem.m_pTrsJog.FlagY1 = MSystem.m_pTrsJog.FlagZ1 = MSystem.m_pTrsJog.FlagJ1 = false;
                MSystem.m_pTrsJog.FlagX2 = MSystem.m_pTrsJog.FlagY2 = MSystem.m_pTrsJog.FlagZ2 = MSystem.m_pTrsJog.FlagJ2 = false;
                MSystem.m_pTrsJog.FlagX11 = MSystem.m_pTrsJog.FlagY11 = MSystem.m_pTrsJog.FlagZ11 = MSystem.m_pTrsJog.FlagJ11 = false;
                MSystem.m_pTrsJog.FlagX22 = MSystem.m_pTrsJog.FlagY22 = MSystem.m_pTrsJog.FlagZ22 = MSystem.m_pTrsJog.FlagJ22 = false;
                MSystem.m_pTrsJog.FlagStop = true;
            }
        }
        private void PreLoadEvent(object sender, EventArgs e)
        {
            switch ((sender as SUserControls.ColorButton).Name)
            {
                case "BT_SAVE":
                    SaveTeachPositionXY();
                    break;
                case "BT_EXIT":
                    this.Close();
                    break;
                case "BT_MOVE":
                    if (MSystem.MyMsgMemo($"Are you want Move To Position {((PosTeach)GetButtonSelected()).ToString()}?", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
                        return;
                    MoveServor();
                    break;
                case "BT_MOVE_UP":
                    MoveZUP();
                    break;
                case "BT_MOVE_DOWN":
                    MoveZDow();
                    break;
                case "BT_SPEED":
                    if (MSystem.m_pTrsJog._JogMode == 0)
                        MSystem.m_pTrsJog._JogMode = 1;
                    else if (MSystem.m_pTrsJog._JogMode == 1)
                        MSystem.m_pTrsJog._JogMode = 2;
                    else MSystem.m_pTrsJog._JogMode = 0;
                    LoadJogModeDisplay();
                    break;
                default: break;
            }
        }
        void LoadJogModeDisplay()
        {
            if (MSystem.m_pTrsJog._JogMode == 0 && BT_SPEED.Text != "Low") BT_SPEED.Text = "Low";
            else if (MSystem.m_pTrsJog._JogMode == 1 && BT_SPEED.Text != "Mid") BT_SPEED.Text = "Mid";
            else if (MSystem.m_pTrsJog._JogMode == 2 && BT_SPEED.Text != "Hight") BT_SPEED.Text = "Hig";
        }
        #endregion

        #region Jog Function 
        void PreLoadEventPos(object sender, MouseEventArgs e)
        {
            SelectButton(sender as Button);

            if (e.Clicks > 1)
            {
                MoveServor();
            }

        }
        void MoveServor() 
        {
            if (MSystem.SIMULATION) return;
            if (!MSystem.IsOriginAll() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All servo before position move", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            PosTeach _Pmove = (PosTeach)GetButtonSelected();
            if (_Pmove == PosTeach.ePOS_TRANSFER_REAR_LOAD || _Pmove == PosTeach.ePOS_TRANSFER_FRONT_LOAD ||
                _Pmove == PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_LEFT || _Pmove == PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_LEFT ||
                _Pmove == PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_1 || _Pmove == PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_1 ||
                _Pmove == PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_2 || _Pmove == PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_2 ||
                _Pmove == PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_RIGHT || _Pmove == PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_RIGHT ||
                _Pmove == PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_1 || _Pmove == PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_1 ||
                _Pmove == PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_2 || _Pmove == PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_2 || 
                _Pmove == PosTeach.ePOS_TRANSFER_REAR_UNLOAD || _Pmove == PosTeach.ePOS_TRANSFER_NG_CV_LEFT || _Pmove == PosTeach.ePOS_TRANSFER_NG_CV_RIGHT)
            {
                MoveUnloaderXY();
            }
            else if (_Pmove == PosTeach.ePOS_JIG_LEFT_LOAD || _Pmove == PosTeach.ePOS_JIG_RIGHT_LOAD ||
                    _Pmove == PosTeach.ePOS_JIG_LEFT_TRANFER_REAR || _Pmove == PosTeach.ePOS_JIG_RIGHT_TRANFER_REAR ||
                    _Pmove == PosTeach.ePOS_JIG_LEFT_TRANFER_FRONT || _Pmove == PosTeach.ePOS_JIG_RIGHT_TRANFER_FRONT ||
                    _Pmove == PosTeach.ePOS_JIG_LEFT_ATTACH || _Pmove == PosTeach.ePOS_JIG_RIGHT_ATTACH)
            {
                MoveJigAttach();
            }
            else if (_Pmove == PosTeach.ePOS_SHUTTLE_REAR_LOAD || _Pmove == PosTeach.ePOS_SHUTTLE_REAR_BCR_1 || _Pmove == PosTeach.ePOS_SHUTTLE_REAR_BCR_2 || _Pmove == PosTeach.ePOS_SHUTTLE_REAR_UNLOAD ||
                _Pmove == PosTeach.ePOS_SHUTTLE_FRONT_LOAD || _Pmove == PosTeach.ePOS_SHUTTLE_FRONT_BCR_1 || _Pmove == PosTeach.ePOS_SHUTTLE_FRONT_BCR_2 || _Pmove == PosTeach.ePOS_SHUTTLE_FRONT_UNLOAD)
            {
                MoveShuttle();
            }
        }
        void UnSelectButton()
        {
            for (int i = 0; i < (int)PosTeach.ePOS_MAX; i++)
            {
                m_btPoselect[i].SetValeUnSelect();
            }
        }
        void SelectButton(Button _bt)
        {
            UnSelectButton();
            m_btPoselect[GetIndexButton(_bt)].SetValue(CBbutton.SelectColor);
            SelectTargetPost((PosTeach)GetIndexButton(_bt));
        }
        void SelectButton(int _index)
        {
            if ((int)PosTeach.ePOS_MAX <= 0) return;
            UnSelectButton();
            m_btPoselect[_index].SetValue(CBbutton.SelectColor);
        }
        int GetIndexButton(Button _bt)
        {
            if ((int)PosTeach.ePOS_MAX <= 0) return -1;
            for (int i = 0; i < (int)PosTeach.ePOS_MAX; i++)
            {
                if (m_btPoselect[i].Button == _bt)
                    return i;
            }
            return -1;
        }
        int GetButtonSelected()
        {
            if ((int)PosTeach.ePOS_MAX <= 0) return -1;
            for (int i = 0; i < (int)PosTeach.ePOS_MAX; i++)
            {
                if (m_btPoselect[i].m_iSel) return i;
            }
            return -1;
        }
        void ResetColorPosition()
        {
            for (int i = 0; i < (int)AxisTEACHING.eAXIS_MAX; i++)
            {
                m_btCurrentPos[i].ForeColor = Color.Black;
                m_btTargetPos[i].ForeColor = Color.Black;
            }
        }

        void SetColorPosition(AxisTEACHING _idAxis)
        {
            m_btCurrentPos[(int)_idAxis].ForeColor = Color.Red;
            m_btTargetPos[(int)_idAxis].ForeColor = Color.Red;
        }
        void SelectTargetPost(PosTeach _PosSelected)
        {
            ResetColorPosition();
            switch (_PosSelected)
            {
                // SHUTTLE REAR
                case PosTeach.ePOS_SHUTTLE_REAR_LOAD:
                    m_btTargetPos[0].Text = $"{InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_LOAD].X.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_SHUTTLE_REAR);
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_1:
                    m_btTargetPos[0].Text = $"{InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_BCR_1].X.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_SHUTTLE_REAR);
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_2:
                    m_btTargetPos[0].Text = $"{InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_BCR_2].X.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_SHUTTLE_REAR);
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_UNLOAD:
                    m_btTargetPos[0].Text = $"{InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_UNLOAD].X.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_SHUTTLE_REAR);
                    break;

                // SHUTTLE FRONT
                case PosTeach.ePOS_SHUTTLE_FRONT_LOAD:
                    m_btTargetPos[1].Text = $"{InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_LOAD].X.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_SHUTTLE_FRONT);
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_1:
                    m_btTargetPos[1].Text = $"{InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_BCR_1].X.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_SHUTTLE_FRONT);
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_2:
                    m_btTargetPos[1].Text = $"{InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_BCR_2].X.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_SHUTTLE_FRONT);
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_UNLOAD:
                    m_btTargetPos[1].Text = $"{InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_UNLOAD].X.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_SHUTTLE_FRONT);
                    break;

                // TRANSFER REAR
                case PosTeach.ePOS_TRANSFER_REAR_LOAD:
                    if(_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_LOAD].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_LOAD].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_LEFT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_VISION_JIG_LEFT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_VISION_JIG_LEFT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_1:
                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_2:
                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_RIGHT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_VISION_JIG_RIGHT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_VISION_JIG_RIGHT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_1:
                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_2:

                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_UNLOAD:
                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_CONVEYOR].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_CONVEYOR].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_NG_CV_LEFT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_NG_CV_LEFT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_NG_CV_LEFT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_NG_CV_RIGHT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        m_btTargetPos[2].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_NG_CV_RIGHT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_REAR);
                        break;
                    }
                    else
                    {
                        m_btTargetPos[3].Text = $"{InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_NG_CV_RIGHT].X.ToString("f03")}";
                        SetColorPosition(AxisTEACHING.eAXIS_TRANSFER_FRONT);
                        break;
                    }

                // JIG LEFT
                case PosTeach.ePOS_JIG_LEFT_LOAD:
                    m_btTargetPos[4].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_LOAD].Y.ToString("f03")}";
                    m_btTargetPos[6].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_LOAD].T1.ToString("f03")}";
                    m_btTargetPos[7].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_LOAD].T2.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_JIG_LEFT);

                    break;
                case PosTeach.ePOS_JIG_LEFT_TRANFER_REAR:
                    m_btTargetPos[4].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_TRANSFER_REAR].Y.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_JIG_LEFT);
                    break;
                case PosTeach.ePOS_JIG_LEFT_TRANFER_FRONT:
                    m_btTargetPos[4].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_TRANSFER_FRONT].Y.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_JIG_LEFT);
                    break;
                case PosTeach.ePOS_JIG_LEFT_ATTACH:
                    m_btTargetPos[4].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_ATTACH].Y.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_JIG_LEFT);
                    break;

                    // JIG RIGHT
                case PosTeach.ePOS_JIG_RIGHT_LOAD:
                    m_btTargetPos[5].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_LOAD].Y.ToString("f03")}";
                    m_btTargetPos[8].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_LOAD].T1.ToString("f03")}";
                    m_btTargetPos[9].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_LOAD].T2.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_JIG_RIGHT);

                    break;
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_REAR:
                    m_btTargetPos[5].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_TRANSFER_REAR].Y.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_JIG_RIGHT);
                    break;
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_FRONT:
                    m_btTargetPos[5].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_TRANSFER_FRONT].Y.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_JIG_RIGHT);
                    break;
                case PosTeach.ePOS_JIG_RIGHT_ATTACH:
                    m_btTargetPos[5].Text = $"{InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_ATTACH].Y.ToString("f03")}";
                    SetColorPosition(AxisTEACHING.eAXIS_JIG_RIGHT);
                    break;

                default: break;
            }
        }
        void SaveDataPosition(PosTeach _PosSelected)
        {
            double dPosX = 0;
            double dPosY = 0;
            double dPosZ = 0;

            switch (_PosSelected)
            {
                
                default: break;
            }
        }
        void SaveDataPositionXY(PosTeach _PosSelected) // SAVE BY "SAVE BUTTON"
        {
            double dPosX = 0;
            double dPosY = 0;
            double dPosZ = 0;

            switch (_PosSelected)
            {
                // SHUTTLE REAR
                case PosTeach.ePOS_SHUTTLE_REAR_LOAD:
                    dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_SHUTTLE_REAR);
                    InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_LOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_REAR].Text = $"{dPosX.ToString("f03")}";
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_1:
                    dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_SHUTTLE_REAR);
                    InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_BCR_1].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_REAR].Text = $"{dPosX.ToString("f03")}";
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_2:
                    dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_SHUTTLE_REAR);
                    InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_BCR_2].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_REAR].Text = $"{dPosX.ToString("f03")}";
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_UNLOAD:
                    dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_SHUTTLE_REAR);
                    InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_UNLOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_REAR].Text = $"{dPosX.ToString("f03")}";
                    break;

                    // SHUTTLE FRONT
                case PosTeach.ePOS_SHUTTLE_FRONT_LOAD:
                    dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_SHUTTLE_FRONT);
                    InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_LOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_FRONT].Text = $"{dPosX.ToString("f03")}";
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_1:
                    dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_SHUTTLE_FRONT);
                    InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_BCR_1].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_FRONT].Text = $"{dPosX.ToString("f03")}";
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_2:
                    dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_SHUTTLE_FRONT);
                    InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_BCR_2].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_FRONT].Text = $"{dPosX.ToString("f03")}";
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_UNLOAD:
                    dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_SHUTTLE_FRONT);
                    InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_UNLOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_FRONT].Text = $"{dPosX.ToString("f03")}";
                    break;

                    // TRANSFER REAR
                case PosTeach.ePOS_TRANSFER_REAR_LOAD:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_LOAD].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_LOAD].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }

                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_LEFT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_VISION_JIG_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_VISION_JIG_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_1:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_2:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_RIGHT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_VISION_JIG_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_VISION_JIG_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;

                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_1:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_2:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;

                    }
                case PosTeach.ePOS_TRANSFER_REAR_UNLOAD:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_CONVEYOR].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_CONVEYOR].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_NG_CV_LEFT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_NG_CV_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_NG_CV_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_NG_CV_RIGHT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_REAR);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_NG_CV_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }
                    else
                    {
                        dPosX = MSystem.GetCurrentPos((int)Axis.AXIS_TRANSFER_FRONT);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_NG_CV_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text = $"{dPosX.ToString("f03")}";
                        break;
                    }

                // JIG LEFT
                case PosTeach.ePOS_JIG_LEFT_LOAD:
                    dPosY = MSystem.GetCurrentPos((int)Axis.AXIS_JIG_LEFT);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_LOAD].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_LEFT].Text = $"{dPosY.ToString("f03")}";
                    break;
                case PosTeach.ePOS_JIG_LEFT_TRANFER_REAR:
                    dPosY = MSystem.GetCurrentPos((int)Axis.AXIS_JIG_LEFT);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_TRANSFER_REAR].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_LEFT].Text = $"{dPosY.ToString("f03")}";
                    break;
                case PosTeach.ePOS_JIG_LEFT_TRANFER_FRONT:
                    dPosY = MSystem.GetCurrentPos((int)Axis.AXIS_JIG_LEFT);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_TRANSFER_FRONT].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_LEFT].Text = $"{dPosY.ToString("f03")}";
                    break;
                case PosTeach.ePOS_JIG_LEFT_ATTACH:
                    dPosY = MSystem.GetCurrentPos((int)Axis.AXIS_JIG_LEFT);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_ATTACH].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_LEFT].Text = $"{dPosY.ToString("f03")}";
                    break;

                // JIG RIGHT
                case PosTeach.ePOS_JIG_RIGHT_LOAD:
                    dPosY = MSystem.GetCurrentPos((int)Axis.AXIS_JIG_RIGHT);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_LOAD].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_RIGHT].Text = $"{dPosY.ToString("f03")}";
                    break;
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_REAR:
                    dPosY = MSystem.GetCurrentPos((int)Axis.AXIS_JIG_RIGHT);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_TRANSFER_REAR].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_RIGHT].Text = $"{dPosY.ToString("f03")}";
                    break;
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_FRONT:
                    dPosY = MSystem.GetCurrentPos((int)Axis.AXIS_JIG_RIGHT);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_TRANSFER_FRONT].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_RIGHT].Text = $"{dPosY.ToString("f03")}";
                    break;
                case PosTeach.ePOS_JIG_RIGHT_ATTACH:
                    dPosY = MSystem.GetCurrentPos((int)Axis.AXIS_JIG_RIGHT);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_ATTACH].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_RIGHT].Text = $"{dPosY.ToString("f03")}";
                    break;

                default: break; 
            }
        }
        void SaveDataPosition(int _Posindex) // SAVE WHEN CHANGE POSITION
        {
            double dPosX = 0;
            double dPosY = 0;
            double dPosZ = 0;
            PosTeach _posSelect = (PosTeach)_Posindex;
            switch (_posSelect)
            {
                // SHUTTLE REAR
                case PosTeach.ePOS_SHUTTLE_REAR_LOAD:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_REAR].Text);
                    InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_LOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_1:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_REAR].Text);
                    InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_BCR_1].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_2:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_REAR].Text);
                    InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_BCR_2].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_SHUTTLE_REAR_UNLOAD:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_REAR].Text);
                    InforTeaching.Instance.P_Shuttle[MSystem.eREAR, (int)PosShuttle.ePOS_SHUTTLE_UNLOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;

                    // SHUTTLE FRONT
                case PosTeach.ePOS_SHUTTLE_FRONT_LOAD:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_FRONT].Text);
                    InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_LOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_1:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_FRONT].Text);
                    InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_BCR_1].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_2:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_FRONT].Text);
                    InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_BCR_2].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_SHUTTLE_FRONT_UNLOAD:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_SHUTTLE_FRONT].Text);
                    InforTeaching.Instance.P_Shuttle[MSystem.eFRONT, (int)PosShuttle.ePOS_SHUTTLE_UNLOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;

                    //TRANSFER REAR
                case PosTeach.ePOS_TRANSFER_REAR_LOAD:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_LOAD].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_LOAD].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_LEFT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_VISION_JIG_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_VISION_JIG_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_1:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_2:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_RIGHT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_VISION_JIG_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_VISION_JIG_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_1:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_2:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_REAR_UNLOAD:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_CONVEYOR].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_CONVEYOR].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }

                case PosTeach.ePOS_TRANSFER_NG_CV_LEFT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_NG_CV_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_NG_CV_LEFT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                case PosTeach.ePOS_TRANSFER_NG_CV_RIGHT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_REAR].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eREAR, (int)PosUnloader.ePOS_NG_CV_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }
                    else
                    {
                        dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                        InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_NG_CV_RIGHT].X = dPosX;
                        InforTeaching.Instance.SaveSettings();
                        break;
                    }


                // TRANSFER FRONT
                case PosTeach.ePOS_TRANSFER_FRONT_LOAD:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                    InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_LOAD].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_LEFT:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                    InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_VISION_JIG_LEFT].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_1:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                    InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_2:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                    InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_RIGHT:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                    InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_VISION_JIG_RIGHT].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_1:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                    InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_2:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                    InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_TRANSFER_FRONT_UNLOAD:
                    dPosX = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_TRANSFER_FRONT].Text);
                    InforTeaching.Instance.P_Unload[MSystem.eFRONT, (int)PosUnloader.ePOS_CONVEYOR].X = dPosX;
                    InforTeaching.Instance.SaveSettings();
                    break;

                    // JIG LEFT
                case PosTeach.ePOS_JIG_LEFT_LOAD:
                    dPosY = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_LEFT].Text);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_LOAD].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_JIG_LEFT_TRANFER_REAR:
                    dPosY = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_LEFT].Text);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_TRANSFER_REAR].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_JIG_LEFT_TRANFER_FRONT:
                    dPosY = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_LEFT].Text);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_TRANSFER_FRONT].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_JIG_LEFT_ATTACH:
                    dPosY = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_LEFT].Text);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eLEFT, (int)PosJigAttach.ePOS_JIG_ATTACH].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    break;

                    // JIG RIGHT
                case PosTeach.ePOS_JIG_RIGHT_LOAD:
                    dPosY = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_RIGHT].Text);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_LOAD].Y = dPosY;

                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_REAR:
                    dPosY = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_RIGHT].Text);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_TRANSFER_REAR].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_FRONT:
                    dPosY = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_RIGHT].Text);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_TRANSFER_FRONT].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    break;
                case PosTeach.ePOS_JIG_RIGHT_ATTACH:
                    dPosY = double.Parse(m_btTargetPos[(int)AxisTEACHING.eAXIS_JIG_RIGHT].Text);
                    InforTeaching.Instance.P_Jig_Attach[MSystem.eRIGHT, (int)PosJigAttach.ePOS_JIG_ATTACH].Y = dPosY;
                    InforTeaching.Instance.SaveSettings();
                    break;

                default: break;
            }
        }
        void SetTargetPort(object sender, EventArgs e)
        {
            int _index = GetButtonSelected();
            double tmpData = 0;
            Button _tmpBt = sender as Button;
            if (_tmpBt.ForeColor != Color.Red) return;
            string old_value = _tmpBt.Text;
            if (!MSystem.Getdouble(sender, ref tmpData))
            {
                _tmpBt.Text = old_value;
                return;
            }
            else 
            {
                SaveDataPosition(_index);
            }
            
        }
        void SaveTeachPosition()
        {
            if (!MSystem.IsOriginAll() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All servo before Save data position", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.MyMsgMemo($"Are you want Save Position {((PosTeach)GetButtonSelected()).ToString()}?", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (MSystem.SIMULATION)
            {
                return;
            }
           // Save data
            var Result = Task.Run(async () => 
            {
                await Task.Delay(100);
                SaveDataPosition((PosTeach)GetButtonSelected());
                await Task.Delay(100);
                MSystem.KillMsgDisplay();
            });
            MSystem.MyMsgDisplay("Start Save Data.....\r\nWait a moment....");
        }
        void SaveTeachPositionXY() // SAVE BY "SAVE BUTTON"
        {
            if (!MSystem.IsSavePosEnable() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All servo before Save data position", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.MyMsgMemo($"Are you want Save Position {((PosTeach)GetButtonSelected()).ToString()}?", "Question", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (MSystem.SIMULATION)
            {
                return;
            }
            // Save data
            var Result = Task.Run(async () =>
            {
                await Task.Delay(100);
                SaveDataPositionXY((PosTeach)GetButtonSelected());
                await Task.Delay(100);
                MSystem.KillMsgDisplay();
            });
            MSystem.MyMsgDisplay("Start Save Data.....\r\nWait a moment....");
        }
        void MoveJigAttach()
        {
            TimerDelay TimCheck = new TimerDelay();
            //Check Safety
            if (MSystem.IsDetectDoorOpen())
            {
                MSystem.MyMsgMemo("Door Open!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.IsDetectEmergency())
            {
                MSystem.MyMsgMemo("EMG Detect!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (!MSystem.IsOriginAll() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All Servo before run Program", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            //Check LEFT and RIGHT Jig
            PosTeach _PMove = (PosTeach)GetButtonSelected();
            int m_iJig = 0;
            int m_iPos = (int)PosJigAttach.ePOS_JIG_LOAD;

            if (_PMove == PosTeach.ePOS_JIG_LEFT_LOAD)
            {
                m_iPos = (int)PosJigAttach.ePOS_JIG_LOAD;
                m_iJig = MSystem.eLEFT;
            }
            else if (_PMove == PosTeach.ePOS_JIG_LEFT_TRANFER_REAR)
            {
                m_iPos = (int)PosJigAttach.ePOS_JIG_TRANSFER_REAR;
                m_iJig = MSystem.eLEFT;
            }
            else if (_PMove == PosTeach.ePOS_JIG_LEFT_TRANFER_FRONT)
            {
                m_iPos = (int)PosJigAttach.ePOS_JIG_TRANSFER_FRONT;
                m_iJig = MSystem.eLEFT;
            }
            else if (_PMove == PosTeach.ePOS_JIG_LEFT_ATTACH)
            {
                m_iPos = (int)PosJigAttach.ePOS_JIG_ATTACH;
                m_iJig = MSystem.eLEFT;
            }

            else if (_PMove == PosTeach.ePOS_JIG_RIGHT_LOAD)
            {
                m_iPos = (int)PosJigAttach.ePOS_JIG_LOAD;
                m_iJig = MSystem.eRIGHT;
            }
            else if (_PMove == PosTeach.ePOS_JIG_RIGHT_TRANFER_REAR)
            {
                m_iPos = (int)PosJigAttach.ePOS_JIG_TRANSFER_REAR;
                m_iJig = MSystem.eRIGHT;
            }
            else if (_PMove == PosTeach.ePOS_JIG_RIGHT_TRANFER_FRONT)
            {
                m_iPos = (int)PosJigAttach.ePOS_JIG_TRANSFER_FRONT;
                m_iJig = MSystem.eRIGHT;
            }
            else if (_PMove == PosTeach.ePOS_JIG_RIGHT_ATTACH)
            {
                m_iPos = (int)PosJigAttach.ePOS_JIG_ATTACH;
                m_iJig = MSystem.eRIGHT;
            }

            else
            {
                MSystem.MyMsgMemo("Select Post Fail In PGM !", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }

            //if (MSystem.m_pTrsJig[m_iJig].IsDetecLightCurtain())
            //{
            //    if(m_iJig == MSystem.eRIGHT)
            //        MSystem.MyMsgMemo(" Right Jig Light Curtain Detected", "Error", msgButton.OK, msgIcon.Error);
            //    else
            //        MSystem.MyMsgMemo(" Left Jig Light Curtain Detected", "Error", msgButton.OK, msgIcon.Error);
            //    return;
            //}

            //Check Cover Jig
            MSystem.m_pTrsJig[m_iJig].CoverUp();
            TimCheck.StartTimer();
            while (true)
            {
                if (MSystem.m_pTrsJig[m_iJig].IsCoverUp()) break;
                if (TimCheck.MoreThan(3.0)) break;
                Thread.Sleep(20);
            }

            if (!MSystem.m_pTrsJig[m_iJig].IsCoverUp())
            {
                if (m_iJig == MSystem.eLEFT)
                {
                    MSystem.MyMsgMemo("Jig Left Cover UP Fail!", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                else
                {
                    MSystem.MyMsgMemo("Jig Right Cover UP Fail!", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
            }

            // Check Transfer Head
            if (!MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadUp() || !MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadUp())
            {
                MSystem.MyMsgMemo("Rear Transfer Unit Head Not Up!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (!MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadUp() || !MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadUp())
            {
                MSystem.MyMsgMemo("Front Transfer Unit Head Not Up!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }

            //Calculator for Move
            double dPosY = InforTeaching.Instance.P_Jig_Attach[m_iJig, m_iPos].Y;
            
            //Start Move 
            var Result = Task.Run(async () =>
            {
                await Task.Delay(200);
                MSystem.SetMsgDisplay("Start Move JIG Attach ....");
                if (m_iJig == MSystem.eLEFT)
                {
                    if (MSystem.SetMove((int)Axis.AXIS_JIG_LEFT, dPosY) != EziMOTIONPlusRLib.FMM_OK)
                    {
                        MSystem.MyMsgMemo("Jig Left Move Fail", "Alarm", msgButton.OK, msgIcon.Error);
                        MSystem.KillMsgDisplay();
                        return;
                    }
                }
                else
                {
                    if (MSystem.SetMove((int)Axis.AXIS_JIG_RIGHT, dPosY) != EziMOTIONPlusRLib.FMM_OK)
                    {
                        MSystem.MyMsgMemo("Jig Right Move Fail", "Alarm", msgButton.OK, msgIcon.Error);
                        MSystem.KillMsgDisplay();
                        return;
                    }
                }

                await Task.Delay(100);
                MSystem.KillMsgDisplay();
            });
            MSystem.MyMsgDisplay("Start Move to Position Select .....\r\nWait a moment....");
        }
       
        void MoveUnloaderXY() 
        {
            TimerDelay TimCheck = new TimerDelay();


            if (MSystem.IsDetectDoorOpen())
            {
                MSystem.MyMsgMemo("Door Open!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.IsDetectEmergency())
            {
                MSystem.MyMsgMemo("EMG Detect!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }

            if (!MSystem.IsOriginAll() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All Servo before run Program", "Error", msgButton.OK, msgIcon.Error);
                return;
            }
            //Get Position 

            PosTeach _PosMove = (PosTeach)GetButtonSelected();
            int m_iJig = 0;
            int m_iPos = (int)PosUnloader.ePOS_LOAD;

            if (_PosMove == PosTeach.ePOS_TRANSFER_REAR_LOAD)
            {
                m_iPos = (int)PosUnloader.ePOS_LOAD;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }
            else if (_PosMove == PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_LEFT)
            {
                m_iPos = (int)PosUnloader.ePOS_VISION_JIG_LEFT;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }
            else if (_PosMove == PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_1)
            {
                m_iPos = (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }
            else if (_PosMove == PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_2)
            {
                m_iPos = (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }
            else if (_PosMove == PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_RIGHT)
            {
                m_iPos = (int)PosUnloader.ePOS_VISION_JIG_RIGHT;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }
            else if (_PosMove == PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_1)
            {
                m_iPos = (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }

            else if (_PosMove == PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_2)
            {
                m_iPos = (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }
            else if (_PosMove == PosTeach.ePOS_TRANSFER_REAR_UNLOAD)
            {
                m_iPos = (int)PosUnloader.ePOS_CONVEYOR;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }

            else if (_PosMove == PosTeach.ePOS_TRANSFER_NG_CV_LEFT)
            {
                m_iPos = (int)PosUnloader.ePOS_NG_CV_LEFT;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }
            else if (_PosMove == PosTeach.ePOS_TRANSFER_NG_CV_RIGHT)
            {
                m_iPos = (int)PosUnloader.ePOS_NG_CV_RIGHT;
                if (_SelectType == MSystem.eREAR)
                    m_iJig = MSystem.eREAR;
                else
                    m_iJig = MSystem.eFRONT;
            }
            //else if (_PosMove == PosTeach.ePOS_TRANSFER_FRONT_LOAD)
            //{
            //    m_iPos = (int)PosUnloader.ePOS_LOAD;
            //    m_iJig = MSystem.eFRONT;
            //}
            //else if (_PosMove == PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_LEFT)
            //{
            //    m_iPos = (int)PosUnloader.ePOS_VISION_JIG_LEFT;
            //    m_iJig = MSystem.eFRONT;
            //}
            //else if (_PosMove == PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_1)
            //{
            //    m_iPos = (int)PosUnloader.ePOS_JIG_LEFT_LOAD_LEFT;
            //    m_iJig = MSystem.eFRONT;
            //}
            //else if (_PosMove == PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_2)
            //{
            //    m_iPos = (int)PosUnloader.ePOS_JIG_LEFT_LOAD_RIGHT;
            //    m_iJig = MSystem.eFRONT;
            //}
            //else if (_PosMove == PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_RIGHT)
            //{
            //    m_iPos = (int)PosUnloader.ePOS_VISION_JIG_RIGHT;
            //    m_iJig = MSystem.eFRONT;
            //}
            //else if (_PosMove == PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_1)
            //{
            //    m_iPos = (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_LEFT;
            //    m_iJig = MSystem.eFRONT;
            //}

            //else if (_PosMove == PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_2)
            //{
            //    m_iPos = (int)PosUnloader.ePOS_JIG_RIGHT_LOAD_RIGHT;
            //    m_iJig = MSystem.eFRONT;
            //}
            //else if (_PosMove == PosTeach.ePOS_TRANSFER_FRONT_UNLOAD)
            //{
            //    m_iPos = (int)PosUnloader.ePOS_CONVEYOR;
            //    m_iJig = MSystem.eFRONT;
            //}

            else
            {
                MSystem.MyMsgMemo("Select Post Fail In PGM !", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }

            MSystem.m_pTrsUnloader[m_iJig].LHeadUp();
            MSystem.m_pTrsUnloader[m_iJig].RHeadUp();
            TimCheck.StartTimer();
            while (true)
            {
                if (MSystem.m_pTrsUnloader[m_iJig].IsLHeadUp() && MSystem.m_pTrsUnloader[m_iJig].IsRHeadUp()) break;
                if (TimCheck.MoreThan(3.0)) break;
                Thread.Sleep(20);
            }
            if (!MSystem.m_pTrsUnloader[m_iJig].IsLHeadUp())
            {
                if (m_iJig == MSystem.eREAR)
                {
                    MSystem.MyMsgMemo(" Rear Transfer Left Head UP Fail!", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                else
                {
                    MSystem.MyMsgMemo(" Front Transfer Left Head UP Fail!", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }

            }
            if (!MSystem.m_pTrsUnloader[m_iJig].IsRHeadUp())
            {
                if (m_iJig == MSystem.eREAR)
                {
                    MSystem.MyMsgMemo(" Rear Transfer Right Head UP Fail!", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                else
                {
                    MSystem.MyMsgMemo(" Front Transfer Right Head UP Fail!", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
            }
            double dPosx = InforTeaching.Instance.P_Unload[m_iJig, m_iPos].X;

            //Start Move 
            var Result = Task.Run(async () =>
            {
                await Task.Delay(200);
                MSystem.SetMsgDisplay("Start Move Transfer X Axis ....");
                if (m_iJig == MSystem.eFRONT)
                {
                    if (MSystem.SetMove((int)Axis.AXIS_TRANSFER_FRONT, dPosx) != EziMOTIONPlusRLib.FMM_OK)
                    {
                        MSystem.MyMsgMemo("Transfer Front Move Fail", "Alarm", msgButton.OK, msgIcon.Error);
                        MSystem.KillMsgDisplay();
                        return;
                    }
                }
                else
                {
                    if (MSystem.SetMove((int)Axis.AXIS_TRANSFER_REAR, dPosx) != EziMOTIONPlusRLib.FMM_OK)
                    {
                        MSystem.MyMsgMemo("Transfer Rear Move Fail", "Alarm", msgButton.OK, msgIcon.Error);
                        MSystem.KillMsgDisplay();
                        return;
                    }
                }
                await Task.Delay(100);
                MSystem.KillMsgDisplay();
            });
            MSystem.MyMsgDisplay("Start Move to Position Select .....\r\nWait a moment....");
        }

        void MoveShuttle()
        {
            TimerDelay TimCheck = new TimerDelay();

            //Check Safety
            if (MSystem.IsDetectDoorOpen())
            {
                MSystem.MyMsgMemo("Door Open!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.IsDetectEmergency())
            {
                MSystem.MyMsgMemo("EMG Detect!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }

            if (!MSystem.IsOriginAll() && !MSystem.SIMULATION)
            {
                MSystem.MyMsgMemo("Please Origin All Servo before run Program", "Error", msgButton.OK, msgIcon.Error);
                return;
            }

            //Check FRONT or REAR Shuttle
            PosTeach _PMove = (PosTeach)GetButtonSelected();
            int m_iJig = 0;
            int m_iPos = (int)PosShuttle.ePOS_SHUTTLE_LOAD;

            if (_PMove == PosTeach.ePOS_SHUTTLE_REAR_LOAD)
            {
                m_iPos = (int)PosShuttle.ePOS_SHUTTLE_LOAD;
                m_iJig = MSystem.eREAR;
            }
            else if (_PMove == PosTeach.ePOS_SHUTTLE_REAR_BCR_1)
            {
                m_iPos = (int)PosShuttle.ePOS_SHUTTLE_BCR_1;
                m_iJig = MSystem.eREAR;
            }
            else if (_PMove == PosTeach.ePOS_SHUTTLE_REAR_BCR_2)
            {
                m_iPos = (int)PosShuttle.ePOS_SHUTTLE_BCR_2;
                m_iJig = MSystem.eREAR;
            }
            else if (_PMove == PosTeach.ePOS_SHUTTLE_REAR_UNLOAD)
            {
                m_iPos = (int)PosShuttle.ePOS_SHUTTLE_UNLOAD;
                m_iJig = MSystem.eREAR;
            }

            else if (_PMove == PosTeach.ePOS_SHUTTLE_FRONT_LOAD)
            {
                m_iPos = (int)PosShuttle.ePOS_SHUTTLE_LOAD;
                m_iJig = MSystem.eFRONT;
            }
            else if (_PMove == PosTeach.ePOS_SHUTTLE_FRONT_BCR_1)
            {
                m_iPos = (int)PosShuttle.ePOS_SHUTTLE_BCR_1;
                m_iJig = MSystem.eFRONT;
            }
            else if (_PMove == PosTeach.ePOS_SHUTTLE_FRONT_BCR_2)
            {
                m_iPos = (int)PosShuttle.ePOS_SHUTTLE_BCR_2;
                m_iJig = MSystem.eFRONT;
            }
            else if (_PMove == PosTeach.ePOS_SHUTTLE_FRONT_UNLOAD)
            {
                m_iPos = (int)PosShuttle.ePOS_SHUTTLE_UNLOAD;
                m_iJig = MSystem.eFRONT;
            }
            else
            {
                MSystem.MyMsgMemo("PGM Error!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }

            if (MSystem.m_pTrsShuttle[m_iJig].IsDetectLightCurtain())
            {
                MSystem.MyMsgMemo("Light Curtain Detected", "Error", msgButton.OK, msgIcon.Error);
                return;
            }

            // Check Reverse Cylinder
            MSystem.m_pTrsReverse[m_iJig].HeadUp();
            TimCheck.StartTimer();
            while (true)
            {
                if (MSystem.m_pTrsReverse[m_iJig].IsHeadUp()) break;
                if (TimCheck.MoreThan(3.0)) break;
                Thread.Sleep(20);
            }

            if (!MSystem.m_pTrsReverse[m_iJig].IsHeadUp())
            {
                if (m_iJig == MSystem.eREAR)
                {
                    MSystem.MyMsgMemo("Rear Shuttle Reverse UP Fail!", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                else
                {
                    MSystem.MyMsgMemo("Front Shuttle Reverse UP Fail!", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }

            }

            // Check Transfer Unit
            if (!MSystem.m_pTrsUnloader[m_iJig].IsLHeadUp() || !MSystem.m_pTrsUnloader[m_iJig].IsRHeadUp())
            {
                MSystem.MyMsgMemo("Transfer Unit Head Not Up!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            //Calculator for Move
            double dPosX = InforTeaching.Instance.P_Shuttle[m_iJig, m_iPos].X;
            //Start Move 
            var Result = Task.Run(async () =>
            {
                await Task.Delay(200);
                MSystem.SetMsgDisplay("Start Move Shuttle ....");
                if (m_iJig == MSystem.eFRONT)
                {
                    if (MSystem.SetMove((int)Axis.AXIS_SHUTTLE_FRONT, dPosX) != EziMOTIONPlusRLib.FMM_OK)
                    {
                        MSystem.MyMsgMemo("Shuttle Front Move Fail", "Alarm", msgButton.OK, msgIcon.Error);
                        MSystem.KillMsgDisplay();
                        return;
                    }
                }
                else
                {
                    if (MSystem.SetMove((int)Axis.AXIS_SHUTTLE_REAR, dPosX) != EziMOTIONPlusRLib.FMM_OK)
                    {
                        MSystem.MyMsgMemo("Shuttle Rear Move Fail", "Alarm", msgButton.OK, msgIcon.Error);
                        MSystem.KillMsgDisplay();
                        return;
                    }
                }
                await Task.Delay(100);
                MSystem.KillMsgDisplay();
            });
            MSystem.MyMsgDisplay("Start Move to Position Select .....\r\nWait a moment....");
        }
        void MoveZUP()
        {
            TimerDelay TimCheck = new TimerDelay();
            //Check Safety
            if (MSystem.IsDetectDoorOpen())
            {
                MSystem.MyMsgMemo("Door Open!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.IsDetectEmergency())
            {
                MSystem.MyMsgMemo("Deetect EMC!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
        }
        void MoveZDow()
        {
            TimerDelay TimCheck = new TimerDelay();
            if (MSystem.IsDetectDoorOpen())
            {
                MSystem.MyMsgMemo("Door Open!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.IsDetectEmergency())
            {
                MSystem.MyMsgMemo("EMG Detect!", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (!MSystem.IsOriginAll())
            {
                MSystem.MyMsgMemo("Please Origin All Servo before run Program", "Error", msgButton.OK, msgIcon.Error);
                return;
            }


            //Get Position 
            double _dX = 0.0;
            double _dY = 0.0;
            double _dZ = 0.0;
            PosTeach _PosMove = (PosTeach)GetButtonSelected();
            
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            /**********************************************************************************/
          
            /**********************************************************************************/
            

        }
        void JogXPlus() 
        {
            PosTeach _PosJog = (PosTeach)GetButtonSelected();
            switch (_PosJog)
            {
                case PosTeach.ePOS_TRANSFER_REAR_LOAD:
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_LEFT:
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_1:
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_2:
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_RIGHT:
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_1:
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_2:
                case PosTeach.ePOS_TRANSFER_REAR_UNLOAD:
                case PosTeach.ePOS_TRANSFER_NG_CV_LEFT:
                case PosTeach.ePOS_TRANSFER_NG_CV_RIGHT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_REAR] = true;
                        MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_REAR] = 1;
                        break;
                    }
                    else
                    {
                        MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_FRONT] = true;
                        MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_FRONT] = 1;
                        break;
                    }

                case PosTeach.ePOS_TRANSFER_FRONT_LOAD:
                case PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_LEFT:
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_1:
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_2:
                case PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_RIGHT:
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_1:
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_2:
                case PosTeach.ePOS_TRANSFER_FRONT_UNLOAD:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_FRONT] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_FRONT] = 1;
                    break;
                default: break;
            }
        }
        void JogXMinus()
        {
            PosTeach _PosJog = (PosTeach)GetButtonSelected();
            switch (_PosJog)
            {
                case PosTeach.ePOS_TRANSFER_REAR_LOAD:
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_LEFT:
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_1:
                case PosTeach.ePOS_TRANSFER_REAR_JIG_LEFT_2:
                case PosTeach.ePOS_TRANSFER_REAR_VISION_JIG_RIGHT:
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_1:
                case PosTeach.ePOS_TRANSFER_REAR_JIG_RIGHT_2:
                case PosTeach.ePOS_TRANSFER_REAR_UNLOAD:
                case PosTeach.ePOS_TRANSFER_NG_CV_LEFT:
                case PosTeach.ePOS_TRANSFER_NG_CV_RIGHT:
                    if (_SelectType == MSystem.eREAR)
                    {
                        MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_REAR] = true;
                        MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_REAR] = 0;
                        break;
                    }
                    else
                    {
                        MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_FRONT] = true;
                        MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_FRONT] = 0;
                        break;
                    }

                case PosTeach.ePOS_TRANSFER_FRONT_LOAD:
                case PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_LEFT:
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_1:
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_LEFT_2:
                case PosTeach.ePOS_TRANSFER_FRONT_VISION_JIG_RIGHT:
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_1:
                case PosTeach.ePOS_TRANSFER_FRONT_JIG_RIGHT_2:
                case PosTeach.ePOS_TRANSFER_FRONT_UNLOAD:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_FRONT] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_FRONT] = 0;
                    break;
                default: break;
            }
        }

        void JogShuttlePlus()
        {
            PosTeach _PosJog = (PosTeach)GetButtonSelected();
            switch (_PosJog)
            {
                case PosTeach.ePOS_SHUTTLE_REAR_LOAD:
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_1:
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_2:
                case PosTeach.ePOS_SHUTTLE_REAR_UNLOAD:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_SHUTTLE_REAR] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_SHUTTLE_REAR] = 1;
                    break;

                case PosTeach.ePOS_SHUTTLE_FRONT_LOAD:
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_1:
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_2:
                case PosTeach.ePOS_SHUTTLE_FRONT_UNLOAD:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_SHUTTLE_FRONT] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_SHUTTLE_FRONT] = 1;
                    break;
                default: break;
            }
        }
        void JogShuttleMinus()
        {
            PosTeach _PosJog = (PosTeach)GetButtonSelected();
            switch (_PosJog)
            {
                case PosTeach.ePOS_SHUTTLE_REAR_LOAD:
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_1:
                case PosTeach.ePOS_SHUTTLE_REAR_BCR_2:
                case PosTeach.ePOS_SHUTTLE_REAR_UNLOAD:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_SHUTTLE_REAR] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_SHUTTLE_REAR] = 0;
                    break;

                case PosTeach.ePOS_SHUTTLE_FRONT_LOAD:
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_1:
                case PosTeach.ePOS_SHUTTLE_FRONT_BCR_2:
                case PosTeach.ePOS_SHUTTLE_FRONT_UNLOAD:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_SHUTTLE_FRONT] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_SHUTTLE_FRONT] = 0;
                    break;
                default: break;
            }
        }

        void JogYPlus()
        {
            PosTeach _PosJog = (PosTeach)GetButtonSelected();

            switch (_PosJog)
            {
                case PosTeach.ePOS_JIG_LEFT_LOAD:
                case PosTeach.ePOS_JIG_LEFT_TRANFER_REAR:
                case PosTeach.ePOS_JIG_LEFT_TRANFER_FRONT:
                case PosTeach.ePOS_JIG_LEFT_ATTACH:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_JIG_LEFT] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_JIG_LEFT] = 1;
                    break;

                case PosTeach.ePOS_JIG_RIGHT_LOAD:
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_REAR:
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_FRONT:
                case PosTeach.ePOS_JIG_RIGHT_ATTACH:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_JIG_RIGHT] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_JIG_RIGHT] = 1;
                    break;
                default: break;
            }
        }
        void JogYMinus()
        {
            PosTeach _PosJog = (PosTeach)GetButtonSelected();

            switch (_PosJog)
            {
                case PosTeach.ePOS_JIG_LEFT_LOAD:
                case PosTeach.ePOS_JIG_LEFT_TRANFER_REAR:
                case PosTeach.ePOS_JIG_LEFT_TRANFER_FRONT:
                case PosTeach.ePOS_JIG_LEFT_ATTACH:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_JIG_LEFT] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_JIG_LEFT] = 0;
                    break;

                case PosTeach.ePOS_JIG_RIGHT_LOAD:
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_REAR:
                case PosTeach.ePOS_JIG_RIGHT_TRANFER_FRONT:
                case PosTeach.ePOS_JIG_RIGHT_ATTACH:
                    MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_JIG_RIGHT] = true;
                    MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_JIG_RIGHT] = 0;
                    break;
                default: break;
            }
        }
        void JogZPlus() 
        {
            PosTeach _PosJog = (PosTeach)GetButtonSelected();
        }
        void JogZMinus()
        {
            PosTeach _PosJog = (PosTeach)GetButtonSelected();
        }

        void JogTP()
        {
            if (JogRemember == false)
            {
                if (MSystem.m_pTrsJog.FlagX11)
                {
                    //X- Minus
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_REAR] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_REAR] = 0;
                    JogShuttleMinus();
                    JogXMinus();

                }
                else if (MSystem.m_pTrsJog.FlagX1)
                {
                    //X-Plus
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_REAR] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_REAR] = 1;
                    JogShuttlePlus();
                    JogXPlus();
                }
                else if (MSystem.m_pTrsJog.FlagX22)
                {
                    //X-Plus
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_FRONT] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_FRONT] = 0;
                }
                else if (MSystem.m_pTrsJog.FlagX2)
                {
                    //X-Plus
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_TRANSFER_FRONT] = true;
                   // MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_TRANSFER_FRONT] = 1;
                }

                if (MSystem.m_pTrsJog.FlagY11)
                {
                    // JogYMinus();
                    // MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_UNLOADER_Y] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_UNLOADER_Y] = 0;
                    JogYMinus();
                }
                else if (MSystem.m_pTrsJog.FlagY1)
                {
                    //JogYPlus();
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_UNLOADER_Y] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_UNLOADER_Y] = 1;
                    JogYPlus();
                }

                if (MSystem.m_pTrsJog.FlagZ11)
                {
                    //JogZPlus();
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_UNLOADER_Z] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_UNLOADER_Z] = 1;
                }
                else if (MSystem.m_pTrsJog.FlagZ1)
                {
                    //JogZMinus();
                   // MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_UNLOADER_Z] = true;
                   // MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_UNLOADER_Z] = 0;
                }

                if (MSystem.m_pTrsJog.FlagJ11)
                {
                    // JogJMinus();
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_JIG_LEFT] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_JIG_LEFT] = 0;
                }
                else if (MSystem.m_pTrsJog.FlagJ1)
                {
                    //JogJPlus();
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_JIG_LEFT] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_JIG_LEFT] = 1;
                }

                if (MSystem.m_pTrsJog.FlagJ22)
                {
                    // JogJMinus();
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_JIG_RIGHT] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_JIG_RIGHT] = 0;
                }
                else if (MSystem.m_pTrsJog.FlagJ2)
                {
                    //JogJPlus();
                    //MSystem.m_pTrsJog._CMDRun[(int)Axis.AXIS_JIG_RIGHT] = true;
                    //MSystem.m_pTrsJog._indexDir[(int)Axis.AXIS_JIG_RIGHT] = 1;
                }
            }
        }

        #endregion
        void SensorUpdate() 
        {
            // SHUTTLE REAR
            MSystem.IsSensorOn(IDC_SHUTTLE_REAR_DETECT_SET_LEFT1, MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectSet(0));
            MSystem.IsSensorOn(IDC_SHUTTLE_REAR_DETECT_SET_LEFT2, MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectSet(1));
            MSystem.IsSensorOn(IDC_SHUTTLE_REAR_DETECT_SET_RIGHT1, MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectSet(2));
            MSystem.IsSensorOn(IDC_SHUTTLE_REAR_DETECT_SET_RIGHT2, MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectSet(3));
           

            //SHUTTLE FRONT
            MSystem.IsSensorOn(IDC_SHUTTLE_FRONT_DETECT_SET_LEFT1, MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectSet(0));
            MSystem.IsSensorOn(IDC_SHUTTLE_FRONT_DETECT_SET_LEFT2, MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectSet(1));
            MSystem.IsSensorOn(IDC_SHUTTLE_FRONT_DETECT_SET_RIGHT1, MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectSet(2));
            MSystem.IsSensorOn(IDC_SHUTTLE_FRONT_DETECT_SET_RIGHT2, MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectSet(3));
            

            // REVERSE REAR
            MSystem.IsSensorOn(IDC_REVERSE_REAR_GRIP_LEFT, MSystem.m_pTrsReverse[MSystem.eREAR].IsLHeadGripOnSen(), MSystem.m_pTrsReverse[MSystem.eREAR].IsLHeadUnGrip());
            MSystem.IsSensorOn(IDC_REVERSE_REAR_GRIP_RIGHT, MSystem.m_pTrsReverse[MSystem.eREAR].IsRHeadGripOnSen(), MSystem.m_pTrsReverse[MSystem.eREAR].IsRHeadUnGrip());
            MSystem.IsSensorOn(IDC_REVERSE_REAR_TURN, MSystem.m_pTrsReverse[MSystem.eREAR].IsTurn(), MSystem.m_pTrsReverse[MSystem.eREAR].IsReTurn());
            MSystem.IsSensorOn(IDC_REVERSE_REAR_UP_DOWN, MSystem.m_pTrsReverse[MSystem.eREAR].IsHeadDown(), MSystem.m_pTrsReverse[MSystem.eREAR].IsHeadUp());

            if (MSystem.m_pTrsReverse[MSystem.eREAR].IsLHeadGripOnSen())
                IDC_REVERSE_REAR_GRIP_LEFT.Text = "Left Grip";
            if (MSystem.m_pTrsReverse[MSystem.eREAR].IsLHeadUnGrip())
                IDC_REVERSE_REAR_GRIP_LEFT.Text = "Left UnGrip";

            if (MSystem.m_pTrsReverse[MSystem.eREAR].IsRHeadGripOnSen())
                IDC_REVERSE_REAR_GRIP_RIGHT.Text = "Right Grip";
            if (MSystem.m_pTrsReverse[MSystem.eREAR].IsRHeadUnGrip())
                IDC_REVERSE_REAR_GRIP_RIGHT.Text = "Right UnGrip";

            if (MSystem.m_pTrsReverse[MSystem.eREAR].IsTurn())
                IDC_REVERSE_REAR_TURN.Text = "Turn";
            if (MSystem.m_pTrsReverse[MSystem.eREAR].IsReTurn())
                IDC_REVERSE_REAR_TURN.Text = "Return";

            if (MSystem.m_pTrsReverse[MSystem.eREAR].IsHeadDown())
                IDC_REVERSE_REAR_UP_DOWN.Text = "Down";
            if (MSystem.m_pTrsReverse[MSystem.eREAR].IsHeadUp())
                IDC_REVERSE_REAR_UP_DOWN.Text = "Up";

            // REVERSE FRONT
            MSystem.IsSensorOn(IDC_REVERSE_FRONT_GRIP_LEFT, MSystem.m_pTrsReverse[MSystem.eFRONT].IsLHeadGripOnSen(), MSystem.m_pTrsReverse[MSystem.eFRONT].IsLHeadUnGrip());
            MSystem.IsSensorOn(IDC_REVERSE_FRONT_GRIP_RIGHT, MSystem.m_pTrsReverse[MSystem.eFRONT].IsRHeadGripOnSen(), MSystem.m_pTrsReverse[MSystem.eFRONT].IsRHeadUnGrip());
            MSystem.IsSensorOn(IDC_REVERSE_FRONT_TURN, MSystem.m_pTrsReverse[MSystem.eFRONT].IsTurn(), MSystem.m_pTrsReverse[MSystem.eFRONT].IsReTurn());
            MSystem.IsSensorOn(IDC_REVERSE_FRONT_UP_DOWN, MSystem.m_pTrsReverse[MSystem.eFRONT].IsHeadDown(), MSystem.m_pTrsReverse[MSystem.eFRONT].IsHeadUp());

            if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsLHeadGripOnSen())
                IDC_REVERSE_FRONT_GRIP_LEFT.Text = "Left Grip";
            if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsLHeadUnGrip())
                IDC_REVERSE_FRONT_GRIP_LEFT.Text = "Left UnGrip";

            if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsRHeadGripOnSen())
                IDC_REVERSE_FRONT_GRIP_RIGHT.Text = "Right Grip";
            if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsRHeadUnGrip())
                IDC_REVERSE_FRONT_GRIP_RIGHT.Text = "Right UnGrip";

            if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsTurn())
                IDC_REVERSE_FRONT_TURN.Text = "Turn";
            if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsReTurn())
                IDC_REVERSE_FRONT_TURN.Text = "Return";

            if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsHeadDown())
                IDC_REVERSE_FRONT_UP_DOWN.Text = "Down";
            if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsHeadUp())
                IDC_REVERSE_FRONT_UP_DOWN.Text = "Up";

            // TRANSFER REAR
            MSystem.IsSensorOn(IDC_TRANSFER_REAR_HEAD_LEFT_UP_DOWN, MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadDown(), MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadUp());
            MSystem.IsSensorOn(IDC_TRANSFER_REAR_HEAD_RIGHT_UP_DOWN, MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadDown(), MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadUp());
            MSystem.IsSensorOn(IDC_TRANSFER_REAR_GRIP_LEFT, MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadGripOnSen(), MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadUnGrip());
            MSystem.IsSensorOn(IDC_TRANSFER_REAR_GRIP_RIGHT, MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadGripOnSen(), MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadUnGrip());

            if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadGripOnSen())
                IDC_TRANSFER_REAR_GRIP_LEFT.Text = "Left Grip";
            if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadUnGrip())
                IDC_TRANSFER_REAR_GRIP_LEFT.Text = "Left\nUnGrip";

            if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadGripOnSen())
                IDC_TRANSFER_REAR_GRIP_RIGHT.Text = "Right Grip";
            if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadUnGrip())
                IDC_TRANSFER_REAR_GRIP_RIGHT.Text = "Right\nUnGrip";

            if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadDown())
                IDC_TRANSFER_REAR_HEAD_LEFT_UP_DOWN.Text = "Left\nDown";
            if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadUp())
                IDC_TRANSFER_REAR_HEAD_LEFT_UP_DOWN.Text = "Left Up";

            if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadDown())
                IDC_TRANSFER_REAR_HEAD_RIGHT_UP_DOWN.Text = "Right\nDown";
            if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadUp())
                IDC_TRANSFER_REAR_HEAD_RIGHT_UP_DOWN.Text = "Right Up";

            // TRANSFER FRONT
            MSystem.IsSensorOn(IDC_TRANSFER_FRONT_HEAD_LEFT_UP_DOWN, MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadDown(), MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadUp());
            MSystem.IsSensorOn(IDC_TRANSFER_FRONT_HEAD_RIGHT_UP_DOWN, MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadDown(), MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadUp());
            MSystem.IsSensorOn(IDC_TRANSFER_FRONT_GRIP_LEFT, MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadGripOnSen(), MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadUnGrip());
            MSystem.IsSensorOn(IDC_TRANSFER_FRONT_GRIP_RIGHT, MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadGripOnSen(), MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadUnGrip());

            if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadGripOnSen())
                IDC_TRANSFER_FRONT_GRIP_LEFT.Text = "Left Grip";
            if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadUnGrip())
                IDC_TRANSFER_FRONT_GRIP_LEFT.Text = "Left\nUnGrip";

            if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadGripOnSen())
                IDC_TRANSFER_FRONT_GRIP_RIGHT.Text = "Right Grip";
            if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadUnGrip())
                IDC_TRANSFER_FRONT_GRIP_RIGHT.Text = "Right\nUnGrip";

            if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadDown())
                IDC_TRANSFER_FRONT_HEAD_LEFT_UP_DOWN.Text = "Left\nDown";
            if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadUp())
                IDC_TRANSFER_FRONT_HEAD_LEFT_UP_DOWN.Text = "Left Up";

            if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadDown())
                IDC_TRANSFER_FRONT_HEAD_RIGHT_UP_DOWN.Text = "Right\nDown";
            if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadUp())
                IDC_TRANSFER_FRONT_HEAD_RIGHT_UP_DOWN.Text = "Right Up";
            // JIG LEFT
            MSystem.IsSensorOn(IDC_JIG_LEFT_UP, MSystem.m_pTrsJig[MSystem.eLEFT].IsCoverDown(), MSystem.m_pTrsJig[MSystem.eLEFT].IsCoverUp());

            if (MSystem.m_pTrsJig[MSystem.eLEFT].IsCoverDown())
                IDC_JIG_LEFT_UP.Text = "Cover Down";
            if (MSystem.m_pTrsJig[MSystem.eLEFT].IsCoverUp())
                IDC_JIG_LEFT_UP.Text = "Cover Up";


            MSystem.IsSensorOn(IDC_JIG_LEFT_VACUUM_ON, MSystem.m_pTrsJig[MSystem.eLEFT].IsVacuumDecoOn());

            // JIG RIGHT
            MSystem.IsSensorOn(IDC_JIG_RIGHT_UP, MSystem.m_pTrsJig[MSystem.eRIGHT].IsCoverDown(), MSystem.m_pTrsJig[MSystem.eRIGHT].IsCoverUp());

            if (MSystem.m_pTrsJig[MSystem.eRIGHT].IsCoverDown())
                IDC_JIG_RIGHT_UP.Text = "Cover Down";
            if (MSystem.m_pTrsJig[MSystem.eRIGHT].IsCoverUp())
                IDC_JIG_RIGHT_UP.Text = "Cover Up";

            MSystem.IsSensorOn(IDC_JIG_RIGHT_VACUUM_ON, MSystem.m_pTrsJig[MSystem.eRIGHT].IsVacuumDecoOn());

            // CONVEYOR REAR
            MSystem.IsSensorOn(IDC_CONVEYOR_REAR_RUN, MSystem.m_pTrsOutCV[MSystem.eREAR].IsCVRun());
            MSystem.IsSensorOn(IDC_CONVEYOR_REAR_SENSOR_INPUT, MSystem.m_pTrsOutCV[MSystem.eREAR].IsDetecInput());
            MSystem.IsSensorOn(IDC_CONVEYOR_REAR_SENSOR_OUTPUT, MSystem.m_pTrsOutCV[MSystem.eREAR].IsDetecOutput());

            // CONVEYOR FRONT
            MSystem.IsSensorOn(IDC_CONVEYOR_FRONT_RUN, MSystem.m_pTrsOutCV[MSystem.eFRONT].IsCVRun());
            MSystem.IsSensorOn(IDC_CONVEYOR_FRONT_SENSOR_INPUT, MSystem.m_pTrsOutCV[MSystem.eFRONT].IsDetecInput());
            MSystem.IsSensorOn(IDC_CONVEYOR_FRONT_SENSOR_OUTPUT, MSystem.m_pTrsOutCV[MSystem.eFRONT].IsDetecOutput());

            MSystem.IsSensorOn(IDC_CONVEYOR_NG_RUN, MSystem.m_pTrsNGCV.IsCVRun());
        }

        private void BT_SELECT_TYPE_Click(object sender, EventArgs e)
        {
            if (_SelectType == MSystem.eFRONT)
            {
                BT_SELECT_TYPE.Text = "Rear";
                BT_SELECT_TYPE.ForeColor = Color.Crimson;
                //GR_UNLOADER.Text = "Unloader Rear";
                _SelectType = MSystem.eREAR;
            }
            else
            {
                BT_SELECT_TYPE.Text = "Front";
                //GR_UNLOADER.Text = "Unloader Front";
                _SelectType = MSystem.eFRONT;
                BT_SELECT_TYPE.ForeColor = Color.BlueViolet;
            }
           // SelectButton(0);
            SelectTargetPost((PosTeach)GetButtonSelected());

        }

        private void IDC_REVERSE_REAR_UP_DOWN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectLightCurtain())
            {
                MSystem.MyMsgMemo("Shuttle - Light Curtain is Detected", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.m_pDIO.IsOn(IO.OUT_SHUTTLE_REAR_REVERSE_DOWN_ON_SOL))
                MSystem.m_pTrsReverse[MSystem.eREAR].HeadUp();
            else
            {
                if (MSystem.m_pTrsReverse[MSystem.eREAR].IsLHeadGripOnSen() || MSystem.m_pTrsReverse[MSystem.eREAR].IsRHeadGripOnSen())
                {
                    MSystem.MyMsgMemo("Rear Shuttle - Reverse Grip On", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }

                if (MSystem.m_pTrsShuttle[MSystem.eREAR].IsPosLoad() && ( MSystem.m_pTrsReverse[MSystem.eREAR].IsLHeadGrip() || MSystem.m_pTrsReverse[MSystem.eREAR].IsRHeadGrip()) &&
                    ( MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectSet(2) || MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectSet(3)))
                {
                    MSystem.MyMsgMemo("Rear Shuttle - Right Jig Set Detect", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                if (MSystem.m_pTrsShuttle[MSystem.eREAR].IsPosUnload() && (MSystem.m_pTrsReverse[MSystem.eREAR].IsLHeadGrip() || MSystem.m_pTrsReverse[MSystem.eREAR].IsRHeadGrip()) &&
                    (MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectSet(0) || MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectSet(1)))
                {
                    MSystem.MyMsgMemo("Rear Shuttle - Left Jig Set Detect", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }

                MSystem.m_pTrsReverse[MSystem.eREAR].HeadDown();
            }
                
        }

        private void IDC_REVERSE_REAR_TURN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsShuttle[MSystem.eREAR].IsDetectLightCurtain())
            {
                MSystem.MyMsgMemo("Shuttle - Light Curtain is Detected", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }

            if (!MSystem.m_pDIO.IsOn(IO.IN_SHUTTLE_REAR_REVERSE_UP_SENS))
            {
                MSystem.MyMsgMemo("Rear Shuttle - Reverse Not Up", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            
            if (MSystem.m_pDIO.IsOn(IO.OUT_SHUTTLE_REAR_REVERSE_TURN_ON_SOL))
                MSystem.m_pTrsReverse[MSystem.eREAR].ReTurn();
            else
                MSystem.m_pTrsReverse[MSystem.eREAR].Turn();
        }
        private void IDC_REVERSE_REAR_GRIP_LEFT_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_SHUTTLE_REAR_REVERSE_LEFT_SET_GRIP_ON_SOL))
                MSystem.m_pTrsReverse[MSystem.eREAR].LHeadUnGrip();
            else
                MSystem.m_pTrsReverse[MSystem.eREAR].LHeadGrip();
        }

        private void IDC_REVERSE_REAR_GRIP_RIGHT_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_SHUTTLE_REAR_REVERSE_RIGHT_SET_GRIP_ON_SOL))
                MSystem.m_pTrsReverse[MSystem.eREAR].RHeadUnGrip();
            else
                MSystem.m_pTrsReverse[MSystem.eREAR].RHeadGrip();
        }

        private void IDC_REVERSE_FRONT_UP_DOWN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectLightCurtain())
            {
                MSystem.MyMsgMemo("Shuttle - Light Curtain is Detected", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }

            if (MSystem.m_pDIO.IsOn(IO.OUT_SHUTTLE_FRONT_REVERSE_DOWN_ON_SOL))
                MSystem.m_pTrsReverse[MSystem.eFRONT].HeadUp();
            else
            {
                if (MSystem.m_pTrsReverse[MSystem.eFRONT].IsLHeadGripOnSen() || MSystem.m_pTrsReverse[MSystem.eFRONT].IsRHeadGripOnSen())
                {
                    MSystem.MyMsgMemo("Front Shuttle - Reverse Grip On", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                if (MSystem.m_pTrsShuttle[MSystem.eFRONT].IsPosLoad() && (MSystem.m_pTrsReverse[MSystem.eFRONT].IsLHeadGrip() || MSystem.m_pTrsReverse[MSystem.eFRONT].IsRHeadGrip()) &&
                    (MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectSet(2) || MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectSet(3)))
                {
                    MSystem.MyMsgMemo("Front Shuttle - Right Jig Set Detect", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                if (MSystem.m_pTrsShuttle[MSystem.eFRONT].IsPosUnload() && (MSystem.m_pTrsReverse[MSystem.eFRONT].IsLHeadGrip() || MSystem.m_pTrsReverse[MSystem.eFRONT].IsRHeadGrip()) &&
                    (MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectSet(0) || MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectSet(1)))
                {
                    MSystem.MyMsgMemo("Front Shuttle - Left Jig Set Detect", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                MSystem.m_pTrsReverse[MSystem.eFRONT].HeadDown();
            }
                
        }
        private void IDC_REVERSE_FRONT_TURN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsShuttle[MSystem.eFRONT].IsDetectLightCurtain())
            {
                MSystem.MyMsgMemo("Shuttle - Light Curtain is Detected", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (!MSystem.m_pDIO.IsOn(IO.IN_SHUTTLE_FRONT_REVERSE_UP_SENS))
            {
                MSystem.MyMsgMemo("Front Shuttle - Reverse Not Up", "Alarm", msgButton.OK, msgIcon.Error);
                return;
            }
            if (MSystem.m_pDIO.IsOn(IO.OUT_SHUTTLE_FRONT_REVERSE_TURN_ON_SOL))
                MSystem.m_pTrsReverse[MSystem.eFRONT].ReTurn();
            else
                MSystem.m_pTrsReverse[MSystem.eFRONT].Turn();
        }
        private void IDC_REVERSE_FRONT_GRIP_LEFT_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_SHUTTLE_FRONT_REVERSE_LEFT_SET_GRIP_ON_SOL))
                MSystem.m_pTrsReverse[MSystem.eFRONT].LHeadUnGrip();
            else
                MSystem.m_pTrsReverse[MSystem.eFRONT].LHeadGrip();
        }

        private void IDC_REVERSE_FRONT_GRIP_RIGHT_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_SHUTTLE_FRONT_REVERSE_RIGHT_SET_GRIP_ON_SOL))
                MSystem.m_pTrsReverse[MSystem.eFRONT].RHeadUnGrip();
            else
                MSystem.m_pTrsReverse[MSystem.eFRONT].RHeadGrip();
        }

        private void IDC_TRANSFER_REAR_HEAD_LEFT_UP_DOWN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_REAR_TRANSFER_LEFT_DOWN_SOL))
                MSystem.m_pTrsUnloader[MSystem.eREAR].LHeadUp();
            else
            {
                if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsLHeadGripOnSen())
                {
                    MSystem.MyMsgMemo("Rear Transfer - Left Head Grip On", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                MSystem.m_pTrsUnloader[MSystem.eREAR].LHeadDown();
            }
                
        }

        private void IDC_TRANSFER_REAR_HEAD_RIGHT_UP_DOWN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_REAR_TRANSFER_RIGHT_DOWN_SOL))
                MSystem.m_pTrsUnloader[MSystem.eREAR].RHeadUp();
            else
            {
                if (MSystem.m_pTrsUnloader[MSystem.eREAR].IsRHeadGripOnSen())
                {
                    MSystem.MyMsgMemo("Rear Transfer - Right Head Grip On", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                MSystem.m_pTrsUnloader[MSystem.eREAR].RHeadDown();
            }
                
        }

        private void IDC_TRANSFER_REAR_GRIP_LEFT_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_REAR_TRANSFER_LEFT_GRIP_ON_SOL))
                MSystem.m_pTrsUnloader[MSystem.eREAR].LHeadUnGrip();
            else
                MSystem.m_pTrsUnloader[MSystem.eREAR].LHeadGrip();
        }

        private void IDC_TRANSFER_REAR_GRIP_RIGHT_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_REAR_TRANSFER_RIGHT_GRIP_ON_SOL))
                MSystem.m_pTrsUnloader[MSystem.eREAR].RHeadUnGrip();
            else
                MSystem.m_pTrsUnloader[MSystem.eREAR].RHeadGrip();
        }

        private void IDC_TRANSFER_FRONT_HEAD_LEFT_UP_DOWN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_FRONT_TRANSFER_LEFT_DOWN_SOL))
                MSystem.m_pTrsUnloader[MSystem.eFRONT].LHeadUp();
            else
            {
                if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsLHeadGripOnSen() )
                {
                    MSystem.MyMsgMemo("Front Transfer - Left Head Grip On", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                MSystem.m_pTrsUnloader[MSystem.eFRONT].LHeadDown();
            }
                
        }

        private void IDC_TRANSFER_FRONT_HEAD_RIGHT_UP_DOWN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_FRONT_TRANSFER_RIGHT_DOWN_SOL))
                MSystem.m_pTrsUnloader[MSystem.eFRONT].RHeadUp();
            else
            {
                if (MSystem.m_pTrsUnloader[MSystem.eFRONT].IsRHeadGripOnSen())
                {
                    MSystem.MyMsgMemo("Front Transfer - Right Head Grip On", "Alarm", msgButton.OK, msgIcon.Error);
                    return;
                }
                MSystem.m_pTrsUnloader[MSystem.eFRONT].RHeadDown();
            }
                
        }

        private void IDC_TRANSFER_FRONT_GRIP_LEFT_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_FRONT_TRANSFER_LEFT_GRIP_ON_SOL))
                MSystem.m_pTrsUnloader[MSystem.eFRONT].LHeadUnGrip();
            else
                MSystem.m_pTrsUnloader[MSystem.eFRONT].LHeadGrip();
        }

        private void IDC_TRANSFER_FRONT_GRIP_RIGHT_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pDIO.IsOn(IO.OUT_FRONT_TRANSFER_RIGHT_GRIP_ON_SOL))
                MSystem.m_pTrsUnloader[MSystem.eFRONT].RHeadUnGrip();
            else
                MSystem.m_pTrsUnloader[MSystem.eFRONT].RHeadGrip();
        }

        private void IDC_JIG_LEFT_UP_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsJig[MSystem.eLEFT].IsCoverUp())
                MSystem.m_pTrsJig[MSystem.eLEFT].CoverDown();
            else if (MSystem.m_pTrsJig[MSystem.eLEFT].IsCoverDown())
                MSystem.m_pTrsJig[MSystem.eLEFT].CoverUp();
            else
                MSystem.m_pTrsJig[MSystem.eLEFT].CoverUp();
        }

        private void IDC_JIG_RIGHT_UP_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsJig[MSystem.eRIGHT].IsCoverUp())
                MSystem.m_pTrsJig[MSystem.eRIGHT].CoverDown();
            else if (MSystem.m_pTrsJig[MSystem.eRIGHT].IsCoverDown())   
                MSystem.m_pTrsJig[MSystem.eRIGHT].CoverUp();
            else
                MSystem.m_pTrsJig[MSystem.eRIGHT].CoverUp();
        }

        private void IDC_CONVEYOR_REAR_RUN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsOutCV[MSystem.eREAR].IsCVRun())
                MSystem.m_pTrsOutCV[MSystem.eREAR].CVStop();
            else
                MSystem.m_pTrsOutCV[MSystem.eREAR].CVRun();
        }

        private void IDC_CONVEYOR_FRONT_RUN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsOutCV[MSystem.eFRONT].IsCVRun())
                MSystem.m_pTrsOutCV[MSystem.eFRONT].CVStop();
            else
                MSystem.m_pTrsOutCV[MSystem.eFRONT].CVRun();
        }

        private void IDC_SHUTTLE_REAR_FWD_Click(object sender, EventArgs e)
        {
          
        }

        private void IDC_SHUTTLE_FRONT_FWD_Click(object sender, EventArgs e)
        {
            
        }

        private void IDC_SHUTTLE_FRONT_DETECT_SET_RIGHT1_Click(object sender, EventArgs e)
        {

        }

        private void IDC_JIG_LEFT_VACUUM_ON_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsJig[MSystem.eLEFT].IsVacuumDecoOnSol())
                MSystem.m_pTrsJig[MSystem.eLEFT].VacuumDecoOff();
            else
                MSystem.m_pTrsJig[MSystem.eLEFT].VacuumDecoOn();
        }

        private void IDC_JIG_RIGHT_VACUUM_ON_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsJig[MSystem.eRIGHT].IsVacuumDecoOnSol())
                MSystem.m_pTrsJig[MSystem.eRIGHT].VacuumDecoOff();
            else
                MSystem.m_pTrsJig[MSystem.eRIGHT].VacuumDecoOn();
        }

        private void IDC_CONVEYOR_NG_RUN_Click(object sender, EventArgs e)
        {
            if (MSystem.m_pTrsNGCV.IsCVRun())
                MSystem.m_pTrsNGCV.CVStop();
            else
                MSystem.m_pTrsNGCV.CVRun();
        }
    }
}
