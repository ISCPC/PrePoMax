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
    public partial class FrmSelectGeometry : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private Controller _controller;
        private bool _prevShowFaceOrientation;
        private GeometrySelection _geometrySelection;


        // Properties                                                                                                               
        public GeometrySelection GeometrySelection
        {
            get { return _geometrySelection; }
            set { _geometrySelection = value.DeepClone(); }
        }


        // Constructors                                                                                                             
        public FrmSelectGeometry(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            //
            _geometrySelection = new GeometrySelection("Selection");
        }


        // Event handlers                                                                                                           
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (GeometrySelection.GeometryIds != null && GeometrySelection.GeometryIds.Length > 0)
                {
                    _controller.FlipFaceOrientationsCommand(GeometrySelection);
                }
                // Clear items
                _geometrySelection.Clear();
                lvItems.Items.Clear();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            //
            OnHide();
            //
            Hide();
        }
        private void FrmSelectEntity_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.DialogResult = DialogResult.Cancel;
                //
                OnHide();
                //
                Hide();
            }
        }
        private void FrmSelectEntity_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) lvItems.ResizeColumnHeaders();
        }




        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string itemName)
        {
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;
            // Clear items
            _geometrySelection.Clear();
            lvItems.Items.Clear();
            // Set selection
            SetSelectItem();
            // Set view
            _prevShowFaceOrientation = _controller.ShowFaceOrientation;
            _controller.ShowFaceOrientation = true;
            return true;
        }
        private void SetSelectItem()
        {
            // Set selection
            _controller.SetSelectItemToGeometry();
            _controller.SelectBy = vtkSelectBy.QuerySurface;
            _controller.SetSelectAngle(-1);
        }

        private void OnHide()
        {
            _controller.ShowFaceOrientation = _prevShowFaceOrientation;
            _controller.SetSelectionToDefault();
        }

        public void SelectionChanged(int[] ids)
        {
            lvItems.Items.Clear();
            //
            if (ids != null && ids.Length > 0)
            {
                GeometrySelection.GeometryIds = ids;
                GeometrySelection.CreationData = _controller.Selection.DeepClone();
                //
                string partName;
                string itemName;
                int[] itemTypePartIds;
                ListViewItem listViewItem;
                FeMesh mesh = _controller.DisplayedMesh;
                //
                foreach (int id in ids)
                {
                    itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(id);
                    itemName = null;
                    partName = mesh.GetPartById(itemTypePartIds[2]).Name;
                    if (itemTypePartIds[1] == 3)    // 3 for surface
                    {
                        itemName = "Surface " + itemTypePartIds[0];
                    }
                    //
                    if (itemName != null)
                    {
                        listViewItem = lvItems.Items.Add(new ListViewItem(partName + " : " + itemName));
                        //listViewItem.Selected = true;
                    }
                }
            }
        }


    }
}
