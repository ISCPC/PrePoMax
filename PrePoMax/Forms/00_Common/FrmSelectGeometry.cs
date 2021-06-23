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
using CaeMesh;


namespace PrePoMax.Forms
{
    public enum SelectGeometryEnum
    {
        Vertex,
        Edge,
        Surface
    }
    public partial class FrmSelectGeometry : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private Controller _controller;
        private GeometrySelection _geometrySelection;
        private bool _hideFormOnOK;
        private SelectGeometryEnum _selectionFilter;


        // Properties                                                                                                               
        public GeometrySelection GeometrySelection
        {
            get { return _geometrySelection; }
            set { _geometrySelection = value.DeepClone(); }
        }
        public int MaxNumberOfSelectedItems
        {
            get { return _controller.Selection.MaxNumberOfIds; }
            set { _controller.Selection.MaxNumberOfIds = value; }
        }
        public bool HideFormOnOK { get { return _hideFormOnOK; } set { _hideFormOnOK = value; } }
        public SelectGeometryEnum SelectionFilter { get { return _selectionFilter; } set { _selectionFilter = value; } }


        // Callbacks
        public Action<GeometrySelection> OnOKCallback;

        // Constructors                                                                                                             
        public FrmSelectGeometry(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            //
            _geometrySelection = new GeometrySelection("Selection");
            _hideFormOnOK = true;
            _selectionFilter = SelectGeometryEnum.Surface;
        }


        // Event handlers                                                                                                           
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (GeometrySelection.GeometryIds != null && GeometrySelection.GeometryIds.Length > 0)
                {
                    if (_controller.Selection.MaxNumberOfIds > 0 &&
                        _controller.Selection.MaxNumberOfIds != GeometrySelection.GeometryIds.Length)
                        throw new CaeException("Number of selected items: " + GeometrySelection.GeometryIds.Length +
                                               Environment.NewLine + 
                                               "Number of required items: " + _controller.Selection.MaxNumberOfIds);
                    //
                    if (OnOKCallback != null)
                    {
                        OnOKCallback(GeometrySelection);
                        // Clear items - calls SelectionChanged which clears the GeometrySelection
                        _controller.ClearAllSelection();
                    }
                    DialogResult = DialogResult.OK;
                    //
                    if (_hideFormOnOK) Hide();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void FrmSelectEntity_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                //
                btnClose_Click(null, null);
            }
        }
        private void FrmSelectEntity_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) OnShow();
            else OnHide();
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string itemName)
        {
            // Clear items
            _geometrySelection.Clear();
            lvItems.Items.Clear();
            // Set selection
            SetSelectItem();
            //
            return true;
        }
        private void SetSelectItem()
        {
            // Set selection
            _controller.SetSelectItemToGeometry();
            //
            if (_selectionFilter == SelectGeometryEnum.Vertex) _controller.SelectBy = vtkSelectBy.GeometryVertex;
            else if (_selectionFilter == SelectGeometryEnum.Edge) _controller.SelectBy = vtkSelectBy.GeometryEdge;
            else if (_selectionFilter == SelectGeometryEnum.Surface) _controller.SelectBy = vtkSelectBy.GeometrySurface;
            else throw new NotSupportedException();
            //
            _controller.SetSelectAngle(-1);
        }
        //
        private void OnShow()
        {
            // Clear items - calls SelectionChanged which clears the GeometrySelection
            _controller.ClearAllSelection();
        }
        private void OnHide()
        {
            // Reset the number of selection nodes
            _controller.Selection.MaxNumberOfIds = -1;
            // Reset the hide on OK
            _hideFormOnOK = true;
            // Dereference the callback
            OnOKCallback = null;
        }
        //
        public void SelectionChanged(int[] ids)
        {
            lvItems.Items.Clear();
            // Clear previously selected ids - must be here
            GeometrySelection.GeometryIds = null;
            GeometrySelection.CreationData = null;
            //
            if (ids != null && ids.Length > 0)
            {
                GeometrySelection.GeometryIds = ids;
                GeometrySelection.CreationData = _controller.Selection.DeepClone();
                //
                string partName;
                string itemName;
                int[] itemTypePartIds;
                GeometryType geomType;
                ListViewItem listViewItem;
                FeMesh mesh = _controller.DisplayedMesh;
                //
                foreach (int id in ids)
                {
                    itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(id);
                    itemName = null;
                    partName = mesh.GetPartById(itemTypePartIds[2]).Name;
                    geomType = (GeometryType)itemTypePartIds[1];
                    //
                    if (geomType == GeometryType.Vertex)
                    {
                        itemName = "Vertex " + itemTypePartIds[0];
                    }
                    else if (geomType == GeometryType.Edge || geomType == GeometryType.ShellEdgeSurface)
                    {
                        itemName = "Edge " + itemTypePartIds[0];
                    }
                    else if (geomType == GeometryType.SolidSurface ||
                             geomType == GeometryType.ShellFrontSurface ||
                             geomType == GeometryType.ShellBackSurface)
                    {
                        itemName = "Surface " + itemTypePartIds[0];
                    }
                    else throw new NotSupportedException();
                    //
                    if (itemName != null)
                    {
                        listViewItem = lvItems.Items.Add(new ListViewItem(partName + " : " + itemName));
                    }
                }
            }
        }


    }
}
