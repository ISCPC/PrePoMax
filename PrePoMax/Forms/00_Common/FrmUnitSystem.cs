using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmUnitSystem : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string _geometryAndModelOrResults;
        private ViewUnitSystem _viewUnitSystem;
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
        public FrmUnitSystem(Controller controller)
            : base(1.8)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewUnitSystem = null;
            //
            this.Height = 330 + 19 * 19;
            //
            btnOkAddNew.Visible = false;
            btnCancel.Visible = false;
            btnOK.Location = btnCancel.Location;
            //
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 129);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 101);
            this.lvTypes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvTypes_MouseDoubleClick);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 147);
            this.gbProperties.Size = new System.Drawing.Size(310, 438);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 410);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 591);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 591);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 591);
            // 
            // FrmUnitSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 626);
            this.Name = "FrmUnitSystem";
            this.Text = "Edit Unit System";
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
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
        }
        protected override bool OnPrepareForm(string stepName, string geometryAndModelOrResults)
        {
            propertyGrid.SelectedObject = null;
            //
            if (geometryAndModelOrResults == "Geometry & Model" || geometryAndModelOrResults == "Results")
                _geometryAndModelOrResults = geometryAndModelOrResults;
            else throw new ArgumentException();
            // Add list view items
            UnitSystemType unitSystemType;
            ListViewItem item;
            lvTypes.Items.Clear();
            //
            unitSystemType = UnitSystemType.UNIT_LESS;
            item = new ListViewItem("Unit system: " + unitSystemType.GetDescription());
            item.Tag = new ViewUnitSystem(new UnitSystem(unitSystemType));
            lvTypes.Items.Add(item);//
            unitSystemType = UnitSystemType.M_KG_S_C;
            item = new ListViewItem("Unit system: " + unitSystemType.GetDescription());
            item.Tag = new ViewUnitSystem(new UnitSystem(unitSystemType));
            lvTypes.Items.Add(item);
            //
            unitSystemType = UnitSystemType.MM_TON_S_C;
            item = new ListViewItem("Unit system: " + unitSystemType.GetDescription());
            item.Tag = new ViewUnitSystem(new UnitSystem(unitSystemType));
            lvTypes.Items.Add(item);
            //
            unitSystemType = UnitSystemType.M_TON_S_C;
            item = new ListViewItem("Unit system: " + unitSystemType.GetDescription());
            item.Tag = new ViewUnitSystem(new UnitSystem(unitSystemType));
            lvTypes.Items.Add(item);
            //
            unitSystemType = UnitSystemType.IN_LB_S_C;
            item = new ListViewItem("Unit system: " + unitSystemType.GetDescription());
            item.Tag = new ViewUnitSystem(new UnitSystem(unitSystemType));
            lvTypes.Items.Add(item);
            // Select the second item
            //lvTypes.Items[1].Selected = true;
            //
            return true;
        }


        // Event handlers
        private void lvTypes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnApply(false);
            Hide();
        }

        // Methods                                                                                                                  

    }
}
