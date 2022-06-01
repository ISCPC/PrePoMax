using CaeGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class NodeWidget : WidgetBase
    {
        // Variables                                                                                                                
        private int _nodeId;


        // Properties                                                                                                               
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }


        // Constructors                                                                                                             
        public NodeWidget(string name, int nodeId, Controller controller)
            : base(name, controller)
        {
            _nodeId = nodeId;
        }


        // Methods
        public override void GetWidgetData(out string text, out double[] coor)
        {
            Vec3D nodeVec;
            Vec3D arrowVec;
            string fieldData = "";
            text = "";
            string numberFormat = _controller.Settings.Widgets.GetNumberFormat();
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry ||
                _controller.CurrentView == ViewGeometryModelResults.Model)
            {
                nodeVec = new Vec3D(_controller.GetNode(_nodeId).Coor);
                arrowVec = nodeVec;
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                nodeVec = new Vec3D(_controller.GetScaledNode(1, _nodeId).Coor);
                //
                float fieldValue = _controller.GetNodalValue(_nodeId);
                string fieldUnit = _controller.GetCurrentResultsUnitAbbreviation();
                // Arrow
                float scale = _controller.GetScale();
                arrowVec = new Vec3D(_controller.GetScaledNode(scale, _nodeId).Coor); // for the arrow
                // Data
                fieldData = string.Format("Value: {1} {2}", Environment.NewLine, fieldValue.ToString(numberFormat), fieldUnit);
            }
            else throw new NotSupportedException();
            //
            string lengthUnit = _controller.GetLengthUnit();
            //
            bool addNodeIdData = _controller.Settings.Widgets.ShowNodeId;
            bool addCoorData = _controller.Settings.Widgets.ShowCoordinates;
            bool addFieldData = fieldData.Length > 0;
            if (!addCoorData && !addFieldData) addNodeIdData = true;
            // Node data
            if (addNodeIdData) text = "Node id: " + _nodeId;
            // Coordinates data
            if (addCoorData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                //
                text += string.Format("X: {1} {4}{0}Y: {2} {4}{0}Z: {3} {4}", Environment.NewLine,
                    nodeVec.Coor[0].ToString(numberFormat),
                    nodeVec.Coor[1].ToString(numberFormat),
                    nodeVec.Coor[2].ToString(numberFormat),
                    lengthUnit);
            }
            // Field value data
            if (addFieldData)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += fieldData;
            }
            //
            coor = arrowVec.Coor;
            //
            if (IsTextOverriden) text = OverridenText;
        }

    }
}
