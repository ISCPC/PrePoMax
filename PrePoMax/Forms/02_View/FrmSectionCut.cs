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
    public partial class FrmSectionCut : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private Controller _controller;
        private SectionCutParameters _sectionCutParameters;
        private bool _pause;
        private double min;
        private double max;
        private Vec3D _projHalfSize;

        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmSectionCut(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _sectionCutParameters = new SectionCutParameters();
            propertyGrid.SelectedObject = _sectionCutParameters;
            _pause = false;

            propertyGrid.SetParent(this);   // for the Tab key to work
            propertyGrid.SetLabelColumnWidth(1.9);
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
                    _pause = true;
                    //
                    Octree.Plane plane = _controller.GetSectionCutPlane();
                    if (plane == null)
                    {
                        resetToolStripMenuItem_Click(null, null);
                    }
                    else
                    {
                        _sectionCutParameters.Point = plane.Point.Coor;
                        _sectionCutParameters.Normal = plane.Normal.Coor;
                        SetScrollBarPositionFromPoint();
                    }

                    _controller.ApplySectionCut(_sectionCutParameters.Point, _sectionCutParameters.Normal);
                }
                else
                {
                    if (this.DialogResult == DialogResult.Cancel) _controller.RemoveSectionCut();
                }
            }
            catch
            {}
            finally
            {
                _pause = false;
            }
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PointOrNormalChanged();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionCutParameters.Clear();
            _sectionCutParameters.Point = GetBBCenter().Coor;
            //
            PointOrNormalChanged();
        }
        private void xDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionCutParameters.Normal = new double[] { 1, 0, 0 };
            //
            PointOrNormalChanged();
        }
        private void yDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionCutParameters.Normal = new double[] { 0, 1, 0 };
            //
            PointOrNormalChanged();
        }
        private void zDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionCutParameters.Normal = new double[] { 0, 0, 1 };
            //
            PointOrNormalChanged();
        }
        private void reverseDirectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sectionCutParameters.Normal[0] = -_sectionCutParameters.Normal[0];
            _sectionCutParameters.Normal[1] = -_sectionCutParameters.Normal[1];
            _sectionCutParameters.Normal[2] = -_sectionCutParameters.Normal[2];
            //
            PointOrNormalChanged();
        }

        private void hsbPosition_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll) return;

            UpdateProjHalfSize();

            double ratio = (double)(hsbPosition.Value - hsbPosition.Minimum) / (hsbPosition.Maximum - hsbPosition.Minimum);
            ratio = 2 * ratio - 1;
            Vec3D point = GetBBCenter() + _projHalfSize * ratio;
            _sectionCutParameters.Point = point.Coor;

            timerUpdate.Start();    // use timer to speed things up
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Hide();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            UpdateSectionCut();
        }

        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string partToEditName)
        {
            _controller.ClearSelectionHistory();
            _sectionCutParameters.Clear();


            // Get start point grid item
            GridItem gi = propertyGrid.EnumerateAllItems().First((item) =>
                          item.PropertyDescriptor != null &&
                          item.PropertyDescriptor.Name == "PointItemSet");

            // Select it
            gi.Select();

            propertyGrid.Refresh();

            return true;
        }
        public void PickedIds(int[] ids)
        {
            this.Enabled = true;

            _controller.SelectBy = vtkSelectBy.Off;
            _controller.Selection.SelectItem = vtkSelectItem.None;
            _controller.ClearSelectionHistory();

            if (ids != null)
            {
                if (ids.Length == 1)
                {
                    FeNode node = _controller.DisplayedMesh.Nodes[ids[0]];
                    string propertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                    if (propertyName == "PointItemSet")
                    {
                        _sectionCutParameters.X = node.X;
                        _sectionCutParameters.Y = node.Y;
                        _sectionCutParameters.Z = node.Z;
                        //
                        PointOrNormalChanged();
                    }
                    else throw new NotSupportedException();
                }
                //else if (ids.Length == 2)
                //{
                //    FeNode node = _controller.DisplayedMesh.Nodes[ids[0]];
                //    string propertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                //    if (propertyName == "PointItemSet")
                //    {
                //        _sectionCutParameters.X = node.X;
                //        _sectionCutParameters.Y = node.Y;
                //        _sectionCutParameters.Z = node.Z;
                //        //
                //        PointOrNormalChanged();
                //    }
                //    //else throw new NotSupportedException();
                //}
            }

            UpdateSectionCut();
        }
        private void PointOrNormalChanged()
        {
            SetScrollBarPositionFromPoint();
            timerUpdate.Start();
        }
        private void UpdateProjHalfSize()
        {
            double[] box = _controller.GetBoundingBox();

            Vec3D v = new Vec3D(); // half box diagonal
            v.X = (box[1] - box[0]) / 2;
            v.Y = (box[3] - box[2]) / 2;
            v.Z = (box[5] - box[4]) / 2;

            Vec3D n = new Vec3D(_sectionCutParameters.Normal);
            n.Abs();
            n.Normalize();

            // project 1/2 diagonal on the positive normal
            double l = Vec3D.DotProduct(v, n) * 1.05;

            _projHalfSize = new Vec3D(_sectionCutParameters.Normal);
            _projHalfSize.Normalize();
            _projHalfSize = l * _projHalfSize;
        }
        private Vec3D GetBBCenter()
        {
            double[] box = _controller.GetBoundingBox();
            Vec3D center = new Vec3D();
            center.X = CaeGlobals.Tools.RoundToSignificantDigits((box[0] + box[1]) / 2, 4);
            center.Y = CaeGlobals.Tools.RoundToSignificantDigits((box[2] + box[3]) / 2, 4);
            center.Z = CaeGlobals.Tools.RoundToSignificantDigits((box[4] + box[5]) / 2, 4);
            return center;
        }
        private void SetScrollBarPositionFromPoint()
        {
            try
            {
                UpdateProjHalfSize();

                Vec3D c = GetBBCenter();
                Vec3D n = new Vec3D(_sectionCutParameters.Normal);
                n.Normalize();

                Vec3D p = new Vec3D(_sectionCutParameters.Point);
                Vec3D v = p - c;

                double l = Vec3D.DotProduct(v, n);
                double ratio = l / _projHalfSize.Len;

                if (ratio > 1) ratio = 1;
                else if (ratio < -1) ratio = -1;

                ratio = (ratio + 1) / 2;        // to iterval from 0 to 1;

                hsbPosition.Value = (int)Math.Round((hsbPosition.Maximum - hsbPosition.Minimum) * ratio, 0);
            }
            catch
            { }
        }
        private void UpdateSectionCut()
        {
            try
            {
                if (_pause) return;

                propertyGrid.Refresh();     // must be here to update values

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                //
                UpdateProjHalfSize();
                //
                _sectionCutParameters = (SectionCutParameters)propertyGrid.SelectedObject;
                double[] point = _sectionCutParameters.Point;
                double[] normal = _sectionCutParameters.Normal;
                //
                _controller.UpdateSectionCut(point, normal);
                //
                System.Diagnostics.Debug.WriteLine("Section cut time: " + DateTime.Now.ToLongTimeString() + "   Duration: " + watch.ElapsedMilliseconds);
            }
            catch
            { }
        }

       

        
    }
}
