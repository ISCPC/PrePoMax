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
    class FrmFieldOutput : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _fieldOutputNames;
        private string _fieldOutputToEditName;
        private ViewFieldOutput _viewFieldOutput;
        private Controller _controller;


        // Properties                                                                                                               
        public FieldOutput FieldOutput
        {
            get { return _viewFieldOutput.Base; }
            set
            {
                var clone = value.DeepClone();
                if (value is NodalFieldOutput) _viewFieldOutput = new ViewNodalFieldOutput((NodalFieldOutput)clone);
                else if (value is ElementFieldOutput) _viewFieldOutput = new ViewElementFieldOutput((ElementFieldOutput)clone);
                else if (clone == null) _viewFieldOutput = null;
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmFieldOutput(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _viewFieldOutput = null;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmTmp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmFieldOutput";
            this.Text = "Edit Field Output";
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
        protected override void Apply()
        {
            if (propertyGrid.SelectedObject == null) throw new CaeException("No item selected.");

            _viewFieldOutput = (ViewFieldOutput)propertyGrid.SelectedObject;

            if ((_fieldOutputToEditName == null && _fieldOutputNames.Contains(_viewFieldOutput.Name)) ||                // create
               (_viewFieldOutput.Name != _fieldOutputToEditName && _fieldOutputNames.Contains(_viewFieldOutput.Name)))  // rename
                throw new CaeGlobals.CaeException("The selected field output name already exists.");

            if (_fieldOutputToEditName == null)
            {
                // Create
                _controller.AddFieldOutputCommand(_stepName, FieldOutput);
            }
            else
            {
                // Replace
                if (_propertyItemChanged) _controller.ReplaceFieldOutputCommand(_stepName, _fieldOutputToEditName, FieldOutput);
            }
        }
        protected override bool OnPrepareForm(string stepName, string fieldOutputToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = fieldOutputToEditName == null;

            _propertyItemChanged = false;
            _stepName = null;
            _fieldOutputNames = null;
            _fieldOutputToEditName = null;
            _viewFieldOutput = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;

            _stepName = stepName;
            _fieldOutputNames = _controller.GetFieldOutputNamesForStep(_stepName);
            _fieldOutputToEditName = fieldOutputToEditName;

            if (_fieldOutputNames == null)
                throw new CaeGlobals.CaeException("The field output names must be defined first.");

            PopulateListOfFieldOutputs();

            if (fieldOutputToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewFieldOutput = null;
            }
            else
            {
                FieldOutput = _controller.GetFieldOutput(_stepName, fieldOutputToEditName); // to clone

                // select the appropriate constraint in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewFieldOutput.Base is NodalFieldOutput) lvTypes.Items[0].Selected = true;
                else if (_viewFieldOutput.Base is ElementFieldOutput) lvTypes.Items[1].Selected = true;
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;

                propertyGrid.SelectedObject = _viewFieldOutput;
                propertyGrid.Select();
            }

            return true;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string fieldOutputToEditName)
        {
            return OnPrepareForm(stepName, fieldOutputToEditName);
        }
        private void PopulateListOfFieldOutputs()
        {
            // populate list view                                                                               
            ListViewItem item;

            // initialize
            item = new ListViewItem("Node output");
            ViewNodalFieldOutput vnfo = new ViewNodalFieldOutput(new NodalFieldOutput(GetFieldOutputName("N"), NodalFieldVariable.U));
            item.Tag = vnfo;
            lvTypes.Items.Add(item);

            item = new ListViewItem("Element output");
            ViewElementFieldOutput vefo = new ViewElementFieldOutput(new ElementFieldOutput(GetFieldOutputName("E"), ElementFieldVariable.S | ElementFieldVariable.E));
            item.Tag = vefo;
            lvTypes.Items.Add(item);
        }
        private string GetFieldOutputName(string prefix)
        {
            return NamedClass.GetNewValueName(_fieldOutputNames, prefix + "F-Output-");
        }
    }
}
