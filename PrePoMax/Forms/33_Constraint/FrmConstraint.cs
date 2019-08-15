using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmConstraint : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _constraintNames;
        private string _constraintToEditName;
        private ViewConstraint _viewConstraint;
        private ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem tsmiSwapMasterSlave;
        private Controller _controller;


        // Properties                                                                                                               
        public Constraint Constraint
        {
            get { return _viewConstraint.GetBase(); }
            set
            {
                var clone = value.DeepClone();
                if (value is RigidBody) _viewConstraint = new ViewRigidBody((RigidBody)clone);
                else if (value is Tie) _viewConstraint = new ViewTie((Tie)clone);
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmConstraint(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _viewConstraint = null;

            _selectedPropertyGridItemChangedEventActive = true;
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSwapMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.cmsPropertyGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.ContextMenuStrip = this.cmsPropertyGrid;
            // 
            // cmsPropertyGrid
            // 
            this.cmsPropertyGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSwapMasterSlave});
            this.cmsPropertyGrid.Name = "cmsPropertyGrid";
            this.cmsPropertyGrid.Size = new System.Drawing.Size(173, 26);
            // 
            // tsmiSwapMasterSlave
            // 
            this.tsmiSwapMasterSlave.Name = "tsmiSwapMasterSlave";
            this.tsmiSwapMasterSlave.Size = new System.Drawing.Size(172, 22);
            this.tsmiSwapMasterSlave.Text = "Swap master/slave";
            this.tsmiSwapMasterSlave.Click += new System.EventHandler(this.tsmiSwapMasterSlave_Click);
            // 
            // FrmConstraint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmConstraint";
            this.Text = "Edit Constraint";
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.gbType, 0);
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.cmsPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.Enabled && lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                propertyGrid.SelectedObject = lvTypes.SelectedItems[0].Tag;
                propertyGrid.Select();

                if (propertyGrid.SelectedObject is ViewRigidBody vrb)
                    propertyGrid.ContextMenuStrip = null;
                else if (propertyGrid.SelectedObject is ViewTie vt)
                    propertyGrid.ContextMenuStrip = cmsPropertyGrid;
            }
        }
        protected override void OnPropertyGridSelectedGridItemChanged()
        {
            object value = propertyGrid.SelectedGridItem.Value;
            if (value != null)
            {
                string valueString = value.ToString();
                object[] objects = null;

                if (propertyGrid.SelectedObject == null) return;
                else if (propertyGrid.SelectedObject is ViewError) return;
                else if (propertyGrid.SelectedObject is ViewRigidBody vrb)
                {
                    if (valueString == vrb.NodeSetName) objects = new object[] { vrb.NodeSetName };
                    else if (valueString == vrb.ReferencePointName) objects = new object[] { vrb.ReferencePointName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is ViewTie vt)
                {
                    if (valueString == vt.SlaveSurfaceName) objects = new object[] { vt.SlaveSurfaceName };
                    else if (valueString == vt.MasterSurfaceName) objects = new object[] { vt.MasterSurfaceName };
                    else objects = null;
                }
                else throw new NotImplementedException();

                _controller.Highlight3DObjects(objects);
            }
        }
        protected override void Apply()
        {
            if (propertyGrid.SelectedObject == null) throw new CaeException("No item selected.");

            _viewConstraint = (ViewConstraint)propertyGrid.SelectedObject;

            if ((_constraintToEditName == null && _constraintNames.Contains(_viewConstraint.Name)) ||                       // named to existing name
                (_viewConstraint.Name != _constraintToEditName && _constraintNames.Contains(_viewConstraint.Name)))         // renamed to existing name
                throw new CaeException("The selected constraint name already exists.");

            if (_viewConstraint is ViewTie vt)
            {
                // Check for errors with constructor
                var tie = new Tie(vt.Name, vt.SlaveSurfaceName, vt.MasterSurfaceName);
            }

            if (_constraintToEditName == null)
            {
                // Create
                _controller.AddConstraintCommand(Constraint);
            }
            else
            {
                // Replace
                if (_propertyItemChanged) _controller.ReplaceConstraintCommand(_constraintToEditName, Constraint);
            }
        }
        protected override bool OnPrepareForm(string stepName, string constraintToEditName)
        {
            _selectedPropertyGridItemChangedEventActive = false;                             // to prevent clear of the selection

            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = constraintToEditName == null;

            _propertyItemChanged = false;
            _constraintNames = null;
            _constraintToEditName = null;
            _viewConstraint = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;

            _constraintNames = _controller.GetConstraintNames();
            _constraintToEditName = constraintToEditName;

            string[] referencePointNames = _controller.GetReferencePointNames();
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] surfaceNames = _controller.GetSurfaceNames();
            if (referencePointNames == null) referencePointNames = new string[0];
            if (nodeSetNames == null) nodeSetNames = new string[0];
            if (surfaceNames == null) surfaceNames = new string[0];

            if (_constraintNames == null)
                throw new CaeException("The constraint names must be defined first.");

            PopulateListOfConstraints(referencePointNames, nodeSetNames, surfaceNames);

            // add constraints                                                                                            
            if (_constraintToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewConstraint = null;

                if (lvTypes.Items.Count == 1) lvTypes.Items[0].Selected = true;
            }
            else
            {
                Constraint = _controller.GetConstraint(_constraintToEditName);    // to clone

                // select the appropriate constraint in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewConstraint is ViewRigidBody) lvTypes.Items[0].Selected = true;
                else if (_viewConstraint is ViewTie) lvTypes.Items[1].Selected = true;
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;

                if (_viewConstraint is ViewRigidBody vrb)
                {
                    CheckMissingValueRef(ref referencePointNames, vrb.ReferencePointName, s => { vrb.ReferencePointName = s; });
                    if (vrb.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vrb.NodeSetName, s => { vrb.NodeSetName = s; });
                    else if (vrb.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vrb.SurfaceName, s => { vrb.SurfaceName = s; });
                    else throw new NotSupportedException();

                    vrb.PopululateDropDownLists(referencePointNames, nodeSetNames, surfaceNames);

                    propertyGrid.ContextMenuStrip = null;
                }
                else if (_viewConstraint is ViewTie vt)
                {
                    CheckMissingValueRef(ref surfaceNames, vt.SlaveSurfaceName, s => { vt.SlaveSurfaceName = s; });
                    CheckMissingValueRef(ref surfaceNames, vt.MasterSurfaceName, s => { vt.MasterSurfaceName = s; });

                    vt.PopululateDropDownLists(surfaceNames);

                    propertyGrid.ContextMenuStrip = cmsPropertyGrid;
                }
                else throw new NotSupportedException();

                propertyGrid.SelectedObject = _viewConstraint;
                propertyGrid.Select();
            }
            _selectedPropertyGridItemChangedEventActive = true;

            return true;
        }
        

        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string constraintToEditName)
        {
            return OnPrepareForm(stepName, constraintToEditName);
        }
        private void PopulateListOfConstraints(string[] referencePointNames, string[] nodeSetNames, string[] surfaceNames)
        {
            // populate list view                                                                               
            ListViewItem item;
            ViewRigidBody vrb = null;
            ViewTie vt = null;

            // rigid body
            item = new ListViewItem("Rigid body");
            if (referencePointNames.Length > 0)
            {
                if (nodeSetNames.Length > 0)
                    vrb = new ViewRigidBody(new RigidBody(GetConstraintName("Rigid-body-"), referencePointNames[0], nodeSetNames[0], RegionTypeEnum.NodeSetName));
                else if (surfaceNames.Length > 0)
                    vrb = new ViewRigidBody(new RigidBody(GetConstraintName("Rigid-body-"), referencePointNames[0], surfaceNames[0], RegionTypeEnum.SurfaceName));

                if (vrb != null)
                {
                    vrb.PopululateDropDownLists(referencePointNames, nodeSetNames, surfaceNames);
                    item.Tag = vrb;
                }
                else item.Tag = new ViewError("There is no node set/surface defined for the rigid body constraint definition.");
            }
            else item.Tag = new ViewError("There is no reference point defined for the rigid body constraint definition.");
            lvTypes.Items.Add(item);

            // Tie
            item = new ListViewItem("Tie");
            if (surfaceNames.Length >= 2)
            {
                vt = new ViewTie(new Tie(GetConstraintName("Tie-"), surfaceNames[0], surfaceNames[1]));
                vt.PopululateDropDownLists(surfaceNames);
                item.Tag = vt;
            }
            else item.Tag = new ViewError("At least two surfaces are needed for the tie constraint definition.");
            lvTypes.Items.Add(item);
        }
        private string GetConstraintName(string namePrefix)
        {
            return NamedClass.GetNewValueName(_constraintNames, namePrefix);
        }

        private void tsmiSwapMasterSlave_Click(object sender, EventArgs e)
        {
            if (propertyGrid.SelectedObject is ViewTie vt)
            {
                string tmp = vt.SlaveSurfaceName;
                vt.SlaveSurfaceName = vt.MasterSurfaceName;
                vt.MasterSurfaceName = tmp;
                propertyGrid.Refresh();

                OnPropertyGridSelectedGridItemChanged();    // highlight
                _propertyItemChanged = true;
            }
        }
    }
}
