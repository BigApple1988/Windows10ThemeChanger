using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Windows10ThemeChanger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var currentProcess = Process.GetCurrentProcess();
            var name = currentProcess.ProcessName;
            Process[] processes = Process.GetProcessesByName(name);
            if (processes.Length > 1)
            {
                MessageBox.Show("Process already running!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            Application.Run(new TrayIcon());
        }
    }
}
