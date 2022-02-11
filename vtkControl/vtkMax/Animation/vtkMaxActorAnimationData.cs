using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;
using CaeGlobals;

namespace vtkControl
{
    public class vtkMaxActorAnimationData
    {
        // Variables                                                                                                                
        private vtkPoints _points;
        private vtkDataArray _pointNormals;
        private vtkPoints _modelEdgesPoints;
        private vtkFloatArray _values;
        private vtkMaxExtreemeNode _minNode;
        private vtkMaxExtreemeNode _maxNode;
        private vtkPoints _locatorPoints;
        private vtkFloatArray _locatorValues;


        // Properties                                                                                                               
        public vtkPoints Points { get { return _points; } set { _points = value; } }
        public vtkDataArray PointNormals { get { return _pointNormals; } set { _pointNormals = value; } }
        public vtkPoints ModelEdgesPoints { get { return _modelEdgesPoints; } set { _modelEdgesPoints = value; } }
        public vtkFloatArray Values { get { return _values; } set { _values = value; } }
        public vtkMaxExtreemeNode MinNode { get { return _minNode; } set { _minNode = value; } }
        public vtkMaxExtreemeNode MaxNode { get { return _maxNode; } set { _maxNode = value; } }
        public vtkPoints LocatorPoints { get { return _locatorPoints; } set { _locatorPoints = value; } }
        public vtkFloatArray LocatorValues { get { return _locatorValues; } set { _locatorValues = value; } }

        // Constructors                                                                                                             
        public vtkMaxActorAnimationData(double[][] nodes, double[][] modelEdgesNodes, float[] values,
                                        NodesExchangeData extremeNodes, double[][] locatorNodes, float[] locatorValues)
        {
            // Nodes
            if (nodes == null) _points = null;
            else
            {
                _points = vtkPoints.New();
                _points.SetNumberOfPoints(nodes.Length);
                for (int i = 0; i < nodes.Length; i++) _points.SetPoint(i, nodes[i][0], nodes[i][1], nodes[i][2]);
            }
            // Normals
            _pointNormals = null;
            // Model edges nodes
            if (modelEdgesNodes == null) _modelEdgesPoints = null;
            else
            {
                _modelEdgesPoints = vtkPoints.New();
                _modelEdgesPoints.SetNumberOfPoints(modelEdgesNodes.Length);
                for (int i = 0; i < modelEdgesNodes.Length; i++)
                    _modelEdgesPoints.SetPoint(i, modelEdgesNodes[i][0], modelEdgesNodes[i][1], modelEdgesNodes[i][2]);
            }
            // Values
            if (values == null) _values = null;
            else
            {
                _values = vtkFloatArray.New();
                _values.SetName(Globals.ScalarArrayName);
                _values.SetNumberOfValues(values.Length);
                for (int i = 0; i < values.Length; i++) _values.SetValue(i, values[i]);
            }
            // Extreme nodes
            if (extremeNodes == null)
            {
                _minNode = null;
                _maxNode = null;
            }
            else
            {
                _minNode = new vtkMaxExtreemeNode(extremeNodes.Ids[0], extremeNodes.Coor[0], extremeNodes.Values[0]);
                _maxNode = new vtkMaxExtreemeNode(extremeNodes.Ids[1], extremeNodes.Coor[1], extremeNodes.Values[1]);
            }
            // Locator nodes
            if (locatorNodes == null) _locatorPoints = null;
            else
            {
                _locatorPoints = vtkPoints.New();
                _locatorPoints.SetNumberOfPoints(locatorNodes.Length);
                for (int i = 0; i < locatorNodes.Length; i++)
                    _locatorPoints.SetPoint(i, locatorNodes[i][0], locatorNodes[i][1], locatorNodes[i][2]);
            }
            // Locator values
            if (locatorValues == null) _locatorValues = null;
            else
            {
                _locatorValues = vtkFloatArray.New();
                _locatorValues.SetName(Globals.ScalarArrayName);
                _locatorValues.SetNumberOfValues(locatorValues.Length);
                for (int i = 0; i < locatorValues.Length; i++) _locatorValues.SetValue(i, locatorValues[i]);
            }
        }



    }
}
