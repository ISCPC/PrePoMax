using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Windows.Forms;
using System.Drawing;

namespace PrePoMax.Forms
{
    class FrmTranslate : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private TranslateParameters _translateParameters;
        private Controller _controller;
        private string[] _partNames;
        private double[][] _coorNodesToDraw;
        private double[][] _coorLinesToDraw;


        // Properties                                                                                                               
        public string[] PartNames { get { return _partNames; } set { _partNames = value; } }
        public double[] TranslateVector
        {
            get
            {
                return new double[] { _translateParameters.X2 - _translateParameters.X1,
                                      _translateParameters.Y2 - _translateParameters.Y1,
                                      _translateParameters.Z2 - _translateParameters.Z1};
            }
        }


        // Constructors                                                                                                             
        public FrmTranslate(Controller controller) 
            : base(2)
        {
            InitializeComponent();

            _controller = controller;
            _translateParameters = new TranslateParameters();
            propertyGrid.SelectedObject = _translateParameters;

            _coorNodesToDraw = new double[1][];
            _coorNodesToDraw[0] = new double[3];

            _coorLinesToDraw = new double[2][];
            _coorLinesToDraw[0] = new double[3];
            _coorLinesToDraw[1] = new double[3];

            btnOkAddNew.Visible = false;
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmTranslate
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmTranslate";
            this.Text = "Translate Parameters";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            HighlightNodes();

            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void Apply()
        {
            _translateParameters = (TranslateParameters)propertyGrid.SelectedObject;

            double[] translateVector = TranslateVector;
            _controller.TranlsateModelPartsCommand(_partNames, translateVector, _translateParameters.Copy);
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string partToEditName)
        {
            _controller.ClearSelectionHistory();
            _translateParameters.Clear();
            

            // Get start point grid item
            GridItem gi = propertyGrid.EnumerateAllItems().First((item) =>
                              item.PropertyDescriptor != null &&
                              item.PropertyDescriptor.Name == "StartPointItemSet");

            // Select it
            gi.Select();

            propertyGrid.Refresh();

            return OnPrepareForm(stepName, partToEditName);
        }
        public void PickedIds(int[] ids)
        {
            this.Enabled = true;

            _controller.SelectBy = vtkSelectBy.Off;
            _controller.Selection.SelectItem = vtkSelectItem.None;
            _controller.ClearSelectionHistory();

            if (ids != null && ids.Length == 1)
            {
                FeNode node = _controller.Model.Mesh.Nodes[ids[0]];
                string propertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                if (propertyName == "StartPointItemSet")
                {
                    _translateParameters.X1 = node.X;
                    _translateParameters.Y1 = node.Y;
                    _translateParameters.Z1 = node.Z;
                }
                else if(propertyName == "EndPointItemSet")
                {
                    _translateParameters.X2 = node.X;
                    _translateParameters.Y2 = node.Y;
                    _translateParameters.Z2 = node.Z;
                }

                propertyGrid.Refresh();

                HighlightNodes();
            }
        }
        private void HighlightNodes()
        {
            Color color = Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;

            _coorNodesToDraw[0][0] = _translateParameters.X2;
            _coorNodesToDraw[0][1] = _translateParameters.Y2;
            _coorNodesToDraw[0][2] = _translateParameters.Z2;

            _coorLinesToDraw[0][0] = _translateParameters.X1;
            _coorLinesToDraw[0][1] = _translateParameters.Y1;
            _coorLinesToDraw[0][2] = _translateParameters.Z1;
            _coorLinesToDraw[1] = _coorNodesToDraw[0];

            _controller.DrawNodes("Translate", _coorNodesToDraw, color, layer, 7);
            _controller.HighlightConnectedLines(_coorLinesToDraw, 7);
        }
    }
}
