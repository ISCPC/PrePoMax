using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Windows.Forms;
using System.Drawing;

namespace PrePoMax.Forms
{
    class FrmReferencePoint : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private HashSet<string> _allExistingNames;
        private string _referencePointToEditName;
        private ViewFeReferencePoint _viewReferencePoint;
        private Controller _controller;
        private double[][] _coorNodesToDraw;


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

            _coorNodesToDraw = new double[1][];
            _coorNodesToDraw[0] = new double[3];
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
                if (value != FeReferencePointCreatedFrom.Coordinates.ToString() && ReferencePoint.CreatedFromNodeSetName != null)
                {
                    if (_controller.GetUserNodeSetNames().Contains(ReferencePoint.CreatedFromNodeSetName))
                        ReferencePoint.UpdateCoordinates(_controller.GetNodeSet(ReferencePoint.CreatedFromNodeSetName));
                }
            }

            HighlightNodes();

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

            HighlightNodes();
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
        protected override bool OnPrepareForm(string stepName, string referencePointToEditName)
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
                if (_propertyItemChanged && ReferencePoint.CreatedFromNodeSetName != null
                    && _controller.GetReferencePointNames().Contains(ReferencePoint.CreatedFromNodeSetName))
                    ReferencePoint.UpdateCoordinates(_controller.GetNodeSet(ReferencePoint.CreatedFromNodeSetName));
            }

            _viewReferencePoint.PopululateDropDownList(nodeSetNames);

            propertyGrid.SelectedObject = _viewReferencePoint;
            propertyGrid.Select();

            return true;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string referencePointToEditName)
        {
            return OnPrepareForm(stepName, referencePointToEditName);
        }
        public void PickedIds(int[] ids)
        {
            this.Enabled = true;

            _controller.SelectBy = vtkSelectBy.Off;
            _controller.Selection.SelectItem = vtkSelectItem.None;
            _controller.ClearSelectionHistory();

            if (ids != null && ids.Length == 1)
            {
                FeNode node = _controller.Model.Mesh.Nodes[ids[0]];
                _viewReferencePoint.X = node.X;
                _viewReferencePoint.Y = node.Y;
                _viewReferencePoint.Z = node.Z;

                propertyGrid.Refresh();

                HighlightNodes();
            }
        }
        private string GetReferencePointName()
        {
            return NamedClass.GetNewValueName(_allExistingNames.ToArray(), "RP-");
        }

        private void HighlightNodes()
        {
            Color color = Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;

            _coorNodesToDraw[0][0] = _viewReferencePoint.X;
            _coorNodesToDraw[0][1] = _viewReferencePoint.Y;
            _coorNodesToDraw[0][2] = _viewReferencePoint.Z;

            _controller.DrawNodes("ReferencePoint", _coorNodesToDraw, color, layer, 7);
        }
    }
}
