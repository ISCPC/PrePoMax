using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vtkControl;

namespace PrePoMax
{
    [Serializable]
    public class SelectionNodeMouse : SelectionNode
    {
        // Variables                                                                                                                
        private double[] _pickedPoint;
        private double[][] _planeParameters; // Ox,Oy,Oz,Nx,Ny,Nz
        private vtkSelectBy _selectBy;
        private double _angle;


        // Properties                                                                                                               
        public double[] PickedPoint { get { return _pickedPoint; } set { _pickedPoint = value; } }
        public double[][] PlaneParameters { get { return _planeParameters; } set { _planeParameters = value; } }
        public vtkSelectBy SelectBy { get { return _selectBy; } set { _selectBy = value; } }
        public double Angle { get { return _angle; } set { _angle = value; } }


        // Constructors                                                                                                             
        public SelectionNodeMouse(double[] pickedPoint, double[][] planeParameters, vtkSelectOperation selectOpreation, vtkSelectBy selectBy, double angle)
            : base(selectOpreation)
        {
            _pickedPoint = pickedPoint;
            _planeParameters = planeParameters;
            _selectBy = selectBy;
            _angle = angle;
        }


        // Methods                                                                                                                  
    }
}
