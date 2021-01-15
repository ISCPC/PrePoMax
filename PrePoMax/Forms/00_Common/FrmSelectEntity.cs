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
        private string _selectOneOrMultipleText = "Select one or multiple items";
        private string _selectOneText = "Select one item";
        private string _entitiyNames;
        private string _stepName;
        private NamedClass[] _entitiesToSelect;
        private string[] _preSelectedEntities;
        private Controller _controller;
        private int _minNumOfEntities;
        private int _maxNumOfEntities;


        // Properties                                                                                                               
        public string EntitiesName 
        {
            get { return _entitiyNames; } 
            set
            {
                _entitiyNames = value;
                Text = _entitiyNames + " selection";
                groupBox1.Text = "Available " + _entitiyNames.ToLower();
                MultiSelect = MultiSelect; // sets the lebel text
            }
        }
        public bool MultiSelect
        {
            get
            {
                return dgvNames.MultiSelect;
            }
            set
            {
                dgvNames.MultiSelect = value;
                //
                if (dgvNames.MultiSelect) label1.Text = _selectOneOrMultipleText; // + _entitiyNames.ToLower();
                else label1.Text = _selectOneText; // + _entitiyNames.ToLower();
            }
        }
        public Action<string> OneEntitySelected { get; set; }
        public Action<string, string> OneEntitySelectedInStep { get; set; }
        public Action<string[]> MultipleEntitiesSelected { get; set; }
        public Action<string, string[]> MultipleEntitiesSelectedInStep { get; set; }
        public int MinNumberOfEntities
        { 
            get { return _minNumOfEntities; }
            set
            {
                _minNumOfEntities = value;
                if (_minNumOfEntities < 1) _minNumOfEntities = 1;
                if (_minNumOfEntities > _maxNumOfEntities) _minNumOfEntities = _maxNumOfEntities;
            }
        }
        public int MaxNumberOfEntities 
        {
            get { return _maxNumOfEntities; }
            set
            {
                _maxNumOfEntities = value;
                if (_maxNumOfEntities < _minNumOfEntities) _maxNumOfEntities = _minNumOfEntities;
            }
        }


        // Constructors                                                                                                             
        public FrmSelectEntity(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            //
            _minNumOfEntities = 1;
            _maxNumOfEntities = int.MaxValue;
        }


        // Event handlers                                                                                                           
        private void btnContinue_Click(object sender, EventArgs e)
        {
            try
            {
                string[] selectedEntityNames = GetSelectedEntityNames();
                // First hide since this form calls itself in the following lines
                Hide();
                //
                if (dgvNames.MultiSelect)
                {
                    if (_stepName == null)
                    {
                        if (MultipleEntitiesSelected != null) MultipleEntitiesSelected(selectedEntityNames);
                    }
                    else
                    {
                        if (MultipleEntitiesSelectedInStep != null) MultipleEntitiesSelectedInStep(_stepName, selectedEntityNames);
                    }
                }
                else
                {
                    if (_stepName == null)
                    {
                        if (OneEntitySelected != null) OneEntitySelected(selectedEntityNames[0]);
                    }
                    else
                    {
                        if (OneEntitySelectedInStep != null) OneEntitySelectedInStep(_stepName, selectedEntityNames[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void FrmSelectEntity_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmSelectEntity_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                SetEntityNamesToSelect();
            }
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
            // Disable selection
            _controller.SetSelectByToOff();
            //
            EntitiesName = title;
            MultiSelect = multiselect;
            _entitiesToSelect = entitiesToSelect;
            _preSelectedEntities = preSelectedEntities;
            _stepName = stepName;
            //
            if (_entitiesToSelect == null) _entitiesToSelect = new NamedClass[0];
            if (_preSelectedEntities == null) _preSelectedEntities = new string[0];
            if (_entitiesToSelect.Length > 0 && _preSelectedEntities.Length == 0)
                _preSelectedEntities = new string[] { _entitiesToSelect[0].Name };
            //
            _minNumOfEntities = 1;
            _maxNumOfEntities = int.MaxValue;
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
            //
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
            if (dgvNames.MultiSelect)
            {
                if (names.Count < _minNumOfEntities)
                    throw new CaeException("Select at least " + _minNumOfEntities + " " + _entitiyNames.ToLower() + ".");
                if (names.Count > _maxNumOfEntities)
                    throw new CaeException("Select at most " + _maxNumOfEntities + " " + _entitiyNames.ToLower() + ".");
            }
            //
            return names.ToArray();
        }

       
    }
}
