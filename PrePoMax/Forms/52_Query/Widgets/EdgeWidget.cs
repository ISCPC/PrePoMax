using CaeGlobals;
using CaeMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class EdgeWidget : WidgetBase
    {
        // Variables                                                                                                                
        private int _geometryId;


        // Properties                                                                                                               
        public int GeometryId { get { return _geometryId; } set { _geometryId = value; } }


        // Constructors                                                                                                             
        public EdgeWidget(string name, int geometryId, Controller controller)
            : base(name, controller)
        {
            _geometryId = geometryId;
        }


        // Methods
        public override void GetWidgetData(out string text, out double[] coor)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(_geometryId);
            FeMesh mesh = _controller.DisplayedMesh;
            int edgeId = itemTypePartIds[0];
            BasePart part = mesh.GetPartById(itemTypePartIds[2]);
            double length;
            string lenUnit = _controller.GetLengthUnit();
            string fieldUnit = "";
            string numberFormat = _controller.Settings.Widgets.GetNumberFormat();
            //
            FeNode n1;
            FeNode n2;
            bool results = false;
            float min = float.MaxValue;
            float max = -float.MaxValue;
            float avg = 0;
            int[] nodeIds;
            double[] nodeWeights;
            //
            mesh.GetEdgeNodeCoor(_geometryId, out nodeIds, out double[][] nodeCoor);
            nodeWeights = new double[nodeIds.Length];
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry ||
                _controller.CurrentView == ViewGeometryModelResults.Model)
            {
                n2 = mesh.Nodes[nodeIds[nodeIds.Length / 2]];
                if (nodeIds.Length == 2)
                {
                    n1 = mesh.Nodes[nodeIds[0]];
                    coor = FeMesh.GetMidNodeCoor(n1, n2);
                }
                else coor = n2.Coor;
                //
                length = mesh.GetEdgeLength(_geometryId);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                results = true;
                //
                n2 = _controller.GetScaledNode(_controller.GetScale(), nodeIds[nodeIds.Length / 2]);
                if (nodeIds.Length == 2)
                {
                    n1 = _controller.GetScaledNode(_controller.GetScale(), nodeIds[0]);
                    coor = FeMesh.GetMidNodeCoor(n1, n2);
                }
                else coor = n2.Coor;
                //
                Vec3D v1;
                Vec3D v2;
                float value;
                double segLen;
                length = 0;
                FeNode[] nodes = _controller.GetScaledNodes(1, nodeIds);
                // Length
                for (int i = 0; i < nodes.Length - 1; i++)
                {
                    v1 = new Vec3D(nodes[i].Coor);
                    v2 = new Vec3D(nodes[i + 1].Coor);
                    segLen = (v2 - v1).Len;
                    nodeWeights[i] += segLen * 0.5;
                    nodeWeights[i + 1] += segLen * 0.5;
                    length += segLen;
                }
                // Values
                for (int i = 0; i < nodes.Length; i++)
                {
                    value = _controller.GetNodalValue(nodeIds[i]);
                    if (value < min) min = value;
                    if (value > max) max = value;
                    avg += (float)(value * nodeWeights[i]);
                }
                avg /= (float)length;
                // Units
                fieldUnit = _controller.GetCurrentResultsUnitAbbreviation();
            }
            else throw new NotSupportedException();
            //
            bool addEdgeIdData = _controller.Settings.Widgets.ShowEdgeSurId;
            bool addEdgeLengthData = _controller.Settings.Widgets.ShowEdgeSurSize;
            bool addEdgeMaxData = _controller.Settings.Widgets.ShowEdgeSurMax && results;
            bool addEdgeMinData = _controller.Settings.Widgets.ShowEdgeSurMin && results;
            bool addEdgeAvgData = _controller.Settings.Widgets.ShowEdgeSurAvg && results;
            if (!addEdgeLengthData && !addEdgeMaxData && !addEdgeMinData && !addEdgeAvgData) addEdgeIdData = true;
            text = "";
            if (addEdgeIdData)
            {
                text += string.Format("Edge id: {0} on {1}", edgeId + 1, part.Name);
            }
            if (addEdgeLengthData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Edge length: {0} {1}", length.ToString(numberFormat), lenUnit);
            }
            if (addEdgeMaxData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Max: {0} {1}", max.ToString(numberFormat), fieldUnit);
            }
            if (addEdgeMinData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Min: {0} {1}", min.ToString(numberFormat), fieldUnit);
            }
            if (addEdgeAvgData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Avg: {0} {1}", avg.ToString(numberFormat), fieldUnit);
            }
            //
            if (IsTextOverriden) text = OverridenText;
        }
    }
}
