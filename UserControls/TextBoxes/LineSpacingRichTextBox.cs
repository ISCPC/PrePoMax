using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
    public class LineSpacingRichTextBox : System.Windows.Forms.RichTextBox
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


        // Properties                                                                                                               
        public new string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                SetLineFormat(5, 23);
            }
        }
        public bool Beep { get { return _beep; } set { _beep = value; } }


        // Constructors                                                                                                             
        public LineSpacingRichTextBox()
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
        /// 3 - The dyLineSpacing member specifies the spacing from one line to the next, in twips.However, if dyLineSpacing specifies a value that is less than single spacing, the control displays single - spaced text.
        /// 4 - The dyLineSpacing member specifies the spacing from one line to the next, in twips.The control uses the exact spacing specified, even if dyLineSpacing specifies a value that is less than single spacing.
        /// 5 - The value of dyLineSpacing / 20 is the spacing, in lines, from one line to the next. Thus, setting dyLineSpacing to 20 produces single-spaced text, 40 is double spaced, 60 is triple spaced, and so on.
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
            if (!_beep && GetLineFromCharIndex(SelectionStart) == 0 && e.KeyData == Keys.Up ||
                GetLineFromCharIndex(SelectionStart) == GetLineFromCharIndex(TextLength) && e.KeyData == Keys.Down ||
                SelectionStart == TextLength && e.KeyData == Keys.Right ||
                SelectionStart == 0 && e.KeyData == Keys.Left)
            {
                e.Handled = true;
            }
            //
            base.OnKeyDown(e);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            System.Drawing.Size size = GetPreferredSize(System.Drawing.Size.Empty);
            Height = (int)(size.Height * 1.15);
            Width = (int)(size.Width * 1.0);
            //System.Drawing.Font tempFont = Font;
            //int textLength = Text.Length;
            //int textLines = GetLineFromCharIndex(textLength) + 1;
            //int Margin = Bounds.Height - ClientSize.Height;
            ////
            //Height = (TextRenderer.MeasureText(" ", tempFont).Height * textLines) + Margin + 2;
            //
            base.OnTextChanged(e);
        }
        protected override void OnVScroll(EventArgs e)
        {
            PostMessage(Handle, WM_VSCROLL, (IntPtr)SB_TOP, IntPtr.Zero);
            //
            //base.OnVScroll(e);
        }
    }
}
