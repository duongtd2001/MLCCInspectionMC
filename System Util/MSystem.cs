using FASTECH;
using MLCCInspectionMC.Device.Camera;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public class MSystem
    {
        public static string PROGRAM_VER = "MLCC INSPECTION V1_0.260122.01";
        public static string PROGRAM_NAME = "MLCC INSPECTION V1";
        public static string Root = @"C:\FA\MLCCInspectionMC\";

        public const int eLEFT = 0;
        public const int eBOTTOM = 1;

        public const int eFRONT = 1;
        public const int eREAR = 0;
        public const bool ON = true;
        public const bool OFF = false;
        public const bool USE = true;
        public const bool NOT_USE = false;
        public static double _dZReady = 0.0;

        public const int NONE = -1;
        public const int NO_MC = -1;
        public const int NG = 0;
        public const int PASS = 1;
        public static bool m_bUpdateTableMain = false;
        public static bool m_bUpdateTableTeach = false;
        public static int[] m_iTowerLampGeim = { 0, 0, 0 };
        public static string[] _LampState = { "100", "100" };
        public static string[] channels = { "A", "B", "C", "D" };
        public static int[,] MAP_ARRAY;
        public int index_X = 0;
        public int index_Y = 0;
        public int index_SW_Start = 0;
        public int IndexRun = 0;

        public static Stopwatch swGrabImg = new Stopwatch();
        public static Stopwatch swVisionDetect = new Stopwatch();

        public static long _daily_ProductPass = 0;
        public static long _daily_ProductNG = 0;
        public static long _daily_ProductTotal = 0;

        ~MSystem()
        {

        }
        #region //TP Jog 
        #endregion

        public class ImageData
        {
            public int TrayIndex { get; set; }
            public Bitmap Image { get; set; }
        }

        #region Status PGM
        public static ModeRun SysMode = new ModeRun();
        public static StatusRun SysStatus = new StatusRun();
        public static ModeTrigger ModeTrigger = new ModeTrigger();

        public static bool[] m_bCallVision = { false, false };
        public static string modelpath = @"C:\FA\MLCCInspectionMC\PGM\models\best.xml";
        public static int CurrentTrayIndex = 0;

        public static ConcurrentQueue<ImageData>[] m_qImage = new ConcurrentQueue<ImageData>[]{ new ConcurrentQueue<ImageData>(), new ConcurrentQueue<ImageData>() };

        public static HikCam[] _camera = {new HikCam(AoiParam.Instance.SET1_LEFT), new HikCam(AoiParam.Instance.SET1_BOTTOM) };
        //public static HikCam _camera = new HikCam(AoiParam.Instance.SET1_LEFT);
        public static MTrsImageSave m_pImgSave = new MTrsImageSave();
        public static LightPort m_pLightControl = new LightPort(9600, "Light Port");
        public static MTrsTransferXY m_pTrsTransferXY = new MTrsTransferXY();
        public static MTrsTransFlying m_pTrsTransFlying = new MTrsTransFlying();
        public static Button[] m_btPoselect;

        public static MTrsTrigger m_pTrsTrigger = new MTrsTrigger();
        public static MTrsVision[] m_pTrsVision = { new MTrsVision(eLEFT), new MTrsVision(eBOTTOM) };
        //public static MTrsVision m_pTrsVision = new MTrsVision(eLEFT);
        public static double[,] ArrayPos;

        public static YoloManager m_pYoloManager = new YoloManager(modelpath);

        public static Thread _mRun;
        public static Thread _mRun2;

        public static void SetOPStatus(StatusRun _Status)
        {
            SysStatus = _Status;
        }
        #endregion

        #region //Variable for run 

        public static bool SIMULATION = false;
        public static bool m_bSearchIO = true;
        public static bool m_bInitWorkFlag = true;
        public static bool m_bErrorOccur = false;
        public static bool m_bLife = true;
        public static bool m_bOrigin = false;
        public static bool m_bInputStop = false;
        public static bool m_bOutputStop = false;
        public static bool m_InTrayStop = false;
        public static bool m_OutTrayStop = false;
        public static bool m_bButtonStop = false;
        public static bool m_bIsDoorOpen = false;
        public static bool EziMotionTypeE = true;
        public static bool m_bTestRun = false;
        public static bool m_bIsLightCurtainTopInTray = false;
        public static bool m_bIsLightCurtainBotInTray = false;
        public static bool m_bIsLightCurtainTopOutTray = false;
        public static bool m_bIsLightCurtainBotOutTray = false;
        public static bool m_bBCRImageLoad = false;
        public static bool m_bIsServoDisconnect = false;

        public static bool m_LoaderStop = false;
        public static bool m_ShuttleStop = false;
        public static bool m_VilynStop = false;
        #endregion

        #region //Data SystemManager
        //public static DataManager m_pInforManager = new DataManager();
        #endregion

        #region //Variable Lock Thread and Task
        //Thread
        public static readonly object LockGEIM = new object();

        public static readonly object LockTactime = new object();
        public static readonly object LockMessagerError = new object();
        public static readonly object LockLog = new object();
        public static readonly object Lock_DisplayLog = new object();
        public static readonly object LockBuzzer = new object();

        public static readonly object LockButtonMessenger = new object();
        public static readonly object LockInsertAlarm = new object();
        public static readonly object LockAxisManager = new object();

        public static readonly object LockTransferGetJig = new object();
        public static readonly object LockTransferGetNextStep = new object();
        public static readonly object LockTransferGetNextStepatCV = new object();
        public static readonly object LockShuttleCallGMES = new object();
        public static readonly object LockGMESPOLL = new object();
        public static readonly object LockTrayTransfer = new object();
        #endregion

        public static TimerDelay m_iRSTPass_Timer = new TimerDelay();
        public static bool PassOK = false;

        #region // Variable List of tact time
        public static List<double> _tactime = new List<double>();
        public static TimerDelay Tacttime = new TimerDelay();

        public static void LoadParameter()
        {
            MSystem.SysStatus = StatusRun.STOP;
            ServoParam.Instance.LoadSetting();
            InforProduct.Instance.LoadSetting();
            InforManager.Instance.LoadSetting();
            InforTeaching.Instance.LoadSetting();
        }

        #endregion

        #region //PRODUCT INFOR
        public static double GetLassTactime()
        {
            int count = _tactime.Count;
            double LasTactime = 0;
            lock (LockTactime)
            {
                count = _tactime.Count;

                if (count > 0)
                {
                    LasTactime = _tactime[count - 1];
                }

            }
            return LasTactime;
        }
        public static void UpdateTactime()
        {
            lock (LockTactime)
            {
                if (!Tacttime._TimStop && Tacttime.GetTime < 80)
                {
                    _tactime.Add(Tacttime.GetTime);
                }
                Tacttime.ResetTimer();
            }
        }
        public static void INC_TrayOut()
        {
            lock (LockTactime)
            {
                InforProduct.Instance.TrayOut++;
                if (SysMode != ModeRun.DryRun)
                {
                    InforProduct.Instance.SaveSettings();
                }
            }
        }
        public static void INC_ProductInputPressMC(int _idx = 0)
        {
            lock (LockTactime)
            {
                // InforProduct.Instance.PR_Jig_Press_Pass[_idx]++;
                if (SysMode != ModeRun.DryRun)
                {
                    InforProduct.Instance.SaveSettings();
                }
            }
        }
        public static void INC_ProductNG()
        {
            lock (LockTactime)
            {
                InforProduct.Instance.ProductNG++;
                if (SysMode != ModeRun.DryRun)
                {
                    InforProduct.Instance.SaveSettings();
                }
            }
        }

        public static void INC_ProductionNG_GMES()
        {
            lock (LockTactime)
            {
                InforProduct.Instance.Product_GMES_FAIL++;
                if (SysMode != ModeRun.DryRun)
                {
                    InforProduct.Instance.SaveSettings();
                }
            }
        }

        public static void INC_ProductionNG_BCR()
        {
            lock (LockTactime)
            {
                InforProduct.Instance.ProductBCR_FAIL++;
                if (SysMode != ModeRun.DryRun)
                {
                    InforProduct.Instance.SaveSettings();
                }
            }
        }

        public static void ResetAllProductInfor()
        {
            lock (LockTactime)
            {
                InforProduct.Instance.Total_product = 0;
                InforProduct.Instance.ProductPass = 0;
                InforProduct.Instance.ProductNG = 0;
                InforProduct.Instance.TrayOut = 0;
                InforProduct.Instance.ProductBCR_PASS = 0;
                InforProduct.Instance.ProductBCR_FAIL = 0;
                InforProduct.Instance.Product_GMES_FAIL = 0;
                InforProduct.Instance.Product_RemoveVinyl_FAIL = 0;
                InforProduct.Instance.Product_RemoveVinyl_FAIL_Lower = 0;
                InforProduct.Instance.SaveSettings();
                Database_ResetProduct();
            }
        }
        public static void Database_ResetProduct()
        {
            string _DateNow;
            if (DateTime.Now.Hour >= 20)
                _DateNow = $"{DateTime.Now.ToString("yyyyMMdd")}";
            else if (DateTime.Now.Hour < 8)
                _DateNow = $"{DateTime.Now.AddDays(-1).ToString("yyyyMMdd")}";
            else
                _DateNow = $"{DateTime.Now.ToString("yyyyMMdd")}";

            string[] _strtmp = new string[4];
            _strtmp[0] = _DateNow;
            _strtmp[1] = 0.ToString();
            _strtmp[2] = 0.ToString();
            _strtmp[3] = 0.ToString();

            MSystem.m_plisView.TB_Product = MSystem.m_pDatabaseProduct.GetDataSearchProduct(_DateNow + "_" + "A");
            _strtmp[0] = _DateNow + "_" + "A";
            if (MSystem.m_plisView.TB_Product.Rows.Count == 0)
                MSystem.m_pDatabaseProduct.InsertData_Product(_strtmp);
            else
                MSystem.m_pDatabaseProduct.UpdateData_Product(_strtmp);
            _strtmp[0] = _DateNow + "_" + "B";
            MSystem.m_plisView.TB_Product = MSystem.m_pDatabaseProduct.GetDataSearchProduct(_DateNow + "_" + "B");
            if (MSystem.m_plisView.TB_Product.Rows.Count == 0)
                MSystem.m_pDatabaseProduct.InsertData_Product(_strtmp);
            else
                MSystem.m_pDatabaseProduct.UpdateData_Product(_strtmp);
        }
        public static void ResetBCRInfor()
        {
            lock (LockTactime)
            {
                InforProduct.Instance.ProductBCR_PASS = 0;
                InforProduct.Instance.ProductBCR_FAIL = 0;

                InforProduct.Instance.SaveSettings();
            }
        }

        public static void ResetGMESInfor()
        {
            lock (LockTactime)
            {
                InforProduct.Instance.Product_GMES_FAIL = 0;

                InforProduct.Instance.SaveSettings();
            }
        }
        public static void Database_UpdateProduct()
        {
            string _DateNow;
            if (DateTime.Now.Hour >= 20)
                _DateNow = $"{DateTime.Now.ToString("yyyyMMdd")}" + "_" + "B";
            else if (DateTime.Now.Hour < 8)
                _DateNow = $"{DateTime.Now.AddDays(-1).ToString("yyyyMMdd")}" + "_" + "B";
            else
                _DateNow = $"{DateTime.Now.ToString("yyyyMMdd")}" + "_" + "A";

            string[] _strtmp = new string[4];
            _strtmp[0] = _DateNow;
            _strtmp[1] = (InforProduct.Instance.ProductPass + InforProduct.Instance.ProductNG).ToString();
            _strtmp[2] = InforProduct.Instance.ProductPass.ToString();
            _strtmp[3] = InforProduct.Instance.ProductNG.ToString();

            MSystem.m_plisView.TB_Product = MSystem.m_pDatabaseProduct.GetDataSearchProduct(_DateNow);
            if (MSystem.m_plisView.TB_Product.Rows.Count == 0)
                MSystem.m_pDatabaseProduct.InsertData_Product(_strtmp);
            else
                MSystem.m_pDatabaseProduct.UpdateData_Product(_strtmp);
        }
        #endregion

        #region //Display LOG
        public static CtrListView m_plisView = new CtrListView();
        #endregion

        #region Variable dlg display
        public static bool m_bManualIOFlag = false;
        public static bool m_bIsTeachDlg = false;
        public static bool m_bInitManual = false;
        public static int Resolution_X = 1280;
        public static int Resolution_Y = 1024;
        #endregion

        public static bool m_bIsIOMonitorOpen = false;

        #region Flag Unit Working
        public static bool m_bIsWorkingTopInTray_InCV = false;
        public static bool m_bIsWorkingTopInTray_OutCV = false;
        public static bool m_bIsWorkingBotInTray_InCV = false;
        public static bool m_bIsWorkingBotInTray_OutCV = false;
        public static bool m_bIsWorkingInLift = false;
        public static bool m_bIsWorkingTopOutTray_InCV = false;
        public static bool m_bIsWorkingTopOutTray_OutCV = false;
        public static bool m_bIsWorkingBotOutTray_InCV = false;
        public static bool m_bIsWorkingBotOutTray_OutCV = false;
        public static bool m_bIsWorkingOutLift = false;
        public static bool m_bIsWorkingLoader = false;
        public static bool m_bIsWorkingTrayTransfer = false;
        public static bool m_bIsWorkingUpperShuttle = false;
        public static bool m_bIsWorkingLowerShuttle = false;
        public static bool m_bIsWorkingShuttle = false;
        public static bool m_bIsWorkingVilynRemove = false;
        public static bool m_bIsWorkingOutCV = false;
        #endregion

        public static int NumDisplay = 1, NumDisplayOld = 0;
        public static FormTitle TitleForm = new FormTitle();
        public static FormBottom BottomForm = new FormBottom();
        public static FormAuto AutoForm = new FormAuto();
        public static FormLog LogForm = new FormLog();
        public static FormTeach TeachForm = new FormTeach();
        public static FormData DataForm = new FormData();
        public static FormTeachArrPos ArrPosForm = new FormTeachArrPos();
        public static FormMotorVelocity formMotorVelocity = new FormMotorVelocity();

        #region IO Manager
        public static SystemIO m_pDIO = new SystemIO();
        #endregion

        #region // Thread
        //public static Thread _mRun;
        //public static Thread _mRun_VS_BT;
        //public static Thread _mRun_VS_Left;
        #endregion


        #region //Main thread Base Thread 
        public static Alarm m_pAlarm = new Alarm();
        public static MTrsAutoManager m_pTrsAutoManager = new MTrsAutoManager();
        public static MTrLog m_pLogSave = new MTrLog();
        public static MTrsOP m_pTrsOP = new MTrsOP();
        public static MTrLamp m_pTrsLamp = new MTrLamp();

        #endregion

        public static void RunAllThread()
        {
            var MtrLog = Task.Factory.StartNew(() => MSystem.m_pLogSave.LogprocessData());
            var Mtrsauto = Task.Factory.StartNew(() => MSystem.m_pTrsAutoManager.dorunStep());
            var MtrOP = Task.Factory.StartNew(() => MSystem.m_pTrsOP.dorunStep());
            var _MTrslamp = Task.Factory.StartNew(() => MSystem.m_pTrsLamp.Lamp_Poll());
        }

        #region BARCODE AND GMES
        //public const int eShuttle = 0;

        //public static GMESPort m_pGMes = new GMESPort("GMESSYSTEM");
        #endregion

        #region //DataTable for Log
        public static string TableLog = "MLCCInspectionMC";
        public static SQLiteYJ m_pDatabaseLog = new SQLiteYJ(TableLog, Config.DataLogError);
        public static SQLiteYJ m_pDatabaseProduct = new SQLiteYJ("FrontAutoLoader_Product", Config.ProductLog);
        #endregion

        #region //Form Main Initial 

        #region // Initial Main Form
        public static void DisplayMain(System.Windows.Forms.Panel panel, int num)
        {
            switch (num)
            {
                //Auto form
                case (int)NumViewMain.eViewAuto:
                {
                    if (AutoForm.InvokeRequired)
                    {
                        AutoForm.Invoke(new Action(() =>
                        {
                            NumDisplay = 0;
                            MSystem.HideAllForm(panel);
                            MSystem.AutoForm.Visible = true;
                            MSystem.checkBorderBottomForm(BottomForm, (int)NumViewMain.eViewAuto);
                        }));
                    }
                    else
                    {
                        NumDisplay = 0;
                        MSystem.HideAllForm(panel);
                        MSystem.AutoForm.Visible = true;
                        MSystem.checkBorderBottomForm(BottomForm, (int)NumViewMain.eViewAuto);
                    }
                }
                break;
                //Teach Form
                case (int)NumViewMain.eViewTeach:
                {
                    if (TeachForm.InvokeRequired)
                    {
                        TeachForm.Invoke(new Action(() =>
                        {
                            NumDisplay = 0;
                            MSystem.HideAllForm(panel);
                            MSystem.TeachForm.Visible = true;
                            MSystem.checkBorderBottomForm(BottomForm, (int)NumViewMain.eViewTeach);
                        }));
                    }
                    else
                    {
                        NumDisplay = 0;
                        MSystem.HideAllForm(panel);
                        MSystem.TeachForm.Visible = true;
                        MSystem.checkBorderBottomForm(BottomForm, (int)NumViewMain.eViewTeach);
                    }

                }
                break;
                //Data Form
                case (int)NumViewMain.eViewData:
                {
                    if (DataForm.InvokeRequired)
                    {
                        DataForm.Invoke(new Action(() =>
                        {
                            NumDisplay = 0;
                            MSystem.HideAllForm(panel);
                            MSystem.DataForm.Visible = true;
                            MSystem.checkBorderBottomForm(BottomForm, (int)NumViewMain.eViewData);
                        }));
                    }
                    else
                    {
                        NumDisplay = 0;
                        MSystem.HideAllForm(panel);
                        MSystem.DataForm.Visible = true;
                        MSystem.checkBorderBottomForm(BottomForm, (int)NumViewMain.eViewData);
                    }
                }
                break;
                //Log Form
                case (int)NumViewMain.eViewLog:
                {
                    if (LogForm.InvokeRequired)
                    {
                        LogForm.Invoke(new Action(() =>
                        {
                            NumDisplay = 0;
                            MSystem.HideAllForm(panel);
                            MSystem.LogForm.Visible = true;
                            MSystem.checkBorderBottomForm(BottomForm, (int)NumViewMain.eViewLog);
                        }));
                    }
                    else
                    {
                        NumDisplay = 0;
                        MSystem.HideAllForm(panel);
                        MSystem.LogForm.Visible = true;
                        MSystem.checkBorderBottomForm(BottomForm, (int)NumViewMain.eViewLog);
                    }
                }
                break;
                default: break;
            }
        }
        public static void InitialMainForm(System.Windows.Forms.Panel panel)
        {
            MSystem.AddAllForm(panel);
            MSystem.HideAllForm(panel);
        }
        public static void HideAllForm(System.Windows.Forms.Panel panel)
        {
            foreach (System.Windows.Forms.Form p in panel.Controls)
            {
                p.Visible = false;
            }
        }
        public static void AddAllForm(System.Windows.Forms.Panel panel)
        {
            //Auto Form
            MSystem.AutoForm.TopLevel = false;
            panel.Controls.Add(MSystem.AutoForm);
            MSystem.AutoForm.Show();
            //LogForm
            MSystem.LogForm.TopLevel = false;
            panel.Controls.Add(MSystem.LogForm);
            MSystem.LogForm.Show();
            //Teach Form
            MSystem.TeachForm.TopLevel = false;
            panel.Controls.Add(MSystem.TeachForm);
            MSystem.TeachForm.Show();
            //Data Form
            MSystem.DataForm.TopLevel = false;
            panel.Controls.Add(MSystem.DataForm);
            MSystem.DataForm.Show();

        }
        #endregion

        #region//Initial Sub form
        public static void DisplayForm(System.Windows.Forms.Panel panel, System.Windows.Forms.Form form)
        {
            if (form == null)
            {
                form = new System.Windows.Forms.Form();
            }
            form.TopLevel = false;
            panel.Controls.Add(form);
            form.Show();
        }
        public static void checkBorderBottomForm(System.Windows.Forms.Form form, int index)
        {
            foreach (var pb in form.Controls.OfType<SUserControls.ColorButton>())
            {
                pb.Checked = false;
            }
            switch (index)
            {
                case 1:
                {
                    MSystem.BottomForm.BtAuto.Checked = true;
                }
                break;
                case 2:
                {
                    MSystem.BottomForm.BtTeach.Checked = true;
                }
                break;
                case 3:
                {
                    MSystem.BottomForm.BtData.Checked = true;
                }
                break;
                case 4:
                {
                    MSystem.BottomForm.BtExit.Checked = true;
                }
                break;

                default: break;
            }
        }
        #endregion

        #endregion

        #region Teach Form
        public static void AddAllFormTeach(System.Windows.Forms.Panel panel)
        {
            ArrPosForm.TopLevel = false;
            panel.Controls.Add(ArrPosForm);
        }
        public static void HideAllFormTeach(System.Windows.Forms.Panel panel)
        {
            foreach (System.Windows.Forms.Form p in panel.Controls)
            {
                p.Visible = false;
            }
        }
        public static void InitialMainFormTeach(System.Windows.Forms.Panel panel)
        {
            MSystem.AddAllFormTeach(panel);
            MSystem.HideAllFormTeach(panel);
        }
        #endregion

        #region // Messenger Display
        public static void SetError(int _IDERROR, string strName)
        {
            //SysStatus = StatusRun.ERROR;
            m_pAxisManager.m_pFastechAxis[(int)Axis.AXIS_X].Stop();
            m_pAxisManager.m_pFastechAxis[(int)Axis.AXIS_Y].Stop();
            m_InTrayStop = m_OutTrayStop = true;
            if (MSystem.SysStatus == StatusRun.ERROR)
                return;
            m_pAlarm.SetErrorMsg(_IDERROR, strName);
        }
        public static System.Windows.Forms.DialogResult MyMsgMemo(string strmsg, string strTitle, msgButton _bt = msgButton.OK, msgIcon _icon = msgIcon.Infor)
        {
            return MSystem.BottomForm.msgMemo(strmsg, strTitle, _bt, _icon);
        }
        public static void MyMsgDisplay(string strcontent)
        {
            MSystem.BottomForm.msgDisplay(strcontent);
        }
        public static void SetMsgDisplay(string strcontent)
        {
            FMsgDisplay.strMsg = strcontent;
        }
        public static void KillMsgDisplay()
        {
            FMsgDisplay.KillLife = true;
        }

        public static void MyMessagerBottom(string messager)
        {
            messager = DateTime.Now.ToString("yyyy-MM-dd : HH:mm:ss : ") + messager;
            lock (LockButtonMessenger)
            {
                BottomForm.LbHistory.Items.Insert(0, messager);
                if (BottomForm.LbHistory.Items.Count > 200)
                {
                    BottomForm.LbHistory.Items.RemoveAt(200);
                }
            }
        }
        #endregion

        #region //Status PGM

        #endregion

        #region // Servo Data
        public static int eAXIS_MAX = (int)Axis.eAXIS_MAX;
        public static sAxisInfo[] sAxisInfo = new sAxisInfo[eAXIS_MAX];
        public static AxisManager m_pAxisManager = new AxisManager();
        //public static AxisManager m_pAxisManager = new AxisManager(InforManager.Instance.ServoPort);

        #region Function For Servo

        public static bool IsOriginAll()
        {
            for (int i = 0; i < eAXIS_MAX; i++)
            {
                //if (m_pAxisManager.m_pFastechAxis[i] == null || !m_pAxisManager.m_pFastechAxis[i].m_bOriginFlag)
                if (m_pAxisManager.m_pFastechAxis[i] == null || !m_pAxisManager.m_pFastechAxis[i].IsOrigin() || !IsServoOn(i))
                    return false;
            }
            return true;
        }

        public static bool IsSavePosEnable()
        {
            for (int i = 0; i < eAXIS_MAX; i++)
            {
                //if (m_pAxisManager.m_pFastechAxis[i] == null || !m_pAxisManager.m_pFastechAxis[i].m_bOriginFlag)
                if (m_pAxisManager.m_pFastechAxis[i] == null || !m_pAxisManager.m_pFastechAxis[i].IsOrigin())
                    return false;
            }
            return true;
        }

        public static bool IsServoOn(int _Axis)
        {
            return m_pAxisManager.m_pFastechAxis[_Axis].GetAmpEnable();
        }

        public static bool IsServoOrigin(Axis _iaxis)
        {
            if (m_pAxisManager.m_pFastechAxis[(int)_iaxis] == null || !m_pAxisManager.m_pFastechAxis[(int)_iaxis].m_bOriginFlag)
                return false;
            else
                return true;
        }
        public static int Origin_SetMove(int iAxis, double dPosition)
        {
            int iRtn;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].StartMove(dPosition);
            if (iRtn != 0)
            {
                return iRtn;
            }

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].Origin_WaitDone();
            if (iRtn != 0)
            {
                return iRtn;
            }
            return 0;
        }

        public static int SetMove(int iAxis, double dPosition)
        {
            if (MSystem.SIMULATION) return 0;
            int iRtn;
            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].StartMove(dPosition);
            if (iRtn != 0)
            {
                return iRtn;
            }

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].WaitDone();
            if (iRtn != 0)
            {
                return iRtn;
            }

            return 0;
        }
        public static int SetMove(int iAxis, double dPosition, double _Speed)
        {
            if (MSystem.SIMULATION) return 0;
            int iRtn;
            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].StartMove(dPosition, _Speed);
            if (iRtn != 0)
            {
                return iRtn;
            }

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].WaitDone();
            if (iRtn != 0)
            {
                return iRtn;
            }

            return 0;
        }
        public static int SetMoveNoWait(int iAxis, double dPosition, double _Speed)
        {
            if (MSystem.SIMULATION) return 0;
            int iRtn;
            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].StartMove(dPosition, _Speed);
            if (iRtn != 0)
            {
                return iRtn;
            }
            return 0;
        }
        public static int SetMoveNoWait(int iAxis, double dPosition)
        {
            if (MSystem.SIMULATION) return 0;
            int iRtn;
            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].StartMove(dPosition);
            if (iRtn != 0)
            {
                return iRtn;
            }
            return 0;
        }
        public static int SetSlowMove(int iAxis, double dPosition, double dSpeed)
        {
            int iRtn;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].StartMove(dPosition, dSpeed);
            if (iRtn != 0)
            {
                return iRtn;
            }

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis].WaitDone();
            if (iRtn != 0)
            {
                return iRtn;
            }

            return 0;
        }

        public static bool IsAxisPositionCheck(int iAxis, double dPosition)
        {
            if (MSystem.SIMULATION)
                return false;

            double dCurPos = m_pAxisManager.m_pFastechAxis[iAxis].GetCurrentPos();
            double tar_pos = dPosition;

            if (Math.Abs(dCurPos - tar_pos) < 0.5)
                return true;// OK

            return false; // No
        }

        public static double IsAxisCurrentPos(int iAxis)
        {
            if (MSystem.SIMULATION)
                return 0;

            return m_pAxisManager.m_pFastechAxis[iAxis].GetCurrentPos();
        }

        public static bool IsInterLockPositionCheck(int iAxis, double dPosition)
        {
            if (SIMULATION) return true;
            double dCurPos = m_pAxisManager.m_pFastechAxis[iAxis].GetCurrentPos();
            double tar_pos = dPosition;

            if (Math.Abs(dCurPos - tar_pos) < 100)
                return true;// OK

            return false; // No
        }

        public static int SetMoveLinear1(int iAxis1, double dPos1, int iAxis2, double dPos2)
        {
            //보간으로 이동이 아닌 각각 이동 한다.
            //Move individually, not by interpolation 
            int iRtn;
            int iRtn2;
            //string strMsg;
            if (MSystem.SIMULATION)
                return 0;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis1].StartMove(dPos1);
            if (iRtn != 0) return iRtn;
            iRtn2 = m_pAxisManager.m_pFastechAxis[iAxis2].StartMove(dPos2);
            if (iRtn2 != 0) return iRtn2;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis1].WaitDone();
            if (iRtn != 0) return iRtn;
            iRtn2 = m_pAxisManager.m_pFastechAxis[iAxis2].WaitDone();

            if (iRtn2 != 0) return iRtn2;

            return 0;
        }

        public static int SetMoveLinear2(int iAxis1, double dPos1, int iAxis2, double dPos2)
        {
            int iRtn;
            int iRtn2;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis1].StartLinerMove(iAxis1, dPos1, iAxis2, dPos1);

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis1].WaitDone();
            iRtn2 = m_pAxisManager.m_pFastechAxis[iAxis2].WaitDone();
            if (iRtn != 0) m_pAxisManager.m_pFastechAxis[iAxis1].ResetOrigin();
            if (iRtn2 != 0) m_pAxisManager.m_pFastechAxis[iAxis2].ResetOrigin();
            if (iRtn != 0) return iRtn;
            if (iRtn2 != 0) return iRtn2;

            return 0;
        }
        public static int SetMoveSlowLinear(int iAxis1, double dPos1, double dSpeed1, int iAxis2, double dPos2, double dSpeed2)
        {
            int iRtn;
            int iRtn2;
            //string strMsg;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis1].StartMove(dPos1, dSpeed1);
            iRtn2 = m_pAxisManager.m_pFastechAxis[iAxis2].StartMove(dPos2, dSpeed2);
            if (iRtn != 0) return iRtn;
            if (iRtn2 != 0) return iRtn2;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis1].WaitDone();
            iRtn2 = m_pAxisManager.m_pFastechAxis[iAxis2].WaitDone();
            if (iRtn != 0) m_pAxisManager.m_pFastechAxis[iAxis1].ResetOrigin();
            if (iRtn2 != 0) m_pAxisManager.m_pFastechAxis[iAxis2].ResetOrigin();
            if (iRtn != 0) return iRtn;
            if (iRtn2 != 0) return iRtn2;
            return 0;
        }

        public static int SetMove(int iAxis1, double dPos1, int iAxis2, double dPos2, int iAxis3, double dPos3)
        {
            int iRtn;
            int iRtn2;
            int iRtn3;
            //string strMsg;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis1].StartMove(dPos1);
            iRtn2 = m_pAxisManager.m_pFastechAxis[iAxis2].StartMove(dPos2);
            iRtn3 = m_pAxisManager.m_pFastechAxis[iAxis3].StartMove(dPos3);
            if (iRtn != 0) return iRtn;
            if (iRtn2 != 0) return iRtn2;
            if (iRtn3 != 0) return iRtn3;

            iRtn = m_pAxisManager.m_pFastechAxis[iAxis1].WaitDone();
            iRtn2 = m_pAxisManager.m_pFastechAxis[iAxis2].WaitDone();
            iRtn3 = m_pAxisManager.m_pFastechAxis[iAxis3].WaitDone();
            //  if (iRtn != 0) m_pAxisManager.m_pFastechAxis[iAxis1].ResetOrigin();
            // if (iRtn2 != 0) m_pAxisManager.m_pFastechAxis[iAxis2].ResetOrigin();
            // if (iRtn3 != 0) m_pAxisManager.m_pFastechAxis[iAxis3].ResetOrigin();
            if (iRtn != 0) return iRtn;
            if (iRtn2 != 0) return iRtn2;
            if (iRtn3 != 0) return iRtn3;

            return 0;
        }
        public static bool waitServoInitOk()
        {
            int delayTime = 0;
            bool KillServo = true;
            bool _Result = true;
            Task.Run(async () =>
            {
                await Task.Delay(10);
                while (delayTime <= 50)
                {
                    SetMsgDisplay($"Start Init Servor for Origin {delayTime * 100}");
                    delayTime++;
                    KillServo = true;
                    for (int i = 0; i < (int)Axis.eAXIS_MAX; i++)
                    {
                        if (m_pAxisManager.m_pFastechAxis[0] == null)
                        {
                            KillServo = false;
                            _Result = false;
                        }
                    }
                    if (KillServo)
                    {
                        KillMsgDisplay();
                        break;
                    }
                    Thread.Sleep(100);
                }
                KillMsgDisplay();
            });
            MyMsgDisplay("Start Init Servor for Origin ......");
            KillMsgDisplay();
            return _Result;
        }
        public static double GetCurrentPos(int iAxis)
        {
            if (m_pAxisManager.m_pFastechAxis[iAxis] == null) return -1;
            double dCurPos = m_pAxisManager.m_pFastechAxis[iAxis].GetCurrentPos();
            return dCurPos;
        }
        public static void MoveStop(Axis _axis)
        {
            try
            {
                if (m_pAxisManager.m_pFastechAxis[(int)_axis] == null) return;
                m_pAxisManager.m_pFastechAxis[(int)_axis].Stop();
            }
            catch (Exception ex)
            {
                string temp = ex.ToString();
            }
        }
        public static void SetAllStop()
        {
            try
            {
                for (int i = 0; i < eAXIS_MAX; i++)
                {
                    if (m_pAxisManager.m_pFastechAxis[i] == null) continue;
                    m_pAxisManager.m_pFastechAxis[i].Stop();
                }
            }
            catch (Exception ex)
            {
                string temp = ex.ToString();
            }
        }
        public static void SetAllVStop()
        {
            try
            {
                for (int i = 0; i < eAXIS_MAX; i++)
                {
                    if (m_pAxisManager.m_pFastechAxis[i] == null) continue;
                    m_pAxisManager.m_pFastechAxis[i].VStop();
                }
            }
            catch (Exception ex)
            {
                string temp = ex.ToString();
            }
        }
        public static bool IsServoError(Axis _axis)
        {
            return MSystem.m_pAxisManager.m_pFastechAxis[(int)_axis].GetAmpFault();
        }
        public static bool IsServoMoveDone(Axis _axis)
        {
            return m_pAxisManager.m_pFastechAxis[(int)_axis].IsAxisDone();
        }
        #endregion

        #endregion
        #region //CHECK SAFETY

        public static bool IsDetectEmergency()
        {
            if (SIMULATION) return false;
            return m_pDIO.IsOff(IO.IN_EMERGENCY);
        }

        #endregion

        #region //INIT UNIT
        public static int[] prior_Init = {
                                0,
		                        0
	    };
        public static int[] _Init_Result = {
                                -1,
		                        -1
        };
        public static bool IsOkeyTeaching()
        {
            int _MaxAxis = (int)Axis.eAXIS_MAX;
            for (int i = 0; i < _MaxAxis; i++)
            {
                if (m_pAxisManager.m_pFastechAxis[i] == null || !m_pAxisManager.m_pFastechAxis[i].IsOrigin())
                    return false;
            }
            return true;
        }
        public static void ResetInitAll()
        {
            for (int i = 0; i < _Init_Result.Length; i++)
            {
                _Init_Result[i] = -1;
            }
        }
        public static void ResetInit(int _Unit)
        {
            _Init_Result[_Unit] = -1;
        }
        
        #endregion

        #region //Keyboard get num
        public static bool GetIn(object sender, ref int _data)
        {
            KeyboardNum getValue = new KeyboardNum();
            getValue.TxCurrent.Text = (sender as SUserControls.ColorButton).Text;
            DialogResult Result = getValue.ShowDialog();
            if (Result == DialogResult.OK)
            {
                int TEMP = 0;
                if (int.TryParse(getValue.TxValue.Text, out TEMP))
                {
                    (sender as SUserControls.ColorButton).Text = getValue.TxValue.Text;
                    _data = TEMP;
                    return true;
                }
            }
            return false;
        }
        public static bool GetFloat(object sender, ref float _data)
        {
            KeyboardNum getValue = new KeyboardNum();
            getValue.TxCurrent.Text = (sender as SUserControls.ColorButton).Text;
            DialogResult Result = getValue.ShowDialog();
            if (Result == DialogResult.OK)
            {
                float TEMP = 0;
                if (float.TryParse(getValue.TxValue.Text, out TEMP))
                {
                    (sender as SUserControls.ColorButton).Text = getValue.TxValue.Text;
                    _data = TEMP;
                    return true;
                }
            }
            return false;
        }
        public static bool Getdouble(object sender, ref double _data)
        {
            KeyboardNum getValue = new KeyboardNum();
            getValue.TxCurrent.Text = (sender as SUserControls.ColorButton).Text;
            DialogResult Result = getValue.ShowDialog();
            if (Result == DialogResult.OK)
            {
                double TEMP = 0;
                if (double.TryParse(getValue.TxValue.Text, out TEMP))
                {
                    (sender as SUserControls.ColorButton).Text = TEMP.ToString("f3");
                    _data = TEMP;
                    return true;
                }
            }
            return false;
        }

        public static bool Getdouble_Exceed(object sender, ref double _data)
        {
            KeyboardNum getValue = new KeyboardNum();
            getValue.TxCurrent.Text = (sender as SUserControls.ColorButton).Text;
            DialogResult Result = getValue.ShowDialog();
            if (Result == DialogResult.OK)
            {
                double TEMP = 0;
                if (double.TryParse(getValue.TxValue.Text, out TEMP))
                {
                    (sender as SUserControls.ColorButton).Text = TEMP.ToString("f0");
                    _data = TEMP;
                    return true;
                }
            }
            return false;
        }

        public static bool GetCharKeyboard(object sender)
        {
            KeyboardGetValue dlgvalue = new KeyboardGetValue(0);
            dlgvalue.TxValue.Text = (sender as SUserControls.ColorButton).Text;
            DialogResult result = dlgvalue.ShowDialog();
            if (result == DialogResult.OK)
            {
                (sender as SUserControls.ColorButton).Text = dlgvalue.TxValue.Text;
                return true;
            }
            return false;
        }
        public static string GetPass()
        {
            KeyboardGetValue dlgvalue = new KeyboardGetValue(1);
            DialogResult result = dlgvalue.ShowDialog();
            if (result == DialogResult.OK)
            {
                string _tmp = dlgvalue.Keyword.Trim();
                dlgvalue = null;
                return _tmp;
            }
            dlgvalue = null;
            return null;
        }
        #endregion

        #region IO Update Sensor
        static Image IO_ON = Properties.Resources.On;
        static Image IO_OFF = Properties.Resources.Off;
        static Image IO_DIS = Properties.Resources.Dis3;

        static public Image Watch = Properties.Resources.watch_1;
        static public Image W_None = null;

        static public Image IF_Out_Off = Properties.Resources.Left_Arrow_Dis;
        static public Image IF_Out_On = Properties.Resources.Left_Arrow_En;
        static public Image IF_In_Off = Properties.Resources.Right_Arrow_Dis;
        static public Image IF_In_On = Properties.Resources.Right_Arrow_En;


        static public Image Unit_None = null;

        //static public Image Unit_InputCV = Properties.Resources.Input;

        public static void IsSensorOn(AxBTNENHLib4.AxBtnEnh _bt, bool _SS)
        {
            if (_SS)
            {
                if (_bt.Value != 1)
                {
                    _bt.Value = 1;
                }
            }
            else
            {
                if (_bt.Value != 0)
                {
                    _bt.Value = 0;
                }
            }
        }

        public static void IsSensorOn(MyButton.ButtonPress _bt, bool _SS)
        {
            if (_SS)
            {
                if (_bt.Image != IO_ON || _bt.Press != true)
                {
                    _bt.Press = true;
                    _bt.Image = IO_ON;
                }
            }
            else
            {
                if (_bt.Image != IO_DIS || _bt.Press == true)
                {
                    _bt.Press = false;
                    _bt.Image = IO_DIS;
                }
            }
        }
        public static void IsSensorOn(MyButton.ButtonPress _bt, bool _SS, int _Image)
        {
            if (_SS)
            {
                if (_bt.Press != true)
                {
                    _bt.SetPress = true;
                }
            }
            else
            {
                if (_bt.Press == true)
                {
                    _bt.SetPress = false;
                }
            }
        }
        public static void IsSensorOn(MyButton.ButtonPress _bt, bool _SS1, bool _SS2)
        {
            if (_SS1)
            {
                if (_bt.Image != IO_ON || _bt.Press != true)
                {
                    _bt.Press = true;
                    _bt.Image = IO_ON;
                }
            }
            else if (_SS2)
            {
                if (_bt.Image != IO_ON || _bt.Press == true)
                {
                    _bt.Press = false;
                    _bt.Image = IO_ON;
                }
            }
            else
            {
                if (_bt.Image != IO_DIS || _bt.Press == true)
                {
                    _bt.Image = IO_DIS;
                    _bt.Press = false;
                }
            }
        }
        public static void ProductDisPlay(PictureBox _Image, Image _ImgDisplay)
        {
            if (_Image.Image != _ImgDisplay)
            {
                _Image.Image = _ImgDisplay;
            }
        }
        public static void ProductDisPlay(PictureBox _Image, bool _SS1, bool _SS2)
        {
            if (_SS1)
                _Image.Image = Watch;
            else if (_SS2)
                _Image.Image = W_None;
            else
                _Image = null;
        }
        public static void DisPlayString(SUserControls.ColorButton _Tb, string _StringPrint)
        {
            if (_Tb.Text != _StringPrint)
            {
                _Tb.Text = _StringPrint;
            }
        }
        public static void DisplayString(AxBTNENHLib4.AxBtnEnh _Tb, string _StringPrint)
        {
            if (_Tb.Caption != _StringPrint)
            {
                _Tb.Caption = _StringPrint;
            }
        }
        public static void DisPlayString(MyButton.ButtonPress _Tb, string _StringPrint)
        {
            if (_Tb.Text != _StringPrint)
            {
                _Tb.Text = _StringPrint;
            }
        }
        public static void DisPlayString(System.Windows.Forms.Label _Tb, string _StringPrint)
        {
            if (_Tb.Text != _StringPrint)
            {
                _Tb.Text = _StringPrint;
            }
        }
        #endregion

        #region GET MODE RUN
        public static bool IsDry()
        {
            return (SysMode == ModeRun.DryRun);
        }
        public static bool IsBypass()
        {
            return (SysMode == ModeRun.ByPass);
        }
        public static bool IsAuto()
        {
            return (SysMode == ModeRun.Auto);
        }
        public static bool IsBypassTest()
        {
            return (SysMode == ModeRun.ByPassTest);
        }
        public static bool IsRun()
        {
            return (SysStatus == StatusRun.RUN) ? true : false;
        }
        public static bool IsStop()
        {
            return (SysStatus == StatusRun.STOP) ? true : false;
        }
        #endregion
    }
}
