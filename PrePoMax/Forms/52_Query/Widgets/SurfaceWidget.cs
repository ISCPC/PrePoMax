using CaeMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class SurfaceWidget : WidgetBase
    {
        // Variables                                                                                                                
        private int _geometryId;


        // Properties                                                                                                               
        public int GeometryId { get { return _geometryId; } set { _geometryId = value; } }


        // Constructors                                                                                                             
        public SurfaceWidget(string name, int geometryId, Controller controller)
            : base(name, controller)
        {
            _geometryId = geometryId;
        }


        // Methods
        public override void GetWidgetData(out string text, out double[] coor)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(_geometryId);
            FeMesh mesh = _controller.DisplayedMesh;
            int surfaceId = itemTypePartIds[0];
            BasePart part = mesh.GetPartById(itemTypePartIds[2]);
            double area;
            string areaUnit = _controller.GetAreaUnit();
            string fieldUnit = "";
            string numberFormat = _controller.Settings.Widgets.GetNumberFormat();
            //
            bool results = false;
            float min = float.MaxValue;
            float max = -float.MaxValue;
            float avg = 0;
            int[] nodeIds;
            double[][] nodeCoor;
            //
            mesh.GetFaceNodes(_geometryId, out nodeIds);
            nodeCoor = new double[nodeIds.Length][];
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry ||
                _controller.CurrentView == ViewGeometryModelResults.Model)
            {
                // Area
                area = mesh.GetSurfaceArea(_geometryId);
                // Coor
                for (int i = 0; i < nodeIds.Length; i++) nodeCoor[i] = mesh.Nodes[nodeIds[i]].Coor;
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                results = true;
                //
                float value;
                FeNode[] nodes = _controller.GetScaledNodes(1, nodeIds);
                // Area
                Dictionary<int, double> weights;
                Dictionary<int, FeNode> nodesDic = new Dictionary<int, FeNode>();
                for (int i = 0; i < nodes.Length; i++) nodesDic.Add(nodes[i].Id, nodes[i]);
                mesh.GetFaceNodeLumpedWeights(part.Visualization, surfaceId, nodesDic, out weights, out area);
                // Values
                for (int i = 0; i < nodes.Length; i++)
                {
                    value = _controller.GetNodalValue(nodeIds[i]);
                    if (value < min) min = value;
                    if (value > max) max = value;
                    avg += (float)(value * weights[nodeIds[i]]);
                }
                avg /= (float)area;
                // Coor
                nodes = _controller.GetScaledNodes(_controller.GetScale(), nodeIds);
                // Area
                for (int i = 0; i < nodes.Length; i++) nodeCoor[i] = nodes[i].Coor;
                // Units
                fieldUnit = _controller.GetCurrentResultsUnitAbbreviation();
            }
            else throw new NotSupportedException();
            // Coor
            int[] distributedNodeIds = _controller.GetSpatiallyEquallyDistributedCoor(nodeCoor, 1);
            coor = nodeCoor[distributedNodeIds[0]];
            //
            bool showSurfaceId = _controller.Settings.Widgets.ShowEdgeSurId;
            bool showSurfaceLength = _controller.Settings.Widgets.ShowEdgeSurSize;
            bool showSurfaceMax = _controller.Settings.Widgets.ShowEdgeSurMax && results;
            bool showSurfaceMin = _controller.Settings.Widgets.ShowEdgeSurMin && results;
            bool showSurfaceAvg = _controller.Settings.Widgets.ShowEdgeSurAvg && results;
            if (!showSurfaceLength && !showSurfaceMax && !showSurfaceMin && !showSurfaceAvg) showSurfaceId = true;
            text = "";
            if (showSurfaceId)
            {
                text += string.Format("Surface id: {0} on {1}", surfaceId + 1, part.Name);
            }
            if (showSurfaceLength)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Surface area: {0} {1}", area.ToString(numberFormat), areaUnit);
            }
            if (showSurfaceMax)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Max: {0} {1}", max.ToString(numberFormat), fieldUnit);
            }
            if (showSurfaceMin)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Min: {0} {1}", min.ToString(numberFormat), fieldUnit);
            }
            if (showSurfaceAvg)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Avg: {0} {1}", avg.ToString(numberFormat), fieldUnit);
            }
        }
    }
}
