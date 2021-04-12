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
        private ViewExplodedViewParameters _explodedViewParameters;
        private double _cancelScaleFactor;
        private double _cancelMagnification;
        private string _drawSymbolsForStep;
        private Octree.Plane _sectionViewPlane;

        // Properties                                                                                                               
        public double ScaleFactor
        {
            get { return _explodedViewParameters.ScaleFactor; }
            set
            {
                _cancelScaleFactor = value;
                if (value == -1) value = 0.5;   // -1 stands for disabled scale factor
                _explodedViewParameters.ScaleFactor = value;
                UpdateScrollbarPosition();
            }
        }
        public double Magnification
        {
            get { return _explodedViewParameters.Magnification; }
            set
            {
                _cancelMagnification = value;
                _explodedViewParameters.Magnification = value;
                UpdateScrollbarPosition();
            }
        }


        // Callbacks
        public Action Clear3D;


        // Constructors                                                                                                             
        public FrmExplodedView(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _explodedViewParameters = new ViewExplodedViewParameters();
            propertyGrid.SelectedObject = _explodedViewParameters;
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
                _sectionViewPlane = _controller.GetSectionViewPlane();
                if (_sectionViewPlane != null) _controller.RemoveSectionView();
                //
                _drawSymbolsForStep = _controller.GetDrawSymbolsForStep();
                _controller.DrawSymbolsForStep("None", false);
                UpdateScrollbarPosition();
            }
            else
            {
                if (_sectionViewPlane != null) _controller.ApplySectionView(_sectionViewPlane.Point.Coor,
                                                                            _sectionViewPlane.Normal.Coor);
                //
                _controller.DrawSymbolsForStep(_drawSymbolsForStep, false);
            }
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateScrollbarPosition();
        }
        //
        private void hsbPosition_Scroll(object sender, ScrollEventArgs e)
        {
            //timerUpdate.Start();    // use timer to speed things up
            UpdateExplodedViewFromScrollBar();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _controller.ApplyExplodedView(_explodedViewParameters.ScaleFactor, _explodedViewParameters.Magnification);
            //
            this.DialogResult = DialogResult.OK;
            Hide();
        }
        private void btnDisable_Click(object sender, EventArgs e)
        {
            _controller.RemoveExplodedView();
            //
            this.DialogResult = DialogResult.Abort;
            Hide();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cancelScaleFactor == -1) _controller.RemoveExplodedView();
            else
            {
                _explodedViewParameters.ScaleFactor = _cancelScaleFactor;
                _controller.ApplyExplodedView(_explodedViewParameters.ScaleFactor, _explodedViewParameters.Magnification);
            }
            //
            this.DialogResult = DialogResult.Cancel;
            Hide();
        }
        //
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            UpdateExplodedViewFromScrollBar();
        }

        // Methods                                                                                                                  

        // IFormBase
        public bool PrepareForm(string stepName, string itemToEditName)
        {
            propertyGrid.Refresh();
            //
            _controller.RemoveExplodedView();   // this redraws the scene and redraws selection
            _controller.ClearSelectionHistory();
            _controller.SetSelectByToOff();
            //
            return true;
        }
        private void UpdateScrollbarPosition()
        {
            hsbPosition.Value = (int)(_explodedViewParameters.ScaleFactor * 1000);
            //
            UpdateExplodedViewFromScrollBar();
        }
        private void UpdateExplodedViewFromScrollBar()
        {
            try
            {
                double scaleFactor = (double)hsbPosition.Value / hsbPosition.Maximum;
                _explodedViewParameters.ScaleFactor = scaleFactor;
                propertyGrid.Refresh();
                //
                if (this.Visible)
                    _controller.PreviewExplodedView(_explodedViewParameters.ScaleFactor, _explodedViewParameters.Magnification);
            }
            catch
            { }
        }
    }
}
