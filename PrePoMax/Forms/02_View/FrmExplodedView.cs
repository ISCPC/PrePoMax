using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using CaeGlobals;


namespace PrePoMax.Forms
{    
    public partial class FrmExplodedView : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private Controller _controller;
        private ViewExplodedViewParameters _viewExplodedViewParameters;
        private ExplodedViewParameters _cancelParam;
        private string _drawSymbolsForStep;
        private Octree.Plane _sectionViewPlane;
        private bool _continueExplodedView;

        // Properties                                                                                                               
        public void SetExplodedViewParameters(ExplodedViewParameters parameters)
        {
            _cancelParam = parameters.DeepClone();
            //
            if (parameters.ScaleFactor == -1) parameters.ScaleFactor = 0.5;   // -1 stands for disabled scale factor
            _viewExplodedViewParameters.Parameters = parameters.DeepClone();
            //
            UpdateScrollbarPosition(false);
        }


        // Callbacks
        public Action Clear3D;


        // Constructors                                                                                                             
        public FrmExplodedView(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewExplodedViewParameters = new ViewExplodedViewParameters();
            propertyGrid.SelectedObject = _viewExplodedViewParameters;
            //
            propertyGrid.SetParent(this);   // for the Tab key to work
            propertyGrid.SetLabelColumnWidth(1.75);
        }


        // Event handlers                                                                                                           
        private void FrmSectionView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                btnCancel_Click(null, null);
            }
        }
        private void FrmSectionView_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                // Suppress section view
                _sectionViewPlane = _controller.GetSectionViewPlane();
                if (_sectionViewPlane != null) _controller.RemoveSectionView();
                //
                _drawSymbolsForStep = _controller.GetDrawSymbolsForStep();
                _controller.DrawSymbolsForStep("None", false);
                UpdateScrollbarPosition(true);
            }
            else
            {
                // Resume section view
                if (_sectionViewPlane != null) _controller.ApplySectionView(_sectionViewPlane.Point.Coor,
                                                                            _sectionViewPlane.Normal.Coor);
                //
                _controller.DrawSymbolsForStep(_drawSymbolsForStep, false);
                //
                if (DialogResult == DialogResult.OK) _controller.ApplyExplodedView(_viewExplodedViewParameters.Parameters);
                else if (DialogResult == DialogResult.Abort) _controller.RemoveExplodedView(true);
                else if (DialogResult == DialogResult.Cancel) Cancel();
                else if (DialogResult == DialogResult.None) Cancel(); // the form was closed from frmMain.CloseAllForms
            }
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateScrollbarPosition(true);
        }
        //
        private void hsbPosition_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateExplodedViewFromScrollBar(false);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Hide();
        }
        private void btnDisable_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            Hide();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Hide();
        }
        

        // Methods                                                                                                                  

        // IFormBase
        public bool PrepareForm(string stepName, string itemToEditName)
        {
            this.DialogResult = DialogResult.None;
            //
            propertyGrid.Refresh();
            // Set exploded view
            ExplodedViewParameters parameters;
            if (_cancelParam.ScaleFactor == -1)
            {
                parameters = _cancelParam.DeepClone();
                parameters.ScaleFactor = 0;
                _continueExplodedView = false;
            }
            else
            {
                parameters = _viewExplodedViewParameters.Parameters.DeepClone();
                _continueExplodedView = true;   // animation of exploded view is not needed
            }
            _controller.RemoveExplodedView(true);   // this redraws the scene and redraws selection
            _controller.PreviewExplodedView(parameters, false);
            _controller.ClearSelectionHistory();
            _controller.SetSelectByToOff();
            //
            return true;
        }
        //
        private void Cancel()
        {
            if (_cancelParam.ScaleFactor == -1) _controller.RemoveExplodedView(true);
            else _controller.ApplyExplodedView(_cancelParam);
        }
        private void UpdateScrollbarPosition(bool animate)
        {
            hsbPosition.Value = (int)(_viewExplodedViewParameters.ScaleFactor * 1000);
            //
            UpdateExplodedViewFromScrollBar(animate);
        }
        private void UpdateExplodedViewFromScrollBar(bool animate)
        {
            try
            {
                double scaleFactor = (double)hsbPosition.Value / hsbPosition.Maximum;
                _viewExplodedViewParameters.ScaleFactor = scaleFactor;
                propertyGrid.Refresh();
                //
                if (this.Visible)
                {
                    if (_continueExplodedView)
                    {
                        animate = false;    // animation of exploded view is not needed
                        _continueExplodedView = false;  // next time animate
                    }
                    //
                    _controller.PreviewExplodedView(_viewExplodedViewParameters.Parameters, animate);
                }
            }
            catch
            { }
        }
    }
}
