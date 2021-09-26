using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    public partial class FrmSearchContactPairs : Form
    {
        private bool _propertyItemChanged;


        public FrmSearchContactPairs()
        {
            InitializeComponent();
        }

        private void FrmSearchContactPairs_Load(object sender, EventArgs e)
        {
            List<ViewSearchContactPair> pairs = new List<ViewSearchContactPair>();
            pairs.Add(new ViewSearchContactPair("One", false));
            pairs.Add(new ViewSearchContactPair("Two", true));
            pairs.Add(new ViewSearchContactPair("Last", true));
            //
            dgvData.DataSource = null;
            dgvData.Columns.Clear();

            DataGridViewTextBoxColumn dgvcName = new DataGridViewTextBoxColumn();
            dgvcName.HeaderText = "Name";
            dgvcName.DataPropertyName = "Name";
            dgvData.Columns.Add(dgvcName);
            //
            DataGridViewComboBoxColumn dgvcType = new DataGridViewComboBoxColumn();
            dgvcType.HeaderText = "Type";
            dgvcType.DataPropertyName = "Type";
            dgvcType.Items.Add(SearchContactPairType.Tie);
            dgvcType.Items.Add(SearchContactPairType.Contact);
            //dgvcType.DisplayIndex = 0;
            dgvData.Columns.Add(dgvcType);
            //
            DataGridViewComboBoxColumn dgvcAdjust = new DataGridViewComboBoxColumn();
            dgvcAdjust.HeaderText = "Adjust";
            dgvcAdjust.DataPropertyName = "Adjust";
            dgvcAdjust.Items.Add(SearchContactPairAdjust.No);
            dgvcAdjust.Items.Add(SearchContactPairAdjust.Yes);
            dgvData.Columns.Add(dgvcAdjust);
            //
            SetDataGridViewBinding(pairs);
            //
            ListViewItem lvi = listView1.Items.Add(new ListViewItem());


        }

        private void SetDataGridViewBinding(object data)
        {
            BindingSource binding = new BindingSource();
            binding.DataSource = data;
            dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
            binding.ListChanged += Binding_ListChanged;
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            _propertyItemChanged = true;
        }
    }
}
