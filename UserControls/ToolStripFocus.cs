using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControls
{
    public partial class ToolStripFocus : ToolStrip
    {
        // Variables                                                                                                                
        private const int WM_MOUSEMOVE = 0x0200;
        private bool _disableMouse;


        // Properties                                                                                                               
        public bool DisableMouseButtons { get { return _disableMouse; } set { _disableMouse = value; } }


        // Constructors                                                                                                             
        public ToolStripFocus()
        {
            InitializeComponent();
            _disableMouse = false;
        }


        // Methods                                                                                                                  
        protected override void WndProc(ref Message m)
        {
            //
            // 0200        512     WM_MOUSEFIRST
            // 0200        512     WM_MOUSEMOVE
            // 0201        513     WM_LBUTTONDOWN
            // 0202        514     WM_LBUTTONUP
            // 0203        515     WM_LBUTTONDBLCLK
            // 0204        516     WM_RBUTTONDOWN
            // 0205        517     WM_RBUTTONUP
            // 0206        518     WM_RBUTTONDBLCLK
            // 0207        519     WM_MBUTTONDOWN
            // 0208        520     WM_MBUTTONUP
            // 0209        521     WM_MBUTTONDBLCLK
            // 0209        521     WM_MOUSELAST
            // 020a        522     WM_MOUSEWHEEL
            //
            
            // Eat left and right mouse clicks
            if (_disableMouse && (m.Msg >= 512 && m.Msg <= 522))
            {
                // eat message
            }
            else
            {
                // Eat button highlighting if the form is not in focus
                if (m.Msg == WM_MOUSEMOVE)
                {
                    if (!this.TopLevelControl.ContainsFocus)
                    {
                        // eat message
                    }
                    else
                    {
                        // handle messages normally
                        base.WndProc(ref m);
                    }
                }
                else
                {
                    // handle messages normally
                    base.WndProc(ref m);
                }
            }
        }
    }
}
