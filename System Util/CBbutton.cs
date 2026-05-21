using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLCCInspectionMC
{
    public class CBbutton
    {
        public static Color NormalColor = Color.FromArgb(153, 180, 209); //normal - blue
        public static Color SelectColor = Color.Lime;
        public static Color StatusOK = Color.FromArgb(0, 192, 0);//origin OK - green
        public static Color ErrorColor = Color.Red;

        public bool m_iSel = false;

        public SUserControls.ColorButton Button;
        public CBbutton(SUserControls.ColorButton _bt, bool _isel) {
            Button = _bt;
            m_iSel = _isel;
        }
        
        public void SetValue(Color color) {
            Button.GradientBottom = color;
            Button.GradientTop = color;
            if (color == SelectColor)
                m_iSel = true;
            else
                m_iSel = false;
        }
        public void SetValeUnSelect()
        {
            Button.GradientBottom = System.Drawing.SystemColors.GradientActiveCaption;
            Button.GradientTop = System.Drawing.SystemColors.GradientActiveCaption; ;
            m_iSel = false;
        }
        public void SetValeUnSelect_2()
        {
            Button.GradientBottom = System.Drawing.Color.White;
            Button.GradientTop = System.Drawing.Color.White;
            Button.BorderLineColor = System.Drawing.Color.Black;
            m_iSel = false;
        }
        public void SetValeUnSelect_3()
        {
            Button.GradientBottom = System.Drawing.Color.FromArgb(230,230,230);
            Button.GradientTop = System.Drawing.Color.FromArgb(230, 230, 230);
            Button.BorderLineColor = System.Drawing.Color.Black;
            m_iSel = false;
        }
        public Color GetColor
        {
            
            get {
                return Button.GradientBottom;
            }
        }
    }

    public class CBbutton2
    {
        public static Color NormalColor = Color.FromArgb(153, 180, 209); //normal - blue
        public static Color SelectColor = Color.Yellow;
        public static Color StatusOK = Color.FromArgb(0, 255, 0);//origin OK - green
        public static Color ErrorColor = Color.Red;

        public bool m_iSel = false;
        public AxBTNENHLib4.AxBtnEnh Button;
        public CBbutton2(AxBTNENHLib4.AxBtnEnh _bt, bool _isel)
        {
            Button = _bt;
            m_iSel = _isel;
        }
        public void SetValue(Color color)
        {
            try
            {
                Button.BackColorInterior = color;
                Button.BackColorPressed = color;
                Button.Value = 1;
                if (color == SelectColor) m_iSel = true;
                else m_iSel = false;
            }
            catch (Exception)
            {

            }

        }
        
        public void SetValeUnSelect()
        {
            Button.BackColorInterior = Color.Gainsboro;
            Button.BackColorPressed = Color.WhiteSmoke;
            m_iSel = false;
            Button.Value = 0;
        }
        public Color GetColor
        {
            get
            {
                return Button.BackColorContainer;
            }
        }
    }
}
