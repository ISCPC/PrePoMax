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
    public partial class NumericTextBox : TextBox
    {
        // Variables                                                                                                                
        private const int WM_PASTE = 0x0302;


        // Constructors                                                                                                             
        public NumericTextBox()
        {
            InitializeComponent();
        }


        // Methods                                                                                                                  
        protected override void WndProc(ref Message m)
        {
            if (m.Msg != WM_PASTE)
            {
                // Handle all other messages normally
                base.WndProc(ref m);
            }
            else
            {
                // Some simplified example code that complete replaces the
                // text box content only if the clipboard contains a valid double.
                // I'll leave improvement of this behavior as an exercise :)
                double value;
                if (double.TryParse(Clipboard.GetText(), out value))
                {
                    Text = value.ToString();
                }
            }
        }
        private void NumTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        public double Value
        {
            get
            {
                double value = 0;
                if (double.TryParse(this.Text, out value)) return value;
                else return 0;
            }
        }

    }
}
