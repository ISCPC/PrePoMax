using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace UserControls
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PARAFORMAT
    {
        public int cbSize;
        public uint dwMask;
        public short wNumbering;
        public short wReserved;
        public int dxStartIndent;
        public int dxRightIndent;
        public int dxOffset;
        public short wAlignment;
        public short cTabCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] rgxTabs;
        // PARAFORMAT2 from here onwards
        public int dySpaceBefore;
        public int dySpaceAfter;
        public int dyLineSpacing;
        public short sStyle;
        public byte bLineSpacingRule;
        public byte bOutlineLevel;
        public short wShadingWeight;
        public short wShadingStyle;
        public short wNumberingStart;
        public short wNumberingStyle;
        public short wNumberingTab;
        public short wBorderSpace;
        public short wBorderWidth;
        public short wBorders;
    }
    public class AnnotationEditorRichTextBox : RichTextBox
    {
        // DllImports                                                                                                                
        const int PFM_SPACEBEFORE = 0x00000040;
        const int PFM_SPACEAFTER = 0x00000080;
        const int PFM_LINESPACING = 0x00000100;
        const int SCF_SELECTION = 1;
        const int EM_SETPARAFORMAT = 1095;
        //
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref PARAFORMAT lParam);
        // Scroll
        private const UInt32 SB_TOP = 0x6;
        private const UInt32 WM_VSCROLL = 0x115;
        //
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);


        // Variables                                                                                                                
        private bool _beep;
        private Size _minSize;
        private Size _maxSize;


        // Properties                                                                                                               
        public bool Beep { get { return _beep; } set { _beep = value; } }
        public Size MinSize
        {
            get { return _minSize; }
            set
            {
                _minSize = value;
                if (Width < _minSize.Width) Size = new Size(_minSize.Width, Height);
                if (Height < _minSize.Height) Size = new Size(Width, _minSize.Height);
            }
        }
        public Size MaxSize
        {
            get { return _maxSize; }
            set
            {
                _maxSize = value;
                if (Width > _maxSize.Width) Size = new Size(_maxSize.Width, Height);
                if (Height > _maxSize.Height) Size = new Size(Width, _maxSize.Height);
            }
        }
        public new string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                SetLineFormat(5, 23);
            }
        }


        // Constructors                                                                                                             
        public AnnotationEditorRichTextBox()
        {
            _beep = false;
        }


        // Methods                                                                                                                  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule">
        /// 0 - Single spacing.The dyLineSpacing member is ignored.
        /// 1 - One - and - a - half spacing.The dyLineSpacing member is ignored.
        /// 2 - Double spacing.The dyLineSpacing member is ignored.
        /// 3 - The dyLineSpacing member specifies the spacing from one line to the next, in twips.However, if dyLineSpacing
        /// specifies a value that is less than single spacing, the control displays single - spaced text.
        /// 4 - The dyLineSpacing member specifies the spacing from one line to the next, in twips.The control uses the exact
        /// spacing specified, even if dyLineSpacing specifies a value that is less than single spacing.
        /// 5 - The value of dyLineSpacing / 20 is the spacing, in lines, from one line to the next. Thus, setting
        /// dyLineSpacing to 20 produces single-spaced text, 40 is double spaced, 60 is triple spaced, and so on.
        /// </param>
        /// <param name="space">Line spacing</param>
        private void SetLineFormat(byte rule, int space)
        {
            PARAFORMAT fmt = new PARAFORMAT();
            fmt.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(fmt);
            fmt.dwMask = PFM_LINESPACING;
            fmt.dyLineSpacing = space;
            fmt.bLineSpacingRule = rule;
            this.SelectAll();
            SendMessage(new HandleRef(this, this.Handle), EM_SETPARAFORMAT, SCF_SELECTION, ref fmt);
            //
            this.SelectionLength = 0;
            this.SelectionStart = 0;
        }
        //
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                Paste(DataFormats.GetFormat("Text"));
                e.Handled = true;
            }
            //
            if (!_beep)
            {
                if (GetLineFromCharIndex(SelectionStart) == 0 && (e.KeyData == Keys.Up || e.KeyData == Keys.PageUp))
                    e.Handled = true;
                else if (GetLineFromCharIndex(SelectionStart) == GetLineFromCharIndex(TextLength) &&
                    (e.KeyData == Keys.Down || e.KeyData == Keys.PageDown))
                    e.Handled = true;
                else if (SelectionStart == TextLength && (e.KeyData == Keys.Right || e.KeyData == Keys.End))
                    e.Handled = true;
                else if (SelectionStart == 0 && (e.KeyData == Keys.Left || e.KeyData == Keys.Home))
                    e.Handled = true;
            }
            //
            base.OnKeyDown(e);
        }
        protected override void OnVScroll(EventArgs e)
        {
            // Prevents autoscroll
            PostMessage(Handle, WM_VSCROLL, (IntPtr)SB_TOP, IntPtr.Zero);
            //
            base.OnVScroll(e);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            Size size = GetPreferredSize(Size.Empty);
            Size newSize = new Size((int)(size.Width * 1.0), (int)(size.Height * 1.15));
            //
            if (newSize.Width < _minSize.Width) newSize.Width = _minSize.Width;
            if (newSize.Height < _minSize.Height) newSize.Height = _minSize.Height;
            if (newSize.Width > _maxSize.Width) newSize.Width = _maxSize.Width;
            if (newSize.Height > _maxSize.Height) newSize.Height = _maxSize.Height;
            //
            Size = newSize;
            //
            base.OnTextChanged(e);
        }
        
    }
}
