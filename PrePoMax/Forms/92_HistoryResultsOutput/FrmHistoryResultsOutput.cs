using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax
{
    public partial class FrmHistoryResultsOutput : Form
    {
        // Variables                                                                                                                
        private Controller _controller;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmHistoryResultsOutput(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
        }


        // Event handlers                                                                                                           
        private void FrmHistoryOutput_Load(object sender, EventArgs e)
        {
            dgvHistory.PreviewKeyDown += DgvHistory_PreviewKeyDown;
        }

        private void DgvHistory_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void FrmHistoryOutput_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmHistoryOutput_VisibleChanged(object sender, EventArgs e)
        {
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }


        // Methods                                                                                                                  
        public void SetData(string[] columnNames, object[][] rowBasedData)
        {
            if (rowBasedData.Length <= 0) return;

            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();

            DataGridViewTextBoxCell cell;
            cell = new DataGridViewTextBoxCell();
            DataGridViewColumn column;
            int maxNumCol = 100;

            if (columnNames.Length > maxNumCol)
            {
                MessageBox.Show("Only first " + maxNumCol + " columns of " + columnNames.Length
                                + " will be displayed.", "Warning", MessageBoxButtons.OK);
            }

            for (int i = 0; i < Math.Min(columnNames.Length, maxNumCol); i++)
            {
                column = new DataGridViewColumn(cell);
                column.HeaderText = columnNames[i];
                column.FillWeight = 1;
                //column.CellType = typeof(double);
                dgvHistory.Columns.Add(column);
            }
            foreach (var row in rowBasedData) dgvHistory.Rows.Add(row);


            return;
            DataTable table = new DataTable();
            

            for (int i = 0; i < columnNames.Length; i++) table.Columns.Add(columnNames[i], typeof(double));

            foreach (var row in rowBasedData) table.Rows.Add(row);
                
            dgvHistory.DataSource = table;
        }

        private void dgvHistory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C | e.KeyCode == Keys.V | e.KeyCode == Keys.X))
            {
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                //Copy to clipboard
                DataObject dataObj = dgvHistory.GetClipboardContent();
                if (dataObj != null)
                    Clipboard.SetDataObject(dataObj, true);
            }
        }
    }
}
