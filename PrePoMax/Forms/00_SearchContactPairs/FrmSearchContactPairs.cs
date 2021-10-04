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
    public partial class FrmSearchContactPairs : Form
    {
        // Variables                                                                                                                
        private Controller _controller;
        private string missing = "Missing";
        private string[] _surfaceInteractionNames;
        private List<SearchContactPair> _selectedContactPairs;
        private bool _firstTime;


        // Constructors                                                                                                             
        public FrmSearchContactPairs(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            //
            tbDistance.UnitConverter = new StringLengthConverter();
            tbAngle.UnitConverter = new StringAngleDegConverter();            
            // Type
            cbType.Items.Add(SearchContactPairType.Tie);
            cbType.Items.Add(SearchContactPairType.Contact);
            cbType.SelectedIndex = 0;
            cbSurfaceInteraction.Enabled = false;
            // Group
            cbGroupBy.Items.Add("None");
            cbGroupBy.Items.Add("Parts");
            cbGroupBy.SelectedIndex = 1;
            // Adjust
            cbAbjustMesh.Items.Add("Yes");
            cbAbjustMesh.Items.Add("No");
            cbAbjustMesh.SelectedIndex = 1;
            //
            _selectedContactPairs = new List<SearchContactPair>(); 
            _firstTime = true;
        }


        // Event hadlers                                                                                                            
        private void FrmSearchContactPairs_Load(object sender, EventArgs e)
        {
            PrepareFoam();
        }
        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbSurfaceInteraction.Enabled = cbType.SelectedIndex == 1;
        }
        private void dgvData_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvData.SelectedRows.Count > 0)
                {
                    _selectedContactPairs = GetSelectedContactPairs();
                    //
                    _selectedContactPairs[0].MultiView = _selectedContactPairs.Count > 1;
                    tsmiMergeByMasterSlave.Enabled = _selectedContactPairs.Count > 1;
                    propertyGrid.SelectedObject = _selectedContactPairs[0];
                    //
                    HighlightContactPairs();
                }
                else propertyGrid.SelectedObject = null;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            try
            {
                string propertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                propertyGrid.Refresh();     // update property visibilities
                //
                if (_selectedContactPairs.Count > 1)
                {
                    SearchContactPair source = _selectedContactPairs[0];
                    //
                    foreach (SearchContactPair selectedContactPair in _selectedContactPairs)
                    {
                        if (selectedContactPair != source) selectedContactPair.SetProperty(propertyName, source);
                    }
                    //
                    dgvData.Refresh();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // Group by
                CaeMesh.GroupContactPairsByEnum groupBy;
                if (cbGroupBy.SelectedIndex == 0) groupBy = CaeMesh.GroupContactPairsByEnum.None;
                else if (cbGroupBy.SelectedIndex == 1) groupBy = CaeMesh.GroupContactPairsByEnum.ByParts;
                else throw new NotSupportedException();
                // Type
                SearchContactPairType type;
                if (cbType.SelectedIndex == 0) type = SearchContactPairType.Tie;
                else if (cbGroupBy.SelectedIndex == 1) type = SearchContactPairType.Contact;
                else throw new NotSupportedException();
                // Adjust
                bool adjust = cbAbjustMesh.SelectedIndex == 0;
                //
                double distance = tbDistance.Value;
                double angleDeg = tbAngle.Value;
                string[] surfaceInteracionNames = _controller.GetSurfaceInteractionNames();
                if (surfaceInteracionNames.Length == 0) surfaceInteracionNames = new string[] { missing };
                // Search
                _controller.SuppressExplodedViews();
                CaeMesh.ContactSearch contactSearch = new CaeMesh.ContactSearch(_controller.Model.Mesh, _controller.Model.Geometry);
                contactSearch.GroupContactPairsBy = groupBy;
                List<CaeMesh.MasterSlaveItem> masterSlaveItems = contactSearch.FindContactPairs(distance, angleDeg);
                _controller.ResumeExplodedViews(false);
                // Fill data
                SearchContactPair contactPair;
                List<SearchContactPair> contactPairs = new List<SearchContactPair>();
                foreach (CaeMesh.MasterSlaveItem masterSlaveItem in masterSlaveItems)
                {
                    contactPair = new SearchContactPair(masterSlaveItem.Name, adjust, distance);
                    contactPair.Type = type;
                    contactPair.SurfaceInteractionName = cbSurfaceInteraction.SelectedItem.ToString();
                    contactPair.PopululateDropDownLists(_surfaceInteractionNames);
                    contactPair.MasterSlaveItem = masterSlaveItem;                    
                    contactPairs.Add(contactPair);
                }
                // Binding
                SetDataGridViewBinding(contactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                BindingSource binding = dgvData.DataSource as BindingSource;
                if (binding != null)
                {
                    List<SearchContactPair> contactPairs = binding.DataSource as List<SearchContactPair>;
                    List<SearchContactPair> tiedContactPairs = new List<SearchContactPair>();
                    List<SearchContactPair> contactContactPairs = new List<SearchContactPair>();
                    //
                    foreach (SearchContactPair contactPair in contactPairs)
                    {
                        if (contactPair.Type == SearchContactPairType.Tie) tiedContactPairs.Add(contactPair);
                        else if (contactPair.Type == SearchContactPairType.Contact) contactContactPairs.Add(contactPair);
                        else throw new NotSupportedException();
                        //
                        //if (contactPair.SurfaceInteractionName == missing)
                        //{
                        //    MessageBox.Show("Some contact pairs have a missing surface interaction.");
                        //    break;
                        //}
                    }
                    //
                    _controller.AutoCreateTiedPairs(tiedContactPairs);
                    _controller.AutoCreateContactPairs(contactContactPairs);
                    //
                    btnCancel_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void FrmSearchContactPairs_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                //
                btnCancel_Click(null, null);
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
        // Context menu strip
        private void tsmiSwapMasterSlave_Click(object sender, EventArgs e)
        {

            if (_selectedContactPairs.Count > 0)
            {
                foreach (SearchContactPair selectedContactPair in _selectedContactPairs)
                {
                    selectedContactPair.MasterSlaveItem.SwapMasterSlave();
                    selectedContactPair.Name = selectedContactPair.MasterSlaveItem.Name;
                }
                //
                dgvData.Refresh();
                propertyGrid.Refresh();
                HighlightContactPairs();
            }
        }
        private void tsmiMergeByMasterSlave_Click(object sender, EventArgs e)
        {
            if (_selectedContactPairs.Count > 0)
            {
                HashSet<int> masterGeometryIds = new HashSet<int>();
                HashSet<int> slaveGeometryIds = new HashSet<int>();
                // Get merged geometry ids
                foreach (SearchContactPair contactPair in _selectedContactPairs)
                {
                    masterGeometryIds.UnionWith(contactPair.MasterSlaveItem.MasterGeometryIds);
                    slaveGeometryIds.UnionWith(contactPair.MasterSlaveItem.SlaveGeometryIds);
                }
                //
                BindingSource binding = dgvData.DataSource as BindingSource;
                List<SearchContactPair> contactPairs = binding.DataSource as List<SearchContactPair>;
                // Gat all partial names
                List<string> partialNames = new List<string>();
                foreach (SearchContactPair selectedContactPair in contactPairs)
                {
                    partialNames.Add(selectedContactPair.MasterSlaveItem.MasterName);
                    partialNames.Add(selectedContactPair.MasterSlaveItem.SlaveName);
                }
                // Get new master/slave names
                string masterName = CaeMesh.ContactGraph.GetNameFromItemIds(masterGeometryIds, partialNames, _controller.Model.Mesh);
                partialNames.Add(masterName);
                string slaveName = CaeMesh.ContactGraph.GetNameFromItemIds(slaveGeometryIds, partialNames, _controller.Model.Mesh);
                // Create merged item
                SearchContactPair first = _selectedContactPairs[0];
                first.MasterSlaveItem = new CaeMesh.MasterSlaveItem(masterName, slaveName, masterGeometryIds, slaveGeometryIds);
                first.Name = first.MasterSlaveItem.Name;
                // Remove merged items
                foreach (SearchContactPair selectedContactPair in _selectedContactPairs)
                {
                    if (selectedContactPair != first) contactPairs.Remove(selectedContactPair);
                }
                // Find new item row id
                int rowId = 0;
                foreach (SearchContactPair selectedContactPair in contactPairs)
                {
                    if (selectedContactPair == first) break;
                    rowId++;
                }
                //
                SetDataGridViewBinding(contactPairs);
                dgvData.ClearSelection();   // after binding the first row is selcted
                dgvData.Rows[rowId].Selected = true;
            }
        }


        // Methods                                                                                                                  
        public void PrepareFoam()
        {
            dgvData.DataSource = null;
            propertyGrid.SelectedObject = null;
            // Surface interaction
            cbSurfaceInteraction.Items.Clear();
            //
            _surfaceInteractionNames = _controller.GetSurfaceInteractionNames();
            if (_surfaceInteractionNames.Length == 0) _surfaceInteractionNames = new string[] { missing };
            foreach (string surfaceInteracionName in _surfaceInteractionNames) cbSurfaceInteraction.Items.Add(surfaceInteracionName);
            cbSurfaceInteraction.SelectedIndex = 0;
            //
            cbSurfaceInteraction.DropDownWidth = DropDownWidth(cbSurfaceInteraction);
            //
            if (_firstTime)
            {
                // Initiation is here to account for the unit system change
                tbDistance.Text = "0.01";
                tbAngle.Text = "35";
                //
                _firstTime = false;
            }
        }
        private List<SearchContactPair> GetSelectedContactPairs()
        {
            List<SearchContactPair> selectedContactPairs = new List<SearchContactPair>();
            if (dgvData.SelectedRows.Count > 0)
            {
                BindingSource binding = dgvData.DataSource as BindingSource;
                List<SearchContactPair> contactPairs = binding.DataSource as List<SearchContactPair>;
                //
                int rowNum;
                foreach (DataGridViewRow selectedRow in dgvData.SelectedRows)
                {
                    rowNum = selectedRow.Index;
                    selectedContactPairs.Add(contactPairs[rowNum]);
                }
            }
            return selectedContactPairs;
        }
        int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0, temp = 0;
            foreach (string obj in myCombo.Items)
            {
                temp = TextRenderer.MeasureText(obj.ToString(), myCombo.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            return maxWidth;
        }
        private void SetDataGridViewBinding(object data)
        {
            dgvData.DataSource = null;
            //
            BindingSource binding = new BindingSource();
            binding.DataSource = data;
            dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
            int columnWidth;
            foreach (DataGridViewColumn column in dgvData.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columnWidth = column.Width;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.Width = columnWidth;
                //
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
            }
            binding.ListChanged += Binding_ListChanged;
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
        }
        //
        private void HighlightContactPairs()
        {
            if (_selectedContactPairs.Count > 0)
            {
                HashSet<int> masterGeometryIds = new HashSet<int>();
                HashSet<int> slaveGeometryIds = new HashSet<int>();
                //
                foreach (SearchContactPair contactPair in _selectedContactPairs)
                {
                    masterGeometryIds.UnionWith(contactPair.MasterSlaveItem.MasterGeometryIds);
                    if (!contactPair.MasterSlaveItem.Unresolved)
                        slaveGeometryIds.UnionWith(contactPair.MasterSlaveItem.SlaveGeometryIds);
                }
                //
                Selection masterSelection = new Selection();
                masterSelection.SelectItem = vtkSelectItem.Surface;
                masterSelection.Add(new SelectionNodeIds(vtkSelectOperation.Add, false, masterGeometryIds.ToArray(), true));
                Selection slaveSelection = new Selection();
                slaveSelection.SelectItem = vtkSelectItem.Surface;
                slaveSelection.Add(new SelectionNodeIds(vtkSelectOperation.Add, false, slaveGeometryIds.ToArray(), true));
                //
                _controller.Selection = masterSelection;
                _controller.HighlightSelection(true, true, false);
                //
                _controller.Selection = slaveSelection;
                _controller.HighlightSelection(false, true, true);
            }
            else _controller.ClearAllSelection();
        }


    }
}
