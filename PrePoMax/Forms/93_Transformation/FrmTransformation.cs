using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;
using CaeGlobals;
using PrePoMax.PropertyViews;
using System.Runtime.CompilerServices;
using System.Reflection;
using CaeResults;

namespace PrePoMax.Forms
{
    public partial class FrmTransformation : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        //private List<Transformation> _transformations;
        private Controller _controller;
        private TabPage[] _pages;
        private double[][] _coorNodesToDraw;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmTransformation(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            //
            propertyGrid.SetParent(this);   // for the Tab key to work
            propertyGrid.SetLabelColumnWidth(1.7);
            //
            int i = 0;
            _pages = new TabPage[tcProperties.TabPages.Count];
            foreach (TabPage tabPage in tcProperties.TabPages)
            {
                tabPage.Paint += TabPage_Paint;
                _pages[i++] = tabPage;
            }
            //
            ClearControls();
        }


        // Event handling
        private void TabPage_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush fillBrush = new SolidBrush(((TabPage)sender).BackColor);
            e.Graphics.FillRectangle(fillBrush, e.ClipRectangle);
            // Enable copy/paste without first selecting the cell 0,0
            if (sender == tpDataPoints)
            {
                ActiveControl = dgvData;
                dgvData[0, 0].Selected = true;
            }
        }
        private void tvTransformations_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(null, null);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tvTransformations.SelectedNode != null && tvTransformations.SelectedNode.Tag != null)
            {
                string propertyName = tvTransformations.SelectedNode.Name;
                //
                ListViewItem item = new ListViewItem(propertyName);
                if (tvTransformations.SelectedNode.Tag is Transformation tr)
                {
                    if (tr is Symetry sym) item.Tag = new ViewSymetry(sym.DeepClone());
                    else throw new NotSupportedException();
                }
                else throw new NotSupportedException();
                //
                lvActiveTransformations.Items.Add(item);                    
                int id = lvActiveTransformations.Items.IndexOf(item);
                lvActiveTransformations.Items[id].Selected = true;
                lvActiveTransformations.Select();
                //
                _propertyItemChanged = true;
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvActiveTransformations.SelectedItems.Count == 1)
            {
                lvActiveTransformations.SelectedItems[0].Remove();
                if (lvActiveTransformations.Items.Count > 0) lvActiveTransformations.Items[0].Selected = true;
                else ClearControls();
            }
            _propertyItemChanged = true;
        }
        private void lvActiveTransformations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvActiveTransformations.SelectedItems.Count == 1)
            {
                if (lvActiveTransformations.SelectedItems[0].Tag is ViewSymetry)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[0]);
                    propertyGrid.SelectedObject = lvActiveTransformations.SelectedItems[0].Tag;
                }
                else throw new NotSupportedException();
            }
            lvActiveTransformations.Select();
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            _propertyItemChanged = true;
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Entered value is not a valid/numeric value.", "Error", MessageBoxButtons.OK);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyTransformation();
                //
                this.DialogResult = DialogResult.OK;       // use this value to update the model tree selected item highlight
                Hide();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }            
        }
        private void bntApply_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyTransformation();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.SetTransformations(null);

                //lvActiveTransformations.Items.Clear();
                //propertyGrid.SelectedObject = null;
                ////
                //_propertyItemChanged = true;
                ////
                //ApplyTransformation();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            dgvData.HidePlot();
            Hide();
        }
        private void FrmTrasformations_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                //
                dgvData.HidePlot();
                Hide();
            }
        }
    

        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string materialToEditName)
        {
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;
            //
            _propertyItemChanged = false;
            _propertyItemChanged = false;
            lvActiveTransformations.Clear();
            ClearControls();
            //
            List<Transformation> transformations;
            if (_controller.GetTransformations() != null) transformations = _controller.GetTransformations().DeepClone();
            else transformations = new List<Transformation>();
            // Initialize material properties
            tvTransformations.Nodes.Find("Symetry", true)[0].Tag = new Symetry("Symetry", new double[3], SymetryPlaneEnum.X);
            tvTransformations.ExpandAll();
            //
            if (transformations.Count > 0)
            {
                ListViewItem item;
                ViewTransformation view;
                //
                foreach (var transformation in transformations)
                {
                    if (transformation is Symetry sym) view = new ViewSymetry(sym);
                    else throw new NotSupportedException();
                    //
                    item = new ListViewItem(view.Name);
                    item.Tag = view;
                    lvActiveTransformations.Items.Add(item);
                }
                //
                lvActiveTransformations.Items[0].Selected = true;
                lvActiveTransformations.Select();
            }
            //
            return true;
        }
        private void ClearControls()
        {
            propertyGrid.SelectedObject = null;
            dgvData.DataSource = null;
            //
            tcProperties.TabPages.Clear();
            tcProperties.TabPages.Add(_pages[0]);
        }
        public void ApplyTransformation()
        {
            if (_propertyItemChanged)
            {
                List<Transformation> transformations = null;
                if (lvActiveTransformations.Items.Count > 0)
                {
                    transformations = new List<Transformation>();
                    ViewTransformation viewTransformation;
                    foreach (ListViewItem item in lvActiveTransformations.Items)
                    {
                        viewTransformation = (ViewTransformation)item.Tag;
                        transformations.Add(viewTransformation.Base);
                    }
                    
                }
                //
                _controller.SetTransformations(transformations);
            }
        }
        //
        public void PickedIds(int[] ids)
        {
            this.Enabled = true;
            //
            _controller.SelectBy = vtkSelectBy.Off;
            _controller.Selection.SelectItem = vtkSelectItem.None;
            _controller.ClearSelectionHistoryAndSelectionChanged();
            //
            if (ids != null && ids.Length == 1)
            {
                _propertyItemChanged = true;
                //
                float scale = _controller.GetScale();
                Vec3D deformed = new Vec3D(_controller.GetScaledNode(scale, ids[0]).Coor);
                //
                if (propertyGrid.SelectedObject is ViewSymetry vs)
                {
                    vs.SymetryPointX = deformed.X;
                    vs.SymetryPointY = deformed.Y;
                    vs.SymetryPointZ = deformed.Z;
                }
                else throw new NotSupportedException();
                //
                propertyGrid.Refresh();
                //
                HighlightNodes();
            }
        }
        private void HighlightNodes()
        {
            Color color = Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;
            //
            if (propertyGrid.SelectedObject is ViewSymetry vs)
            {
                _coorNodesToDraw = new double[1][];
                _coorNodesToDraw[0] = new double[3];
                //
                _coorNodesToDraw[0][0] = vs.SymetryPointX;
                _coorNodesToDraw[0][1] = vs.SymetryPointY;
                _coorNodesToDraw[0][2] = vs.SymetryPointZ;
            }
            else throw new NotSupportedException();
            //
            _controller.DrawNodes("Transformation", _coorNodesToDraw, color, layer, 7);
        }
    }
}
