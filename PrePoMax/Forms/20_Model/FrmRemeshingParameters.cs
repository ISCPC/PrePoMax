using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmRemeshingParameters : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private ViewRemeshingParameters _viewRemeshingParameters;
        private Controller _controller;


        // Properties                                                                                                               
        public RemeshingParameters RemeshingParameters
        {
            get { return _viewRemeshingParameters.GetBase(); }
            set { _viewRemeshingParameters = new ViewRemeshingParameters(value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmRemeshingParameters(Controller controller) 
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewRemeshingParameters = null;
            //
            SelectionClear = _controller.Selection.Clear;
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmRemeshingParameters
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmRemeshingParameters";
            this.Text = "Remeshing Parameters";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewRemeshingParameters.RegionType))
            {
                ShowHideSelectionForm();
                //
                HighlightElementSet();
            }
            else if (property == nameof(_viewRemeshingParameters.ElementSetName))
            {
                HighlightElementSet();
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewRemeshingParameters = (ViewRemeshingParameters)propertyGrid.SelectedObject;
            //
            if (_viewRemeshingParameters == null) throw new CaeException("No remeshing was selected.");
            //
            if (RemeshingParameters.RegionType == RegionTypeEnum.Selection &&
                (RemeshingParameters.CreationIds == null || RemeshingParameters.CreationIds.Length == 0))
                throw new CaeException("The element set for remeshing must contain at least one item.");
            //
            _controller.RemeshElementsCommand(RemeshingParameters);
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string itemToEditName)
        {
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;
            this.btnOkAddNew.Visible = false;
            //
            _propertyItemChanged = false;
            _viewRemeshingParameters = null;
            propertyGrid.SelectedObject = null;
            //
            string[] elementSetNames = _controller.GetUserElementSetNames();
            // Create new
            RemeshingParameters = new RemeshingParameters("", RegionTypeEnum.Selection);
            _controller.Selection.Clear();                
            //
            _viewRemeshingParameters.PopululateDropDownLists(elementSetNames);
            //
            propertyGrid.SelectedObject = _viewRemeshingParameters;
            propertyGrid.Focus();
            // Get start point grid item and select it
            GridItem gi = propertyGrid.EnumerateAllItems().First((item) =>
                          item.PropertyDescriptor != null &&
                          item.PropertyDescriptor.Name == nameof(_viewRemeshingParameters.RegionType));
            gi.Select();
            //
            ShowHideSelectionForm();
            //
            HighlightElementSet();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void HighlightElementSet()
        {
            try
            {
                _controller.ClearSelectionHistory();
                //
                if (_viewRemeshingParameters == null) { }
                else 
                {
                    if (RemeshingParameters.RegionType == RegionTypeEnum.ElementSetName)
                    {
                        _controller.Highlight3DObjects(new object[] { RemeshingParameters.RegionName });
                    }
                    else if (RemeshingParameters.RegionType == RegionTypeEnum.Selection)
                    {
                        SetSelectItem();
                        //
                        if (RemeshingParameters.CreationData != null)
                        {
                            _controller.Selection = RemeshingParameters.CreationData.DeepClone();
                            _controller.HighlightSelection();
                        }
                    }
                    else throw new NotSupportedException();
                }
            }
            catch { }
        }
        //
        private void ShowHideSelectionForm()
        {
            if (RemeshingParameters != null && RemeshingParameters.RegionType == RegionTypeEnum.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (RemeshingParameters != null && RemeshingParameters.RegionType == RegionTypeEnum.Selection)
                _controller.SetSelectItemToElement();
            else
                _controller.SetSelectByToOff();
        }
        public void SelectionChanged(int[] ids)
        {
            if (RemeshingParameters != null && RemeshingParameters.RegionType == RegionTypeEnum.Selection)
            {
                RemeshingParameters.CreationIds = ids;
                RemeshingParameters.CreationData = _controller.Selection.DeepClone();
                //
                propertyGrid.Refresh();
                //
                _propertyItemChanged = true;
            }
        }

        // IFormHighlight
        public void Highlight()
        {
            HighlightElementSet();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (RemeshingParameters == null || RemeshingParameters.CreationData == null) return true;
            return RemeshingParameters.CreationData.IsGeometryBased();
        }
    }


}
