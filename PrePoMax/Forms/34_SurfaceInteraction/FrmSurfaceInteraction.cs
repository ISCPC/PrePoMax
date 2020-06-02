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
using System.Collections;

namespace PrePoMax.Forms
{    
    public partial class FrmSurfaceInteraction : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private bool _propertyChanged;
        private string[] _surfraceInteractionNames;
        private string _surfaceInteractionToEditName;
        private SurfaceInteraction _surfaceInteraction;
        private Controller _controller;
        private TabPage[] _pages;
        
        // Properties                                                                                                               
        public SurfaceInteraction SurfaceInteraction 
        { 
            get { return _surfaceInteraction; } 
            set { _surfaceInteraction = value.DeepClone(); } 
        }


        // Constructors                                                                                                             
        public FrmSurfaceInteraction(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _surfaceInteraction = null;
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
            this.lvAddedProperties.ListViewItemSorter = new ListViewItemComparer(0);
            //
            ClearControls();
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
                string propertyName = tvProperties.SelectedNode.Text;
                //
                if (lvAddedProperties.FindItemWithText(propertyName) == null)
                {
                    ListViewItem item = new ListViewItem(propertyName);
                    if (tvProperties.SelectedNode.Tag is SurfaceInteractionProperty sip)
                    {
                        if (sip is SurfaceBehavior sb) item.Tag = new ViewSurfaceBehavior(sb.DeepClone());
                        else if (sip is Friction fr) item.Tag = new ViewFriction(fr.DeepClone());
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
            //
            _propertyChanged = true;
        }
        private void lvAddedProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAddedProperties.SelectedItems.Count == 1)
            {
                if (lvAddedProperties.SelectedItems[0].Tag is ViewSurfaceBehavior vsb)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[0]);
                    tcProperties.TabPages.Add(_pages[1]);
                    //
                    BindingSource binding = new BindingSource();
                    binding.DataSource = vsb.DataPoints;
                    dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
                    binding.ListChanged += Binding_ListChanged;

                    if (true)
                    {
                        // Unit
                        string unitPressure = _controller.Model.UnitSystem.PressureUnitAbbreviation;
                        string unitLength = _controller.Model.UnitSystem.LengthUnitAbbreviation;
                        // HeaderText
                        string headerText;
                        string pressureName = nameof(PressureOverclosureDataPoint.Pressure);
                        string overclosureName = nameof(PressureOverclosureDataPoint.Overclosure);
                        //
                        headerText = dgvData.Columns[pressureName].HeaderText;
                        if (headerText != null) dgvData.Columns[pressureName].HeaderText = headerText.Replace("?", unitPressure);
                        headerText = dgvData.Columns[overclosureName].HeaderText;
                        if (headerText != null) dgvData.Columns[overclosureName].HeaderText = headerText.Replace("?", unitLength);
                        // Alignment
                        dgvData.Columns[pressureName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        dgvData.Columns[overclosureName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                        //
                    }
                    propertyGrid.SelectedObject = vsb;
                    //
                    propertyGrid_PropertyValueChanged(null, null);
                }
                else if (lvAddedProperties.SelectedItems[0].Tag is ViewFriction vf)
                {
                    tcProperties.TabPages.Clear();
                    tcProperties.TabPages.Add(_pages[0]);
                    //
                    propertyGrid.SelectedObject = vf;
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
            if (propertyGrid.SelectedObject != null && propertyGrid.SelectedObject is ViewSurfaceBehavior vsb)
            {
                if (vsb.PressureOverclosureType == PressureOverclosureEnum.Tabular && tcProperties.TabPages.Count == 1)
                    tcProperties.TabPages.Add(_pages[1]);
                else if (vsb.PressureOverclosureType != PressureOverclosureEnum.Tabular && tcProperties.TabPages.Count == 2)
                    tcProperties.TabPages.Remove(_pages[1]);
            }
            //
            propertyGrid.Refresh();
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
                //
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
            Hide();
        }
        private void FrmSurfaceInteraction_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string surfaceInteractionToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOKAddNew.Visible = surfaceInteractionToEditName == null;
            //
            _propertyChanged = false;
            _propertyItemChanged = false;
            _surfraceInteractionNames = null;
            _surfaceInteractionToEditName = null;
            _surfaceInteraction = null;
            lvAddedProperties.Clear();
            ClearControls();
            //
            _surfraceInteractionNames = _controller.GetSurfaceInteractionNames();
            _surfaceInteractionToEditName = surfaceInteractionToEditName;
            // Initialize surface interaction properties
            tvProperties.Nodes.Find("Surface behavior", true)[0].Tag = new SurfaceBehavior();
            tvProperties.Nodes.Find("Friction", true)[0].Tag = new Friction();
            tvProperties.ExpandAll();
            //
            if (_surfaceInteractionToEditName == null)
            {
                _surfaceInteraction = null;
                tbName.Text = GetSurfaceInteractionName();
            }
            else
            {
                SurfaceInteraction = _controller.GetSurfaceInteraction(_surfaceInteractionToEditName); // to clone
                //
                tbName.Text = _surfaceInteraction.Name;
                if (_surfaceInteraction.Properties.Count > 0)
                {
                    ListViewItem item;
                    ViewSurfaceInteractionProperty view;
                    foreach (var property in _surfaceInteraction.Properties)
                    {
                        if (property is SurfaceBehavior sb) view = new ViewSurfaceBehavior(sb);
                        else if (property is Friction fr) view = new ViewFriction(fr);
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
            if ((_surfaceInteractionToEditName == null && _surfraceInteractionNames.Contains(tbName.Text)) ||           // Create
                    (tbName.Text != _surfaceInteractionToEditName && _surfraceInteractionNames.Contains(tbName.Text)))  // Edit
                throw new CaeGlobals.CaeException("The selected surface interaction name already exists.");
            //
            _surfaceInteraction = new CaeModel.SurfaceInteraction(tbName.Text);
            foreach (ListViewItem item in lvAddedProperties.Items)
            {
                _surfaceInteraction.AddProperty(((ViewSurfaceInteractionProperty)(item.Tag)).Base);
            }
            //
            if (_surfaceInteractionToEditName == null)
            {
                // Create
                _controller.AddSurfaceInteractionCommand(SurfaceInteraction);
            }
            else
            {
                // Replace
                if (_surfaceInteractionToEditName != SurfaceInteraction.Name || _propertyChanged || _propertyItemChanged)
                {
                    _controller.ReplaceSurfaceInteractionCommand(_surfaceInteractionToEditName, SurfaceInteraction);
                }
            }
        }
        private string GetSurfaceInteractionName()
        {
            return NamedClass.GetNewValueName(_surfraceInteractionNames, "SurfaceInteraction-");
        }

       
        //

    }
}
