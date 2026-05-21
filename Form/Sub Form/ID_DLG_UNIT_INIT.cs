using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    public partial class FormUnitInit : Form
    {
        public const int SUCCESS = 0;
        CBbutton2[] m_btUnit = new CBbutton2[(int)Unit.eUNIT_MAX];
        #region// INIT BUTTON
        public FormUnitInit()
        {
            InitializeComponent();
            Point check = FormMain.GetLocation();
            this.Location = new Point(FormMain.GetLocation().X + (1280 - this.Width) / 2, FormMain.GetLocation().Y + (1024 - this.Height) / 2);
            InitButton();
            this.FormClosed += new FormClosedEventHandler(CloseForm);
            MSystem.m_bIsTeachDlg = true;
        }
        ~FormUnitInit()
        {

        }
        private void CloseForm(object sender, EventArgs e)
        {
            MSystem.m_bIsTeachDlg = false;
        }
        void InitButton()
        {
            int index = 0;
            m_btUnit[index++] = new CBbutton2(BT_UNIT_0, false);
            m_btUnit[index++] = new CBbutton2(BT_UNIT_1, false);

            for (index = 0; index < (int)Unit.eUNIT_MAX; index++)
            {
                m_btUnit[index].Button.ClickEvent += new EventHandler(PreLoadEvent);
            }
        }
        private void PreLoadEvent(object sender, EventArgs e)
        {
            for (int i = 0; i < (int)Unit.eUNIT_MAX; i++)
            {
                if ((sender as AxBTNENHLib4.AxBtnEnh) == m_btUnit[i].Button)
                {
                    if (m_btUnit[i].m_iSel)
                    {
                        m_btUnit[i].SetValue(CBbutton2.NormalColor);
                    }
                    else
                    {
                        m_btUnit[i].SetValue(CBbutton2.SelectColor);
                    }
                }
            }
        }
        #endregion
        private void BtExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void colorButton2_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < (int)Unit.eUNIT_MAX; index++)
            {
                m_btUnit[index].SetValue(CBbutton2.SelectColor);
            }
        }

        private void colorButton3_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < (int)Unit.eUNIT_MAX; index++)
            {
                m_btUnit[index].SetValue(CBbutton2.NormalColor);
            }
        }

        private void BT_UNIT_Click(object sender, EventArgs e)
        {
            int idxResult = Init();
            if (idxResult != 100)
            {
                MSystem.KillMsgDisplay();
                if (idxResult == -1) return;
                MSystem.MyMsgMemo($"Init {((Unit)idxResult).ToString()} Fail! Please Check again.", "Unit Init", msgButton.OK, msgIcon.Infor);
            }
            else
            {
                MSystem.MyMsgMemo("Init all Ok! Ready for run", "Unit Init", msgButton.OK, msgIcon.Infor);
            }
        }
        public int Init()
        {
            // INIT
            bool[] iUnitSelect = new bool[(int)Unit.eUNIT_MAX];
            int Result = 100;
            int _NumUnit = (int)Unit.eUNIT_MAX;
            bool m_iSelect = false;

            for (int i = 0; i < _NumUnit; i++)
            {
                if (m_btUnit[i].m_iSel) iUnitSelect[i] = true;
                else iUnitSelect[i] = false;
            }
            /*Check Get Select*/
            for (int i = 0; i < _NumUnit; i++)
            {
                if (m_btUnit[i].m_iSel)
                {
                    m_iSelect = true;
                }
            }
            /*Check Select*/
            if (!m_iSelect)
            {
                MSystem.MyMsgMemo("No Select Unit!", "Unit Init", msgButton.OK, msgIcon.Question);
                return -1;
            }
            /*Question for Unit*/
            if (MSystem.MyMsgMemo("Do you want initial unit Select ?", "Unit Init", msgButton.YESNO, msgIcon.Question) != DialogResult.Yes)
            {
                return -1;
            }
            var TaskRun = Task.Run(async () =>
            {
                await Task.Delay(1000);

                //Transfer Rear
                Task[] task = new Task[2]
                {
                    Task.Run(async () =>
                    {
                        if (iUnitSelect[(int)Unit.eUNIT_TRANSFER_X] && Result == 100)
                        {
                             MSystem.SetMsgDisplay($"Init Unit {Unit.eUNIT_TRANSFER_Y.ToString()} ......");
                            if (MSystem.m_pTrsTransferXY.SetUnitInitialize() != SUCCESS) Result = (int)Unit.eUNIT_TRANSFER_X;
                            MSystem.m_pTrsTransferXY.MoveTransferX(0);
                            if (Result == 100) m_btUnit[(int)Unit.eUNIT_TRANSFER_X].SetValue(CBbutton2.SelectColor);
                        }
                    }),
                    Task.Run(async () =>
                    {
                        MSystem.SetMsgDisplay($"Init Unit {Unit.eUNIT_TRANSFER_Y.ToString()} ......");
                        if (iUnitSelect[(int)Unit.eUNIT_TRANSFER_Y] && Result == 100)
                        {
                            if (MSystem.m_pTrsTransferXY.SetUnitInitialize() != SUCCESS) Result = (int)Unit.eUNIT_TRANSFER_Y;
                            MSystem.m_pTrsTransferXY.MoveTransferY(0);
                            if (Result == 100) m_btUnit[(int)Unit.eUNIT_TRANSFER_Y].SetValue(CBbutton2.SelectColor);
                        }
                    })
                };
                await Task.WhenAll(task);
                MSystem.KillMsgDisplay();

            });
            MSystem.MyMsgDisplay("Start Init Unit ........\r\nWait a moment ...");
            MSystem.KillMsgDisplay();
            return Result;
        }
    }
}
