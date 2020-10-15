using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mail_Auto
{
    static class Program
    {
        /// <
        /// mary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
