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
    public partial class NumericToolStripTextBox : ToolStripTextBox
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        public double Value
        {
            get
            {
                double value = 0;
                if (double.TryParse(this.Text, out value)) return value;
                else return 0;
            }
        }


        // Constructors                                                                                                             
        public NumericToolStripTextBox()
        {
            InitializeComponent();
        }


        // Methods                                                                                                                  
        private void NumTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // Only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as ToolStripTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
