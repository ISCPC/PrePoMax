using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControls
{
    public class DataGridViewCopyPaste : DataGridView
    {
        private System.Windows.Forms.ContextMenuStrip cmsCopyPaste;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem tsmiCut;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiPaste;


        // Constructors                                                                                                             
        public DataGridViewCopyPaste()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsCopyPaste = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCopyPaste.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // cmsCopyPaste
            // 
            this.cmsCopyPaste.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCut,
            this.tsmiCopy,
            this.tsmiPaste});
            this.cmsCopyPaste.Name = "cmsCopyPaste";
            this.cmsCopyPaste.Size = new System.Drawing.Size(145, 70);
            // 
            // tsmiCut
            // 
            this.tsmiCut.Name = "tsmiCut";
            this.tsmiCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.tsmiCut.Size = new System.Drawing.Size(144, 22);
            this.tsmiCut.Text = "Cut";
            this.tsmiCut.Click += new System.EventHandler(this.tsmiCut_Click);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsmiCopy.Size = new System.Drawing.Size(144, 22);
            this.tsmiCopy.Text = "Copy";
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.tsmiPaste.Size = new System.Drawing.Size(144, 22);
            this.tsmiPaste.Text = "Paste";
            this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
            //
            this.cmsCopyPaste.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            //
            this.KeyUp += dgvData_KeyUp;
            this.CellMouseDown += dgvData_CellMouseDown;
        }
       

        // Event handlers - context menu                                                                                            
        private void dgvData_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Delete)
                //{
                //    // Clear selected cells
                //    foreach (DataGridViewCell dgvCell in SelectedCells) dgvCell.Value = 0;
                //}
            }
            catch { }
        }
        private void dgvData_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    {
                        ContextMenuStrip = null;
                        return;
                    }
                    //
                    DataGridViewCell cell = Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (!cell.Selected)
                    {
                        ContextMenuStrip = cmsCopyPaste;
                        //
                        EndEdit();
                        ClearSelection();
                        cell.Selected = true;
                    }
                }
            }
            catch { }
        }
        private void tsmiCut_Click(object sender, EventArgs e)
        {
            try
            {
                // Copy to clipboard
                CopyToClipboard();
                // Clear selected cells
                foreach (DataGridViewCell dgvCell in SelectedCells) dgvCell.Value = 0;
            }
            catch { }
        }
        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            try
            {
                CopyToClipboard();
            }
            catch { }
        }
        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            try
            {
                // Perform paste Operation
                PasteClipboardValue();
            }
            catch { }
        }


        // Methods                                                                                                                  
        private void CopyToClipboard()
        {
            // Copy to clipboard
            DataObject dataObj = GetClipboardContent();
            if (dataObj != null) Clipboard.SetDataObject(dataObj);
        }
        private void PasteClipboardValue()
        {
            // Show Error if no cell is selected
            if (SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Get the starting Cell
            DataGridViewCell startCell = GetStartCell();
            // Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue = ClipBoardValues(Clipboard.GetText());
            //
            double value;
            string valueString;
            int iRowIndex = startCell.RowIndex;
            // Add new rows
            int numOfRows = cbValue.Keys.Count;
            int lastRow = iRowIndex + numOfRows;
            while (RowCount < lastRow + 1)
            {
                ((BindingSource)DataSource).AddNew();
            }
            //
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    // Check if the index is within the limit
                    if (iColIndex <= Columns.Count - 1 && iRowIndex <= Rows.Count - 1)
                    {
                        DataGridViewCell cell = this[iColIndex, iRowIndex];
                        //
                        valueString = cbValue[rowKey][cellKey];
                        if (double.TryParse(valueString, out value)) cell.Value = value;
                        else cell.Value = 0;
                        //
                        cell.Selected = true;
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }
        private DataGridViewCell GetStartCell()
        {
            // Get the smallest row,column index
            if (SelectedCells.Count == 0) return null;
            //
            int rowIndex = Rows.Count - 1;
            int colIndex = Columns.Count - 1;
            //
            foreach (DataGridViewCell dgvCell in SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex) rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex) colIndex = dgvCell.ColumnIndex;
            }
            //
            return this[colIndex, rowIndex];
        }
        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>> copyValues = new Dictionary<int, Dictionary<int, string>>();
            //
            string[] lines = clipboardValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //
            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                string[] lineContent = lines[i].Split('\t');
                // If an empty cell value copied, then set the dictionary with an empty string else Set value to dictionary
                if (lineContent.Length == 0) copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++) copyValues[i][j] = lineContent[j];
                }
            }
            //
            return copyValues;
        }
    }
}
