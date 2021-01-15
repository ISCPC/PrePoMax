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
using System.Runtime.CompilerServices;
using System.Reflection;

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
        private bool _useSimpleEditor;
        
        // Properties                                                                                                               
        public Material Material { get { return _material; } set { _material = value.DeepClone(); } }
        public bool UseSimpleEditor { get { return _useSimpleEditor; } set { _useSimpleEditor = value.DeepClone(); } }


        // Constructors                                                                                                             
        public FrmMaterial(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _material = null;
            //
            propertyGrid.SetParent(this);   // for the Tab key to work
            //
            int i = 0;
            _pages = new TabPage[tcProperties.TabPages.Count];
            foreach (TabPage tabPage in tcProperties.TabPages)
            {
                tabPage.Paint += TabPage_Paint;
                _pages[i++] = tabPage;
            }
            //
            lvAddedProperties.Sorting = SortOrder.Ascending;
            //
            ClearControls();
            //
            _useSimpleEditor = false;
        }


        // Event handling
        private void TabPage_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush fillBrush = new SolidBrush(((TabPage)sender).BackColor);
            e.Graphics.FillRectangle(fillBrush, e.ClipRectangle);
            // Enable copy/paste without first selecting the cell 0,0
            if (sender == tpDataPoints)
            {
                ActiveControl = dgvData;
                dgvData[0, 0].Selected = true;
            }
        }
        private void tvProperties_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(null, null);
        }
        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;  // no beep
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tvProperties.SelectedNode != null && tvProperties.SelectedNode.Tag != null)
            {
                string propertyName = tvProperties.SelectedNode.Name;
                //
                if (lvAddedProperties.FindItemWithText(propertyName) == null)
                {
                    ListViewItem item = new ListViewItem(propertyName);
                    if (tvProperties.SelectedNode.Tag is MaterialProperty mp)
                    {
                        if (mp is Density de) item.Tag = new ViewDensity(de.DeepClone());
                        else if (mp is Elastic el) item.Tag = new ViewElastic(el.DeepClone());
                        else if (mp is Plastic pl) item.Tag = new ViewPlastic(pl.DeepClone());
                        else throw new NotSupportedException();
                    }
                    else throw new NotSupportedException();
                    //
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
                else ClearControls();
            }
            _propertyChanged = true;
        }
        private void lvAddedProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                if (lvAddedProperties.SelectedItems[0].Tag is ViewDensity || lvAddedProperties.SelectedItems[0].Tag is ViewElastic ||
                    lvAddedProperties.SelectedItems[0].Tag is ViewElasticWithDensity)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[0]);
                    propertyGrid.SelectedObject = lvAddedProperties.SelectedItems[0].Tag;
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewPlastic vp)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[0]);
                    tcProperties.TabPages.Add(_pages[1]);
                    //
                    BindingSource binding = new BindingSource();
                    binding.DataSource = vp.DataPoints;
                    dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
                    binding.ListChanged += Binding_ListChanged;
                    // Unit
                    string unitStress = _controller.Model.UnitSystem.PressureUnitAbbreviation;
                    // HeaderText
                    string headerText;
                    string stressName = nameof(MaterialDataPoint.Stress);
                    string strainName = nameof(MaterialDataPoint.Strain);
                    //
                    headerText = dgvData.Columns[stressName].HeaderText;
                    if (headerText != null) dgvData.Columns[stressName].HeaderText = headerText.Replace("?", unitStress);
                    headerText = dgvData.Columns[strainName].HeaderText;
                    if (headerText != null) dgvData.Columns[strainName].HeaderText = headerText.Replace("?", "/");
                    // Alignment
                    dgvData.Columns[stressName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dgvData.Columns[strainName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    //
                    dgvData.XColIndex = 1;
                    dgvData.StartPlotAtZero = true;
                    //
                    propertyGrid.SelectedObject = vp;
                }
                else throw new NotSupportedException();
            }
            lvAddedProperties.Select();
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            _propertyChanged = true;
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Entered value is not a valid/numeric value.", "Error", MessageBoxButtons.OK);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Add();
                //
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
                //
                
                PrepareForm(null, null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            _useSimpleEditor = false;
            dgvData.HidePlot();
            Hide();
        }
        private void FrmMaterial_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                //
                _useSimpleEditor = false;
                dgvData.HidePlot();
                Hide();
            }
        }
    

        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string materialToEditName)
        {
            this.btnOKAddNew.Visible = materialToEditName == null;
            //
            _propertyChanged = false;
            _propertyItemChanged = false;
            _materialNames = null;
            _materialToEditName = null;
            _material = null;
            lvAddedProperties.Clear();
            ClearControls();
            //
            _materialNames = _controller.GetMaterialNames();
            _materialToEditName = materialToEditName;
            // Initialize material properties
            tvProperties.Nodes.Find("Density", true)[0].Tag = new Density(0);
            tvProperties.Nodes.Find("Elastic", true)[0].Tag = new Elastic(0, 0);
            tvProperties.Nodes.Find("Plastic", true)[0].Tag = new Plastic(new double[][] { new double[] { 0, 0 } });
            tvProperties.ExpandAll();
            //
            if (_materialToEditName == null)
            {
                _material = null;
                tbName.Text = GetMaterialName();
            }
            else
            {
                Material = _controller.GetMaterial(_materialToEditName); // to clone
                //
                tbName.Text = _material.Name;
                if (_material.Properties.Count > 0)
                {
                    ListViewItem item;
                    ViewMaterialProperty view = null;
                    foreach (var property in _material.Properties)
                    {
                        if (property is Density den) view = new ViewDensity(den);
                        else if (property is Elastic el) view = new ViewElastic(el);
                        else if (property is ElasticWithDensity ewd)
                        {
                            view = new ViewElasticWithDensity(ewd);
                            _useSimpleEditor = true;
                        }
                        else if (property is Plastic pl) view = new ViewPlastic(pl);
                        else throw new NotSupportedException();
                        //
                        item = new ListViewItem(view.Name);
                        item.Tag = view;
                        lvAddedProperties.Items.Add(item);
                    }
                    //
                    lvAddedProperties.Items[0].Selected = true;
                    lvAddedProperties.Select();
                }
            }
            // Simple material editor
            int delta;
            if (_useSimpleEditor)
            {
                if (_materialToEditName == null)
                {
                    ViewMaterialProperty view = new ViewElasticWithDensity(new ElasticWithDensity(0, 0, 0));
                    ListViewItem item = new ListViewItem(view.Name);
                    item.Tag = view;
                    lvAddedProperties.Items.Add(item);
                    lvAddedProperties.Items[0].Selected = true;
                    lvAddedProperties.Select();
                }
                delta = tcProperties.Top - labAvailable.Top;
                tcProperties.Top = labAvailable.Top;
                tcProperties.Height += delta;
                this.Height -= delta;
            }
            else
            {
                delta = (tvProperties.Bottom + 5) - tcProperties.Top;
                tcProperties.Top = tvProperties.Bottom + 5;
                tcProperties.Height -= delta;
                this.Height += delta;
            }
            //
            _controller.SetSelectByToOff();
            //
            return true;
        }
        private void ClearControls()
        {
            propertyGrid.SelectedObject = null;
            dgvData.DataSource = null;
            //
            tcProperties.TabPages.Clear();
            tcProperties.TabPages.Add(_pages[0]);
        }
        public void Add()
        {
            if ((_materialToEditName == null && _materialNames.Contains(tbName.Text)) ||            // create
                    (tbName.Text != _materialToEditName && _materialNames.Contains(tbName.Text)))   // edit
                throw new CaeGlobals.CaeException("The selected material name already exists.");
            //
            _material = new CaeModel.Material(tbName.Text);
            ViewMaterialProperty property;
            foreach (ListViewItem item in lvAddedProperties.Items)
            {
                property = (ViewMaterialProperty)item.Tag;
                if (property is ViewDensity vd && vd.Value <= 0)
                    throw new CaeGlobals.CaeException("The density must be larger than 0.");
                else if (property is ViewElastic ve && ve.YoungsModulus <= 0)
                    throw new CaeGlobals.CaeException("The Young's modulus must be larger than 0.");
                else if (property is ViewElasticWithDensity ewd)
                {
                    if (ewd.YoungsModulus <= 0) throw new CaeGlobals.CaeException("The Young's modulus must be larger than 0.");
                    if (ewd.Density <= 0) throw new CaeGlobals.CaeException("The density must be larger than 0.");
                }
                //
                _material.AddProperty(property.Base);
            }
            //
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
