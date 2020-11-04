using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeJob;
using System.Reflection;
using System.IO;
using CaeGlobals;

namespace PrePoMax.Forms
{
    public partial class FrmAnalysis : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private string[] _jobNames;
        private string _jobToEditName;
        private ViewJob _viewJob;
        private Controller _controller;
        private double _labelRatio = 3;


        // Properties                                                                                                               
        public AnalysisJob Job 
        {
            get { return _viewJob.GetBase(); }
            set { _viewJob = new ViewJob(value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmAnalysis(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewJob = null;
            //
            propertyGrid.SetParent(this);   // for the Tab key to work
            propertyGrid.SetLabelColumnWidth(_labelRatio);
        }


        // Event hadlers                                                                                                            
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                _viewJob = (ViewJob)propertyGrid.SelectedObject;
                //
                if ((_jobToEditName == null && _jobNames.Contains(_viewJob.Name)) ||                // named to existing name
                    (_viewJob.Name != _jobToEditName && _jobNames.Contains(_viewJob.Name)))         // renamed to existing name
                    throw new CaeGlobals.CaeException("The selected analysis name already exists.");
                //
                if (_jobToEditName == null)
                {
                    // Create
                    _controller.AddJobCommand(Job);
                }
                else
                {
                    // Replace
                    if (_propertyItemChanged) _controller.ReplaceJobCommand(_jobToEditName, Job);
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;       // use this value to update the model tree selected item highlight
                Hide();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Hide();
        }
        private void FrmAnalysis_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Hide();
            }
        }

        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string jobToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            //
            _propertyItemChanged = false;
            _jobNames = null;
            _jobToEditName = null;
            _viewJob = null;
            propertyGrid.SelectedObject = null;
            //
            _jobNames = _controller.GetJobNames();
            _jobToEditName = jobToEditName;
            //
            if (_jobToEditName == null)
            {
                Job = new AnalysisJob(GetJobName(), _controller.Settings.Calculix.CalculixExe, GetJobName(),
                                      _controller.Settings.GetWorkDirectory());
            }
            else
            {
                Job = _controller.GetJob(_jobToEditName); // to clone
            }
            //
            propertyGrid.SelectedObject = _viewJob;
            propertyGrid.Select();
            //
            _controller.SetSelectByToOff();
            //
            return true;
        }
        private string GetJobName()
        {
            int max = 0;
            int tmp;
            string[] parts;
            foreach (var jobName in _jobNames)
            {
                parts = jobName.Split('-');
                if (int.TryParse(parts.Last(), out tmp))
                {
                    if (tmp > max) max = tmp;
                }
            }
            max++;
            //
            return "Analysis-" + max.ToString();
        }

       
      
    }
}
