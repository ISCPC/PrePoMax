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
        //public Action<vtkControl.vtkSelectBy> SetSelectBy;
        //public Action<double> SetSelectAngle;
        //public Action<bool> RemoveLastSelectionNode;

        //public Action ClearSelection;


        // Constructors                                                                                                             
        public FrmSelectItemSet(Controller controller)
        {
            InitializeComponent();

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
            tbSurfaceAngle.Enabled = rbSurfaceAngle.Checked;
            tbEdgeAngle.Enabled = rbEdgeAngle.Checked;

            tbId.Enabled = rbId.Checked;
            btnAddId.Enabled = rbId.Checked;
            btnSubtractId.Enabled = rbId.Checked;

            vtkControl.vtkSelectBy selectBy;
            if (rbNode.Checked) selectBy = vtkControl.vtkSelectBy.Node;
            else if (rbElement.Checked) selectBy = vtkControl.vtkSelectBy.Element;
            else if (rbEdge.Checked) selectBy = vtkControl.vtkSelectBy.Edge;
            else if (rbSurface.Checked) selectBy = vtkControl.vtkSelectBy.Surface;
            else if (rbPart.Checked) selectBy = vtkControl.vtkSelectBy.Part;
            else if (rbEdgeAngle.Checked)
            {
                selectBy = vtkControl.vtkSelectBy.EdgeAngle;
                tbEdgeAngle_TextChanged(null, null);
            }
            else if (rbSurfaceAngle.Checked)
            {
                selectBy = vtkControl.vtkSelectBy.SurfaceAngle;
                tbSurfaceAngle_TextChanged(null, null);
            }
            else if (rbId.Checked) selectBy = vtkControl.vtkSelectBy.Id;
            else return; // do not select

            _controller.SetSelectBy(selectBy);
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
            _controller.AddSelectionNode(new SelectionNodeIds(vtkControl.vtkSelectOperation.None, true), true);
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
                    SelectionNodeIds selectionNodeIds = new SelectionNodeIds(vtkControl.vtkSelectOperation.Add, false, new int[] { id });
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
                    SelectionNodeIds selectionNodeIds = new SelectionNodeIds(vtkControl.vtkSelectOperation.Subtract, false, new int[] { id });
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
