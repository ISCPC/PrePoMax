using CaeMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class PartWidget : WidgetBase
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public PartWidget(string name, string partName)
            : base(name)
        {
            _partId = Controller.DisplayedMesh.Parts[partName].PartId;
        }


        // Methods
        public override void GetWidgetData(out string text, out double[] coor)
        {
            FeMesh mesh = Controller.DisplayedMesh;
            BasePart part = mesh.GetPartById(_partId);
            if (part == null) throw new NotSupportedException();
            //
            string numberFormat = Controller.Settings.Widgets.GetNumberFormat();
            double[][] nodeCoor = new double[part.NodeLabels.Length][];
            if (Controller.CurrentView == ViewGeometryModelResults.Geometry ||
                Controller.CurrentView == ViewGeometryModelResults.Model)
            {
                for (int i = 0; i < part.NodeLabels.Length; i++) nodeCoor[i] = mesh.Nodes[part.NodeLabels[i]].Coor;
            }
            else if (Controller.CurrentView == ViewGeometryModelResults.Results)
            {
                FeNode[] nodes = Controller.GetScaledNodes(Controller.GetScale(), part.NodeLabels);
                for (int i = 0; i < nodes.Length; i++) nodeCoor[i] = nodes[i].Coor;
            }
            else throw new NotSupportedException();
            // Coor
            int[] distributedNodeIds = Controller.GetSpatiallyEquallyDistributedCoor(nodeCoor, 1);
            coor = nodeCoor[distributedNodeIds[0]];
            //
            bool showPartName = Controller.Settings.Widgets.ShowPartName;
            bool showPartId = Controller.Settings.Widgets.ShowPartId;
            bool showPartType = Controller.Settings.Widgets.ShowPartType;
            bool showPartNumberOfElements = Controller.Settings.Widgets.ShowPartNumberOfElements;
            bool showPartNumberOfNodes = Controller.Settings.Widgets.ShowPartNumberOfNodes;
            if (!showPartId && !showPartType && !showPartNumberOfElements && !showPartNumberOfNodes) showPartName = true;
            text = "";
            //
            if (showPartName)
            {
                text += string.Format("Part name: {0}", part.Name);
            }
            if (showPartId)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Part id: {0}", part.PartId);
            }
            if (showPartType)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Part type: {0}", part.PartType);
            }
            if (showPartNumberOfElements)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Number of elements: {0}", part.Labels.Length);
            }
            if (showPartNumberOfNodes)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("Number of nodes: {0}", part.NodeLabels.Length);
            }
            //
            if (IsTextOverriden) text = OverridenText;
        }
    }
}
