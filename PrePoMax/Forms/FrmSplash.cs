using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    public partial class FrmSplash : Form
    {
        public FrmSplash()
        {
            InitializeComponent();

            ShowHelp = false;
            labProgramName.Text = Globals.ProgramName;
        }

        public bool ShowHelp
        {
            set
            {
                labHomePage.Visible = value;
                labClose.Visible = value;
                progressBar.Visible = !value;
            }
        }


        public void Close(int miliseconds)
        {
            System.Threading.Thread.Sleep(miliseconds);
            this.Close();
        }

        private void labClose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void labHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.HomePage);
        }
    }
}
