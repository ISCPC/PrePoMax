using PrePoMax.Properties;
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
            //
            ShowHelp = false;
            labProgramName.Text = Globals.ProgramName;
            //
            Random rand = new Random(DateTime.Now.Millisecond);
            int splashId = rand.Next(2) + 1;
            //
            if (splashId == 1) BackgroundImage = Resources.Splash_01;
            else if (splashId == 2) BackgroundImage = Resources.Splash_02;
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
