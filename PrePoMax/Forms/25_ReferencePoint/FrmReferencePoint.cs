using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmReferencePoint : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private HashSet<string> _allExistingNames;
        private string _referencePointToEditName;
        private ViewFeReferencePoint _viewReferencePoint;
        private Controller _controller;


        // Properties                                                                                                               
        public FeReferencePoint ReferencePoint
        {
            get { return _viewReferencePoint.GetBase(); }
            set { _viewReferencePoint = new ViewFeReferencePoint(value.DeepClone()); }
        }
       

        // Constructors                                                                                                             
        public FrmReferencePoint(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _viewReferencePoint = null;
            _allExistingNames = new HashSet<string>();
            _selectedPropertyGridItemChangedEventActive = true;
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
            this.btnOkAddNew.Location = new System.Drawing.Point(65, 376);
            // 
            // FrmReferencePoint
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmReferencePoint";
            this.Text = "Edit Reference Point";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            if (nameof(_viewReferencePoint.CreateReferencePointFrom) == propertyGrid.SelectedGridItem.PropertyDescriptor.Name)
            {
                string value = (string)propertyGrid.SelectedGridItem.Value.ToString();
                if (value != FeReferencePointCreatedFrom.Coordinates.ToString() && ReferencePoint.NodeSetName != null)
                {
                    if (_controller.GetUserNodeSetNames().Contains(ReferencePoint.NodeSetName))
                        ReferencePoint.UpdateCoordinates(_controller.GetNodeSet(ReferencePoint.NodeSetName));
                }
            }

            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnPropertyGridSelectedGridItemChanged()
        {
            object value = propertyGrid.SelectedGridItem.Value;
            if (value != null)
            {
                string valueString = value.ToString();

                if (propertyGrid.SelectedObject == null) { }
                else if (propertyGrid.SelectedObject is ViewFeReferencePoint)
                {
                    ViewFeReferencePoint vrp = propertyGrid.SelectedObject as ViewFeReferencePoint;
                    if (valueString == vrp.NodeSetName) _controller.Highlight3DObjects(new object[] { vrp.NodeSetName });
                    else _controller.Highlight3DObjects(null);
                }
                else throw new NotImplementedException();
            }
        }
        protected override void Apply()
        {
            _viewReferencePoint = (ViewFeReferencePoint)propertyGrid.SelectedObject;

            if ((_referencePointToEditName == null && _allExistingNames.Contains(_viewReferencePoint.Name)) ||                       // named to existing name
                (_viewReferencePoint.Name != _referencePointToEditName && _allExistingNames.Contains(_viewReferencePoint.Name)))     // renamed to existing name
                throw new CaeGlobals.CaeException("The selected name already exists.");

            if (_referencePointToEditName == null)
            {
                // Create
                _controller.AddReferencePointCommand(ReferencePoint);
            }
            else
            {
                // Replace
                if (_propertyItemChanged) _controller.ReplaceReferencePointCommand(_referencePointToEditName, ReferencePoint);
            }
        }
        protected override void OnPrepareForm(string stepName, string referencePointToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = referencePointToEditName == null;

            _propertyItemChanged = false;
            _allExistingNames.Clear();
            _referencePointToEditName = null;
            _viewReferencePoint = null;

            _allExistingNames.UnionWith(_controller.GetAllMeshEntityNames());
            _referencePointToEditName = referencePointToEditName;

            string[] nodeSetNames = _controller.GetUserNodeSetNames();

            if (_referencePointToEditName == null) ReferencePoint = new FeReferencePoint(GetReferencePointName(), 0, 0, 0);
            else ReferencePoint = _controller.GetReferencePoint(_referencePointToEditName); // to clone

            if (ReferencePoint.CreatedFrom != FeReferencePointCreatedFrom.Coordinates)
            {
                CheckMissingValueRef(ref nodeSetNames, _viewReferencePoint.NodeSetName, s => { _viewReferencePoint.NodeSetName = s; });

                // CheckMissingValue changes _propertyItemChanged -> update coordinates
                if (_propertyItemChanged && ReferencePoint.NodeSetName != null
                    && _controller.GetReferencePointNames().Contains(ReferencePoint.NodeSetName))
                    ReferencePoint.UpdateCoordinates(_controller.GetNodeSet(ReferencePoint.NodeSetName));
            }

            _viewReferencePoint.PopululateDropDownList(nodeSetNames);

            propertyGrid.SelectedObject = _viewReferencePoint;
            propertyGrid.Select();
        }


        // Methods                                                                                                                  
        public void PrepareForm(string stepName, string referencePointToEditName)
        {
            OnPrepareForm(stepName, referencePointToEditName);
        }
        private string GetReferencePointName()
        {
            return NamedClass.GetNewValueName(_allExistingNames.ToArray(), "RP-");
        }
    }
}
