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
                if (value is StaticStep ss) _viewStep = new ViewStaticStep(ss.DeepClone());
                else if (value is FrequencyStep fs) _viewStep = new ViewFrequencyStep(fs.DeepClone());
                else if (value is BuckleStep bs) _viewStep = new ViewBuckleStep(bs.DeepClone());
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
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 97);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 69);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 115);
            this.gbProperties.Size = new System.Drawing.Size(310, 355);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 327);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 476);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 476);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(65, 476);
            // 
            // FrmStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 511);
            this.MinimumSize = new System.Drawing.Size(350, 550);
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
                propertyGrid.SelectedObject = lvTypes.SelectedItems[0].Tag;
                propertyGrid.Select();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            (propertyGrid.SelectedObject as ViewStep).UpdateFieldView();

            //_viewStep.UpdateFieldView(); // update visibility of fields
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void Apply()
        {
            _viewStep = (ViewStep)propertyGrid.SelectedObject;

            if (_viewStep == null)
                throw new CaeGlobals.CaeException("There is no step selected.");

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
        protected override bool OnPrepareForm(string stepName, string stepToEditName)
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
                Step = _controller.GetStep(_stepToEditName);    // to clone and set _viewStep

                // select the appropriate load in the list view
                foreach (ListViewItem item in lvTypes.Items)
                {
                    if (item.Tag != null && item.Tag.GetType() == _viewStep.GetType())
                    {
                        item.Selected = true;
                        break;
                    }
                }

                lvTypes.Enabled = false;

                propertyGrid.SelectedObject = _viewStep;
                propertyGrid.Select();
            }

            return true;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string stepToEditName)
        {
            return OnPrepareForm(stepName, stepToEditName);
        }
        private void PopulateListOfSteps()
        {
            Step newStep = null;
            Step prevOrLastStep = GetPreviousOrLastStep();
            bool addStatic = false;
            bool addFrequency = false;
            bool addBuckle = false;
            bool cannotAdd = false;

            if (prevOrLastStep == null || prevOrLastStep is StaticStep) addStatic = true;
            if (!(prevOrLastStep is FrequencyStep)) addFrequency = true;
            if (!(prevOrLastStep is BuckleStep)) addBuckle = true;

            cannotAdd = !(addStatic || addFrequency || addBuckle);

            ListViewItem item;
            if (cannotAdd)
            {
                item = new ListViewItem("No steps can be added after the last step.");
                item.Tag = null;
                lvTypes.Items.Add(item);
            }
            else
            {
                if (addStatic)
                {
                    // Static step
                    item = new ListViewItem("Static step");
                    newStep = CreateNewOrCloneLast(typeof(StaticStep));
                    item.Tag = new ViewStaticStep(newStep as StaticStep);
                    lvTypes.Items.Add(item);
                }
                if (addFrequency)
                {
                    // Frequency step
                    item = new ListViewItem("Frequency step");
                    newStep = new FrequencyStep(GetStepName());
                    item.Tag = new ViewFrequencyStep(newStep as FrequencyStep);
                    lvTypes.Items.Add(item);
                }
                if (addBuckle)
                {
                    // Frequency step
                    item = new ListViewItem("Buckle step");
                    newStep = new BuckleStep(GetStepName());
                    item.Tag = new ViewBuckleStep(newStep as BuckleStep);
                    lvTypes.Items.Add(item);
                }
            }
            
        }
        private Step CreateNewOrCloneLast(Type stepTypeToCreate)
        {
            if (stepTypeToCreate == null) throw new ArgumentNullException();

            Step newStep = null;
            Step prevOrLastStep = GetPreviousOrLastStep();

            if (prevOrLastStep == null)
            {
                newStep = (Step)Activator.CreateInstance(stepTypeToCreate, new object[] { GetStepName() });
            }
            else
            {
                newStep = prevOrLastStep.DeepClone();
                //newStep.FieldOutputs.Clear();
                newStep.BoundaryConditions.Clear(); // this gets added at the step collection
                newStep.Loads.Clear();              // this gets added at the step collection
                newStep.Name = GetStepName();
            }
            return newStep;
        }
        private Step GetPreviousOrLastStep()
        {
            Step prevOrLastStep = null;

            // find previous step
            if (_stepToEditName != null)
            {
                Step[] steps = _controller.GetAllSteps();
                int prevStepId = -1;
                for (int i = 0; i < steps.Length; i++)
                {
                    if (steps[i].Name == _stepToEditName)
                    {
                        prevStepId = i - 1;
                        break;
                    }
                }
                if (prevStepId >= 0) prevOrLastStep = steps[prevStepId];
            }
            // find last step
            else if (_stepNames.Length > 0)
            {
                prevOrLastStep = _controller.GetAllSteps().Last();
            }

            return prevOrLastStep;
        }
        private string GetStepName()
        {
            return NamedClass.GetNewValueName(_stepNames, "Step-");
        }
    }
}
