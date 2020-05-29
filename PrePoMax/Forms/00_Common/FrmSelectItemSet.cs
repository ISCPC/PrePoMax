using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        private bool _initialSetup;
      

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
            _initialSetup = false;
            //
            btnMoreLess_Click(null, null);
            //
            StringAngleConverter.SetUnit = "deg";
            TryConvertAngle(tbGeometryEdgeAngle);
            TryConvertAngle(tbGeometrySurfaceAngle);
            TryConvertAngle(tbEdgeAngle);
            TryConvertAngle(tbSurfaceAngle);
        }
        public FrmSelectItemSet(Controller controller, ItemSetData itemSetData)
            : this(controller)
        {
            _itemSetData = itemSetData;
        }


        // Event handlers                                                                                                           
        private void FrmSelectItemSet_Move(object sender, EventArgs e)
        {
            return;
            //
            Point location = Location.DeepClone();
            location.X -= ItemSetDataEditor.ParentForm.Width - 15;
            ItemSetDataEditor.ParentForm.Location = location;
        }
        //
        private void FrmSelectItemSet_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                // Set a marker that auto setup is running
                _initialSetup = true;
                // Called every time the form is shown with: form.Show()
                if (this.Visible)
                {
                    // Form was just shown                
                    if (ItemSetDataEditor.ParentForm is Forms.IFormItemSetDataParent fdsp)
                        SetGeometrySelection(fdsp.IsSelectionGeometryBased());
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
                    }
                    //
                    _controller.SetSelectionToDefault();
                }
            }
            catch { }
            finally { _initialSetup = false; }
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
                //
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
            if (!_initialSetup && sender != null && Visible && selectionTypeChanged)
                _controller.ClearSelectionHistoryAndSelectionChanged();
            // Enable/disable Textboxes
            tbGeometryEdgeAngle.Enabled = rbGeometryEdgeAngle.Checked;
            tbGeometrySurfaceAngle.Enabled = rbGeometrySurfaceAngle.Checked;
            tbSurfaceAngle.Enabled = rbSurfaceAngle.Checked;
            tbEdgeAngle.Enabled = rbEdgeAngle.Checked;
            tbId.Enabled = rbId.Checked;
            // Enable/disable buttons
            btnAddId.Enabled = rbId.Checked;
            btnRemoveId.Enabled = rbId.Checked;
            // Check All and Invert buttons
            btnSelectAll.Enabled = currentSelectionType == SelectionType.Mesh;
            btnInvertSelection.Enabled = currentSelectionType == SelectionType.Mesh;
            //
            vtkSelectBy selectBy;
            if (rbGeometry.Checked) selectBy = vtkSelectBy.Geometry;
            else if (rbGeometryEdgeAngle.Checked)
            {
                selectBy = vtkSelectBy.GeometryEdgeAngle;
                SetSelectionAngle(tbGeometryEdgeAngle);
            }
            else if (rbGeometrySurfaceAngle.Checked)
            {
                selectBy = vtkSelectBy.GeometrySurfaceAngle;
                SetSelectionAngle(tbGeometrySurfaceAngle);
            }
            else if (rbNode.Checked) selectBy = vtkSelectBy.Node;
            else if (rbElement.Checked) selectBy = vtkSelectBy.Element;
            else if (rbEdge.Checked) selectBy = vtkSelectBy.Edge;
            else if (rbSurface.Checked) selectBy = vtkSelectBy.Surface;
            else if (rbPart.Checked) selectBy = vtkSelectBy.Part;
            else if (rbEdgeAngle.Checked)
            {
                selectBy = vtkSelectBy.EdgeAngle;
                SetSelectionAngle(tbEdgeAngle);
            }
            else if (rbSurfaceAngle.Checked)
            {
                selectBy = vtkSelectBy.SurfaceAngle;
                SetSelectionAngle(tbSurfaceAngle);
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
        private void btnUndoSelection_Click(object sender, EventArgs e)
        {
            _controller.RemoveLastSelectionNode(true);
        }
        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            _controller.ClearSelectionHistoryAndSelectionChanged();
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
                size = new Size(207, 300);
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
        private void btnRemoveId_Click(object sender, EventArgs e)
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
        //
        private void tbAngle_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    // Convert
                    TryConvertAngle(tb);
                    //
                    e.SuppressKeyPress = true;  // no beep
                }
            }
        }
        private void tbAngle_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is TextBox tb) SetSelectionAngle(tb);
        }
        private void tbAngle_Leave(object sender, EventArgs e)
        {
            if (sender is TextBox tb) TryConvertAngle(tb);
        }
        private void tbId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;  // no beep
            }
        }

        // Methods                                                                                                                  
        public void ShowIfHidden(IWin32Window owner)
        {
            if (!this.Visible)
            {
                ItemSetDataEditor.SelectionForm.ResetLocation();
                ItemSetDataEditor.SelectionForm.Show(owner);
            }
        }
        public void ResetLocation()
        {

            {
                Point location = ItemSetDataEditor.ParentForm.Location.DeepClone();
                location.X += ItemSetDataEditor.ParentForm.Width - 15 + 3;
                Location = location;
            }
        }
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
            try
            {
                double angle;
                if (!double.TryParse(tbAngle.Text, out angle))
                {
                    var converter = new StringAngleConverter();
                    angle = (double)converter.ConvertFromString(tbAngle.Text);
                }
                _controller.SetSelectAngle(angle);
            }
            catch
            {
            }
            
        }
        private void TryConvertAngle(TextBox tb)
        {
            try
            {
                // Convert
                var converter = new StringAngleConverter();
                double angle = (double)converter.ConvertFromString(tb.Text);
                tb.Text = converter.ConvertToString(angle);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Entered value: " + tb.Text + Environment.NewLine + Environment.NewLine + ex.Message, "Error");
                tb.Focus();
            }
        }
        //
        // Disable close X button
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                const int CP_NOCLOSE_BUTTON = 0x200;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        
    }
}
















