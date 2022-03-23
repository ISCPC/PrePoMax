using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;
using CaeJob;

namespace PrePoMax.Forms
{
    public partial class FrmMonitor : UserControls.PrePoMaxChildForm
    {
        // Variables                                                                                                                
        private AnalysisJob _job;
        private Controller _controller;

        // Properties                                                                                                               


        // Events                                                                                                                   
        public event Action<string> KillJob;
        public event Action<string> Results;


        // Constructors                                                                                                             
        public FrmMonitor(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
        }


        // Event handlers                                                                                                           
        private void FrmMonitor_Shown(object sender, EventArgs e)
        {
            tbOutput.Select(tbOutput.TextLength, 0);
            tbOutput.ScrollToCaret();
        }
        private void FrmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_job != null)
            {
                _job.DataOutput -= UpdateOutput;
                _job = null;
            }
            //
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_job.JobStatus == JobStatus.Running)
            {
                if (tabControl1.SelectedTab == tpOutput)
                {
                    tbOutput.Select(tbOutput.TextLength, 0);
                    tbOutput.ScrollToCaret();
                }
                else if (tabControl1.SelectedTab == tpStatus)
                {
                    tbStatus.Select(tbOutput.TextLength, 0);
                    tbStatus.ScrollToCaret();
                }
                else if (tabControl1.SelectedTab == tpCovergence)
                {
                    tbConvergence.Select(tbOutput.TextLength, 0);
                    tbConvergence.ScrollToCaret();
                }
            }
        }

        private void btnKill_Click(object sender, EventArgs e)
        {
            try
            {
                if (KillJob != null) KillJob(_job.Name);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void btnResults_Click(object sender, EventArgs e)
        {
            try
            {
                // function results hides this form if everything ok
                if (Results != null) Results(_job.Name);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_job != null)
            {
                _job.DataOutput -= UpdateOutput;
                _job = null;
            }
            Hide();
        }

        // Methods                                                                                                                  
        public void PrepareForm(string jobName)
        {
            _job = _controller.GetJob(jobName);
            _job.DataOutput += UpdateOutput;
            //
            UpdateProgress();
            //
            tbOutput.Text = _job.AllOutputData;
            tbOutput.Select(tbOutput.TextLength, 0);
            tbOutput.ScrollToCaret();
            //
            tbStatus.Text = _job.StatusFileData;
            tbStatus.Select(tbOutput.TextLength, 0);
            tbStatus.ScrollToCaret();
        }

        private void UpdateOutput()
        {
            try
            {
                if (this.tbOutput.InvokeRequired)
                {
                    // This is a worker thread so delegate the task.
                    this.BeginInvoke(new MethodInvoker(() => UpdateOutput()));
                }
                else
                {
                    if (_job != null)
                    {
                        // It's on the same thread, no need for Invoke
                        tbOutput.AutoScrollAppendText(_job.OutputData);
                        //
                        tbStatus.AutoScrollSetText(_job.StatusFileData);
                        //
                        tbConvergence.AutoScrollSetText(_job.ConvergenceFileData);
                    }
                }
            }
            catch
            { }
        }
        public void UpdateProgress()
        {
            if (_job != null)
            {
                if (_job.JobStatus == JobStatus.Running)
                {
                    pbAnalysisStatus.Style = ProgressBarStyle.Marquee;
                    labAnalysisStatus.Text = "      " + _job.JobStatus.ToString();
                    labAnalysisStatus.Image = global::PrePoMax.Properties.Resources.Running;
                }
                else if (_job.JobStatus == JobStatus.OK)
                {
                    pbAnalysisStatus.Style = ProgressBarStyle.Blocks;
                    labAnalysisStatus.Text = "      " + "Finished";
                    labAnalysisStatus.Image = global::PrePoMax.Properties.Resources.OK;
                }
                else if (_job.JobStatus == JobStatus.FailedWithResults)
                {
                    pbAnalysisStatus.Style = ProgressBarStyle.Blocks;
                    labAnalysisStatus.Text = "      " + "Failed with results";
                    labAnalysisStatus.Image = global::PrePoMax.Properties.Resources.Warning;
                    //
                    CheckForErrors();
                }
                else
                {
                    pbAnalysisStatus.Style = ProgressBarStyle.Blocks;
                    labAnalysisStatus.Text = "      " + _job.JobStatus.ToString();
                    labAnalysisStatus.Image = global::PrePoMax.Properties.Resources.NoResult;
                    //
                    CheckForErrors();
                }
            }
        }
        private void CheckForErrors()
        {
            string output = tbOutput.Text.ToUpper();
            //
            if (output.Contains("*ERROR"))
            {
                if (output.Contains("NONPOSITIVE JACOBIAN"))
                {
                    string[] sets = output.Split(new string[] { "DETERMINANT IN ELEMENT" }, StringSplitOptions.RemoveEmptyEntries);
                    //
                    string[] tmp;
                    HashSet<int> errorElementIds = new HashSet<int>();
                    //
                    for (int i = 1; i < sets.Length; i++)   // skip the first set
                    {
                        tmp = sets[i].Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                        errorElementIds.Add(int.Parse(tmp[0]));
                    }
                    string elementSetName = _controller.Model.Mesh.ElementSets.GetNextNumberedKey("Nonpositive_jacobian");
                    _controller.AddElementSet(new CaeMesh.FeElementSet(elementSetName, errorElementIds.ToArray()));
                    //
                    tbOutput.AppendText(Environment.NewLine);
                    tbOutput.AppendText(" An element set containing elements with nonpositive jacobian determinant was created.");
                    tbOutput.AppendText(Environment.NewLine);
                }
            }
        }
    }
}
