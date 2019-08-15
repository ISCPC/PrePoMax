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

namespace PrePoMax.Forms
{
    public partial class FrmSelectEntity : UserControls.PrePoMaxChildForm
    {
        // Variables                                                                                                                
        private string _entitiyNames;
        private string _stepName;
        private NamedClass[] _entitiesToSelect;
        private string[] _preSelectedEntities;
        private Controller _controller;

        // Properties                                                                                                               
        public string EntitiesName 
        {
            get { return _entitiyNames; } 
            set
            {
                _entitiyNames = value;
                Text = _entitiyNames + " selection";
                groupBox1.Text = "Available " + _entitiyNames.ToLower();
                label1.Text = "Select one or multiple " +_entitiyNames.ToLower();
            }
        }
        public bool MultiSelect { get { return dgvNames.MultiSelect; } set { dgvNames.MultiSelect = value; } }
        public Action<string> OneEntitySelected { get; set; }
        public Action<string, string> OneEntitySelectedInStep { get; set; }
        public Action<string[]> MultipleEntitiesSelected { get; set; }
        public Action<string, string[]> MultipleEntitiesSelectedInStep { get; set; }


        // Constructors                                                                                                             
        public FrmSelectEntity(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
        }


        // Event handlers                                                                                                           
        private void btnContinue_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Hide();  // first hide since this form calls itself in the following lines

            if (dgvNames.MultiSelect)
            {
                if (_stepName == null)
                {
                    if (MultipleEntitiesSelected != null) MultipleEntitiesSelected(GetSelectedEntityNames());
                }
                else
                {
                    if (MultipleEntitiesSelectedInStep != null) MultipleEntitiesSelectedInStep(_stepName, GetSelectedEntityNames());
                }
            }
            else
            {
                if (_stepName == null)
                {
                    if (OneEntitySelected != null) OneEntitySelected(GetSelectedEntityNames()[0]);
                }
                else
                {
                    if (OneEntitySelectedInStep != null) OneEntitySelectedInStep(_stepName, GetSelectedEntityNames()[0]);
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Hide();
        }
        private void FrmSelectEntity_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Hide();
            }
        }
        private void FrmSelectEntity_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) SetEntityNamesToSelect();
        }
        private void cbHighlight_CheckedChanged(object sender, EventArgs e)
        {
            dgvNames_SelectionChanged(null, null);
        }
        private void dgvNames_SelectionChanged(object sender, EventArgs e)
        {
            if (cbHighlight.Checked)
            {
                object[] items = new object[dgvNames.SelectedRows.Count];

                int count = 0;
                foreach (DataGridViewRow row in dgvNames.SelectedRows)
                {
                    items[count++] = row.Cells[dgvNames.Columns[0].Name].Tag;
                }

                _controller.Highlight3DObjects(items);
            }
            else _controller.Highlight3DObjects(null);
        }
        private void dgvNames_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                btnContinue_Click(null, null);
            }
        }

        // Methods                                                                                                                  
        public void PrepareForm(string title, bool multiselect, NamedClass[] entitiesToSelect, string[] preSelectedEntities,
                                string stepName = null)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized

            EntitiesName = title;
            MultiSelect = multiselect;
            _entitiesToSelect = entitiesToSelect;
            _preSelectedEntities = preSelectedEntities;
            _stepName = stepName;
        }
        private void SetEntityNamesToSelect()
        {
            dgvNames.Rows.Clear();
            int rowId;
            foreach (var entity in _entitiesToSelect)
            {
                rowId = dgvNames.Rows.Add(new object[] { entity.Name });
                dgvNames.Rows[rowId].Cells[dgvNames.Columns[0].Name].Tag = entity;
                
            }

            dgvNames.ClearSelection();
            foreach (DataGridViewRow row in dgvNames.Rows)
            {
                // setting row.selected = false in SingleSelection mode clears the selected row which has selection = true
                row.Selected = _preSelectedEntities.Contains(row.Cells[dgvNames.Columns[0].Name].Value);
                if (dgvNames.SelectedRows.Count > 0 && !MultiSelect) break;
            }
        }
        public string[] GetSelectedEntityNames()
        {
            List<string> names = new List<string>();
            foreach (DataGridViewRow row in dgvNames.SelectedRows)
            {
                names.Add((string)row.Cells["colName"].Value);
            }
            return names.ToArray();
        }

        
    }
}
