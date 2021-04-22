﻿using System;
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
        private void cbTemperatureDependent_CheckedChanged(object sender, EventArgs e)
        {
            HideShowTemperature();
            _propertyItemChanged = true;
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
                        else if (mp is ThermalExpansion te) item.Tag = new ViewExpansion(te.DeepClone());
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
            _propertyItemChanged = true;
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                lvAddedProperties.SelectedItems[0].Remove();
                if (lvAddedProperties.Items.Count > 0) lvAddedProperties.Items[0].Selected = true;
                else ClearControls();
            }
            _propertyItemChanged = true;
        }
        private void lvAddedProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                // Clear
                dgvData.DataSource = null;      
                dgvData.Columns.Clear();
                //
                if (lvAddedProperties.SelectedItems[0].Tag is ViewDensity vd)
                {
                    tcProperties.TabPages.Clear();
                    // Properites
                    tcProperties.TabPages.Add(_pages[0]);
                    // Data points
                    tcProperties.TabPages.Add(_pages[1]);
                    //
                    BindingSource binding = new BindingSource();
                    binding.DataSource = vd.DataPoints;
                    dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
                    binding.ListChanged += Binding_ListChanged;
                    // Unit
                    string unitDensity = _controller.Model.UnitSystem.DensityUnitAbbreviation;
                    string unitTemperature = _controller.Model.UnitSystem.TemperatureUnitAbbreviation;
                    // HeaderText
                    string headerText;
                    string densityName = nameof(DensityDataPoint.Density);
                    string temperatureName = nameof(TempDataPoint.Temperature);
                    //
                    headerText = dgvData.Columns[densityName].HeaderText;
                    if (headerText != null) dgvData.Columns[densityName].HeaderText = headerText.Replace("?", unitDensity);
                    headerText = dgvData.Columns[temperatureName].HeaderText;
                    if (headerText != null) dgvData.Columns[temperatureName].HeaderText = headerText.Replace("?", unitTemperature);
                    // Alignment
                    dgvData.Columns[densityName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dgvData.Columns[temperatureName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    //
                    dgvData.XColIndex = 1;
                    dgvData.StartPlotAtZero = true;
                    //
                    propertyGrid.SelectedObject = vd;
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewElastic ve)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[1]);
                    tcProperties.TabPages.Add(_pages[0]);
                    //
                    BindingSource binding = new BindingSource();
                    binding.DataSource = ve.DataPoints;
                    dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
                    binding.ListChanged += Binding_ListChanged;
                    // Unit
                    string unitStress = _controller.Model.UnitSystem.PressureUnitAbbreviation;
                    string unitTemperature = _controller.Model.UnitSystem.TemperatureUnitAbbreviation;
                    // HeaderText
                    string headerText;
                    string youngsName = nameof(ElasticDataPoint.YoungsModulus);
                    string poissonsName = nameof(ElasticDataPoint.PoissonsRatio);
                    string temperatureName = nameof(TempDataPoint.Temperature);
                    //
                    headerText = dgvData.Columns[youngsName].HeaderText;
                    if (headerText != null) dgvData.Columns[youngsName].HeaderText = headerText.Replace("?", unitStress);
                    headerText = dgvData.Columns[poissonsName].HeaderText;
                    if (headerText != null) dgvData.Columns[poissonsName].HeaderText = headerText.Replace("?", "/");
                    headerText = dgvData.Columns[temperatureName].HeaderText;
                    if (headerText != null) dgvData.Columns[temperatureName].HeaderText = headerText.Replace("?", unitTemperature);
                    // Alignment
                    dgvData.Columns[youngsName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dgvData.Columns[poissonsName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dgvData.Columns[temperatureName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    //
                    dgvData.XColIndex = 1;
                    dgvData.StartPlotAtZero = true;
                    //
                    propertyGrid.SelectedObject = ve;
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewElasticWithDensity)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[0]);
                    propertyGrid.SelectedObject = lvAddedProperties.SelectedItems[0].Tag;
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewPlastic vp)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[1]);
                    tcProperties.TabPages.Add(_pages[0]);
                    //
                    BindingSource binding = new BindingSource();
                    binding.DataSource = vp.DataPoints;
                    dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
                    binding.ListChanged += Binding_ListChanged;
                    // Unit
                    string unitStress = _controller.Model.UnitSystem.PressureUnitAbbreviation;
                    string unitTemperature = _controller.Model.UnitSystem.TemperatureUnitAbbreviation;
                    // HeaderText
                    string headerText;
                    string stressName = nameof(PlasticDataPoint.Stress);
                    string strainName = nameof(PlasticDataPoint.Strain);
                    string temperatureName = nameof(TempDataPoint.Temperature);
                    //
                    headerText = dgvData.Columns[stressName].HeaderText;
                    if (headerText != null) dgvData.Columns[stressName].HeaderText = headerText.Replace("?", unitStress);
                    headerText = dgvData.Columns[strainName].HeaderText;
                    if (headerText != null) dgvData.Columns[strainName].HeaderText = headerText.Replace("?", "/");
                    headerText = dgvData.Columns[temperatureName].HeaderText;
                    if (headerText != null) dgvData.Columns[temperatureName].HeaderText = headerText.Replace("?", unitTemperature);
                    // Alignment
                    dgvData.Columns[stressName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dgvData.Columns[strainName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dgvData.Columns[temperatureName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    //
                    dgvData.XColIndex = 1;
                    dgvData.StartPlotAtZero = true;
                    //
                    propertyGrid.SelectedObject = vp;
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewExpansion vex)
                {
                    tcProperties.TabPages.Clear();
                    // Properites
                    tcProperties.TabPages.Add(_pages[0]);
                    // Data points
                    tcProperties.TabPages.Add(_pages[1]);
                    //
                    BindingSource binding = new BindingSource();
                    binding.DataSource = vex.DataPoints;
                    dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
                    binding.ListChanged += Binding_ListChanged;
                    // Unit
                    string unitThermalExpansion = _controller.Model.UnitSystem.ThermalExpansionUnitAbbreviation;
                    string unitTemperature = _controller.Model.UnitSystem.TemperatureUnitAbbreviation;
                    // HeaderText
                    string headerText;
                    string thermalExpansionName = nameof(ExpansionDataPoint.ThermalExpansion);
                    string temperatureName = nameof(TempDataPoint.Temperature);
                    //
                    headerText = dgvData.Columns[thermalExpansionName].HeaderText;
                    if (headerText != null)
                        dgvData.Columns[thermalExpansionName].HeaderText = headerText.Replace("?", unitThermalExpansion);
                    headerText = dgvData.Columns[temperatureName].HeaderText;
                    if (headerText != null)
                        dgvData.Columns[temperatureName].HeaderText = headerText.Replace("?", unitTemperature);
                    // Alignment
                    dgvData.Columns[thermalExpansionName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dgvData.Columns[temperatureName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    //
                    dgvData.XColIndex = 1;
                    dgvData.StartPlotAtZero = true;
                    //
                    propertyGrid.SelectedObject = vex;
                }
                else throw new NotSupportedException();
                //
                HideShowTemperature();
            }
            lvAddedProperties.Select();
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            _propertyItemChanged = true;
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
            tvProperties.Nodes.Find("Density", true)[0].Tag = new Density(new double[][] { new double[] { 0, 0 } });
            tvProperties.Nodes.Find("Elastic", true)[0].Tag = new Elastic(new double[][] { new double[] { 0, 0, 0 } });
            tvProperties.Nodes.Find("Plastic", true)[0].Tag = new Plastic(new double[][] { new double[] { 0, 0, 0 } });
            tvProperties.Nodes.Find("Expansion", true)[0].Tag = new ThermalExpansion(new double[][] { new double[] { 0, 0 } });
            tvProperties.ExpandAll();
            //
            if (_materialToEditName == null)
            {
                _material = null;
                tbName.Text = GetMaterialName();
                tbDescription.Text = "";
                cbTemperatureDependent.Checked = false;
            }
            else
            {
                Material = _controller.GetMaterial(_materialToEditName); // to clone
                //
                tbName.Text = _material.Name;
                tbDescription.Text = _material.Description;
                cbTemperatureDependent.Checked = _material.TemperatureDependent;
                //
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
                        else if (property is ThermalExpansion te) view = new ViewExpansion(te);
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
            _material.Description = tbDescription.Text;
            _material.TemperatureDependent = cbTemperatureDependent.Checked;
            //
            ViewMaterialProperty property;
            foreach (ListViewItem item in lvAddedProperties.Items)
            {
                property = (ViewMaterialProperty)item.Tag;
                if (property is ViewDensity vd)
                {
                    for (int i = 0; i < vd.DataPoints.Count; i++)
                    {
                        if (vd.DataPoints[i].Density <= 0) throw new CaeException("The density must be larger than 0.");
                    }
                }
                else if (property is ViewElastic ve && ve.YoungsModulus <= 0)
                {
                    throw new CaeGlobals.CaeException("The Young's modulus must be larger than 0.");
                }
                else if (property is ViewElasticWithDensity ewd)
                {
                    if (ewd.YoungsModulus <= 0) throw new CaeGlobals.CaeException("The Young's modulus must be larger than 0.");
                    if (ewd.Density <= 0) throw new CaeGlobals.CaeException("The density must be larger than 0.");
                }
                else if (property is ViewExpansion vex)
                {
                    for (int i = 0; i < vex.DataPoints.Count; i++)
                    {
                        if (vex.DataPoints[i].ThermalExpansion <= 0)
                            throw new CaeException("The thermal expansion coefficient must be larger than 0.");
                    }
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
                if (_materialToEditName != Material.Name || _propertyItemChanged)
                {
                    _controller.ReplaceMaterialCommand(_materialToEditName, Material);
                }
            }
        }
        private string GetMaterialName()
        {
            return NamedClass.GetNewValueName(_materialNames, "Material-");
        }
        private void HideShowTemperature()
        {
            if (lvAddedProperties.SelectedItems.Count > 0 &&
                (lvAddedProperties.SelectedItems[0].Tag is ViewDensity ||
                 lvAddedProperties.SelectedItems[0].Tag is ViewElastic ||
                 lvAddedProperties.SelectedItems[0].Tag is ViewExpansion))
            {
                tcProperties.TabPages.Clear();
                // Data points
                if (cbTemperatureDependent.Checked) tcProperties.TabPages.Add(_pages[1]);
                // Properites
                if (!cbTemperatureDependent.Checked) tcProperties.TabPages.Add(_pages[0]);
            }
            //
            string temperatureName = nameof(TempDataPoint.Temperature);
            DataGridViewColumn col = dgvData.Columns[temperatureName];
            if (col != null) col.Visible = cbTemperatureDependent.Checked;
        }

        

       
    }
}
