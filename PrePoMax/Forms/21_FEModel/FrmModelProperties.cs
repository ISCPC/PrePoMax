using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;


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
            // btnOkAddNew
            // 
            this.btnOkAddNew.Text = "Apply";
            // 
            // FrmModelProperties
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
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
                    throw new CaeException("The selected global results file " + fileName +" does not exist.");
            }
            else if (_viewModelProperties.ModelType == CaeModel.ModelType.SlipWearModel)
            {
                if (_viewModelProperties.NumberOfCycles % _viewModelProperties.CyclesIncrement != 0)
                    throw new CaeException("The number of slip wear cycles must be divided by the cycles increment.");
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
            _viewModelProperties = new ViewModelProperties(_controller.Model.Properties.DeepClone());
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
