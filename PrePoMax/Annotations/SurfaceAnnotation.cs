using CaeMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class SurfaceAnnotation : AnnotationBase
    {
        // Variables                                                                                                                
        private int _geometryId;


        // Properties                                                                                                               
        public int GeometryId { get { return _geometryId; } set { _geometryId = value; } }


        // Constructors                                                                                                             
        public SurfaceAnnotation(string name, int geometryId)
            : base(name)
        {
            _geometryId = geometryId;
            _partId = FeMesh.GetPartIdFromGeometryId(geometryId);
        }


        // Methods
        public override void GetAnnotationData(out string text, out double[] coor)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(_geometryId);
            FeMesh mesh = Controller.DisplayedMesh;
            int surfaceId = itemTypePartIds[0];            
            BasePart part = mesh.GetPartById(itemTypePartIds[2]);
            double area;
            string areaUnit = Controller.GetAreaUnit();
            string fieldUnit = "";
            string numberFormat = Controller.Settings.Annotations.GetNumberFormat();
            //
            bool results = false;
            float min = float.MaxValue;
            float max = -float.MaxValue;
            float sum = 0;
            float avg = 0;
            int[] nodeIds;
            double[][] nodeCoor;
            //
            GeomFaceType faceType = GeomFaceType.Unknown;
            if (part.Visualization.FaceTypes != null) faceType = part.Visualization.FaceTypes[surfaceId];
            //
            mesh.GetFaceNodes(_geometryId, out nodeIds);
            nodeCoor = new double[nodeIds.Length][];
            //
            if (Controller.CurrentView == ViewGeometryModelResults.Geometry ||
                Controller.CurrentView == ViewGeometryModelResults.Model)
            {
                // Area
                area = mesh.GetSurfaceArea(_geometryId);
                // Coor
                for (int i = 0; i < nodeIds.Length; i++) nodeCoor[i] = mesh.Nodes[nodeIds[i]].Coor;
            }
            else if (Controller.CurrentView == ViewGeometryModelResults.Results)
            {
                float value;
                FeNode[] nodes = Controller.GetScaledNodes(1, nodeIds);
                // Area
                Dictionary<int, double> weights;
                Dictionary<int, FeNode> nodesDic = new Dictionary<int, FeNode>();
                for (int i = 0; i < nodes.Length; i++) nodesDic.Add(nodes[i].Id, nodes[i]);
                mesh.GetFaceNodeLumpedWeights(part.Visualization, surfaceId, nodesDic, out weights, out area);
                // Coor
                nodes = Controller.GetScaledNodes(Controller.GetScale(), nodeIds);
                for (int i = 0; i < nodes.Length; i++) nodeCoor[i] = nodes[i].Coor;
                //
                if (Controller.ViewResultsType == ViewResultsTypeEnum.ColorContours)
                {
                    results = true;
                    // Values
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        value = Controller.GetNodalValue(nodeIds[i]);
                        if (value < min) min = value;
                        if (value > max) max = value;
                        sum += value;
                        //avg += (float)(value * weights[nodeIds[i]]);
                    }
                    avg = sum / nodes.Length;
                    // Units
                    fieldUnit = Controller.GetCurrentResultsUnitAbbreviation();
                    if (fieldUnit == "/") fieldUnit = "";
                }
            }
            else throw new NotSupportedException();
            // Coor
            int[] distributedNodeIds = Controller.GetSpatiallyEquallyDistributedCoor(nodeCoor, 1);
            coor = nodeCoor[distributedNodeIds[0]];
            //
            bool showSurfaceId = Controller.Settings.Annotations.ShowEdgeSurId;
            bool showSurfaceType = Controller.Settings.Annotations.ShowEdgeSurType;
            bool showSurfaceLength = Controller.Settings.Annotations.ShowEdgeSurSize;
            bool showSurfaceMax = Controller.Settings.Annotations.ShowEdgeSurMax && results;
            bool showSurfaceMin = Controller.Settings.Annotations.ShowEdgeSurMin && results;
            bool showSurfaceSum = Controller.Settings.Annotations.ShowEdgeSurSum && results;
            bool showSurfaceAvg = Controller.Settings.Annotations.ShowEdgeSurAvg && results;
            if (!showSurfaceType && !showSurfaceLength && !showSurfaceMax &&
                !showSurfaceMin && !showSurfaceSum && !showSurfaceAvg) showSurfaceId = true;
            //
            text = "";
            if (showSurfaceId)
            {
                text += string.Format("Surface id: {0} on {1}", surfaceId + 1, part.Name);
            }
            if (showSurfaceType)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Surface type: {0}", faceType.ToString());
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
            if (showSurfaceSum)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Nodal sum: {0} {1}", sum.ToString(numberFormat), fieldUnit);
            }
            if (showSurfaceAvg)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Avg: {0} {1}", avg.ToString(numberFormat), fieldUnit);
            }
            //
            if (IsTextOverridden) text = OverriddenText;
        }
    }
}
