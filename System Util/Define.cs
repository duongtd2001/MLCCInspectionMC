#define HRM
namespace MLCCInspectionMC
{
    public class sAxisInfo
    {
        public string AxisName;

        public double dOriginLimitTime;
        public double dLimitMinusValue;
        public double dLimitPluseValue;
        public double dOriginPoisionSet;
        public double dOriginOffset;
        public double dOriginSearchSpeed;
        public int iGain;
        public double dOriginSpeed;
        public double dJogFastSpeed;
        public double dJogMidAccDec;
        public double dJogMidSpeed;
        public double dJogSlowAccDec;
        public double dJogSlowSpeed;
        public double dAxisDec;
        public double dAxisAcc;
        public double dAxisMaxSpeed;
        public double dJogFastAccDec;
        public long MotionDir;
        public long OrgDir;
    }
    public enum Output_Mode
    {
        eNextMC,
        eNGCV,
        eMAX
    };

    public enum Loader_Type
    {
        eGrip = 0,
        eVacuum = 1,
        eMAX = 2
    };
    public enum MCPRESS_STEP
    {
        ePRESS_MC_STEP_ANYTHING,
        ePRESS_MC_STEP_SET_RUN,
        ePRESS_MC_STEP_SET_RUNING,
        ePRESS_MC_STEP_GET_READY,
        ePRESS_MC_STEP_SET_PRESSURE,
        ePRESS_MC_STEP_SET_TIME,
        ePRESS_MC_STEP_GET_TIME_AND_PRESSURE,
        ePRESS_MC_STEP_SET_CALIBRATION,
        ePRESS_MC_STEP_MAX
    }
    public enum Axis
    {
        AXIS_X = 0,
        AXIS_Y = 1,
        eAXIS_MAX
    }
    public enum IO
    {
        //EZI-IO-EN-I16O16N-MODUL_1
        IN_EMERGENCY = 1000, //X000
        IN_START_SW = 1001, //X001
        IN_SPARE_2 = 1002, //X002
        IN_SPARE_3 = 1003, //X003
        IN_SPARE_4 = 1004, //X004
        IN_LIGHT_CURTAIN_SENS = 1005, //X005
        IN_SPARE_6 = 1006, //X006
        IN_SPARE_7 = 1007, //X007
        IN_SPARE_8 = 1008, //X008
        IN_SPARE_9 = 1009, //X009
        IN_SPARE_A = 1010, //X00A
        IN_SPARE_B = 1011, //X00B
        IN_SPARE_C = 1012, //X00C
        IN_SPARE_D = 1013, //X00D
        IN_SPARE_E = 1014, //X00E
        IN_SPARE_F = 1015, //X00F
        IN_MAX = 1016,

        //EZI-IO-EN-I16O16N-MODUL_1
        OUT_SPARE_0 = 2000, //Y000
        OUT_START_SW_LAMP = 2001, //Y001
        OUT_SPARE_2 = 2002, //Y002
        OUT_SPARE_3 = 2003, //Y003
        OUT_TOWER_LAMP_GREEN = 2004, //Y004
        OUT_TOWER_LAMP_RED = 2005, //Y005
        OUT_TOWER_LAMP_YELLOW = 2006, //Y006
        OUT_SPARE_7 = 2007, //Y007
        OUT_SPARE_8 = 2008, //Y008
        OUT_SPARE_9 = 2009, //Y009
        OUT_SPARE_A = 2010, //Y00A
        OUT_SPARE_B = 2011, //Y00B
        OUT_SPARE_C = 2012, //Y00C
        OUT_SPARE_D = 2013, //Y00D
        OUT_SPARE_E = 2014, //Y00E
        OUT_SPARE_F = 2015, //Y00F

        OUT_MAX = 2016
    }
    public enum Unit
    {
        eUNIT_TRANSFER_X = 0,
        eUNIT_TRANSFER_Y = 1,
        eUNIT_MAX
    }
    public enum msgIcon
    {
        Question = 0,
        Infor = 1,
        Error = 2,
        eMessgMax = 3
    }
    public enum msgButton
    {
        YESNO = 0,
        OK = 1,
        SAFETY = 2,
        eBtMax = 3
    }
    public enum StatusRun
    {
        RUN = 0,
        STOP = 1,
        ERROR = 2,
        OPRATOR_CALL = 3
    }
    public enum ModeRun
    {
        Auto = 0,
        DryRun = 1,
        ByPass = 2,
        ByPassTest = 3,
        eModeMax = 4
    }
    public enum ModeTrigger
    {
        Point,
        Flying
    }

    public enum CameraRun
    {
        eCameraLeft = 0,
        eCameraBottom = 1,
        eCameraMax = 2
    }
    public enum NumViewMain
    {
        eViewAuto = 1,
        eViewTeach = 2,
        eViewData = 3,
        eViewLog = 4,
        eViewHiden = 5,
        eViewExit = 6,
        eViewMax = 7
    }
    public enum InitResult
    {
        UNIT_INIT_SUCCESS = 0,
        UINT_INIT_FALSE = 1,
        UNIT_INIT_MAX = 2
    }
}
