using CaeGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class NodeWidget : WidgetBase
    {
        // Variables                                                                                                                
        private int _nodeId;


        // Properties                                                                                                               
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }


        // Constructors                                                                                                             
        public NodeWidget(string name, int nodeId)
            : base(name)
        {
            _nodeId = nodeId;
            //
            CaeMesh.FeMesh mesh = Controller.DisplayedMesh;
            foreach (var entry in mesh.Parts)
            {
                if (entry.Value.NodeLabels.Contains(_nodeId))
                {
                    _partId = entry.Value.PartId;
                    break;
                }
            }
        }


        // Methods
        public override void GetWidgetData(out string text, out double[] coor)
        {
            Vec3D nodeVec;
            Vec3D arrowVec;
            string fieldData = "";
            text = "";
            string numberFormat = Controller.Settings.Widgets.GetNumberFormat();
            //
            if (Controller.CurrentView == ViewGeometryModelResults.Geometry ||
                Controller.CurrentView == ViewGeometryModelResults.Model)
            {
                nodeVec = new Vec3D(Controller.GetNode(_nodeId).Coor);
                arrowVec = nodeVec;
            }
            else if (Controller.CurrentView == ViewGeometryModelResults.Results)
            {
                nodeVec = new Vec3D(Controller.GetScaledNode(1, _nodeId).Coor);
                // Arrow
                float scale = Controller.GetScale();
                arrowVec = new Vec3D(Controller.GetScaledNode(scale, _nodeId).Coor); // for the arrow
                //
                if (Controller.ViewResultsType == ViewResultsType.ColorContours)
                {
                    float fieldValue = Controller.GetNodalValue(_nodeId);
                    string fieldUnit = Controller.GetCurrentResultsUnitAbbreviation();
                    if (fieldUnit == "/") fieldUnit = "";
                    //
                    fieldData = string.Format("Value: {1} {2}", Environment.NewLine,
                                              fieldValue.ToString(numberFormat), fieldUnit);
                }
            }
            else throw new NotSupportedException();
            //
            string lengthUnit = Controller.GetLengthUnit();
            //
            bool addNodeIdData = Controller.Settings.Widgets.ShowNodeId;
            bool addCoorData = Controller.Settings.Widgets.ShowCoordinates;
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
