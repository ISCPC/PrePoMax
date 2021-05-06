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
            : base(1.75)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewModelProperties = null;
            //
            _addNew = false;
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProperties
            // 
            this.gbProperties.Size = new System.Drawing.Size(340, 364);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(328, 336);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(190, 376);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(271, 376);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(109, 376);
            this.btnOkAddNew.Text = "Apply";
            // 
            // FrmModelProperties
            // 
            this.ClientSize = new System.Drawing.Size(364, 411);
            this.MinimumSize = new System.Drawing.Size(380, 450);
            this.Name = "FrmModelProperties";
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
                fileName = System.IO.Path.Combine(_controller.Settings.GetWorkDirectory(), fileName);
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
