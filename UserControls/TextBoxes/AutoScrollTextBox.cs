using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UserControls
{
    public partial class AutoScrollTextBox : TextBox
    {
        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);
        [DllImport("user32.dll")]
        private static extern bool PostMessageA(IntPtr hWnd, int nBar, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);


        // Variables                                                                                                                
        private const int SB_HORZ = 0x0;
        private const int SB_VERT = 0x1;
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int SB_THUMBPOSITION = 4;
        private const int SB_BOTTOM = 7;
        private const int SB_OFFSET = 13;


        // Properties                                                                                                               
       

        // Constructors                                                                                                             
        public AutoScrollTextBox()
        {
            InitializeComponent();
        }


        // Methods                                                                                                                  
        public void AutoScrollAppendText(string text)
        {
            if (text == null) return;
            //
            bool bottomFlag = false;
            int VSmin;
            int VSmax;
            int sbOffset;
            int savedVpos;
            // Win32 magic to keep the textbox scrolling to the newest append to the textbox unless
            // the user has moved the scrollbox up
            sbOffset = (int)((this.ClientSize.Height - SystemInformation.HorizontalScrollBarHeight) / (this.Font.Height));
            savedVpos = GetScrollPos(this.Handle, SB_VERT);
            GetScrollRange(this.Handle, SB_VERT, out VSmin, out VSmax);
            if (savedVpos >= (VSmax - sbOffset - 1))
                bottomFlag = true;
            this.AppendText(text);
            if (bottomFlag)
            {
                GetScrollRange(this.Handle, SB_VERT, out VSmin, out VSmax);
                savedVpos = VSmax - sbOffset;
                bottomFlag = false;
            }
            SetScrollPos(this.Handle, SB_VERT, savedVpos, true);
            PostMessageA(this.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * savedVpos, 0);
        }

        public void AutoScrollSetText(string text)
        {
            bool bottomFlag = false;
            int VSmin;
            int VSmax;
            int sbOffset;
            int savedVpos;
            // Win32 magic to keep the textbox scrolling to the newest append to the textbox unless
            // the user has moved the scrollbox up
            sbOffset = (int)((this.ClientSize.Height - SystemInformation.HorizontalScrollBarHeight) / (this.Font.Height));
            savedVpos = GetScrollPos(this.Handle, SB_VERT);
            GetScrollRange(this.Handle, SB_VERT, out VSmin, out VSmax);
            if (savedVpos >= (VSmax - sbOffset - 1))
                bottomFlag = true;
            this.Text = text;
            if (bottomFlag)
            {
                GetScrollRange(this.Handle, SB_VERT, out VSmin, out VSmax);
                savedVpos = VSmax - sbOffset;
                bottomFlag = false;
            }
            SetScrollPos(this.Handle, SB_VERT, savedVpos, true);
            PostMessageA(this.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * savedVpos, 0);
        }

    }
}
