using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;
using CaeGlobals;
using PrePoMax.PropertyViews;

namespace PrePoMax.Forms
{
    public partial class FrmMaterial : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private bool _propertyChanged;
        private string[] _materialNames;
        private string _materialToEditName;
        private Material _material;
        private Controller _controller;
        private TabPage[] _pages;
        
        // Properties                                                                                                               
        public Material Material { get { return _material; } set { _material = value.DeepClone(); } }


        // Constructors                                                                                                             
        public FrmMaterial(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _material = null;

            propertyGrid.SetParent(this);   // for the Tab key to work

            int i = 0;
            _pages = new TabPage[tcProperties.TabPages.Count];
            foreach (TabPage tabPage in tcProperties.TabPages)
            {
                tabPage.Paint += TabPage_Paint;
                _pages[i++] = tabPage;
            }
            tcProperties.TabPages.Clear();
            tcProperties.TabPages.Add(_pages[0]);

            lvAddedProperties.Sorting = SortOrder.Ascending;
        }


        // Event handling
        private void TabPage_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush fillBrush = new SolidBrush(((TabPage)sender).BackColor);
            e.Graphics.FillRectangle(fillBrush, e.ClipRectangle);
        }
        private void tvProperties_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(null, null);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tvProperties.SelectedNode != null && tvProperties.SelectedNode.Tag != null)
            {
                string propertyName = tvProperties.SelectedNode.Name;

                if (lvAddedProperties.FindItemWithText(propertyName) == null)
                {
                    ListViewItem item = new ListViewItem(propertyName);
                    item.Tag = tvProperties.SelectedNode.Tag;
                    lvAddedProperties.Items.Add(item);                    
                    int id = lvAddedProperties.Items.IndexOf(item);
                    lvAddedProperties.Items[id].Selected = true;
                    lvAddedProperties.Select();
                }
            }
            _propertyChanged = true;
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                lvAddedProperties.SelectedItems[0].Remove();
                if (lvAddedProperties.Items.Count > 0) lvAddedProperties.Items[0].Selected = true;
            }
            _propertyChanged = true;
        }
        private void lvAddedProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                if (lvAddedProperties.SelectedItems[0].Tag is ViewDensity || lvAddedProperties.SelectedItems[0].Tag is ViewElastic)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[0]);
                    propertyGrid.SelectedObject = lvAddedProperties.SelectedItems[0].Tag;
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewPlastic)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[1]);

                    BindingSource binding = new BindingSource();
                    binding.DataSource = (lvAddedProperties.SelectedItems[0].Tag as ViewPlastic).DataPoints;
                    dgvData.DataSource = binding; //bind datagridview to binding source - enables adding of new lines
                }
            }
            lvAddedProperties.Select();
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        private void dgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _propertyItemChanged = true;
        }
        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Property value is not valid.", "Error", MessageBoxButtons.OK);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Add();

                this.DialogResult = DialogResult.OK;       // use this value to update the model tree selected item highlight
                Hide();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
            
        }
        private void btnOKAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                Add();

                PrepareForm(null, null);
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
        private void FrmMaterial_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string materialToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOKAddNew.Visible = materialToEditName == null;

            _propertyChanged = false;
            _propertyItemChanged = false;
            _materialNames = null;
            _materialToEditName = null;
            _material = null;
            lvAddedProperties.Clear();
            propertyGrid.SelectedObject = null;

            _materialNames = _controller.GetMaterialNames();
            _materialToEditName = materialToEditName;
            
            // Initialize material properties
            tvProperties.Nodes.Find("Density", true)[0].Tag = new ViewDensity(new Density(0));
            tvProperties.Nodes.Find("Elastic", true)[0].Tag = new ViewElastic(new Elastic(0, 0));
            tvProperties.Nodes.Find("Plastic", true)[0].Tag = new ViewPlastic(new Plastic(new double[][] { new double[] { 0, 0 } }));
            tvProperties.ExpandAll();

            if (_materialToEditName == null)
            {
                _material = null;
                tbName.Text = GetMaterialName();
            }
            else
            {
                Material = _controller.GetMaterial(_materialToEditName); // to clone

                tbName.Text = _material.Name;
                if (_material.Properties.Count > 0)
                {
                    ListViewItem item;
                    IViewMaterialProperty view = null;
                    foreach (var property in _material.Properties)
                    {
                        if (property is Density) view = new ViewDensity((Density)property);
                        else if (property is Elastic) view = new ViewElastic((Elastic)property);
                        else if (property is Plastic) view = new ViewPlastic((Plastic)property);
                        else throw new NotSupportedException();

                        item = new ListViewItem(view.Name);
                        item.Tag = view;
                        lvAddedProperties.Items.Add(item);
                    }

                    lvAddedProperties.Items[0].Selected = true;
                    lvAddedProperties.Select();
                }
            }

            return true;
        }
        public void Add()
        {
            if ((_materialToEditName == null && _materialNames.Contains(tbName.Text)) ||            // create
                    (tbName.Text != _materialToEditName && _materialNames.Contains(tbName.Text)))       // edit
                throw new CaeGlobals.CaeException("The selected material name already exists.");

            _material = new CaeModel.Material(tbName.Text);
            foreach (ListViewItem item in lvAddedProperties.Items)
            {
                _material.AddProperty(((IViewMaterialProperty)(item.Tag)).Base);
            }

            if (_materialToEditName == null)
            {
                // Create
                _controller.AddMaterialCommand(Material);
            }
            else
            {
                // Replace
                if (_materialToEditName != Material.Name || _propertyChanged || _propertyItemChanged)
                {
                    _controller.ReplaceMaterialCommand(_materialToEditName, Material);
                }
            }
        }
        private string GetMaterialName()
        {
            return NamedClass.GetNewValueName(_materialNames, "Material-");
        }

       
    }
}
