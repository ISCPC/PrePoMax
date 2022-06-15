using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;


// http://stackoverflow.com/questions/479284/mouse-wheel-event-c


namespace UserControls
{
    public class MainMouseWheelManagedForm : Form, IMessageFilter
    {
        /************************************/
        /* IMessageFilter implementation    */
        /* **********************************/

        //WM_MOUSEFIRST = 0x200
        //private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        //WM_LBUTTONDBLCLK = 0x203
        //WM_RBUTTONDOWN = 0x204
        //WM_RBUTTONUP = 0x205
        //WM_RBUTTONDBLCLK = 0x206
        //WM_MBUTTONDOWN = 0x207
        //WM_MBUTTONUP = 0x208
        //WM_MBUTTONDBLCLK = 0x209
        //WM_MOUSEWHEEL = 0x20A
        //WM_MOUSEHWHEEL = 0x20E
        //private const int WM_MOUSEWHEEL = 0x20a;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;

        // P/Invoke declarations
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);


        // Variables                                                                                                                
        private bool managed;
        public Control PassThroughControl;
        public bool DisableAllMouseEvents;


        // Constructors                                                                                                             
        public MainMouseWheelManagedForm()
            : this(true)
        {
        }
        public MainMouseWheelManagedForm(bool start)
        {
            DisableAllMouseEvents = false;
            managed = false;

            // Add this form to message filter - in order for PreFilterMessage to work
            if (start) ManagedMouseWheelStart();
        }
        //
        protected override void Dispose(bool disposing)
        {
            // Remove this form to message filter - in order for PreFilterMessage to work
            if (disposing) ManagedMouseWheelStop();

            base.Dispose(disposing);
        }


        // Mehods                                                                                                                   

        /******************************************/
        /* MouseWheelManagedForm specific methods */
        /* ****************************************/
        public void ManagedMouseWheelStart()
        {
            if (!managed)
            {
                managed = true;
                Application.AddMessageFilter(this);
            }
        }
        public void ManagedMouseWheelStop()
        {
            if (managed)
            {
                managed = false;
                Application.RemoveMessageFilter(this);
            }
        }
        private bool IsChildControlOnTheSameForm(Control ctrl)
        {
            Control loopCtrl = ctrl;
            //
            while (loopCtrl != null && loopCtrl != this && !(loopCtrl is Form))
            {
                if (loopCtrl.Parent == null && loopCtrl is Form) loopCtrl = ((Form)loopCtrl).Owner;
                else loopCtrl = loopCtrl.Parent;
            }
            //
            return (loopCtrl == this);
        }
        public bool PreFilterMessage(ref Message m) // Return true if the message was handeled
        {
            IntPtr hWnd;
            //
            if (DisableAllMouseEvents && m.Msg >= 512 && m.Msg <= 527) return true;  // Disable all mouse events
            //
            if (m.Msg == WM_LBUTTONDOWN)
            {
                if (IsChildControlOnTheSameForm(FromHandle(m.HWnd)))
                {
                    LeftMouseDownOnForm(FromHandle(m.HWnd));
                }
            }
            // Prevent arrow keys to change focus from PassThroughControl
            if (m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP)
            {
                //System.Diagnostics.Debug.WriteLine(DateTime.Now + " Focus: " + PassThroughControl.ContainsFocus);
                if (PassThroughControl != null && PassThroughControl.ContainsFocus)
                {
                    if (m.WParam.ToInt32() >= 37 && m.WParam.ToInt32() <= 40) // arrow keys
                    {
                        //System.Diagnostics.Debug.WriteLine(DateTime.Now + " m.Msg = " + m.Msg);
                        //System.Diagnostics.Debug.WriteLine(DateTime.Now + " m.LParam = " + m.LParam);
                        //System.Diagnostics.Debug.WriteLine(DateTime.Now + " m.WParam = " + m.WParam);

                        hWnd = PassThroughControl.Handle;
                        SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                }
            }

            //if (m.Msg == WM_MOUSEWHEEL)
            //{
            //    // Ensure the message was sent to a child of the current form
            //    if (IsChildControlOnTheSameForm(Control.FromHandle(m.HWnd)))
            //    {
            //        // Find the position at m.LParam
            //        Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);

            //        if (PassThroughControl != null)
            //        {
            //            Rectangle rect = new Rectangle(PassThroughControl.PointToScreen(PassThroughControl.Location), PassThroughControl.Size);
            //            if (rect.Contains(pos))
            //            {
            //                hWnd = PassThroughControl.Handle;
            //                SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
            //                return true;
            //            }
            //        }

            //        hWnd = WindowFromPoint(pos);
            //        if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
            //        {
            //            SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
            //            return true;
            //        }
            //    }
            //}

            //if (m.Msg == WM_MOUSEMOVE || m.Msg == 275 || m.Msg == 1848 || m.Msg == 15 || m.Msg == 96 || m.Msg == 160 || m.Msg == 674 || m.Msg == 675) return false;
            //System.Diagnostics.Debug.WriteLine(DateTime.Now + " m.Msg: " + m.Msg);
            //if (m.Msg == 513)
            //{
            //    if (PassThroughControl != null && PassThroughControl.ContainsFocus)
            //    {
            //        hWnd = this.Handle;
            //        SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
            //        return true;
            //    }
            //}

            return false;
        }
        public virtual void LeftMouseDownOnForm(Control sender)
        {
        }

    }
}