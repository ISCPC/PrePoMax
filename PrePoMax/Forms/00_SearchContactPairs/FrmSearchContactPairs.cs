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


        // Constructors                                                                                                             
        public FrmSearchContactPairs(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            //
            tbDistance.Text = 0.001.ToString();
            tbAngle.Text = 35.ToString();
            //
            tbDistance.UnitConverter = new CaeGlobals.StringLengthConverter();
            tbAngle.UnitConverter = new CaeGlobals.StringAngleDegConverter();
            //
            cbProperty.Items.Add("Tie");
            cbProperty.SelectedIndex = 0;
            cbProperty.Enabled = false;
            //
            cbGroupBy.Items.Add("None");
            cbGroupBy.Items.Add("Parts");
            cbGroupBy.SelectedIndex = 1;
        }


        // Event hadlers                                                                                                            
        private void FrmSearchContactPairs_Load(object sender, EventArgs e)
        {
        }
        private void dgvData_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvData.SelectedRows.Count > 0)
                {
                    BindingSource binding = dgvData.DataSource as BindingSource;
                    List<ViewSearchContactPair> contactPairs = binding.DataSource as List<ViewSearchContactPair>;
                    int rowNum = dgvData.SelectedRows[0].Index;
                    contactPairs[rowNum].MultiView = dgvData.SelectedRows.Count > 1;
                    propertyGrid.SelectedObject = contactPairs[rowNum];
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
                //
                if (dgvData.SelectedRows.Count > 1)
                {
                    BindingSource binding = dgvData.DataSource as BindingSource;
                    List<ViewSearchContactPair> contactPairs = binding.DataSource as List<ViewSearchContactPair>;
                    //
                    int rowNum = dgvData.SelectedRows[0].Index;
                    ViewSearchContactPair source = contactPairs[rowNum];
                    //
                    for (int i = 1; i < dgvData.SelectedRows.Count; i++)
                    {
                        rowNum = dgvData.SelectedRows[i].Index;
                        contactPairs[rowNum].SetProperty(propertyName, source);
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
            CaeMesh.GroupContactPairsByEnum groupBy;
            if (cbGroupBy.SelectedIndex == 0) groupBy = CaeMesh.GroupContactPairsByEnum.None;
            else if (cbGroupBy.SelectedIndex == 1) groupBy = CaeMesh.GroupContactPairsByEnum.ByParts;
            else throw new NotSupportedException();
            double distance = tbDistance.Value;
            double angleDeg = tbAngle.Value + 90;
            bool adjust = cbAdjust.Checked;
            //
            _controller.SuppressExplodedViews();
            CaeMesh.ContactSearch contactSearch = new CaeMesh.ContactSearch(_controller.Model.Mesh, _controller.Model.Geometry);
            contactSearch.GroupContactPairsBy = groupBy;
            List<CaeMesh.MasterSlaveItem> masterSlaveItems = contactSearch.FindContactPairs(distance, angleDeg);
            _controller.ResumeExplodedViews(false);
            //
            ViewSearchContactPair contactPair;
            List<ViewSearchContactPair> contactPairs = new List<ViewSearchContactPair>();
            foreach (var item in masterSlaveItems)
            {
                contactPair = new ViewSearchContactPair(item.Name, adjust, distance);
                contactPair.Type = SearchContactPairType.Tie;
                contactPair.Tag = item;
                contactPairs.Add(contactPair);
            }
            //
            SetDataGridViewBinding(contactPairs);
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        // Methods                                                                                                                  
        private void SetDataGridViewBinding(object data)
        {
            dgvData.DataSource = null;
            //
            BindingSource binding = new BindingSource();
            binding.DataSource = data;
            dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
            if (dgvData.Columns.Count > 0)
                dgvData.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            binding.ListChanged += Binding_ListChanged;
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
        }

        
    }
}
