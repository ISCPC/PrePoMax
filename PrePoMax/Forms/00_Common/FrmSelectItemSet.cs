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
        // Variables                                                                                                                
        private bool _checkBoxEventRunning;
        private ItemSetData _itemSetData;
        private Controller _controller;

        // Properties                                                                                                               
        public ItemSetData ItemSetData
        {
            get { return _itemSetData; } 
            set { if (_itemSetData != value) _itemSetData = value; }
        }


        // Callbacks                                                                                                                
        //public Func<int[]> GetSelectionIds;
        //public Action SetSelectionOff;
        //public Action<vtkSelectBy> SetSelectBy;
        //public Action<double> SetSelectAngle;
        //public Action<bool> RemoveLastSelectionNode;

        //public Action ClearSelection;


        // Constructors                                                                                                             
        public FrmSelectItemSet(Controller controller)
        {
            InitializeComponent();

            _checkBoxEventRunning = false;
            _controller = controller;
            _itemSetData = null;
        }
        public FrmSelectItemSet(Controller controller, ItemSetData itemSetData)
            : this(controller)
        {
            _itemSetData = itemSetData;
        }


        // Event handlers                                                                                                           
        private void FrmSelectItemSet_Shown(object sender, EventArgs e)
        {
            // called on minimize - maximize
            this.DialogResult = DialogResult.None;    // minimizing the control

            rbSelectBy_CheckedChanged(null, null);
        }
        private void FrmSelectItemSet_VisibleChanged(object sender, EventArgs e)
        {
            // called every time the form is shown with: form.Show()
            if (this.Visible) rbSelectBy_CheckedChanged(null, null);
        }
        private void FrmSelectItemSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                _controller.SetSelectionOff();
                Hide();
            }
        }

        private void rbSelectBy_CheckedChanged(object sender, EventArgs e)
        {
            if (_checkBoxEventRunning) return;  // allow only one running function
            else _checkBoxEventRunning = true;


            // clear selection - if geometry check changed
            if (rbGeometry.Checked || _controller.SelectBy == vtkSelectBy.Geometry)
                _controller.ClearSelectionHistory();

            // connect two group boxes of radio buttons
            if (sender != null)
            {
                if (sender == rbGeometry)
                {
                    if (rbNode.Checked) rbNode.Checked = false;
                    if (rbElement.Checked) rbElement.Checked = false;
                    if (rbEdge.Checked) rbEdge.Checked = false;
                    if (rbSurface.Checked) rbSurface.Checked = false;
                    if (rbPart.Checked) rbPart.Checked = false;
                    if (rbEdgeAngle.Checked) rbEdgeAngle.Checked = false;
                    if (rbSurfaceAngle.Checked) rbSurfaceAngle.Checked = false;
                    if (rbId.Checked) rbId.Checked = false;
                }
                else
                {
                    if (rbGeometry.Checked) rbGeometry.Checked = false;
                }
            }

            // enable/disable Textboxes
            tbSurfaceAngle.Enabled = rbSurfaceAngle.Checked;
            tbEdgeAngle.Enabled = rbEdgeAngle.Checked;
            tbId.Enabled = rbId.Checked;
            // enable/disable buttons
            btnAddId.Enabled = rbId.Checked;
            btnSubtractId.Enabled = rbId.Checked;
            // check All and Invert buttons
            btnSelectAll.Enabled = !rbGeometry.Checked;
            btnInvertSelection.Enabled = !rbGeometry.Checked;

            vtkSelectBy selectBy;
            if (rbGeometry.Checked) selectBy = vtkSelectBy.Geometry;
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
            else return; // do not select

            _controller.SelectBy = selectBy;

            _checkBoxEventRunning = false;
        }
        private void tbSurfaceAngle_TextChanged(object sender, EventArgs e)
        {
            double angle;
            if (double.TryParse(tbSurfaceAngle.Text, out angle)) _controller.SetSelectAngle(angle);
            else MessageBox.Show("The selection angle is not a valid number.");
        }
        private void tbEdgeAngle_TextChanged(object sender, EventArgs e)
        {
            double angle;
            if (double.TryParse(tbEdgeAngle.Text, out angle)) _controller.SetSelectAngle(angle);
            else MessageBox.Show("The selection angle is not a valid number.");
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            _controller.AddSelectionNode(new SelectionNodeIds(vtkSelectOperation.None, true), true);
        }
        private void btnInvertSelection_Click(object sender, EventArgs e)
        {
            _controller.AddSelectionNode(new SelectionNodeInvert(), true);
        }
        private void btnUndoSelection_Click(object sender, EventArgs e)
        {
            _controller.RemoveLastSelectionNode(true);
        }
        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            _controller.ClearSelectionHistory();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnClearSelection_Click(null, null);
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _controller.SetSelectionOff();
            Hide();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _itemSetData.ItemIds = _controller.GetSelectionIds();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            _controller.SetSelectionOff();
            Hide();
        }

    

       
       


        // Methods                                                                                                                  
       

      




    }
}
