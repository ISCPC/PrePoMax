using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmModelProperties : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private ViewModelProperties _viewModelProperties;
        private Controller _controller;

        // Properties                                                                                                               
        public CaeModel.ModelProperties ModelProperties
        {
            get { return _viewModelProperties.GetBase(); }
            set {_viewModelProperties = new ViewModelProperties(value);}
        }


        // Constructors                                                                                                             
        public FrmModelProperties(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _viewModelProperties = null;

            _addNew = false;

            propertyGrid.SetLabelColumnWidth(1.9);
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 376);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 376);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 376);
            this.btnOkAddNew.Size = new System.Drawing.Size(75, 23);
            this.btnOkAddNew.Text = "Apply";
            // 
            // FrmPartProperties
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmPartProperties";
            this.Text = "Edit Part";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);
        }


        // Overrides                                                                                                                
        protected override void OnApply(bool onOkAddNew)
        {
            _viewModelProperties = (ViewModelProperties)propertyGrid.SelectedObject;
            //
            if (_viewModelProperties.ModelType == CaeModel.ModelType.Submodel)
            {
                string fileName = _viewModelProperties.GlobalResultsFileName;
                fileName = System.IO.Path.Combine(_controller.Settings.Calculix.WorkDirectory, fileName);
                //
                if (!System.IO.File.Exists(fileName))
                    throw new CaeGlobals.CaeException("The selected global results file " + fileName +" does not exist.");
            }
            // Replace
            if (_propertyItemChanged)
            {
                _controller.ReplaceModelPropertiesCommand(_viewModelProperties.Name, _viewModelProperties.GetBase());
            }
        }
        protected override bool OnPrepareForm(string stepName, string modelToEditName)
        {
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;
            // Disable selection
            _controller.SetSelectByToOff();
            //
            _propertyItemChanged = false;
            _viewModelProperties = null;
            //
            _viewModelProperties = new ViewModelProperties(_controller.Model.Properties);
            _viewModelProperties.Name = _controller.Model.Name;
            //
            propertyGrid.SelectedObject = _viewModelProperties;
            propertyGrid.Select();
            //
            return true;
        }


        // Methods                                                                                                                  
        

        
    }
}
