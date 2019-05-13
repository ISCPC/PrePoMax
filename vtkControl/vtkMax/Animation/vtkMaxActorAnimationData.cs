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


        // Properties                                                                                                               
        public vtkPoints Points { get { return _points; } set { _points = value; } }
        public vtkDataArray PointNormals { get { return _pointNormals; } set { _pointNormals = value; } }
        public vtkPoints ModelEdgesPoints { get { return _modelEdgesPoints; } set { _modelEdgesPoints = value; } }
        public vtkFloatArray Values { get { return _values; } set { _values = value; } }
        public vtkMaxExtreemeNode MinNode { get { return _minNode; } set { _minNode = value; } }
        public vtkMaxExtreemeNode MaxNode { get { return _maxNode; } set { _maxNode = value; } }

        // Constructors                                                                                                             
        public vtkMaxActorAnimationData()
            : this(null, null, null, null)
        {
        }

        public vtkMaxActorAnimationData(float[] values)
            : this(null, null, values, null)
        {
        }

        public vtkMaxActorAnimationData(double[][] nodes, double[][] modelEdgesNodes)
            : this(nodes, modelEdgesNodes, null, null)
        {
        }

        public vtkMaxActorAnimationData(double[][] nodes, double[][] modelEdgesNodes, float[] values, NodesExchangeData extremeNodes)
        {
            // nodes
            if (nodes == null) _points = null;
            else
            {
                _points = vtkPoints.New();
                _points.SetNumberOfPoints(nodes.GetLength(0));
                for (int i = 0; i < nodes.GetLength(0); i++) _points.SetPoint(i, nodes[i][0], nodes[i][1], nodes[i][2]);
            }
            // normals
            _pointNormals = null;
            // model edges nodes
            if (modelEdgesNodes == null) _modelEdgesPoints = null;
            else
            {
                _modelEdgesPoints = vtkPoints.New();
                _modelEdgesPoints.SetNumberOfPoints(modelEdgesNodes.GetLength(0));
                for (int i = 0; i < modelEdgesNodes.GetLength(0); i++) _modelEdgesPoints.SetPoint(i, modelEdgesNodes[i][0], modelEdgesNodes[i][1], modelEdgesNodes[i][2]);
            }
            // values
            if (values == null) _values = null;
            else
            {
                _values = vtkFloatArray.New();
                _values.SetName(Globals.ScalarArrayName);
                _values.SetNumberOfValues(values.Length);
                for (int i = 0; i < values.Length; i++) _values.SetValue(i, values[i]);
            }

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
        }



    }
}
