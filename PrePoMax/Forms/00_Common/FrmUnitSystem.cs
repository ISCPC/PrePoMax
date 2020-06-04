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
            _selectedPropertyGridItemChangedEventActive = true;
            //
            this.Height = 311 + 17 * 19;
            //
            btnOkAddNew.Visible = false;
            btnCancel.Visible = false;
            btnOK.Location = btnCancel.Location;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 101);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 73);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 119);
            this.gbProperties.Size = new System.Drawing.Size(310, 344);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 316);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 469);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 469);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 469);
            // 
            // FrmUnitSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 504);
            this.Name = "FrmUnitSystem";
            this.Text = "Edit Unit System";
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnHideOrClose()
        {
            this.DialogResult = DialogResult.Cancel;
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
            }
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewUnitSystem = (ViewUnitSystem)propertyGrid.SelectedObject;
            //
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
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            //
            if (geometryAndModelOrResults == "Geometry & Model" || geometryAndModelOrResults == "Results")
                _geometryAndModelOrResults = geometryAndModelOrResults;
            else throw new ArgumentException();
            // Add list view items
            ListViewItem item;
            lvTypes.Items.Clear();
            //
            item = new ListViewItem("Unit system: m, kg, s, °C");
            item.Tag = new ViewUnitSystem(new UnitSystem(UnitSystemType.M_KG_S_C));
            lvTypes.Items.Add(item);
            //
            item = new ListViewItem("Unit system: mm, ton, s, °C");
            item.Tag = new ViewUnitSystem(new UnitSystem(UnitSystemType.MM_TON_S_C));
            lvTypes.Items.Add(item);
            // Select the first item
            lvTypes.Items[1].Selected = true;
            //
            return true;
        }

        


        // Methods                                                                                                                  

    }
}
