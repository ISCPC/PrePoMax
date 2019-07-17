using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;

namespace PrePoMax.Forms
{
    public partial class FrmQuery : UserControls.PrePoMaxChildForm
    {
        // Variables                                                                                                                
        private int _numNodesToSelect;
        private double[][] _coorNodesToDraw;
        private double[][] _coorLinesToDraw;
        private Controller _controller;


        // Callbacks                                                                                                               
        public Action<string> Form_WriteDataToOutput;

        
        // Constructors                                                                                                             
        public FrmQuery()
        {
            InitializeComponent();
            _numNodesToSelect = -1;
        }


        // Event hadlers                                                                                                            
        private void lvQueries_MouseDown(object sender, MouseEventArgs e)
        {
            lvQueries.SelectedItems.Clear();
        }
        private void lvQueries_MouseUp(object sender, MouseEventArgs e)
        {
        }
        private void lvQueries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvQueries.SelectedItems.Count > 0)
            {
                switch (lvQueries.SelectedItems[0].Text)
                {
                    case ("Bounding box size"):
                        _controller.SelectBy = vtkSelectBy.Off;
                        _controller.Selection.SelectItem = vtkSelectItem.None;
                        OutputBoundingBox();
                        _controller.ClearSelectionHistory();
                        _numNodesToSelect = -1;
                        break;
                    case ("Assembly"):
                        _controller.SelectBy = vtkSelectBy.Off;
                        _controller.Selection.SelectItem = vtkSelectItem.None;
                        OutputAssemblyData();
                        _controller.ClearSelectionHistory();
                        _numNodesToSelect = -1;
                        break;
                    case ("Part"):
                        _controller.SelectBy = vtkSelectBy.QueryPart;
                        _controller.Selection.SelectItem = vtkSelectItem.Part;
                        _controller.ClearSelectionHistory();
                        _numNodesToSelect = -1;
                        break;
                    case ("Point/Node"):
                        _controller.SelectBy = vtkSelectBy.QueryNode;
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        _controller.ClearSelectionHistory();
                        _numNodesToSelect = 1;
                        break;
                    case ("Element"):
                        _controller.SelectBy = vtkSelectBy.QueryElement;
                        _controller.Selection.SelectItem = vtkSelectItem.Element;
                        _controller.ClearSelectionHistory();
                        _numNodesToSelect = -1;
                        break;
                    case ("Distance"):
                        _controller.SelectBy = vtkSelectBy.QueryNode;
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        _controller.ClearSelectionHistory();
                        _numNodesToSelect = 2;
                        break;
                    case ("Angle"):
                        _controller.SelectBy = vtkSelectBy.QueryNode;
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        _controller.ClearSelectionHistory();
                        _numNodesToSelect = 3;
                        break;
                    case ("Circle"):
                        _controller.SelectBy = vtkSelectBy.QueryNode;
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        _controller.ClearSelectionHistory();
                        _numNodesToSelect = 3;
                        break;
                    default:
                        break;
                }
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void FrmQuery_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmQuery_VisibleChanged(object sender, EventArgs e)
        {
            // this is called if some other form is shown to close all other forms
            // this is called after the form visibility changes
            // the form was hidden 
            if (!this.Visible)
            {
                _controller.SelectBy = vtkSelectBy.Off;
            }                
        }


        // Methods                                                                                                                  
        public void PrepareForm(Controller controller)
        {
            
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized

            _controller = controller;
            lvQueries.HideSelection = false;
            lvQueries.SelectedIndices.Clear();
        }
        private void OutputBoundingBox()
        {
            double[] bb = _controller.GetBoundingBox();
            double[] size = new double[] { bb[1] - bb[0], bb[3] - bb[2], bb[5] - bb[4] };

            Form_WriteDataToOutput("");
            string data = string.Format("Bounding box");
            Form_WriteDataToOutput(data);
            data = string.Format("Min             x, y, z: {0,16:E}, {1,16:E}, {2,16:E}", bb[0], bb[2], bb[4]);
            Form_WriteDataToOutput(data);
            data = string.Format("Max             x, y, z: {0,16:E}, {1,16:E}, {2,16:E}", bb[1], bb[3], bb[5]);
            Form_WriteDataToOutput(data);
            data = string.Format("Size            x, y, z: {0,16:E}, {1,16:E}, {2,16:E}", size[0], size[1], size[2]);
            Form_WriteDataToOutput(data);
        }
        private void OutputAssemblyData()
        {
            CaeMesh.FeMesh mesh = _controller.DisplayedMesh;
            if (mesh == null) return;
            double[] bb = _controller.GetBoundingBox();
            double[] size = new double[] { bb[1] - bb[0], bb[3] - bb[2], bb[5] - bb[4] };

            Form_WriteDataToOutput("");
            string data = string.Format("Assembly");
            Form_WriteDataToOutput(data);
            data = string.Format("Number of parts: {0}", mesh.Parts.Count);
            Form_WriteDataToOutput(data);
            data = string.Format("Number of elements: {0}", mesh.Elements.Count);
            Form_WriteDataToOutput(data);
            data = string.Format("Number of nodes: {0}", mesh.Nodes.Count);
            Form_WriteDataToOutput(data);
            Form_WriteDataToOutput("");
        }
        public void PickedIds(int[] ids)
        {
            try
            {
                if (_controller.SelectBy == vtkSelectBy.QueryPart && ids.Length == 1) OnePartPicked(ids[0]);
                else if (_controller.SelectBy == vtkSelectBy.QueryElement && ids.Length == 1) OneElementPicked(ids[0]);
                else if (ids.Length == _numNodesToSelect)
                {
                    // one node
                    if (ids.Length == 1) OneNodePicked(ids[0]);
                    // two nodes
                    else if (ids.Length == 2) TwoNodesPicked(ids[0], ids[1]);
                    // three nodes
                    else if (ids.Length == 3) ThreeNodesPicked(ids[0], ids[1], ids[2]);

                    _controller.ClearSelectionHistory();
                    HighlightNodes();
                }
            }
            catch
            {}
        }
        public void OnePartPicked(int id)
        {
            CaeMesh.FeMesh mesh = _controller.DisplayedMesh;
            
            CaeMesh.BasePart part = null;
            foreach (var entry in mesh.Parts)
            {
                if (entry.Value.PartId == id) part = entry.Value;
            }

            if (part == null) throw new NotSupportedException();

            Form_WriteDataToOutput("");
            string data = string.Format("Part name: {0}", part.Name);
            Form_WriteDataToOutput(data);
            data = string.Format("Part type: {0}", part.PartType);
            Form_WriteDataToOutput(data);
            //data = string.Format("Part id: {0}{1}", part.PartId, Environment.NewLine);
            //WriteLineToOutputWithDate(data);
            data = string.Format("Number of elements: {0}", part.Labels.Length);
            Form_WriteDataToOutput(data);
            data = string.Format("Number of nodes: {0}", part.NodeLabels.Length);
            Form_WriteDataToOutput(data);
            Form_WriteDataToOutput("");

            _controller.ClearSelectionHistory();
            _controller.Highlight3DObjects(_controller.CurrentView, new object[] { part });
        }
        public void OneElementPicked(int id)
        {
            CaeMesh.FeMesh mesh;
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) mesh = _controller.Model.Geometry;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) mesh = _controller.Model.Mesh;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results) mesh = _controller.Results.Mesh;
            else throw new NotSupportedException();
            
            Form_WriteDataToOutput("");
            string data = string.Format("Element id: {0}", id);
            Form_WriteDataToOutput(data);
            Form_WriteDataToOutput("");

            _controller.ClearSelectionHistory();
            _controller.HighlightElement(id);
        }
        public void OneNodePicked(int nodeId)
        {
            if (Form_WriteDataToOutput != null)
            {
                string data;
                _coorNodesToDraw = new double[_numNodesToSelect][];

                Vec3D baseV = new Vec3D(_controller.GetNode(nodeId).Coor);

                Form_WriteDataToOutput("");
                data = string.Format("Node                 id: {0,16}", nodeId);
                Form_WriteDataToOutput(data);
                data = string.Format("Base            x, y, z: {0,16:E}, {1,16:E}, {2,16:E}", baseV.X, baseV.Y, baseV.Z);
                Form_WriteDataToOutput(data);

                if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    float fieldValue = _controller.GetNodalValue(nodeId);
                    Vec3D trueScaledV = new Vec3D(_controller.GetScaledNode(1, nodeId).Coor);
                    Vec3D disp = trueScaledV - baseV;

                    data = string.Format("Deformed        x, y, z: {0,16:E}, {1,16:E}, {2,16:E}", trueScaledV.X, trueScaledV.Y, trueScaledV.Z);
                    Form_WriteDataToOutput(data);
                    data = string.Format("Displacement    x, y, z: {0,16:E}, {1,16:E}, {2,16:E}", disp.X, disp.Y, disp.Z);
                    Form_WriteDataToOutput(data);
                    data = string.Format("Field value            : {0,16:E}", fieldValue);
                    Form_WriteDataToOutput(data);

                    float scale = _controller.GetScale();
                    baseV = new Vec3D(_controller.GetScaledNode(scale, nodeId).Coor); // for the _coorNodesToDraw
                }

                Form_WriteDataToOutput("");

                _coorNodesToDraw[0] = baseV.Coor;
                _coorLinesToDraw = null;
            }
        }
        public void TwoNodesPicked(int nodeId1, int nodeId2)
        {
            if (Form_WriteDataToOutput != null)
            {
                string data;
                _coorNodesToDraw = new double[_numNodesToSelect][];

                Vec3D baseV1 = new Vec3D(_controller.GetNode(nodeId1).Coor);
                Vec3D baseV2 = new Vec3D(_controller.GetNode(nodeId2).Coor);
                Vec3D baseD = baseV2 - baseV1;

                Form_WriteDataToOutput("");
                data = string.Format("Distance       id1, id2: {0,16}, {1,16}", nodeId1, nodeId2);
                Form_WriteDataToOutput(data);
                data = string.Format("Base      dx, dy, dz, D: {0,16:E}, {1,16:E}, {2,16:E}, {3,16:E}", baseD.X, baseD.Y, baseD.Z, baseD.Len);
                Form_WriteDataToOutput(data);

                if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    Vec3D trueScaledV1 = new Vec3D(_controller.GetScaledNode(1, nodeId1).Coor);
                    Vec3D trueScaledV2 = new Vec3D(_controller.GetScaledNode(1, nodeId2).Coor);
                    Vec3D trueScaledD = trueScaledV2 - trueScaledV1;
                    Vec3D delta = trueScaledD - baseD;

                    data = string.Format("Deformed  dx, dy, dz, D: {0,16:E}, {1,16:E}, {2,16:E}, {3,16:E}", trueScaledD.X, trueScaledD.Y, trueScaledD.Z, trueScaledD.Len);
                    Form_WriteDataToOutput(data);
                    data = string.Format("Delta     dx, dy, dz, D: {0,16:E}, {1,16:E}, {2,16:E}, {3,16:E}", delta.X, delta.Y, delta.Z, trueScaledD.Len - baseD.Len);
                    Form_WriteDataToOutput(data);

                    float scale = _controller.GetScale();
                    baseV1 = new Vec3D(_controller.GetScaledNode(scale, nodeId1).Coor);    // for the _coorNodesToDraw
                    baseV2 = new Vec3D(_controller.GetScaledNode(scale, nodeId2).Coor);    // for the _coorNodesToDraw
                }
                Form_WriteDataToOutput("");

                _coorNodesToDraw[0] = baseV1.Coor;
                _coorNodesToDraw[1] = baseV2.Coor;
                _coorLinesToDraw = _coorNodesToDraw;
            }
        }
        public void ThreeNodesPicked(int nodeId1, int nodeId2, int nodeId3)
        {
            if (Form_WriteDataToOutput != null)
            {
                if (lvQueries.SelectedItems[0].Text == "Angle") ComputeAngle(nodeId1, nodeId2, nodeId3);
                else if (lvQueries.SelectedItems[0].Text == "Circle") ComputeCircle(nodeId1, nodeId2, nodeId3);
            }
        }
        private void ComputeAngle(int nodeId1, int nodeId2, int nodeId3)
        {
            string data;
            double angle;
            Vec3D p;
            Vec3D axis;
            Vec3D baseV1 = new Vec3D(_controller.GetNode(nodeId1).Coor);
            Vec3D baseV2 = new Vec3D(_controller.GetNode(nodeId2).Coor);
            Vec3D baseV3 = new Vec3D(_controller.GetNode(nodeId3).Coor);

            angle = ComputeAngle(baseV1, baseV2, baseV3, out p, out axis);
            
            data = string.Format("Angle     id1, id2, id3: {0,16}, {1,16}, {2,16}", nodeId1, nodeId2, nodeId3);
            Form_WriteDataToOutput(data);
            data = string.Format("Base                [°]: {0,16:E}", angle);
            Form_WriteDataToOutput(data);

            if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                baseV1 = new Vec3D(_controller.GetScaledNode(1, nodeId1).Coor);
                baseV2 = new Vec3D(_controller.GetScaledNode(1, nodeId2).Coor);
                baseV3 = new Vec3D(_controller.GetScaledNode(1, nodeId3).Coor);

                double angle2 = ComputeAngle(baseV1, baseV2, baseV3, out p, out axis);
                double delta = angle2 - angle;

                data = string.Format("Deformed            [°]: {0,16:E}", angle2);
                Form_WriteDataToOutput(data);
                data = string.Format("Delta               [°]: {0,16:E}", delta);
                Form_WriteDataToOutput(data);

                float scale = _controller.GetScale();
                baseV1 = new Vec3D(_controller.GetScaledNode(scale, nodeId1).Coor);    // for the _coorNodesToDraw
                baseV2 = new Vec3D(_controller.GetScaledNode(scale, nodeId2).Coor);    // for the _coorNodesToDraw
                baseV3 = new Vec3D(_controller.GetScaledNode(scale, nodeId3).Coor);    // for the _coorNodesToDraw

                angle = ComputeAngle(baseV1, baseV2, baseV3, out p, out axis);
            }
            Form_WriteDataToOutput("");

            _coorNodesToDraw = new double[_numNodesToSelect][];
            _coorNodesToDraw[0] = baseV1.Coor;
            _coorNodesToDraw[1] = baseV2.Coor;
            _coorNodesToDraw[2] = baseV3.Coor;

            List<double[]> coorLines = new List<double[]>() { baseV1.Coor, baseV2.Coor, baseV3.Coor, baseV2.Coor };
            coorLines.AddRange(ComputeCirclePoints(baseV2, axis, p, angle * Math.PI / 180));
            _coorLinesToDraw = coorLines.ToArray();
           
        }
        private void ComputeCircle(int nodeId1, int nodeId2, int nodeId3)
        {
            //https://en.wikipedia.org/wiki/Circumscribed_circle
            string data;
            double r;
            Vec3D center;
            Vec3D axis;
            Vec3D baseV1 = new Vec3D(_controller.GetNode(nodeId1).Coor);
            Vec3D baseV2 = new Vec3D(_controller.GetNode(nodeId2).Coor);
            Vec3D baseV3 = new Vec3D(_controller.GetNode(nodeId3).Coor);

            ComputeCircle(baseV1, baseV2, baseV3, out r, out center, out axis);
            
            Form_WriteDataToOutput("");
            data = string.Format("Circle    id1, id2, id3: {0,16}, {1,16}, {2,16}", nodeId1, nodeId2, nodeId3);
            Form_WriteDataToOutput(data);
            data = string.Format("Base         x, y, z, R: {0,16:E}, {1,16:E}, {2,16:E}, {3,16:E}", center.X, center.Y, center.Z, r);
            Form_WriteDataToOutput(data);
            data = string.Format("Base axis       x, y, z: {0,16:E}, {1,16:E}, {2,16:E}", axis.X, axis.Y, axis.Z);            
            Form_WriteDataToOutput(data);

            if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                baseV1 = new Vec3D(_controller.GetScaledNode(1, nodeId1).Coor);
                baseV2 = new Vec3D(_controller.GetScaledNode(1, nodeId2).Coor);
                baseV3 = new Vec3D(_controller.GetScaledNode(1, nodeId3).Coor);

                ComputeCircle(baseV1, baseV2, baseV3, out r, out center, out axis);

                data = string.Format("Deformed     x, y, z, R: {0,16:E}, {1,16:E}, {2,16:E}, {3,16:E}", center.X, center.Y, center.Z, r);
                Form_WriteDataToOutput(data);
                data = string.Format("Deformed axis   x, y, z: {0,16:E}, {1,16:E}, {2,16:E}", axis.X, axis.Y, axis.Z);
                Form_WriteDataToOutput(data);

                float scale = _controller.GetScale();
                baseV1 = new Vec3D(_controller.GetScaledNode(scale, nodeId1).Coor);    // for the _coorNodesToDraw
                baseV2 = new Vec3D(_controller.GetScaledNode(scale, nodeId2).Coor);    // for the _coorNodesToDraw
                baseV3 = new Vec3D(_controller.GetScaledNode(scale, nodeId3).Coor);    // for the _coorNodesToDraw

                ComputeCircle(baseV1, baseV2, baseV3, out r, out center, out axis);
            }
            Form_WriteDataToOutput("");

            _coorNodesToDraw = new double[_numNodesToSelect + 1][];
            _coorNodesToDraw[0] = baseV1.Coor;
            _coorNodesToDraw[1] = baseV2.Coor;
            _coorNodesToDraw[2] = baseV3.Coor;
            _coorNodesToDraw[3] = center.Coor;

            _coorLinesToDraw = ComputeCirclePoints(center, axis, baseV1, 2 * Math.PI);
        }
        private double ComputeAngle(Vec3D baseV1, Vec3D baseV2, Vec3D baseV3, out Vec3D p, out Vec3D axis)
        {
            Vec3D line1 = baseV1 - baseV2;
            Vec3D line2 = baseV3 - baseV2;

            if (line1.Len2 > line2.Len2)  // find shorter line 
            {
                p = baseV2 + line2 * 0.5;
                axis = Vec3D.CrossProduct(line2, line1);
            }
            else
            {
                p = baseV2 + line1 * 0.5;
                axis = Vec3D.CrossProduct(line1, line2);
            }
            axis.Normalize();
            line1.Normalize();
            line2.Normalize();

            double angle = Math.Acos(Vec3D.DotProduct(line1, line2)) * 180 / Math.PI;
            return angle;
        }
        private void ComputeCircle(Vec3D baseV1, Vec3D baseV2, Vec3D baseV3, out double r, out Vec3D center, out Vec3D axis)
        {
            Vec3D n12 = baseV1 - baseV2;
            Vec3D n21 = baseV2 - baseV1;
            Vec3D n23 = baseV2 - baseV3;
            Vec3D n32 = baseV3 - baseV2;
            Vec3D n13 = baseV1 - baseV3;
            Vec3D n31 = baseV3 - baseV1;
            Vec3D n12xn23 = Vec3D.CrossProduct(n12, n23);

            r = (n12.Len * n23.Len * n31.Len) / (2 * n12xn23.Len);

            double denominator = 2 * n12xn23.Len2;
            double alpha = n23.Len2 * Vec3D.DotProduct(n12, n13) / denominator;
            double beta = n13.Len2 * Vec3D.DotProduct(n21, n23) / denominator;
            double gama = n12.Len2 * Vec3D.DotProduct(n31, n32) / denominator;

            center = alpha * baseV1 + beta * baseV2 + gama * baseV3;

            axis = Vec3D.CrossProduct(n21, n32);
            axis.Normalize();
        }
        private double[][] ComputeCirclePoints(Vec3D c, Vec3D axis, Vec3D p, double angle)
        {
            // the circe is constructed by moving the r vector around the axis
            // d vector is the change in normal n and perpendicular r direction
            int segments = 40;
            double [][] coorLines = new double[segments + 1][];
            double dAngle = angle / segments;
            Vec3D r = p - c;
            double rLen = r.Len;
            Vec3D n;
            Vec3D d;

            coorLines[0] = p.Coor;
            for (int i = 0; i < segments; i++)
            {
                n = Vec3D.CrossProduct(axis, r);
                n.Normalize();

                d = rLen * Math.Sin(dAngle) * n - (1 - Math.Cos(dAngle)) * r;
                p = p + d;
                r = r + d;

                coorLines[i + 1] = p.Coor;
            }
            return coorLines;
        }
        private void HighlightNodes()
        {
            Color color = Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;

            if (_coorNodesToDraw != null)
            {
                if (_coorNodesToDraw.GetLength(0) == 1)
                {
                    _controller.DrawNodes("Querry", _coorNodesToDraw, color, layer, 7);
                }
                else if (_coorNodesToDraw.GetLength(0) >= 2)
                {
                    _controller.DrawNodes("Querry", _coorNodesToDraw, color, layer, 7);
                    _controller.HighlightConnectedLines(_coorLinesToDraw, 7);
                }
            }
        }

       

       

       

        

       
    }
}
