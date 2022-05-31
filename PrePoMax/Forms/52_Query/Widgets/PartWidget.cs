using CaeMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class PartWidget : WidgetBase
    {
        // Variables                                                                                                                
        private string _partName;


        // Properties                                                                                                               
        public string PartName { get { return _partName; } set { _partName = value; } }


        // Constructors                                                                                                             
        public PartWidget(string name, string partName, Controller controller)
            : base(name, controller)
        {
            _partName = partName;
        }


        // Methods
        public override void GetWidgetData(out string text, out double[] coor)
        {
            FeMesh mesh = _controller.DisplayedMesh;
            BasePart part = mesh.Parts[_partName];
            if (part == null) throw new NotSupportedException();
            //
            string numberFormat = _controller.Settings.Widgets.GetNumberFormat();
            double[][] nodeCoor = new double[part.NodeLabels.Length][];
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry ||
                _controller.CurrentView == ViewGeometryModelResults.Model)
            {
                for (int i = 0; i < part.NodeLabels.Length; i++) nodeCoor[i] = mesh.Nodes[part.NodeLabels[i]].Coor;
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                FeNode[] nodes = _controller.GetScaledNodes(_controller.GetScale(), part.NodeLabels);
                for (int i = 0; i < nodes.Length; i++) nodeCoor[i] = nodes[i].Coor;
            }
            else throw new NotSupportedException();
            // Coor
            int[] distributedNodeIds = _controller.GetSpatiallyEquallyDistributedCoor(nodeCoor, 1);
            coor = nodeCoor[distributedNodeIds[0]];
            //
            bool showPartName = _controller.Settings.Widgets.ShowPartName;
            bool showPartId = _controller.Settings.Widgets.ShowPartId;
            bool showPartType = _controller.Settings.Widgets.ShowPartType;
            bool showPartNumberOfElements = _controller.Settings.Widgets.ShowPartNumberOfElements;
            bool showPartNumberOfNodes = _controller.Settings.Widgets.ShowPartNumberOfNodes;
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
        }
    }
}
