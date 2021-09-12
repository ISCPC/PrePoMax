using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UserControls
{
    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            using (_timeoutTimer)
            {
                MessageBox.Show(text, caption, buttons, icon);
            }
        }
        public static void ShowWarning(string text, int timeout)
        {
            new AutoClosingMessageBox(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, timeout);
        }
        public static void ShowError(string text, int timeout)
        {
            new AutoClosingMessageBox(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, timeout);
        }
        public static void ShowInfo(string caption, string text, int timeout)
        {
            new AutoClosingMessageBox(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
