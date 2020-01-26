using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax
{
    public partial class FrmAnalyzeGeometry : Form
    {
        // Variables                                                                                                                
        private Controller _controller;
        private string[] _partNamesToAnalyze;

        // Properties                                                                                                               
        public string[] PartNamesToAnalyze 
        { 
            get { return _partNamesToAnalyze; } 
            set 
            {
                HashSet<string> partNames = new HashSet<string>();
                CaeMesh.BasePart basePart;
                foreach (var partName in value)
                {
                    basePart = _controller.DisplayedMesh.Parts[partName];
                    if (basePart is CaeMesh.CompoundGeometryPart cgp) partNames.UnionWith(cgp.SubPartNames);
                    else partNames.Add(partName);
                }
                _partNamesToAnalyze = partNames.ToArray();
            } 
        }


        // Constructors                                                                                                             
        public FrmAnalyzeGeometry(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
        }


        // Event handlers                                                                                                           
        private void FrmAnalyzeGeometry_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmAnalyzeGeometry_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (_partNamesToAnalyze == null || _partNamesToAnalyze.Length == 0)
                    throw new CaeGlobals.CaeException("No parts were selected for analysis.");
                //
                double shortestEdge = 0;
                double closestEdges = 0;
                double smallestFace = 0;
                try
                {
                    shortestEdge = _controller.GetShortestEdgeLen(_partNamesToAnalyze);
                    closestEdges = _controller.GetClosestUnConnectedEdgesDistance(_partNamesToAnalyze);
                    smallestFace = _controller.GetSmallestFace(_partNamesToAnalyze);
                }
                catch {}
                //
                labShortestEdge.Text = "Model min: " + shortestEdge.ToString("G4");
                if (shortestEdge > 0) tbMinEdgeLen.Text = Math.Pow(10, Math.Ceiling(Math.Log10(shortestEdge))).ToString();
                else tbMinEdgeLen.Text = "0.0";
                //
                labClosestEdges.Text = "Model min: " + closestEdges.ToString("G4");
                if (closestEdges > 0) tbMinEdgesDistance.Text = Math.Pow(10, Math.Ceiling(Math.Log10(closestEdges))).ToString();
                else tbMinEdgesDistance.Text = "0.0";
                //
                labSmallestFace.Text = "Model min: " + smallestFace.ToString("G4");                
                if (smallestFace > 0) tbMinFaceSize.Text = Math.Pow(10, Math.Ceiling(Math.Log10(smallestFace))).ToString();
                else tbMinFaceSize.Text = "0.0";
            }
        }
        private void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbEdgesShorter.Checked || cbFacesSmaller.Checked || cbCloseEdges.Checked)
                    _controller.ClearAllSelection();
                //
                if (cbEdgesShorter.Checked) _controller.ShowShortEdges(tbMinEdgeLen.Value, _partNamesToAnalyze);
                if (cbFacesSmaller.Checked) _controller.ShowSmallFaces(tbMinFaceSize.Value, _partNamesToAnalyze);
                if (cbCloseEdges.Checked) _controller.ShowCloseUnConnectedEdges(tbMinEdgesDistance.Value, _partNamesToAnalyze);
            }
            catch
            {
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        
    }
}
