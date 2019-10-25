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
    public partial class FrmSectionCut : Form
    {
        // Variables                                                                                                                
        private Controller _controller;
        private double min;
        private double max;

        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmSectionCut(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
        }


        // Event handlers                                                                                                           
        private void FrmSectionCut_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmSectionCut_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (Visible)
                {
                    hsbPosition.Value = hsbPosition.Maximum / 2;
                    timerUpdate.Start();
                }
                else
                {
                    _controller.RemoveSectionCut();
                }
            }
            catch
            {}
        }
        private void rbAxis_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Checked == true)
            {
                hsbPosition.Value = hsbPosition.Maximum / 2;
                timerUpdate.Stop();
                timerUpdate.Start();
            }
        }
        private void cbReverse_CheckedChanged(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            timerUpdate.Start();
        }

        private void hsbPosition_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll) return;

            timerUpdate.Stop();
            timerUpdate.Start();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                if (timerUpdate.Enabled)
                {
                    //CaeMesh.BoundingBox box = _controller.DisplayedMesh.BoundingBox;
                    double[] boxy = _controller.GetBoundingBox();
                    CaeMesh.BoundingBox box = new CaeMesh.BoundingBox();
                    box.MinX = boxy[0];
                    box.MaxX = boxy[1];
                    box.MinY = boxy[2];
                    box.MaxY = boxy[3];
                    box.MinZ = boxy[4];
                    box.MaxZ = boxy[5];

                    Octree.Point normal = new Octree.Point();
                    if (rbX.Checked)
                    {
                        min = box.MinX;
                        max = box.MaxX;                        
                        normal.X = 1;
                    }
                    else if (rbY.Checked)
                    {
                        min = box.MinY;
                        max = box.MaxY;
                        normal.Y = 1;
                    }
                    else if (rbZ.Checked)
                    {
                        min = box.MinZ;
                        max = box.MaxZ;
                        normal.Z = 1;
                    }
                    double delta = (max - min) / 100;
                    max += delta;
                    min -= delta;
                    max = CaeGlobals.Tools.RoundToSignificantDigits(max, 4);
                    min = CaeGlobals.Tools.RoundToSignificantDigits(min, 4);

                    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                    watch.Start();

                    timerUpdate.Stop();
                    timerUpdate.Interval = 1;

                    double ratio = (double)hsbPosition.Value / hsbPosition.Maximum;
                    double pos = min + (max - min) * ratio;
                    ntbPosition.Text = pos.ToString();

                    if (cbReverse.Checked)
                    {
                        normal = normal * -1;
                        pos *= -1;
                    }

                    Octree.Plane sectionPlane = new Octree.Plane(normal, -pos);

                    _controller.ApplySectionCut(sectionPlane);

                    System.Diagnostics.Debug.WriteLine("Section cut time: " + DateTime.Now.ToLongTimeString() + "   Duration: " + watch.ElapsedMilliseconds);
                }
            }
            catch
            { }
        }

        
    }
}
