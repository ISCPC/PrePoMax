using System;
using System.Collections.Generic;
using System.Linq;
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
        private GroupBox gbModelingSpace;
        private RadioButton rb2D;
        private RadioButton rb3D;
        private Controller _controller;


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
        }
        private void InitializeComponent()
        {
            this.gbModelingSpace = new System.Windows.Forms.GroupBox();
            this.rb2D = new System.Windows.Forms.RadioButton();
            this.rb3D = new System.Windows.Forms.RadioButton();
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.gbModelingSpace.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Location = new System.Drawing.Point(12, 90);
            this.gbType.Size = new System.Drawing.Size(310, 128);
            this.gbType.Text = "Unit System Type";
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 100);
            this.lvTypes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvTypes_MouseDoubleClick);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 224);
            this.gbProperties.Size = new System.Drawing.Size(310, 296);
            this.gbProperties.Text = "Units";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 268);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 526);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 526);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 526);
            // 
            // gbModelingSpace
            // 
            this.gbModelingSpace.Controls.Add(this.rb2D);
            this.gbModelingSpace.Controls.Add(this.rb3D);
            this.gbModelingSpace.Location = new System.Drawing.Point(12, 12);
            this.gbModelingSpace.Name = "gbModelingSpace";
            this.gbModelingSpace.Size = new System.Drawing.Size(310, 72);
            this.gbModelingSpace.TabIndex = 16;
            this.gbModelingSpace.TabStop = false;
            this.gbModelingSpace.Text = "Modeling Sapce";
            // 
            // rb2D
            // 
            this.rb2D.AutoSize = true;
            this.rb2D.Location = new System.Drawing.Point(6, 47);
            this.rb2D.Name = "rb2D";
            this.rb2D.Size = new System.Drawing.Size(39, 19);
            this.rb2D.TabIndex = 1;
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
            this.rb3D.TabIndex = 0;
            this.rb3D.TabStop = true;
            this.rb3D.Text = "3D";
            this.rb3D.UseVisualStyleBackColor = true;
            // 
            // FrmNewModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 561);
            this.Controls.Add(this.gbModelingSpace);
            this.Name = "FrmNewModel";
            this.Text = "Properties";
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.gbType, 0);
            this.Controls.SetChildIndex(this.gbModelingSpace, 0);
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.gbModelingSpace.ResumeLayout(false);
            this.gbModelingSpace.PerformLayout();
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.Enabled && lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewUnitSystem vus) _viewUnitSystem = vus;
                else throw new NotImplementedException();
                //
                propertyGrid.SelectedObject = itemTag;
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
            //base.OnListViewTypeMouseUp();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewUnitSystem = (ViewUnitSystem)propertyGrid.SelectedObject;
            //
            if (_viewUnitSystem == null) throw new CaeException("No unit system selected.");
            UnitSystem = _viewUnitSystem.GetBase();
            // Replace
            if (_geometryAndModelOrResults == "Geometry & Model")
                _controller.SetModelUnitSystemCommand(UnitSystem.UnitSystemType);
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
            if (geometryAndModelOrResults == "Geometry & Model" || geometryAndModelOrResults == "Results")
                _geometryAndModelOrResults = geometryAndModelOrResults;
            else throw new ArgumentException();
            //
            gbModelingSpace.Enabled = geometryAndModelOrResults == "Geometry & Model";
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
            foreach (var unitSystemType in unitSystemTypes)
            {
                item = new ListViewItem(unitSystemType.GetDescription());
                item.Tag = new ViewUnitSystem(new UnitSystem(unitSystemType));
                lvTypes.Items.Add(item);
                if (unitSystemType == defaultUnitSystemType) item.Selected = true;
            }
            //
            return true;
        }


        // Event handlers
        private void lvTypes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnApply(false);
        }


        // Methods                                                                                                                  
        
    }
}
