using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CaeGlobals;

namespace PrePoMax
{
    public partial class FrmSelectItemSet : Form
    {
        // Enum
        private enum SelectionType
        {
            Geometry,
            Mesh
        }


        // Variables                                                                                                                
        private bool _checkBoxEventRunning;
        private ItemSetData _itemSetData;
        private Controller _controller;
        private SelectionType _prevSelectionType;
        private Size _expandedSize;
        private Point _btnUndoPosition;
        private Point _btnClearPosition;


        // Properties                                                                                                               
        public ItemSetData ItemSetData
        {
            get { return _itemSetData; } 
            set { if (_itemSetData != value) _itemSetData = value; }
        }
        

        // Constructors                                                                                                             
        public FrmSelectItemSet(Controller controller)
        {
            InitializeComponent();
            //
            _checkBoxEventRunning = false;
            _controller = controller;
            _itemSetData = null;
            _prevSelectionType = SelectionType.Geometry;
            _expandedSize = Size;
            _btnUndoPosition = btnUndoSelection.Location;
            _btnClearPosition = btnClearSelection.Location;
            //
            btnMoreLess_Click(null, null);
        }
        public FrmSelectItemSet(Controller controller, ItemSetData itemSetData)
            : this(controller)
        {
            _itemSetData = itemSetData;
        }


        // Event handlers                                                                                                           
        private void FrmSelectItemSet_Move(object sender, EventArgs e)
        {
            Point location = Location.DeepClone();
            location.X -= ItemSetDataEditor.ParentForm.Width - 15;
            ItemSetDataEditor.ParentForm.Location = location;
        }
        //
        private void FrmSelectItemSet_VisibleChanged(object sender, EventArgs e)
        {
            // Called every time the form is shown with: form.Show()
            if (this.Visible)
            {
                // Form was just shown
                Point location = ItemSetDataEditor.ParentForm.Location.DeepClone();
                location.X += ItemSetDataEditor.ParentForm.Width - 15;
                Location = location;
                //
                if (ItemSetDataEditor.ParentForm is Forms.IFormItemSetDataParent fdsp) 
                    SetGeometrySelection(fdsp.IsSelectionGeometryBased());
                //
                ItemSetDataEditor.ParentForm.Enabled = false;
                // To prevent the call to _frmSelectItemSet_VisibleChanged when minimized
                this.DialogResult = DialogResult.None; 
                rbSelectBy_CheckedChanged(null, null);
            }
            else
            {
                // Form was just hidden
                if (this.DialogResult == DialogResult.OK || this.DialogResult == DialogResult.Cancel)
                {
                    ItemSetDataEditor.ParentForm.DialogResult = this.DialogResult;
                    ItemSetDataEditor.ParentForm.Enabled = true;
                }
                //
                _controller.SetSelectionToDefault();
            }
        }
        private void FrmSelectItemSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide(DialogResult.Cancel);
            }
        }
        //
        private void rbSelectBy_CheckedChanged(object sender, EventArgs e)
        {            
            // Allow only one running function - disable check box event
            if (_checkBoxEventRunning) return;  
            else _checkBoxEventRunning = true;

            // Connect two group boxes of radio buttons
            if (sender != null) // radio button check was activated by user
            {
                // If radio button was checked off - do nothing; this must be 
                if (!((RadioButton)sender).Checked)
                {
                    _checkBoxEventRunning = false;
                    return;
                }
                // Set new states of radio buttons before everything else
                if (rbGeometry.Checked && sender != rbGeometry)
                    rbGeometry.Checked = false;
                if (rbGeometryEdgeAngle.Checked && sender != rbGeometryEdgeAngle)
                    rbGeometryEdgeAngle.Checked = false;
                if (rbGeometrySurfaceAngle.Checked && sender != rbGeometrySurfaceAngle)
                    rbGeometrySurfaceAngle.Checked = false;

                if (rbNode.Checked && sender != rbNode) rbNode.Checked = false;
                if (rbElement.Checked && sender != rbElement) rbElement.Checked = false;
                if (rbEdge.Checked && sender != rbEdge) rbEdge.Checked = false;
                if (rbSurface.Checked && sender != rbSurface) rbSurface.Checked = false;
                if (rbPart.Checked && sender != rbPart) rbPart.Checked = false;
                if (rbEdgeAngle.Checked && sender != rbEdgeAngle) rbEdgeAngle.Checked = false;
                if (rbSurfaceAngle.Checked && sender != rbSurfaceAngle) rbSurfaceAngle.Checked = false;
                if (rbId.Checked && sender != rbId) rbId.Checked = false;
            }

            // Determine selection type and change of selection type
            bool selectionTypeChanged = false;
            SelectionType currentSelectionType;
            if (rbGeometry.Checked || rbGeometryEdgeAngle.Checked || rbGeometrySurfaceAngle.Checked)
            {
                selectionTypeChanged = _prevSelectionType != SelectionType.Geometry;
                currentSelectionType = SelectionType.Geometry;
            }
            else
            {
                selectionTypeChanged = _prevSelectionType != SelectionType.Mesh;
                currentSelectionType = SelectionType.Mesh;
            }
            // Clear selection - if geometry selection type changed by the USER: 
            // sender != null                                                    
            // Visible = true - user action                                      
            if (sender != null && Visible && selectionTypeChanged) _controller.ClearSelectionHistory();

            // Enable/disable Textboxes
            tbGeometryEdgeAngle.Enabled = rbGeometryEdgeAngle.Checked;
            tbGeometrySurfaceAngle.Enabled = rbGeometrySurfaceAngle.Checked;
            tbSurfaceAngle.Enabled = rbSurfaceAngle.Checked;
            tbEdgeAngle.Enabled = rbEdgeAngle.Checked;
            tbId.Enabled = rbId.Checked;
            // Enable/disable buttons
            btnAddId.Enabled = rbId.Checked;
            btnSubtractId.Enabled = rbId.Checked;
            // Check All and Invert buttons
            btnSelectAll.Enabled = currentSelectionType == SelectionType.Mesh;
            btnInvertSelection.Enabled = currentSelectionType == SelectionType.Mesh;

            vtkSelectBy selectBy;
            if (rbGeometry.Checked) selectBy = vtkSelectBy.Geometry;
            else if (rbGeometryEdgeAngle.Checked)
            {
                selectBy = vtkSelectBy.GeometryEdgeAngle;
                tbGeometryEdgeAngle_TextChanged(null, null);
            }
            else if (rbGeometrySurfaceAngle.Checked)
            {
                selectBy = vtkSelectBy.GeometrySurfaceAngle;
                tbGeometrySurfaceAngle_TextChanged(null, null);
            }
            else if (rbNode.Checked) selectBy = vtkSelectBy.Node;
            else if (rbElement.Checked) selectBy = vtkSelectBy.Element;
            else if (rbEdge.Checked) selectBy = vtkSelectBy.Edge;
            else if (rbSurface.Checked) selectBy = vtkSelectBy.Surface;
            else if (rbPart.Checked) selectBy = vtkSelectBy.Part;
            else if (rbEdgeAngle.Checked)
            {
                selectBy = vtkSelectBy.EdgeAngle;
                tbEdgeAngle_TextChanged(null, null);
            }
            else if (rbSurfaceAngle.Checked)
            {
                selectBy = vtkSelectBy.SurfaceAngle;
                tbSurfaceAngle_TextChanged(null, null);
            }
            else if (rbId.Checked) selectBy = vtkSelectBy.Id;
            else selectBy = vtkSelectBy.Off;

            // Set selection
            _controller.SelectBy = selectBy;
            // Set previous selection type
            _prevSelectionType = currentSelectionType;
            // Enable check box event
            _checkBoxEventRunning = false;
        }
        private void tbGeometryEdgeAngle_TextChanged(object sender, EventArgs e)
        {
            SetSelectionAngle(tbGeometryEdgeAngle);
        }
        private void tbGeometrySurfaceAngle_TextChanged(object sender, EventArgs e)
        {
            SetSelectionAngle(tbGeometrySurfaceAngle);
        }
        private void btnUndoSelection_Click(object sender, EventArgs e)
        {
            _controller.RemoveLastSelectionNode(true);
        }
        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            _controller.ClearSelectionHistory();
        }
        private void btnMoreLess_Click(object sender, EventArgs e)
        {
            Size size;
            if (btnMoreLess.Text == "More")
            {
                btnMoreLess.Text = "Less";
                size = _expandedSize;
                //
                Point locationAll = btnSelectAll.Location;
                Point locationInvert = btnInvertSelection.Location;
                int delta = locationInvert.Y - locationAll.Y;
                btnUndoSelection.Location = locationInvert;
                btnUndoSelection.Top += delta;
                btnClearSelection.Location = locationInvert;
                btnClearSelection.Top += 2 * delta;
            }
            else
            {
                btnMoreLess.Text = "More";
                size = new Size(237, 300);
                //
                btnUndoSelection.Location = _btnUndoPosition;
                btnClearSelection.Location = _btnClearPosition;
            }
            //
            this.MaximumSize = size;
            this.MinimumSize = size;
            this.Size = size;
        }
        //
        private void tbEdgeAngle_TextChanged(object sender, EventArgs e)
        {
            SetSelectionAngle(tbEdgeAngle);
        }
        private void tbSurfaceAngle_TextChanged(object sender, EventArgs e)
        {
            SetSelectionAngle(tbSurfaceAngle);
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            _controller.AddSelectionNode(new SelectionNodeIds(vtkSelectOperation.None, true), true);
        }
        private void btnInvertSelection_Click(object sender, EventArgs e)
        {
            _controller.AddSelectionNode(new SelectionNodeInvert(), true);
        }
        private void btnAddId_Click(object sender, EventArgs e)
        {
            try 
            {
                int id;
                if (int.TryParse(tbId.Text, out id))
                {
                    SelectionNodeIds selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.Add, false, new int[] { id });
                    _controller.AddSelectionNode(selectionNodeIds, true);
                }
                else
                {
                    MessageBox.Show("The item id is not a valid integer number.");
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }

        }
        private void btnSubtractId_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(tbId.Text, out int id))
                {
                    SelectionNodeIds selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.Subtract, false, new int[] { id });
                    _controller.AddSelectionNode(selectionNodeIds, true);
                }
                else
                {
                    MessageBox.Show("The item id is not a valid integer number.");
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        //
        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnClearSelection_Click(null, null);
            Hide(DialogResult.Cancel);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _itemSetData.ItemIds = _controller.GetSelectionIds();
            Hide(DialogResult.OK);
        }
        public void Hide(DialogResult dialogResult)
        {
            this.DialogResult = dialogResult;
            base.Hide();
        }

        // Methods                                                                                                                  
        public void SetGeometrySelection(bool selectGeometry)
        {
            if (selectGeometry)
            {
                if (rbGeometry.Checked) return;
                //
                rbGeometry.Checked = true;
                if (btnMoreLess.Enabled && btnMoreLess.Text == "Less") btnMoreLess_Click(null, null);
            }
            else
            {
                if (rbNode.Checked) return;
                //
                rbNode.Checked = true;
                if (btnMoreLess.Enabled && btnMoreLess.Text == "More") btnMoreLess_Click(null, null);
            }
        }
        public void SetOnlyGeometrySelection(bool onlyGeometrySelection)
        {
            if (onlyGeometrySelection)
            {
                SetGeometrySelection(true);
                if (btnMoreLess.Text == "Less") btnMoreLess_Click(null, null);
                btnMoreLess.Enabled = false;
            }
            else
            {
                btnMoreLess.Enabled = true;
            }
        }
        //
        private void SetSelectionAngle(TextBox tbAngle)
        {
            double angle;
            if (double.TryParse(tbAngle.Text, out angle)) _controller.SetSelectAngle(angle);
            else MessageBox.Show("The selection angle is not a valid number.");
        }

       
    }
}
















