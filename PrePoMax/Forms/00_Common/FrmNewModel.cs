using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;
using CaeModel;


namespace PrePoMax.Forms
{
    class FrmNewModel : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string _geometryAndModelOrResults;
        private ViewUnitSystem _viewUnitSystem;
        private GroupBox gbModelSpace;
        private RadioButton rb2D;
        private RadioButton rb3D;
        private Controller _controller;
        private int _lvSelectedIndex;
        private bool _focusOK;


        // Properties                                                                                                               
        // SetLoad and GetLoad to distinguish from Load event
        public UnitSystem UnitSystem
        {
            get { return _viewUnitSystem != null ? _viewUnitSystem.GetBase() : null; }
            set
            {
                var clone = value.DeepClone();
                if (clone is UnitSystem us) _viewUnitSystem = new ViewUnitSystem(us);
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmNewModel()
            : this(null)
        {
        }
        public FrmNewModel(Controller controller)
            : base(1.4)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewUnitSystem = null;
            //
            btnOkAddNew.Visible = false;
            btnCancel.Visible = false;
            btnOK.Location = btnCancel.Location;
            //
            MethodInfo m = typeof(RadioButton).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            if (m != null)
            {
                m.Invoke(rb2D, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
                m.Invoke(rb3D, new object[] { ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true });
            }
            rb2D.MouseDoubleClick += rbModelSpace_DoubleClick;
            rb3D.MouseDoubleClick += rbModelSpace_DoubleClick;
        }
        private void InitializeComponent()
        {
            this.gbModelSpace = new System.Windows.Forms.GroupBox();
            this.rb2D = new System.Windows.Forms.RadioButton();
            this.rb3D = new System.Windows.Forms.RadioButton();
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.gbModelSpace.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Location = new System.Drawing.Point(12, 90);
            this.gbType.Size = new System.Drawing.Size(310, 128);
            this.gbType.TabIndex = 101;
            this.gbType.Text = "Unit System Type";
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 100);
            this.lvTypes.TabIndex = 3;
            this.lvTypes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvTypes_MouseDoubleClick);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 224);
            this.gbProperties.Size = new System.Drawing.Size(310, 296);
            this.gbProperties.TabIndex = 102;
            this.gbProperties.Text = "Units";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 268);
            this.propertyGrid.TabIndex = 10;
            this.propertyGrid.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 526);
            this.btnOK.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 526);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 526);
            // 
            // gbModelSpace
            // 
            this.gbModelSpace.Controls.Add(this.rb2D);
            this.gbModelSpace.Controls.Add(this.rb3D);
            this.gbModelSpace.Location = new System.Drawing.Point(12, 12);
            this.gbModelSpace.Name = "gbModelSpace";
            this.gbModelSpace.Size = new System.Drawing.Size(310, 72);
            this.gbModelSpace.TabIndex = 100;
            this.gbModelSpace.TabStop = false;
            this.gbModelSpace.Text = "Model Sapce";
            // 
            // rb2D
            // 
            this.rb2D.AutoSize = true;
            this.rb2D.Location = new System.Drawing.Point(6, 47);
            this.rb2D.Name = "rb2D";
            this.rb2D.Size = new System.Drawing.Size(39, 19);
            this.rb2D.TabIndex = 2;
            this.rb2D.Text = "2D";
            this.rb2D.UseVisualStyleBackColor = true;
            // 
            // rb3D
            // 
            this.rb3D.AutoSize = true;
            this.rb3D.Checked = true;
            this.rb3D.Location = new System.Drawing.Point(6, 22);
            this.rb3D.Name = "rb3D";
            this.rb3D.Size = new System.Drawing.Size(39, 19);
            this.rb3D.TabIndex = 1;
            this.rb3D.TabStop = true;
            this.rb3D.Text = "3D";
            this.rb3D.UseVisualStyleBackColor = true;
            // 
            // FrmNewModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 561);
            this.Controls.Add(this.gbModelSpace);
            this.Name = "FrmNewModel";
            this.Text = "Properties";
            this.VisibleChanged += new System.EventHandler(this.FrmNewModel_VisibleChanged);
            this.Controls.SetChildIndex(this.gbModelSpace, 0);
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.gbType, 0);
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.gbModelSpace.ResumeLayout(false);
            this.gbModelSpace.PerformLayout();
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        private void FrmNewModel_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                if (_lvSelectedIndex != -1)
                {
                    lvTypes.Items[_lvSelectedIndex].Focused = true;
                    lvTypes.Items[_lvSelectedIndex].Selected = true;
                    _lvSelectedIndex = -1;
                }
                if (_focusOK)
                {
                    btnOK.Select();
                    _focusOK = false;
                }
            }
        }
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.Enabled && lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewUnitSystem vus) _viewUnitSystem = vus;
                else throw new NotImplementedException();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
                try
                {
                    // Get start point grid item
                    GridItem gi = propertyGrid.EnumerateAllItems().First((item) =>
                                  item.PropertyDescriptor != null &&
                                  item.PropertyDescriptor.DisplayName.TrimStart(new char[] { '\t' }) == "Length unit");
                    // Select it
                    gi.Select();
                }
                catch { }
                //
                propertyGrid.Refresh();
                //
                lvTypes.Select();
                
            }
        }
        protected override void OnListViewTypeMouseUp()
        {
            // Prevent calling propertyGrid.Select();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            ModelSpaceEnum modelSpace = ModelSpaceEnum.Undefined;
            if (rb2D.Checked) modelSpace = ModelSpaceEnum.Two_D;
            else if (rb3D.Checked) modelSpace = ModelSpaceEnum.Three_D;
            else throw new NotSupportedException();
            //
            _viewUnitSystem = (ViewUnitSystem)propertyGrid.SelectedObject;
            if (_viewUnitSystem == null) throw new CaeException("No unit system selected.");
            UnitSystem = _viewUnitSystem.GetBase();
            // Replace
            if (_geometryAndModelOrResults == "New Model")
            {
                if (modelSpace == ModelSpaceEnum.Two_D)
                {
                    if ((_controller.Model.Geometry != null && !_controller.Model.Geometry.BoundingBox.Is2D())
                        || (_controller.Model.Mesh != null && !_controller.Model.Mesh.BoundingBox.Is2D()))
                        throw new CaeException("Use of the 2D model space is not possible. The geometry or the mesh " +
                                               "do not contain 2D geometry in x-y plane.");
                }
                _controller.SetNewModelPropertiesCommand(modelSpace, UnitSystem.UnitSystemType);
            }
            else if (_geometryAndModelOrResults == "Results")
                _controller.SetResultsUnitSystem(UnitSystem.UnitSystemType);
            else throw new NotSupportedException();
            //
            Hide();
        }
        protected override void OnHideOrClose()
        {
            // This prevents hiding of the form by closing it using X
        }
        protected override bool OnPrepareForm(string stepName, string geometryAndModelOrResults)
        {
            propertyGrid.SelectedObject = null;
            //
            if (geometryAndModelOrResults == "New Model" || geometryAndModelOrResults == "Results")
                _geometryAndModelOrResults = geometryAndModelOrResults;
            else throw new ArgumentException();
            //
            gbModelSpace.Enabled = geometryAndModelOrResults == "New Model";
            Text = "Properties";
            // Add list view items
            UnitSystemType defaultUnitSystemType = _controller.Settings.General.UnitSystemType;
            UnitSystemType[] unitSystemTypes = new UnitSystemType[] { UnitSystemType.UNIT_LESS,
                                                                      UnitSystemType.M_KG_S_C,
                                                                      UnitSystemType.MM_TON_S_C,
                                                                      UnitSystemType.M_TON_S_C,
                                                                      UnitSystemType.IN_LB_S_F};
            //
            ListViewItem item;
            lvTypes.Items.Clear();
            //
            _lvSelectedIndex = -1;
            foreach (var unitSystemType in unitSystemTypes)
            {
                item = new ListViewItem(unitSystemType.GetDescription());
                item.Tag = new ViewUnitSystem(new UnitSystem(unitSystemType));
                lvTypes.Items.Add(item);
                if (unitSystemType == defaultUnitSystemType) _lvSelectedIndex = lvTypes.Items.Count - 1;
            }
            //
            _focusOK = true;
            //
            return true;
        }


        // Event handlers
        private void lvTypes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 2) OnApply(false);
        }
        private void rbModelSpace_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 2) OnApply(false);
        }

        


        // Methods                                                                                                                  

    }
}
