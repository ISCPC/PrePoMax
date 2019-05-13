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


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmAnalyzeGeometry(Controller controller)
        {
            InitializeComponent();

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
                double shortestEdge = 0;
                double smallestFace = 0;
                try
                {
                    shortestEdge = _controller.GetShortestEdgeLen();
                    smallestFace = _controller.GetSmallestFace();
                }
                catch {}

                labShortestEdge.Text = "Model min: " + shortestEdge.ToString("G4");
                if (shortestEdge > 0) tbMinEdgeLen.Text = Math.Pow(10, Math.Ceiling(Math.Log10(shortestEdge))).ToString();
                else tbMinEdgeLen.Text = "0.0";

                labSmallestFace.Text = "Model min: " + smallestFace.ToString("G4");                
                if (smallestFace > 0) tbMinFaceSize.Text = Math.Pow(10, Math.Ceiling(Math.Log10(smallestFace))).ToString();
                else tbMinFaceSize.Text = "0.0";
            }
        }
        private void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbEdgesShorter.Checked || cbFacesSmaller.Checked)
                    _controller.ClearAllSelection();

                if (cbEdgesShorter.Checked) _controller.ShowShortEdges(tbMinEdgeLen.Value);
                if (cbFacesSmaller.Checked) _controller.ShowSmallFaces(tbMinFaceSize.Value);
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
