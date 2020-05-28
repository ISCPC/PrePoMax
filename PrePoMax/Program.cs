using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo ci =
                (System.Globalization.CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();
            ci.NumberFormat.NumberGroupSeparator = "";
            //
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;           // This thread
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = ci;   // All feature threads
            //
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = ci;
            // Set MessageBoxButtons to English defaults
            MessageBoxManager.OK = "OK";
            MessageBoxManager.Cancel = "Cancel";
            MessageBoxManager.Abort = "Abort";
            MessageBoxManager.Retry = "Retry";
            MessageBoxManager.Ignore = "Ignore";
            MessageBoxManager.Yes = "Yes";
            MessageBoxManager.No = "No";
            MessageBoxManager.Register();
            //
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain(args));
        }
    }
}

