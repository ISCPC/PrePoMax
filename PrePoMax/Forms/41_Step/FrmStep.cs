using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmStep : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _stepNames;
        private string _stepToEditName;
        private ViewStep _viewStep;
        private Controller _controller;


        // Properties                                                                                                               
        public string[] StepNames { get { return _stepNames; } set { _stepNames = value; } }
        public Step Step
        {
            get { return _viewStep.Base; }
            set
            {
                if (value is StaticStep) _viewStep = new ViewStaticStep((StaticStep)value.DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmStep(Controller controller)
            : base(1.7)
        {

            InitializeComponent();

            _controller = controller;
            _viewStep = null;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmStep";
            this.Text = "Edit Step";
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);
        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.Enabled && lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                _viewStep = (ViewStep)lvTypes.SelectedItems[0].Tag;
                propertyGrid.SelectedObject = _viewStep;
                propertyGrid.Select();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            _viewStep.UpdateFieldView(); // update visibility of fields
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void Apply()
        {
            _viewStep = (ViewStep)propertyGrid.SelectedObject;

            if ((_stepToEditName == null && _stepNames.Contains(_viewStep.Name)) ||             // create
                (_viewStep.Name != _stepToEditName && _stepNames.Contains(_viewStep.Name)))     // edit
                throw new CaeGlobals.CaeException("The selected step name already exists.");

            if (_stepToEditName == null)
            {
                // Create
                _controller.AddStepCommand(Step);
            }
            else
            {
                // Replace
                if (_propertyItemChanged) _controller.ReplaceStepCommand(_stepToEditName, Step);
            }
        }
        protected override void OnPrepareForm(string stepName, string stepToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = stepToEditName == null;

            _propertyItemChanged = true;
            _stepNames = null;
            _stepToEditName = null;
            _viewStep = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;

            _stepNames = _controller.GetStepNames();
            _stepToEditName = stepToEditName;

            if (_stepNames == null)
                throw new CaeGlobals.CaeException("The step names must be defined first.");

            PopulateListOfSteps();

            if (_stepToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewStep = null;

                if (lvTypes.Items.Count == 1) lvTypes.Items[0].Selected = true;
            }
            else
            {
                lvTypes.Enabled = false;
                Step = _controller.GetStep(_stepToEditName); // to clone

                propertyGrid.SelectedObject = _viewStep;
                propertyGrid.Select();
            }
        }


        // Methods                                                                                                                  
        public void PrepareForm(string stepName, string stepToEditName)
        {
            OnPrepareForm(stepName, stepToEditName);
        }
        private void PopulateListOfSteps()
        {
            ListViewItem item;
            Step[] steps = _controller.GetAllSteps();

            if (steps.Length == 0)
            {
                // Static step
                item = new ListViewItem("Static step");
                item.Tag = new ViewStaticStep(new StaticStep(GetStepName()));
                lvTypes.Items.Add(item);

                // Frequency step
                item = new ListViewItem("Frequency step");
                item.Tag = new ViewFrequencyStep(new FrequencyStep(GetStepName()));
                lvTypes.Items.Add(item);
            }
            else
            {
                Step step = steps.Last().DeepClone();
                if (step is StaticStep)
                {
                    // Static step
                    item = new ListViewItem("Static step");
                    step = steps.Last().DeepClone();
                    step.FieldOutputs.Clear();
                    step.BoundaryConditions.Clear();
                    step.Loads.Clear();
                    step.Name = GetStepName();
                    item.Tag = new ViewStaticStep((StaticStep)step);
                    lvTypes.Items.Add(item);
                }
            }
        }
        private string GetStepName()
        {
            return NamedClass.GetNewValueName(_stepNames, "Step-");
        }
    }
}
