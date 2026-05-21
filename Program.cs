using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLCCInspectionMC
{
    static class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);
        [STAThread]
        static void Main()
     {
            
            Mutex mutex = new System.Threading.Mutex(false, "MLCCInspectionMC");
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    string dllDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dll", "win");
                    SetDllDirectory(dllDirectory);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FormMain());
                   
                }
                else
                {
                    MessageBox.Show("An Instance Of The Application Is Already Running.");
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }
    }
}
